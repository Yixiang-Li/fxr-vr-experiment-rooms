# Course Editor migration — 10 October 2026

## Target and current state

The course version is **2022.3.62f3**, as relayed by the teammate's classroom report in the screenshot. The exact official release is available in [Unity's archive](https://unity.com/releases/editor/whats-new/2022.3.62f3).

The target Apple Silicon Editor has been downloaded and installed at `/Applications/Unity/Hub/Editor/2022.3.62f3/Unity.app`.

| Item | Status |
| --- | --- |
| Unity 6 rollback | Original project retained; tag `unity6-before-2022-migration` and local Git bundle saved |
| Isolated migration | `/Users/yixiangli/Unity/FXR-Unity2022-Migration`, branch `migration/unity-2022-3-62f3` |
| Source adaptations | Prepared; existing metadata GUIDs retained |
| Rosetta 2 | Not installed; user license approval pending |
| Android modules | Not installed for this Editor; user license approval pending |
| Unity 2022 compile / tests / renders / builds | Not verified yet |
| Shared main | Unity 6 baseline; migration not published |

The 2022 Editor's `UnityPackageManager` executable is Intel-only. It currently fails with `bad CPU type in executable`, preventing package resolution and project import. Apple documents [Rosetta support for Intel apps](https://support.apple.com/en-gb/102527). The Editor install itself succeeded. This startup failure is not a memory-task test result.

## Compatibility changes prepared

- URP **17.3.0 → 14.0.12**, the core package bundled with the target Editor.
- Input System **1.20.0 → 1.14.2**.
- XR Interaction Toolkit **3.3.2 → 3.3.1**, with matching official samples and preserved asset GUIDs.
- Unity UI **2.0.0 → 1.0.0**, plus separate TextMesh Pro **3.0.7**.
- Test Framework **1.6.0 → 1.4.6**.
- OpenXR **1.16.1** and XR Management **4.6.1** retained; their package metadata supports Unity 2022.3.
- Physics API and serialized damping fields translated to Unity 2022 names; TMP wrapping API updated.
- Quest build uses UnityPlayerActivity and version 0.3.0 / code 3.
- One-time migration utility prepares native URP 14 settings and reserializes existing room assets. It does not regenerate room layouts.

## Verification before publication

1. Install Rosetta and Android build modules after license approval.
2. Import in the exact Editor; resolve package and compile errors and generate a new package lock.
3. Prepare URP 14 settings; validate MainScene and all three development scenes for missing scripts, meshes and materials.
4. Run existing EditMode / PlayMode tests and inspect office, garden and participant UI renders.
5. Build the Mac preview and a new ARM64 Quest APK; record actual results.
6. Publish the tested migration to shared main and remove this pending notice. Teammates close Unity, pull main, and open using 2022.3.62f3.

Quest headset testing remains a separate step requiring a connected, authorized school device. A successful desktop test or APK build will not be reported as a headset test.
