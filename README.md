# FXR Psychological Experiment Rooms — desktop delivery

**Editor: Unity 2022.3.62f3 (96770f904ca7).** Primary delivery is a keyboard/mouse desktop application, following the group's 14 October agreement. The optional XR/Quest setup is retained. A desktop run is not evidence of a headset experiment.

Private repository: https://github.com/Yixiang-Li/fxr-vr-experiment-rooms

## Run my Memory Room

1. Clone the repository using GitHub Desktop and add the whole project folder to Unity Hub.
2. Open with **2022.3.62f3**. Wait for its first import.
3. Choose **FXR → Desktop → Play Memory Room**. This opens the editable MemoryDev scene, starts Play and focuses the Game view. Stop Play before editing.
4. Choose the environment order and sound on the panel, then click **START ROUND 1** or press **Space**.

No headset, Android modules, Blender, Python or paid Rider installation is needed for this desktop flow. Models, textures and recorded audio are included.

### Run RoomB — Photo-Taking and Visual Memory

RoomB is a separate interactive museum experiment investigating how taking photographs may influence visual memory.

To run RoomB independently:

1. Open `Assets/Rooms/RoomB/Scenes/.unity`.
2. Press **Play** in the Unity Editor.
3. Click **START EXPERIMENT**.
4. Follow the on-screen instructions to observe exhibits, complete the distraction task and answer the memory questions.

RoomB is designed for keyboard and mouse interaction and does not require a VR headset.

**RoomB controls:**

| Input | Action |
|---|---|
| WASD | Move around the museum |
| Mouse | Look around |
| E | Start observing an exhibit |
| P | Raise or lower the virtual camera |
| Left mouse | Capture a photograph when correctly framed |

RoomB currently runs as a standalone experimental scene. Integration with the shared player and MainScene still requires validation.

See [RoomB documentation](Assets/Rooms/RoomB/README.md) for the complete experiment flow and integration notes.

For the integrated project, open `Assets/Shared/Scenes/MainScene.unity` and press Play. The full three-room submission is not yet confirmed as complete.

## Controls

The following controls apply to the **Memory Room**.

| Input | Action |
|---|---|
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

## RoomB — Photo-Taking and Visual Memory

**Research question:** Does taking a photograph during observation affect subsequent visual memory compared with observation alone?

RoomB is a desktop-based 3D museum experiment with 12 exhibits and two conditions:

- **Photo condition:** 6 exhibits requiring observation and one virtual photograph.
- **Observe condition:** 6 exhibits requiring observation without photography.
- **Observation period:** 20 seconds per exhibit.
- **Distraction task:** 60 seconds of mental arithmetic after the exhibition.
- **Memory test:** Questions assessing recognition and memory for exhibit details.
- **Results:** Performance summary and CSV data export.

Participants follow navigation guidance through the exhibits. During observation, movement is temporarily disabled. On-screen instructions indicate the current task and available interactions.

The experiment is implemented using Unity scripts including `ExperimentManager`, `ExhibitInteraction` and the memory quiz system.

**Development scene:** `Assets/Rooms/RoomB/Scenes/.unity`

**Documentation:** [RoomB README](Assets/Rooms/RoomB/README.md)

RoomB is an exploratory HCI prototype. No psychological effect has been established without participant testing and analysis.

## Team workflow and contributions

Each person works in their own scene and folder. The integration owner combines the finished room roots in **MainScene**; the development scenes are retained. The teacher's 6 October email requires one project with the rooms connected through the same scene.

| Area | Owner / workflow |
|---|---|
| `Assets/Rooms/Memory/Scenes/MemoryDev.unity` | Yixiang Li: Memory Room source scene; edit its ordinary MemoryRoom root |
| `Assets/Rooms/RoomB/Scenes/.unity` | Yuming Zhai: Photo-Taking and Visual Memory experiment; standalone source scene |
| `Assets/Rooms/RoomB/Scenes/RoomBDev.unity` | Existing RoomB development/placeholder scene; not the completed experiment scene |
| `Assets/Rooms/RoomC/Scenes/RoomCDev.unity` | RoomC teammate's source scene |
| `Assets/Shared/Scenes/MainScene.unity` | Integration owner; final build starts here only |

