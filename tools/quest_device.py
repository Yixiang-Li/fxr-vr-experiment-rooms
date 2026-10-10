"""Quest install / collect helper for macOS and Windows. Needs Python 3 only."""
from pathlib import Path
import argparse
import os
import shutil
import subprocess
import sys

ROOT = Path(__file__).resolve().parents[1]
EDITOR_VERSION = '2022.3.62f3'


def find_adb(explicit=None):
    """Use an override, PATH, an Android SDK, or the course Editor's bundled SDK."""
    executable = 'adb.exe' if sys.platform == 'win32' else 'adb'
    if explicit:
        found = shutil.which(explicit)
        path = Path(os.path.expandvars(explicit)).expanduser()
        return found or (str(path) if path.is_file() else None)
    found = shutil.which(executable)
    if found:
        return found
    candidates = []
    for variable in ('ANDROID_SDK_ROOT', 'ANDROID_HOME'):
        if os.environ.get(variable):
            candidates.append(Path(os.environ[variable]) / 'platform-tools' / executable)
    if sys.platform == 'win32':
        for variable in ('ProgramW6432', 'ProgramFiles'):
            folder = os.environ.get(variable, 'C:/Program Files')
            candidates.append(Path(folder) / 'Unity/Hub/Editor' / EDITOR_VERSION /
                              'Editor/Data/PlaybackEngines/AndroidPlayer/SDK/platform-tools' / executable)
    else:
        editor = Path('/Applications/Unity/Hub/Editor') / EDITOR_VERSION
        for folder in ('PlaybackEngines', 'Unity.app/Contents/PlaybackEngines'):
            candidates.append(editor / folder / 'AndroidPlayer/SDK/platform-tools' / executable)
    return next((str(path) for path in candidates if path.is_file()), None)


def main():
    parser = argparse.ArgumentParser(description='FXR Quest install / collect helper')
    parser.add_argument('action', choices=['status', 'install', 'results', 'logs'],
                        nargs='?', default='status')
    parser.add_argument('--serial', help='Required when more than one Android device is connected')
    parser.add_argument('--adb', help='Full path to adb / adb.exe for a custom Unity or SDK installation')
    args = parser.parse_args()
    adb = find_adb(args.adb)
    if not adb:
        sys.exit('ADB is missing. Install Android Build Support for Unity ' + EDITOR_VERSION +
                 ', or pass --adb "FULL_PATH_TO_ADB" for a custom installation.')

    def run(parts, select_device=True):
        selector = ['-s', args.serial] if select_device and args.serial else []
        return subprocess.run([adb] + selector + parts, check=True, capture_output=True,
                              text=True, encoding='utf-8', errors='replace').stdout

    listing = run(['devices', '-l'], select_device=False)
    print(listing.strip())
    devices = [line.split()[0] for line in listing.splitlines()[1:] if '\tdevice' in line]
    if args.action == 'status':
        return
    if not devices:
        sys.exit('No authorized Quest is connected. Connect USB and approve USB debugging inside the headset. Ask the school if developer mode is unavailable.')
    if args.serial and args.serial not in devices:
        sys.exit('The selected device is not connected and authorized.')
    if len(devices) > 1 and not args.serial:
        sys.exit('Multiple devices connected. Re-run with --serial DEVICE_ID.')
    if args.action == 'install':
        apk = ROOT / 'Builds/Quest/FXRMemory.apk'
        if not apk.exists():
            sys.exit('Builds/Quest/FXRMemory.apk is missing. Use FXR > Quest > Validate and build updated APK in Unity first.')
        print(run(['install', '-r', str(apk)]).strip())
        print(run(['shell', 'monkey', '-p', 'fr.fxr.memoryrooms',
                   '-c', 'android.intent.category.LAUNCHER', '1']).strip())
        print('App launched. Check the experience inside the headset.')
    else:
        folder = ROOT / 'Builds/Quest/Collected'
        folder.mkdir(parents=True, exist_ok=True)
        if args.action == 'results':
            remote = '/sdcard/Android/data/fr.fxr.memoryrooms/files/MemoryResults'
            print(run(['pull', remote, str(folder)]).strip())
        else:
            # Restrict collection to this prototype, rather than all headset apps.
            pid = run(['shell', 'pidof', 'fr.fxr.memoryrooms']).strip().split()
            if not pid:
                sys.exit('App is not running. Launch it before collecting its logs.')
            log = run(['logcat', '-d', '--pid=' + pid[0]])
            (folder / 'app-log.txt').write_text(log, encoding='utf-8')
            print('App log saved in Builds/Quest/Collected/app-log.txt')


if __name__ == '__main__':
    main()
