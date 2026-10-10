# Course Editor migration — 10 October 2026

## Target and current state

The course version is **2022.3.62f3**, as relayed by the teammate's classroom report in the screenshot. The exact official release is available in [Unity's archive](https://unity.com/releases/editor/whats-new/2022.3.62f3).

The target Apple Silicon Editor has been downloaded and installed at `/Applications/Unity/Hub/Editor/2022.3.62f3/Unity.app`.

| Item | Status |
| --- | --- |
| Unity 6 rollback | Tag `unity6-before-2022-migration` at `b4c9ac3` and local Git bundle retained |
| Isolated migration | `/Users/yixiangli/Unity/FXR-Unity2022-Migration`, branch `migration/unity-2022-3-62f3` |
| Source adaptations | Completed; all 509 existing metadata GUIDs and scene/prefab object IDs retained |
| Rosetta 2 | Installed successfully after the owner explicitly accepted the license |
| Android modules | Installed successfully after license approval: Android player, SDK/NDK tools and OpenJDK 11 |
| Unity 2022 compile / tests / renders / builds | Passed: EditMode 2/2, PlayMode 3/3, actual renders, Mac player and ARM64 APK |
| Working project | Course version 2022.3.62f3; daily work stays in `FXR-Group-Project` |

The 2022 Editor's `UnityPackageManager` executable is Intel-only. Its initial `bad CPU type in executable` startup failure was resolved by installing Rosetta 2 with the owner's explicit authorization. Apple documents [Rosetta support for Intel apps](https://support.apple.com/en-gb/102527). Android component licenses were also explicitly accepted by the owner.

## Compatibility changes prepared

- URP **17.3.0 → 14.0.12**, the core package bundled with the target Editor.
- Input System **1.20.0 → 1.14.2**.
- XR Interaction Toolkit **3.3.2 → 3.3.1**, with matching official samples and preserved asset GUIDs.
- Unity UI **2.0.0 → 1.0.0**, plus separate TextMesh Pro **3.0.7**.
- Test Framework **1.6.0 → 1.4.6**.
- OpenXR **1.16.1** and XR Management **4.6.1** retained; their package metadata supports Unity 2022.3.
- Physics API and serialized damping fields translated to Unity 2022 names; TMP wrapping API updated.
- AudioSource clip references translated from Unity 6 `m_Resource` to Unity 2022 `m_audioClip`. All four recorded ambience clips are retained; the initial PlayMode run detected and prompted this repair.
- TMP font material references translated to TMP 3 native fields and reserialized with embedded material IDs retained. The initial Mac player launch exposed blank text and null references even though Editor tests had passed; the standalone player is therefore checked separately.
- Quest build uses UnityPlayerActivity and version 0.3.0 / code 3.
- One-time migration utility prepares native URP 14 settings and reserializes existing room assets. It does not regenerate room layouts.

## Verification completed

- Exact Editor `2022.3.62f3 (96770f904ca7)` imports and compiles the project with its native URP 14 settings and a resolved package lock.
- All four scenes: zero missing scripts, mesh references, invalid materials, ambience clips or font/material references. TMP's runtime-generated meshes are excluded from missing-mesh checks. The CC0 lamp's two unused empty material groups already existed before migration and are reported separately.
- EditMode **2/2** passed at 19:31 UTC; final PlayMode **3/3** passed at 19:39 UTC on 10 October. MainScene and MemoryDev complete both rounds, ratings, physics, reset and CSV; the UX test checks mouse routing, state locks, timeout and text overflow.
- Office/garden interiors and participant UI camera renders inspected. The final Mac development player built and launched, displayed text correctly after the Quest build's dynamic-font cache clearing step, and produced no runtime exceptions in its checked startup log. Native foreground mouse/controller input still needs a person; automated desktop input passed in PlayMode.
- Quest development APK built successfully: **72,937,816 bytes** (about 73 MB), package `fr.fxr.memoryrooms`, version **0.3.0 / code 3**, ARM64 only, min SDK 29 / target SDK 36, `UnityPlayerActivity`. OpenXR validation: **0 errors, 2 optional future input-control warnings**.
- Final build contains **MainScene only**; all three development scenes remain editable source files. Geometry counts remain unchanged: office 46,796 triangles; nature 441,269. These are not measured frame rates.
- Local build/review evidence lives in the ignored `Builds/` and `Logs/` folders; the source repository includes the documentation and pinned packages.

## Teammates updating

Save and close Unity, commit any local work on your branch, fetch/pull `main` in GitHub Desktop, then open using **2022.3.62f3**. Allow the first import to finish and open your existing development scene. Coordinate before merging Unity 6 branches that contain package/settings changes. See the English or Chinese team guide.

Quest headset testing remains a separate step requiring a connected, authorized school device. A successful desktop test or APK build is not a headset test. No participant study or final submission has been completed.
