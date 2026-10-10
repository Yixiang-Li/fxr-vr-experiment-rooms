"""Generate only the original neutral task cues using the Python stdlib.
Real ambience is prepared by fetch_real_assets.py and is never overwritten here.
"""
from pathlib import Path
import math, random, wave, array, json
ROOT = Path(__file__).resolve().parents[1] / 'Assets/Rooms/Memory/Audio'
RATE, SECONDS = 22050, 48
N = RATE * SECONDS
rng = random.Random(7714)
ROOT.mkdir(parents=True, exist_ok=True)

def noise(lowpass, target):
    out, value = [], 0.0
    for _ in range(N):
        value += lowpass * (rng.uniform(-1, 1) - value)
        out.append(value)
    rms = math.sqrt(sum(x*x for x in out)/N)
    return [x*target/rms for x in out]

def event(samples, time, duration, frequencies, level, decay=18):
    start = int(time*RATE)
    for i in range(int(duration*RATE)):
        if start+i >= len(samples): break
        t=i/RATE
        envelope=(1-math.exp(-t*1200))*math.exp(-t*decay)
        value=sum(math.sin(2*math.pi*f*t) for f in frequencies)/len(frequencies)
        samples[start+i] += level*envelope*value

def save(name, samples, loop=False, target=None):
    if loop:
        # Equal-power join across the boundary avoids a click during the 3-minute trial.
        k=int(.35*RATE)
        for i in range(k):
            ratio=i/(k-1)
            value=samples[-k+i]*(1-ratio)+samples[i]*ratio
            samples[i]=value
        samples=samples[:-k]
    if target:
        rms=math.sqrt(sum(x*x for x in samples)/len(samples))
        samples=[x*target/max(rms,1e-9) for x in samples]
    peak=max(abs(x) for x in samples)
    if peak>.7: samples=[x*.7/peak for x in samples]
    with wave.open(str(ROOT/(name+'.wav')),'wb') as f:
        f.setnchannels(1); f.setsampwidth(2); f.setframerate(RATE)
        f.writeframes(array.array('h',(int(max(-1,min(1,x))*32767) for x in samples)).tobytes())
    return {'clip':name,'seconds':round(len(samples)/RATE,2),'rms':round(math.sqrt(sum(x*x for x in samples)/len(samples)),5),'peak':round(max(abs(x) for x in samples),5)}

# Ambience now comes from licensed recordings. Only neutral UI cues are generated.
metrics=[]
for name,freqs,seconds,level in [('Flip',[820,1230],.065,.1),('Match',[660,880,1100],.22,.11),('Mismatch',[330,440],.13,.07)]:
    samples=[0.0]*int(seconds*RATE)
    event(samples,0,seconds,freqs,level,18)
    metrics.append(save(name,samples))
print(json.dumps(metrics,indent=2))
