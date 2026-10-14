# Memory Room status — 14 October 2026

## Current desktop milestone

Primary target: **keyboard/mouse desktop**, Unity **2022.3.62f3**, app **0.4.0**. The owner explicitly authorized continuing this delivery on 14 October; the group's no-Quest plan comes from the supplied group conversation. The teacher's 6 October email independently confirms one project and a common scene connecting all three rooms.

Implemented in the existing MemoryDev source and its existing MainScene prefab:

- Standard task camera on entry/start/reset; F task view, WASD/arrows and right drag, G glide/fly, Q/E vertical flight; layout-aware keyboard hints.
- Help and pause overlay; task movement locked in both rounds/rating/transition; explicit pause, window focus loss and Reset freeze timing and pending pair resolution.
- Mouse-ray practice cube with rigidbody release, available only outside the measured task; same 12-card task with physical matching tray.
- Background accent light controlled by condition, with task illumination unchanged.
- Clear desktop instructions, New Session/Reset confirmation, automatic CSV with input mode/version, and a results-folder button.
- One-click **FXR → Desktop → Play Memory Room** and host-specific preview build menus. New source/scene IDs are checked, and the original room geometry is retained.

Current automated validation: EditMode **2/2**, PlayMode **5/5** (full flows and synthetic keyboard/mouse/drag/pause/rating/UI), zero missing references in four scenes. Mac **0.4.0** standalone build succeeded. After the owner unlocked the Mac, native mouse/keyboard checks completed both six-pair rounds through MainScene, first rating by keyboard, second by mouse, condition change, results and automatic two-row CSV. Help/Back, G/E flight, F task view, Esc pause, Reset/New Session confirmation/cancel/confirm and the Finder results-folder button worked. The paused timer stayed at **02:53** through confirmation/cancel; saved CSV survived confirmed Reset and contains `keyboard_mouse` / `0.4.0`. Checked runtime log has no exception lines. CUA drag attempts did not visibly move the practice cube, so native drag is **not confirmed**; its MainScene synthetic drag test passed and a real-user drag trial remains. Test CSV was moved to ignored local review evidence, with earlier result files retained. Editor synthetic input and camera renders are verified. Source audit: **99.99 MiB**, all **510** existing asset GUIDs and all existing scene/prefab object IDs retained; no teammate-folder or package changes. Local test XML/logs and native review evidence are in `Logs/desktop-*` and `Builds/Review/desktop-native-review.json`; these are ignored by Git.

## Remaining work

- Native Windows first-open/run check by a Windows teammate; usable headphones/volume check and 1–2 independent usability trials, including the practice cube's mouse drag.
- Classroom clarification of literal touch-pad navigation versus the keyboard equivalent; retained XR simulation can support a demonstration if requested.
- Teammates' rooms, final connected navigation/Reset, full names/actual contributions, complete demo video, teacher repository access and final email. Deadline: **22 October before midnight**.

The course prototype does not establish an environmental psychological effect. No participant study, Windows native run or headset test has been completed.

## Historical baseline — 10 October 2026

## Windows / macOS review

The same GitHub source project supports both development hosts with Unity **2022.3.62f3**. A fresh GitHub clone imported/compiled on Mac with zero missing references across all four scenes; its existing PlayMode integration/UX tests passed **3/3**. Repository checks found no Mac-only project plugin/path dependency or Windows-invalid filenames. Quest ADB discovery now supports Windows Hub/SDK paths, with an optional batch installer and bilingual instructions. See [the review](CROSS_PLATFORM_REVIEW.md) for evidence and scope. Native Windows Editor/graphics/batch/USB and actual Quest hardware still need testing.

## Migration status

Target: **Unity 2022.3.62f3**, following the classroom requirement relayed by a teammate in the user-provided WhatsApp screenshot. This is not independent confirmation from the teacher.

Migration to the exact course Editor is complete. Rosetta 2 and this Editor's Android SDK/NDK/OpenJDK modules are installed following the owner's explicit license authorization. The working path remains `/Users/yixiangli/Unity/FXR-Group-Project`; Unity 6 can be recovered from the rollback tag `unity6-before-2022-migration` and local Git bundle.

## Verified — Unity 2022.3.62f3

- Native import and compilation passed; all 509 existing asset GUIDs and scene/prefab object IDs retained. All four scenes pass references checks, including the four real ambience clips and fonts/materials.
- EditMode **2/2** passed at 19:31 UTC; final PlayMode **3/3** passed at 19:39 UTC on 10 October. Both working scenes complete the experiment; UX coverage includes mouse targeting, state transitions, reset pause/cancel, timeout, rating gates, physical cards, CSV and text overflow.
- Actual office/garden and participant UI camera renders inspected. Final Mac development build launched with working text and no exceptions in its checked startup log. Automated mouse input passed in Editor; native foreground input and Quest controls need a person.
- Quest development APK succeeded: **72,937,816 bytes**, version **0.3.0 / code 3**, package `fr.fxr.memoryrooms`, ARM64 only, min SDK 29 / target SDK 36, classic `UnityPlayerActivity`. OpenXR validation has **0 errors and 2 optional warnings**. The APK is local in `Builds/Quest/FXRMemory.apk`; builds are ignored by Git.
- Office/nature geometry counts are unchanged. MainScene is the only final build scene; all three development scenes are retained.
- Audio and TMP material serialization differences found during migration were repaired; the final runs above followed those repairs. See [migration notes](UNITY_2022_MIGRATION.md).
- Current ADB status: no connected device. **No headset test, participant study or final submission has been completed.**

