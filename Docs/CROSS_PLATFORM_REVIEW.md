# Windows / macOS repository review

Reviewed on 2026-10-10. Source baseline: `900c0af` on GitHub `main`; the following compatibility changes are included with this report.

## Conclusion

**This repository can be shared by Windows and macOS developers using Unity 2022.3.62f3.** Each person clones the complete project to their own computer and works in their own development scene. There is no project-level Mac-only dependency that requires a separate Windows project or repository.

This conclusion is based on the repository checks and Unity's documented platform support below. **A native Windows Editor/graphics run has not been performed.** The Windows batch launcher and an actual Quest USB connection also remain untested on Windows.

## What was checked

| Area | Result |
| --- | --- |
| Repository contents | 982 tracked files at the baseline; `Assets`, `Packages`, and `ProjectSettings` are included. No tracked Library, Temp, Logs, Builds or UserSettings. No Git LFS pointers or symlinks. |
| Editor and packages | Exact Editor is `2022.3.62f3 (96770f904ca7)`. Packages are version-pinned Unity registry/bundled dependencies; no developer's local `file:` package dependency. |
| Runtime and editor code | No project C# dependency on a Mac home folder, Mac-only runtime API or imported native plugin. CSV uses `Application.persistentDataPath` and `Path.Combine`. Room creation normalizes Unity asset paths. Runtime/sample assemblies do not exclude Windows. |
| Models, materials and recordings | Included FBX, Unity mesh assets, images, materials and WAV recordings. No `.blend` source that requires Blender to import. Source preparation scripts are optional; teammates use the included outputs. |
| Git metadata and names | All visible asset files have their tracked `.meta` files. 510 unique asset GUIDs, no duplicates, no Windows-invalid names, no case/Unicode collisions. Longest tracked relative path: 126 characters. |
| Fresh GitHub download | Cloned directly from GitHub into an empty folder with no existing Library. Unity 2022.3.62f3 on macOS resolved packages, compiled and imported it successfully. All four scenes passed reference checks: zero missing scripts, meshes, materials, ambience clips or font/material references. |
| Fresh-clone PlayMode | 3/3 existing integration/UX tests passed on macOS, covering MainScene, MemoryDev, both rounds, reset/physics/CSV and participant UI. Run finished at 2026-10-10 20:07 UTC. This is not a Windows or Quest hardware test. |
| Graphics and XR | URP is used; desktop graphics are not forced to Metal. Android uses Vulkan/OpenXR. Desktop Play uses the keyboard/mouse rig; APK builds use the existing Quest rig. PC VR through Quest Link is not configured by this review. |

## Changes made during review

1. The Quest helper previously searched only the Mac Unity Hub SDK when ADB was absent from PATH. It now also finds Windows `adb.exe` in the course Editor's default Hub installation, SDK environment variables, or an explicit `--adb` path. Process arguments are passed separately, including paths with spaces. Log decoding/saving explicitly uses UTF-8.
2. Added optional `tools/Quest_Install.bat` for Windows. It requires Python 3; room editing, desktop Play and Unity's Android Build And Run do not require Python.
3. Added explicit Git line-ending rules: Python/Mac launchers use LF; Windows batch files use CRLF. Existing Unity scene/asset/meta LF rules are retained.
4. Added a hidden marker to retain an empty XRI sample Editor folder in Git, preserving its existing folder `.meta` GUID.
5. Updated the Chinese/English team and Quest instructions.

The helper passed six checks on macOS: simulated Windows Hub/SDK discovery, custom paths, PATH precedence, refusal without an unambiguous authorized device, serial/space-containing path arguments, and UTF-8 log output. Actual Mac ADB status also succeeded with no connected devices. **These checks simulate Windows discovery and mock device responses; they do not establish a native Windows batch/USB test.**

## Windows first opening

1. Accept the private repository invitation, then clone using GitHub Desktop to a short local folder such as `C:\Projects\FXR`.
2. Install the exact **2022.3.62f3** Windows Editor and add the cloned folder in Unity Hub. Let the first import finish. Its local cache is generated on that computer.
3. Run **FXR → Check project references**, open `MemoryDev`, `RoomBDev` or `RoomCDev`, and press Play. Check text, room materials, sound and mouse selection. The check command inspects all four scenes and restores the scene you were editing.
4. For Quest APKs, install this Editor's Android Build Support, SDK/NDK and OpenJDK, then use **FXR → Quest → Validate and build updated APK**. Test on the school headset afterwards. Windows may need the appropriate USB driver if ADB does not detect the device.

Each developer commits source assets with their `.meta` files. Unity may refresh shared dynamic font data during import/Play; inspect those changes and coordinate before committing shared assets. Packages/ProjectSettings changes also require coordination. Builds and each computer's caches remain ignored.

Local review evidence is in the ignored `Builds/Review/portability-*` files. The APK and Mac `.app` are generated outputs, not part of the shared source repository.

## Official references

- [Unity 2022.3.62f3 release and Windows/Mac installers](https://unity.com/releases/editor/whats-new/2022.3.62f3).
- [Unity 2022.3 system requirements](https://docs.unity3d.com/2022.3/Documentation/Manual/system-requirements.html).
- [Unity asset metadata and preserving references](https://docs.unity3d.com/2022.3/Documentation/Manual/AssetMetadata.html).
- [Unity Android SDK/NDK/JDK setup](https://docs.unity3d.com/2022.3/Documentation/Manual/android-sdksetup.html).
- [Android hardware-device setup, including Windows USB drivers](https://developer.android.com/studio/run/device).