- **Yixiang Li:** Memory Room concept, implementation, environment comparison and review, with AI coding/asset-preparation assistance.
- **Aïda** (`aidasdl`): RoomC collaborator; full name and final contributions to confirm before submission.
- **Yuming Zhai** (`zhaiyuming283-rgb`): RoomB concept, experiment design, 3D museum implementation, photo-taking interaction, observation flow, distraction task, memory test and results export.

Both teammates have accepted the repository invitations. As checked on 14 October, `aida-room` matched the pre-desktop main and had no room changes; no PR was present at that time. This does not establish whether later work has been completed.

Read [English joining/workflow guide](Docs/TEAM_SETUP_EN.md) or [中文协作说明](Docs/TEAM_SETUP_CN.md). Save/close Unity before branch changes. Commit assets with their `.meta` files, push your own branch and create a PR. Coordinate edits to shared scripts/settings/MainScene. Caches, local builds and user settings are ignored.

### RoomB integration notes

RoomB currently includes its own Player, camera, Canvas, EventSystem and experiment management components.

Before integrating RoomB into MainScene:

- Coordinate RoomB's player and camera with the shared desktop navigation setup.
- Avoid duplicate active EventSystems, cameras and AudioListeners.
- Preserve the experiment scripts, exhibits, quiz UI and required assets.
- Ensure the experiment can start and finish correctly when accessed from MainScene.
- Verify that movement, observation controls and navigation between rooms work correctly.
- Re-test the complete experimental flow after integration.

The Unity project uses **Active Input Handling: Both** to support RoomB's legacy input calls alongside the project's newer Input System.

Changes to shared project settings should be coordinated with the team.

## Build and verify

- **FXR → Desktop → Build preview for this computer** builds MainScene as `Builds/Mac/FXRPreview.app` on Mac or `Builds/Windows/FXRMemory.exe` on Windows. Builds are local and ignored by Git. Both hosts use this same source and Editor version.
- **FXR → Check project references** checks all four scenes. **Window → General → Test Runner** contains EditMode scoring tests and PlayMode full-flow, real synthetic keyboard/mouse, drag, pause, rating and UI checks for the Memory Room.
- **FXR → Desktop → Update Memory desktop setup** is an idempotent owner-only maintenance tool. The setup is already applied; daily development does not require this menu. It backs up the files and edits the existing Memory prefab in place to retain identifiers.
- Avoid the broad prototype/room regeneration menus after manual layout changes. Development scenes are working sources; the shared MainScene is an integration snapshot.

**RoomB validation:** The standalone  scene has been tested in the Unity Editor. Its observation, interaction and UI behavior should be tested again after MainScene integration. The shared Memory Room automated tests should not be assumed to cover RoomB.

Actual validation and platform limits are recorded in [STATUS](Docs/STATUS.md). Native Windows validation status should be confirmed by the team. Optional Quest instructions remain in [English](Docs/QUEST_SETUP_EN.md) / [中文](Docs/QUEST_SETUP_CN.md), with the earlier Quest build treated as historical.

## Course requirements and final submission

The supplied [course brief](Docs/Assignment/2026-FXR-MiniProject.pdf) and [marking rubric](Docs/Assignment/Project-Assessment%20Criteria%20with%20Marking%20Rubric.pdf) are included for the private group.

Memory currently demonstrates glide/fly movement, distance ray selection/dragging, runtime activation, Canvas + 3D TextMeshPro, physics, interactive lighting, spatial audio, countdown and scripted experiment logic.

RoomB adds a separate museum-based interaction experiment with exhibit observation, virtual photo-taking, navigation guidance, a distraction task, memory questions and CSV results.

The brief's literal **touch pad** input wording still needs classroom confirmation for the keyboard equivalent; the current desktop plan comes from the group conversation, not a new teacher email.

The updated submission deadline is **Wednesday, 4 November 2026, at 23:59**.

Before the deadline, the team should complete the integration of all three rooms, verify the full experimental workflows, prepare the demo video, and submit the required project materials.
Final project checklist:

1. Finish and integrate all three rooms in MainScene, verify connected navigation and each room's reset/exit behavior.
2. Confirm keyboard navigation requirements and complete a Windows first-open test.
3. Verify each room's full experimental flow and conduct basic usability testing.
4. Confirm all team members' full names and final contributions.
5. Prepare the required compressed application demo video in `Submission/`.
6. Keep the submitted source under 2 GB, arrange teacher access to the private repository and email its link to `huyen.nguyen@universite-paris-saclay.fr`.

No final submission or email delivery has been performed. A usability trial checks the prototype; it does not establish a scientific effect.