## Implemented

- User-owned MemoryDev source scene, integrated MemoryRoom prefab in the existing MainScene. Room B / Room C and their development scenes remain teammate-owned empty shells. Final build includes MainScene only.
- Office: Poly Haven CC0 metal desks, upholstered chairs, desk lamps and clock; original monitors/charts, keyboard, phone, files, storage, blinds and acoustic panels; a photographed city window.
- Indoor garden: three types of Poly Haven CC0 plants with photographed leaf/planter textures; original oak slats, river stones, water feature and bench; a photographed forest window. Shared floor, task console, card layout and neutral task light remain consistent across conditions.
- Photographic wood/tile/plaster PBR materials, environment fill lighting and material-based mesh grouping that preserves source submeshes. Android textures use 1K ASTC; long audio loops use Vorbis streaming.
- Actual CC0 office/keyboard/forest bird/stream recordings from public Freesound HQ MP3 previews, edited for looping and matched source RMS, with spatial detail and fades. Short neutral task cues remain original. Source matching is not measured perceived headset loudness. Credits and adaptations are in ATTRIBUTIONS.md.
- 12 cards / 6 symbol pairs; two counterbalanced rounds; reshuffle, countdown, progress, mismatch count, 1–5 effort rating and local anonymous CSV.
- Settings locked during sessions; scores withheld until both ratings; reset confirmation pauses the task; duplicate ratings cannot create duplicate CSV files; interrupted rounds are flagged. Matched cards settle gently into the tray.
- Desktop mouse UI and shared XRI controller UI selected automatically. Quest uses the shared XR rig; editor XR simulator remains optional.
- Android ARM64 / IL2CPP / Vulkan / OpenXR / Meta Quest / Oculus Touch configuration, UnityPlayerActivity, lightweight URP settings, automated validation/build and USB install/collection helpers. No extra Meta SDK or Rider download is needed for this implementation.
- Chinese/English PRDs, Quest guides and teammate collaboration instructions. Git ignores caches, builds, raw logs and local review images.

## Verified baseline — Unity 6000.3.24f1 (before migration)

- PlayMode on 10 October, 18:48 UTC: **3/3 passed**. MainScene and MemoryDev each complete both rounds, environment changes, physical cards, CSV and Reset. The UX test also covers synthetic mouse actions, first UI ray hits, preview/order behavior, settings locks, reset pause/cancel, interruption flags, rating gates, settled cards, timeout and active text overflow. After the final clock facing correction, the complete UI/visual test passed again (1/1, 18:56 UTC).
- Actual desktop camera renders of office, garden, running, ratings, between-rounds, results and reset confirmation inspected in local `Builds/Review/`. Test scores/times in these images are fixtures, not research evidence.
- Final Mac development app build succeeded. Native app visuals were inspected; background-window automation did not reliably trigger mouse clicks, so that native click attempt is not counted as a passed check. The automated mouse input path passed above; foreground user input and headset controller input still need their respective checks. The updated real-asset Mac app also launched and its live office view was inspected; it was closed after review.
- OpenXR validation: **0 errors, 2 optional warnings** about future PoseControl / StickControl migration. Pinned package versions remain unchanged.
- Final updated Quest development APK build succeeded: **90,521,993 bytes** (about 91 MB), `Builds/Quest/FXRMemory.apk`, package `fr.fxr.memoryrooms`, version 0.2.0 / code 2. APK manifest confirms ARM64, target SDK 36 and Unity GameActivity. This build includes the final source changes. Installation is pending; compiler debug symbols are separate from the APK.
- Geometry count: office 85 active mesh renderers / 46,796 triangles; nature 81 / 441,269. These counts are not measured Quest frame rates.
- EditMode session tests previously passed **2/2**; the pure session engine was unchanged in this round.
- Two teammate collaborator invitations were sent earlier on 10 October; acceptance status should be checked in GitHub.

## Still needed

- A connected, authorized school Quest. Current ADB check has no devices. Reserve the headset and confirm its model and school installation permission. USB debugging authorization and wearing the headset require an on-site person.
- Device checks: tracking, controller ray/trigger, movement comfort, positioning, text size, audio loudness/spatialization, frame rate, reset and system-menu/resume behavior. See the Quest guides for the short install workflow.
- Confirm whether Quest joystick locomotion satisfies the teacher's touchpad gliding/flying requirement.
- Teammates build and integrate their rooms; complete remaining course requirements, names/contributions, teacher access and demonstration video.
- Pilot the experimental instructions and rating scale before collecting participant data. The combined environment manipulation has not established a workload or memory effect.

No headset installation/test, participant study or final submission has been completed.
