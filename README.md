# FXR Psychological Experiment Rooms — desktop delivery

**Editor: Unity 2022.3.62f3 (96770f904ca7).** Primary delivery is a keyboard/mouse desktop application, following the group's 14 October agreement. The optional XR/Quest setup is retained. A desktop run is not evidence of a headset experiment.

Private repository: https://github.com/Yixiang-Li/fxr-vr-experiment-rooms


## Three-room integration preview — 1 November 2026

Branch: **`integration/three-rooms-2026-11-01`**. The owners' `aida-room`, `yuming-room2` and `main` branches are preserved. This preview is reviewed separately before any merge to main.

Open this branch with **Unity 2022.3.62f3**, then use **FXR → Integration → Play three rooms**. The single final scene is still `Assets/Shared/Scenes/MainScene.unity`.

- **MEMORY — Yixiang:** office/nature card-matching experiment.
- **MUSEUM — Yuming:** 12 exhibits, Photo/Observe conditions, distraction task, memory quiz and CSV.
- **STROOP — Aïda:** consent paper, desk controls, ink-colour responses, accuracy and reaction time. After accepting and waiting for the paper to close, use **START TASK** in the guide or the desk Start button.

Use the top room buttons to switch. **Tab** opens the room menu and pauses the current task; this also releases the Museum cursor. **RESTART** restores the whole selected room, including consent, controls and objects. Switching or restarting asks for confirmation and clears the current room's progress. Existing CSV files remain. Memory interruptions and Museum interruptions are recorded; interrupted sessions should be repeated before comparing performance.

MainScene uses one shared desktop player, one on-screen camera, one AudioListener and one EventSystem. Stroop retains a separate off-screen camera for its monitor texture. Only the selected room runs; room snapshots are instantiated within MainScene, with no separate scene launch.

The final course timetable in Ch5 p.2, downloaded on 21 October, gives **4 November 2026 before midnight** for the project, replacing the original brief's 22 October date. Later announcements have not been rechecked in this integration pass.

See [integration review and remaining checks](Docs/INTEGRATION_REVIEW_CN.md) and [English team workflow](Docs/TEAM_SETUP_EN.md). Automated checks use accelerated test timings and generated responses; they are not participant results.

## Run my Memory Room

1. Clone the repository using GitHub Desktop and add the whole project folder to Unity Hub.
2. Open with **2022.3.62f3**. Wait for its first import.
3. Choose **FXR → Desktop → Play Memory Room**. This opens the editable MemoryDev scene, starts Play and focuses the Game view. Stop Play before editing.
4. Choose the environment order and sound on the panel, then click **START ROUND 1** or press **Space**.

No headset, Android modules, Blender, Python or paid Rider installation is needed for this desktop flow. Models, textures and recorded audio are included.

For the integrated project, open `Assets/Shared/Scenes/MainScene.unity` and press Play. This preview connects all three rooms. The earlier RoomBDev/RoomCDev shells are retained as historical starter files.

## Controls

| Input | Action |
| --- | --- |
| WASD / arrow keys + hold right mouse | Explore before or after a session |
| G | Switch glide/fly navigation before or after a session |
| Q / E | Move down/up while flying |
| F | Return to the standard task view |
| Left mouse | Select cards/buttons; drag the orange practice cube before starting |
| Space | Start round 1 or continue to round 2 when ready |
| 1–5 | Submit mental-effort rating when prompted |
| Esc | Pause/resume a running round, cancel Reset or close Help |
| H / HELP | Read the guide outside the measured task |
| CSV FOLDER | Open saved results after completing both rounds |

Starting either round automatically restores the same task camera. Movement, looking around and practice dragging are locked during both rounds and their rating/transition screens. Pause, Reset confirmation and window focus loss stop the timer and pending pair resolution. Interrupted rounds are flagged in CSV and should be repeated before comparing the conditions.

Letter bindings use physical keyboard positions. The on-screen guide adapts their names to the current layout (for example AZERTY); arrow keys provide an alternative.

## Memory Room and research question

**Question:** In a desktop 3D environment, do office and nature-inspired settings change memory-task performance or reported mental effort?

- Same 12 cards, six symbol pairs and 180-second limit in each condition. The arrangement is reshuffled for each round.
- Choose Office → Nature or Nature → Office before starting. Preview does not change the selected round order.
- Free CC0 furniture/plants from Poly Haven, photographic surfaces and window views, plus original architecture and props.
- Real CC0 office/keyboard and forest/stream recordings from Freesound, with spatial detail and fades. Use the same headphones and comfortable system volume in both rounds.
- A background accent light changes color/intensity with the setting. Task light, card faces, task camera and rules stay standardized.
- A separate draggable practice cube demonstrates ray-based distance manipulation; matched cards drop into a physical tray.
- Rate mental effort 1–5 after each round. Both results remain hidden until both ratings are collected.
- Reset restores cards, timer, ratings, practice object and task view. It returns to the selected first condition and keeps the chosen order/sound; previously saved CSV files remain. New Session also asks for confirmation.

CSV saves automatically under `Application.persistentDataPath/MemoryResults`. It contains condition, completion/timeout, elapsed time, pairs, flips, attempts, mismatches, effort, input mode, app version, sound and interruption flag. Do not commit participant results. This is an exploratory course prototype; no participant study or psychological effect has been established.

