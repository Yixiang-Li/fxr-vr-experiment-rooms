"""Fetch a small, explicitly selected CC0 set; convert textures and real recordings.

Run on macOS with numpy/Pillow. Unity imports the checked-in outputs; teammates
do not need to run this script or download a modeling application.
"""
from pathlib import Path
import hashlib, json, re, shutil, subprocess, wave
import numpy as np
from PIL import Image

ROOT=Path(__file__).resolve().parents[1]
CACHE=ROOT/'Builds/AssetSources'
DEST=ROOT/'Assets/Rooms/Memory/ThirdParty'
CACHE.mkdir(parents=True,exist_ok=True)
DEST.mkdir(parents=True,exist_ok=True)
MODEL_IDS=['metal_office_desk','modern_arm_chair_01','potted_plant_01','potted_plant_02','potted_plant_04','wall_clock','desk_lamp_arm_01']
SURFACE_IDS=['wood_table_001','floor_tiles_06','painted_plaster_wall']

def fetch(url,path,md5=None):
    path=Path(path);path.parent.mkdir(parents=True,exist_ok=True)
    if not path.exists() or md5 and hashlib.md5(path.read_bytes()).hexdigest()!=md5:
        subprocess.run(['curl','-L','--fail','--silent','--show-error','-A','FXR-Student-Project/1.0',url,'-o',str(path)],check=True)
    if md5 and hashlib.md5(path.read_bytes()).hexdigest()!=md5:
        raise ValueError('Source checksum mismatch: '+str(path))
    return path

def api(asset):
    p=fetch('https://api.polyhaven.com/files/'+asset,CACHE/(asset+'-files.json'))
    return json.loads(p.read_text())

def source_texture(data,key,asset):
    versions=data[key]['1k']
    kind='png' if 'alpha' in key.lower() else 'jpg'
    if kind not in versions:kind='png'
    info=versions[kind]
    path=fetch(info['url'],CACHE/asset/(key+'.'+kind),info['md5'])
    return Image.open(path).copy()

def make_maps(asset,data,target):
    parts=[]
    for diff in [k for k in data if k.lower()=='diffuse' or k.endswith('_diff')]:
        prefix=diff[:-5] if diff.endswith('_diff') else ''
        name=prefix or 'main'
        def key(generic,suffix):
            return prefix+'_'+suffix if prefix else generic
        color=source_texture(data,diff,asset).convert('RGB')
        alpha=key('Alpha','alpha')
        ext='.jpg'
        if alpha in data:
            color=color.convert('RGBA');color.putalpha(source_texture(data,alpha,asset).convert('L'));ext='.png'
        color.save(target/(name+'_BaseColor'+ext))
        normal=key('nor_gl','nor_gl')
        if normal in data:source_texture(data,normal,asset).convert('RGB').save(target/(name+'_Normal.jpg'),quality=94)
        rough=key('Rough','rough');metal=key('Metal','metal');ao=key('AO','ao')
        r=np.asarray(source_texture(data,rough,asset).convert('L')) if rough in data else np.full((1024,1024),190,dtype=np.uint8)
        m=np.asarray(source_texture(data,metal,asset).convert('L')) if metal in data else np.zeros_like(r)
        packed=np.zeros((*r.shape,4),dtype=np.uint8);packed[:,:,0]=m;packed[:,:,3]=255-r
        Image.fromarray(packed).save(target/(name+'_MetallicSmoothness.png'))
        if ao in data:source_texture(data,ao,asset).convert('RGB').save(target/(name+'_Occlusion.jpg'),quality=92)
        parts.append({'name':name,'baseColor':name+'_BaseColor'+ext,'alphaClip':alpha in data})
    return parts

for asset in MODEL_IDS+SURFACE_IDS:
    data=api(asset);target=DEST/'PolyHaven'/asset;target.mkdir(parents=True,exist_ok=True)
    if asset in MODEL_IDS:
        model=data['fbx']['1k']['fbx'];fetch(model['url'],target/(asset+'.fbx'),model['md5'])
    parts=make_maps(asset,data,target)
    (target/'source.json').write_text(json.dumps({'id':asset,'source':'https://polyhaven.com/a/'+asset,
        'license':'CC0-1.0','licenseUrl':'https://polyhaven.com/license','materials':parts},indent=2)+'\n')
    print('Prepared CC0 asset:',asset,flush=True)

def window_view(panorama,yaw_degrees):
    # Perspective backplate, instead of stretching an equirectangular panorama.
    width,height=1536,614
    x=np.linspace(-1,1,width);y=np.linspace(1,-1,height)
    xx,yy=np.meshgrid(x,y);fov=np.deg2rad(110)
    vx=xx*np.tan(fov/2);vy=yy*np.tan(fov/2)*height/width;vz=np.ones_like(vx)
    yaw=np.deg2rad(yaw_degrees)
    ax=vx*np.cos(yaw)+vz*np.sin(yaw);az=-vx*np.sin(yaw)+vz*np.cos(yaw)
    u=(np.arctan2(ax,az)/(2*np.pi)+.5)%1
    v=.5-np.arctan2(vy,np.sqrt(ax*ax+az*az))/np.pi
    data=np.asarray(panorama.convert('RGB'))
    return Image.fromarray(data[np.clip((v*data.shape[0]).astype(int),0,data.shape[0]-1),(u*data.shape[1]).astype(int)%data.shape[1]])

