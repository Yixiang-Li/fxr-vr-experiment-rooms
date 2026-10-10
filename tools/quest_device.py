"""Small local helper for the school Quest. Never changes headset account/developer settings."""
from pathlib import Path
import argparse, subprocess, shutil, sys
ROOT=Path(__file__).resolve().parents[1]
p=argparse.ArgumentParser(description='FXR Quest install / collect helper')
p.add_argument('action',choices=['status','install','results','logs'],nargs='?',default='status')
p.add_argument('--serial',help='Required when more than one Android device is connected')
args=p.parse_args()
adb=shutil.which('adb')
if not adb:
    editors=Path('/Applications/Unity/Hub/Editor')
    matches=list(editors.glob('2022.3.62f3/PlaybackEngines/AndroidPlayer/SDK/platform-tools/adb'))
    if matches: adb=str(matches[0])
if not adb: sys.exit('ADB is missing. Install Android Build Support for Unity 2022.3.62f3, or put adb on PATH.')
def run(parts,binary=False):
    return subprocess.run([adb]+(['-s',args.serial] if args.serial else [])+parts,check=True,capture_output=True,text=not binary).stdout
listing=run(['devices','-l'])
print(listing.strip())
devices=[line.split()[0] for line in listing.splitlines()[1:] if '\tdevice' in line]
if args.action=='status': sys.exit(0)
if not devices: sys.exit('No authorized Quest is connected. Connect USB and approve USB debugging inside the headset. Ask the school if developer mode is unavailable.')
if args.serial and args.serial not in devices: sys.exit('The selected device is not connected and authorized.')
if len(devices)>1 and not args.serial: sys.exit('Multiple devices connected. Re-run with --serial DEVICE_ID.')
if args.action=='install':
    apk=ROOT/'Builds/Quest/FXRMemory.apk'
    if not apk.exists(): sys.exit('Builds/Quest/FXRMemory.apk is missing. Use FXR > Quest > Validate and build updated APK in Unity first.')
    print(run(['install','-r',str(apk)]).strip())
    print(run(['shell','monkey','-p','fr.fxr.memoryrooms','-c','android.intent.category.LAUNCHER','1']).strip())
    print('App launched. Check the experience inside the headset.')
else:
    folder=ROOT/'Builds/Quest/Collected';folder.mkdir(parents=True,exist_ok=True)
    if args.action=='results':
        remote='/sdcard/Android/data/fr.fxr.memoryrooms/files/MemoryResults'
        print(run(['pull',remote,str(folder)]).strip())
    else:
        # Restrict log collection to this prototype process, rather than all headset apps.
        pid=run(['shell','pidof','fr.fxr.memoryrooms']).strip().split()
        if not pid: sys.exit('App is not running. Launch it before collecting its logs.')
        text=run(['logcat','-d','--pid='+pid[0]])
        (folder/'app-log.txt').write_text(text)
        print('App log saved in Builds/Quest/Collected/app-log.txt')
