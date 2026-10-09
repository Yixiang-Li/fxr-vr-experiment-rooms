# Prototype status — 10 October 2026

## Implemented

- One connected-project MainScene with the Memory Room prefab and two teammate entrance slots.
- Original primitive room geometry, 12 card objects / 6 symbol pairs, collection tray physics.
- Office / nature decoration, lighting, and original synthetic 3D audio placeholders.
- Two counterbalanced rounds, deck reshuffle, countdown, mismatch count, effort rating, local anonymous CSV, and Reset.
- Desktop WASD / right-mouse look / mouse selection; shared XRI rig and optional XR simulator.
- Android ARM64 / IL2CPP / OpenXR configuration, Meta Quest support, Oculus Touch interaction profile.
- Git metadata rules, ignored caches/builds, separate room ownership folders, Chinese/English PRDs and collaboration guides.
- Separate MemoryDev / RoomBDev / RoomCDev development scenes. Owners edit ordinary room objects and press Play; the integration owner packages room prefabs for MainScene. B/C have empty room shells only. Final build settings still enable MainScene only.

## Verified

- EditMode: **2/2 passed** — scoring, ordering, duplicate/pending selection, rating gate, timeout, and state reset.
- PlayMode on 10 October: **2/2 passed** — MainScene and MemoryDev each run both rounds; environment switches, matched cards become physical objects, CSV exports, and Reset restores positions / face-down / kinematic state.
- All three development scenes passed editor checks for script/material references and a single player setup / XR Interaction Manager / EventSystem. MemoryDev was opened and entered Play in the Unity GUI, then left in edit mode with MemoryRoom expanded. MainScene, MemoryRoom.prefab, and final build-scene settings were not changed by scene creation.
- Mac development app build succeeded. Native UI checks confirmed Start, countdown, card reveal, Reset, nature preview, and WASD movement. The desktop panel now includes its mouse raycaster.
- Android development APK build succeeded: about **44 MB**, `Builds/Quest/FXRMemory.apk`. This first APK was built before the subsequent desktop-panel/title adjustment; rebuild from source before the headset session. Local builds and raw test logs are intentionally not tracked in Git.

## Still needed

- Teammates join the repository and build Room B / Room C; integrate their prefabs and connected entrances.
- Reserve the school Quest, confirm its model and installation permissions, then test tracking, controller interaction, locomotion, audio, text readability, performance, and Reset on the device.
- Confirm whether Quest joystick locomotion satisfies the teacher's touchpad gliding/flying requirement.
- Refine the room assets and sound levels if time allows. Plants / office objects and synthetic ambience are first-version placeholders; no workload or memory effect has been established.
- Complete the course's remaining requirements across the final integrated experience, names/contributions, teacher access, and demonstration video.

No headset test, teammate invitation, participant study, or final submission has been completed.
