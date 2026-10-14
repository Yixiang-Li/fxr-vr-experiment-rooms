# Quest handoff — Memory Room

> Current target (14 October): desktop keyboard/mouse. This is an optional XR/Quest fallback guide. The previous 0.3.0 APK is historical and does not include the new desktop work; no headset test has been completed.
The computer-side setup uses Unity **2022.3.62f3**, Android ARM64 / IL2CPP, Vulkan, OpenXR **1.16.1**, Meta Quest Support, and the Oculus Touch controller profile. This is a controller-based VR experience; no hand-tracking or passthrough package is required for its current scope. [Unity OpenXR 1.16 documentation](https://docs.unity3d.com/Packages/com.unity.xr.openxr@1.16/manual/index.html).

## Already automated

- Room/environment generation and integration into the existing MainScene.
- **FXR → Quest → Validate and build updated APK** checks the installed OpenXR rules, configures mobile rendering, and builds `Builds/Quest/FXRMemory.apk`.
- `tools/quest_device.py` detects connected devices, installs/launches the app, and collects this app's CSV or logs. It refuses installation if no authorized device is connected or multiple devices are ambiguous.
- After connecting the headset, double-click `tools/Quest_Install.command` on Mac or `tools/Quest_Install.bat` on Windows. These optional helpers need Python 3. They install the current local APK and open the app.

The helper finds ADB on PATH, through `ANDROID_SDK_ROOT` / `ANDROID_HOME`, or in the default Unity Hub installation for this exact Editor (including Windows `adb.exe`). For a custom Unity/SDK folder, pass `--adb "FULL_PATH_TO_ADB"`. On Windows use `py -3 tools/quest_device.py status` (or `python` if you installed Python without the launcher); on Mac use `python3`. USB driver setup can also be required on Windows if the headset is not detected. Unity's Android **Build And Run** is an alternative that does not need Python.

Builds are ignored by Git: teammates must build their own APK using the menu above. The local generated APK and `Readiness.txt` are available on Yixiang's Mac.

## What still needs the school headset

1. Confirm the model and that the school allows installing a coursework APK. Ask the school to enable developer mode if necessary; this helper does not change its account or developer settings.
2. Connect USB. Inside the headset, approve the computer's USB debugging request.
3. Run the install helper. Verify that head tracking and both controller rays work before starting a participant session.
4. Test the complete Office → Nature flow, then Nature → Office. Check eye height, text size, trigger activation, movement comfort, sound levels, pause/resume, Reset/cancel, and results retrieval. Confirm that the app maintains a comfortable stable frame rate on that actual Quest model.
5. Complete one session, then retrieve its files with `python3 tools/quest_device.py results` on Mac or `py -3 tools/quest_device.py results` on Windows. Replace `results` with `logs` to collect app-only logs while the app is running.

Controls: point and trigger to select; thumbstick to move; turning follows the shared XRI starter rig. In the editor, desktop WASD/right drag/left click is a preview mode. School Quest permissions, controller behaviour and comfort have not yet been verified on hardware.

Keep a single shared player, EventSystem and XR Interaction Manager. Only MainScene is included in the final APK. Confirm the teacher's touchpad gliding/flying wording before claiming this locomotion meets that part of the rubric.