Inspiration: Orti et al. (2026), [Environmental Modulation of Perceived Cognitive Workload During a Memory Task: Evidence from Immersive Virtual Reality](https://doi.org/10.3390/jintelligence14080158) ([PubMed record](https://pubmed.ncbi.nlm.nih.gov/42646006/)). I adapt its environment/memory-task comparison to desktop interaction and one exploratory effort question. This is not a replication of its immersive setup or workload instrument.

See [English room PRD](Docs/Memory_Room_Mini_PRD_EN.md), [中文 PRD](Docs/Memory_Room_Mini_PRD_CN.md), [asset credits](Docs/ATTRIBUTIONS.md) and [review / requirements checklist](Docs/ROOM_UIUX_REVIEW_CN.md).

## Team workflow and contributions

Each person works in their own scene and folder. The integration owner combines the finished room roots in **MainScene**; the development scenes are retained. The teacher's 6 October email requires one project with the rooms connected through the same scene.

| Area | Owner / workflow |
| --- | --- |
| `Assets/Rooms/Memory/Scenes/MemoryDev.unity` | Yixiang Li: Memory Room source scene; edit its ordinary MemoryRoom root |
| `Assets/Rooms/RoomB/Scenes/RoomB.unity` | Yuming: Museum source scene |
| `Assets/Rooms/AidaRoom/Scenes/AidaExperiment.unity` | Aïda: Stroop source scene |
| `Assets/Shared/Scenes/MainScene.unity` | Integration owner; final build starts here only |

- **Yixiang Li:** room concept, implementation, environment comparison and review, with AI coding/asset-preparation assistance.
- **Aïda** (`aidasdl`): Stroop room, furniture/lighting, consent and physical controls; current display name retained for this preview.
- **Yuming** (`zhaiyuming283-rgb`): Museum, exhibit Photo/Observe flow, distraction task, memory quiz and CSV; current display name retained for this preview.

Both teammates have accepted the repository invitations. This integration includes Yuming’s `d562169` (24 October, PR #1) and Aïda’s `1b23eed` (29 October). Their original branches have not been rewritten.  Stroop desk interactions now use Input System coordinates consistently with the shared UI, fixing standalone Mac pointer offsets. Integration fixes and snapshots are confined to this preview branch; GitHub mergeability alone does not prove Unity compatibility.

Read [English joining/workflow guide](Docs/TEAM_SETUP_EN.md) or [中文协作说明](Docs/TEAM_SETUP_CN.md). Save/close Unity before branch changes. Commit assets with their `.meta` files, push your own branch and create a PR. Coordinate edits to shared scripts/settings/MainScene. Caches, local builds and user settings are ignored.

## Build and verify

- **FXR → Desktop → Build preview for this computer** builds MainScene as `Builds/Mac/FXRPreview.app` on Mac or `Builds/Windows/FXRMemory.exe` on Windows. Builds are local and ignored by Git. Both hosts use this same source and Editor version.
- **FXR → Check project references** checks all four scenes. **Window → General → Test Runner** contains EditMode scoring tests and PlayMode full-flow, real synthetic keyboard/mouse, drag, pause, rating and UI checks.
- **FXR → Desktop → Update Memory desktop setup** is an idempotent owner-only maintenance tool. The setup is already applied; daily development does not require this menu. It backs up the files and edits the existing Memory prefab in place to retain identifiers.
- Avoid the broad prototype/room regeneration menus after manual layout changes. Development scenes are working sources; the shared MainScene is an integration snapshot.

Actual validation and platform limits are recorded in [STATUS](Docs/STATUS.md). Native Windows has not yet been tested; a Windows teammate should perform one first-open check. Optional Quest instructions remain in [English](Docs/QUEST_SETUP_EN.md) / [中文](Docs/QUEST_SETUP_CN.md), with the earlier Quest build treated as historical.

## Course requirements and final submission

The supplied [course brief](Docs/Assignment/2026-FXR-MiniProject.pdf) and [marking rubric](Docs/Assignment/Project-Assessment%20Criteria%20with%20Marking%20Rubric.pdf) are included for the private group.

Memory currently demonstrates glide/fly movement, distance ray selection/dragging, runtime activation, Canvas + 3D TextMeshPro, physics, interactive lighting, spatial audio, countdown and scripted experiment logic. The brief's literal **touch pad** input wording still needs classroom confirmation for the keyboard equivalent; the current desktop plan comes from the group conversation, not a new teacher email.

Before **midnight 4 November 2026**, as stated in the Ch5 timetable checked on 21 October:

1. Review the integration branch with both room owners and check connected navigation and each room's Restart on the delivery computer.
2. Confirm the keyboard navigation interpretation and complete one Windows first-open check; try the Memory flow with 1–2 people for usability.
3. Fill the full member names and actual contributions above; provide a compressed application demo video in `Submission/`.
4. Keep the submitted source under 2 GB, arrange teacher access to this private repository and email its link to `huyen.nguyen@universite-paris-saclay.fr`.

No final submission or email delivery has been performed. A usability trial checks the prototype; it does not establish a scientific effect.