for asset,yaw,label in [('forest_slope',-80,'GardenWindow'),('potsdamer_platz',-30,'OfficeWindow')]:
    data=api(asset)['tonemapped'];p=fetch(data['url'],CACHE/(asset+'.jpg'),data['md5'])
    img=Image.open(p);thumb=img.copy();thumb.thumbnail((1536,768));thumb.save(CACHE/(asset+'-review.jpg'))
    target=DEST/'PolyHaven'/asset;target.mkdir(parents=True,exist_ok=True)
    window_view(img,yaw).save(target/(label+'.jpg'),quality=95)
    (target/'source.json').write_text(json.dumps({'id':asset,'source':'https://polyhaven.com/a/'+asset,
       'license':'CC0-1.0','changes':'Perspective crop of the official tonemapped HDRI; not an example render.'},indent=2)+'\n')
    print('Prepared photographed window:',asset,flush=True)

SOUNDS=[('Office','Globofonia',563248,185,.05),('OfficeDetail','Breviceps',447909,60,.018),
        ('Nature','NightVoice',123999,76,.05),('NatureDetail','guidofm',509174,22,.018)]
credits=[]
for name,author,sound_id,duration,rms in SOUNDS:
    url=f'https://freesound.org/people/{author}/sounds/{sound_id}/'
    html=fetch(url,CACHE/(str(sound_id)+'.html')).read_text()
    if 'creativecommons.org/publicdomain/zero/1.0' not in html:
        raise ValueError('Expected CC0 on source page: '+url)
    matches=re.findall(r'https://cdn\.freesound\.org/previews/[^"\s<>]+-hq\.mp3',html)
    if not matches:raise ValueError('No public HQ preview linked by source page: '+url)
    mp3=fetch(matches[0],CACHE/(name+'.mp3'))
    decoded=CACHE/(name+'-decoded.wav')
    if not decoded.exists():subprocess.run(['afconvert','-f','WAVE','-d','LEI16@44100','-c','1',str(mp3),str(decoded)],check=True)
    with wave.open(str(decoded)) as wav:
        rate=wav.getframerate();samples=np.frombuffer(wav.readframes(wav.getnframes()),dtype='<i2').astype(np.float64)/32768
    if name=='OfficeDetail':
        # Irregular bursts made from recorded typing, with real pauses between them.
        result=np.zeros(int(duration*rate));rng=np.random.default_rng(20261010)
        for t in [2,8.2,15.7,24,32.6,41.2,51.4]:
            segment=samples.copy();segment*=float(rng.uniform(.75,1.05))
            start=int(t*rate);result[start:start+len(segment)]+=segment[:len(result)-start]
        samples=result
    else:
        samples=samples[int(rate):int((duration+1)*rate)]
        n=min(int(.9*rate),len(samples)//10);blend=np.linspace(0,1,n)
        seam=samples[-n:]*(1-blend)+samples[:n]*blend
        samples=np.concatenate([samples[n:-n],seam])
    samples-=samples.mean()
    # Smooth peak limiting preserves recorded detail while matching source RMS.
    # Binary search the gain; the tanh ceiling prevents harsh digital clipping.
    lo,hi=0.0,10000.0
    for _ in range(40):
        gain=(lo+hi)/2
        candidate=.9*np.tanh(samples*gain/.9)
        if np.sqrt(np.mean(candidate*candidate))<rms:lo=gain
        else:hi=gain
    samples=.9*np.tanh(samples*((lo+hi)/2)/.9)
    output=ROOT/'Assets/Rooms/Memory/Audio'/ (name+'.wav')
    with wave.open(str(output),'wb') as wav:
        wav.setnchannels(1);wav.setsampwidth(2);wav.setframerate(rate);wav.writeframes((samples*32767).astype('<i2').tobytes())
    actual=float(np.sqrt(np.mean(samples*samples)))
    credits.append({'clip':name,'author':author,'source':url,'download':matches[0],
        'license':'CC0-1.0','changes':'Public HQ MP3 preview decoded, edited for looping/pauses, mono, smoothly peak limited and matched source RMS; no synthesized ambience.',
        'rms':actual,'duration':len(samples)/rate})
    print('Prepared real recording:',name,round(len(samples)/rate,1),'seconds; RMS',round(actual,4),flush=True)
(DEST/'RecordedAudioSources.json').write_text(json.dumps({'sounds':credits},indent=2)+'\n')
