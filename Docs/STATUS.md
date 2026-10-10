# Memory Room status — 10 October 2026

## Implemented

- User-owned MemoryDev source scene, integrated MemoryRoom prefab in the existing MainScene. Room B / Room C and their development scenes remain teammate-owned empty shells. Final build includes MainScene only.
- Office: Poly Haven CC0 metal desks, upholstered chairs, desk lamps and clock; original monitors/charts, keyboard, phone, files, storage, blinds and acoustic panels; a photographed city window.
- Indoor garden: three types of Poly Haven CC0 plants with photographed leaf/planter textures; original oak slats, river stones, water feature and bench; a photographed forest window. Shared floor, task console, card layout and neutral task light remain consistent across conditions.
- Photographic wood/tile/plaster PBR materials, environment fill lighting and material-based mesh grouping that preserves source submeshes. Android textures use 1K ASTC; long audio loops use Vorbis streaming.
- Actual CC0 office/keyboard/forest bird/stream recordings from public Freesound HQ MP3 previews, edited for looping and matched source RMS, with spatial detail and fades. Short neutral task cues remain original. Source matching is not measured perceived headset loudness. Credits and adaptations are in ATTRIBUTIONS.md.
- 12 cards / 6 symbol pairs; two counterbalanced rounds; reshuffle, countdown, progress, mismatch count, 1–5 effort rating and local anonymous CSV.
- Settings locked during sessions; scores withheld until both ratings; reset confirmation pauses the task; duplicate ratings cannot create duplicate CSV files; interrupted rounds are flagged. Matched cards settle gently into the tray.
- Desktop mouse UI and shared XRI controller UI selected automatically. Quest uses the shared XR rig; editor XR simulator remains optional.
- Android ARM64 / IL2CPP / Vulkan / OpenXR / Meta Quest / Oculus Touch configuration, GameActivity, lightweight URP settings, automated validation/build and USB install/collection helpers. No extra Meta SDK or Rider download is needed for this implementation.
- Chinese/English PRDs, Quest guides and teammate collaboration instructions. Git ignores caches, builds, raw logs and local review images.

## Verified this round

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
