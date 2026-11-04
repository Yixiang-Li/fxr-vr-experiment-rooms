# FXR Psychological Experiment Rooms — desktop delivery

**Editor: Unity 2022.3.62f3 (96770f904ca7).** Primary delivery is a keyboard/mouse desktop application, following the group's 14 October agreement. The optional XR/Quest setup is retained. A desktop run is not evidence of a headset experiment.

Private repository: https://github.com/Yixiang-Li/fxr-vr-experiment-rooms

## Three-room integration preview

Branch: **`integration/three-rooms-2026-11-01`**. The owners' `aida-room`, `yuming-room2` and `main` branches are preserved. This preview is reviewed separately before any merge to main.

Open this branch with **Unity 2022.3.62f3**, then use **FXR → Integration → Play three rooms**. The final integrated scene is `Assets/Shared/Scenes/MainScene.unity`.

- **MEMORY — Yixiang:** office/nature card-matching experiment.
- **MUSEUM — Yuming:** 12 exhibits, Photo/Observe conditions, distraction task, memory quiz and CSV export.
- **STROOP — Aïda:** the latest two-block task: 30 seconds without phone notifications, a 3-second transition, then 30 seconds with notifications. Consent, researcher voiceovers, desk controls, paper physics and per-block accuracy/reaction time are retained. Use **START TASK** or the desk Start button after consent closes.

Use the top room buttons to switch. **Tab** opens the room menu and pauses the current task; this also releases the Museum cursor. **RESTART** restores the whole selected room, including consent, controls and objects. Switching or restarting asks for confirmation and clears the current room's progress. Existing CSV files remain. Memory interruptions and Museum interruptions are recorded; interrupted sessions should be repeated before comparing performance.

MainScene uses one shared desktop player, one on-screen camera, one AudioListener and one EventSystem. Stroop retains a separate off-screen camera for its monitor texture. Only the selected room runs; room snapshots are instantiated within MainScene, with no separate scene launch.

The final course timetable in Ch5 p.2, downloaded on 21 October, gives **4 November 2026 before midnight** for the project, replacing the original brief's 22 October date. Later announcements have not been rechecked in this integration pass.

See [integration review and remaining checks](Docs/INTEGRATION_REVIEW_CN.md) and [English team workflow](Docs/TEAM_SETUP_EN.md). Automated checks use accelerated test timings and generated responses; they are not participant results.

## Run the Memory Room

1. Clone the repository using GitHub Desktop and add the whole project folder to Unity Hub.
2. Open with **2022.3.62f3**. Wait for its first import.
3. Choose **FXR → Desktop → Play Memory Room**. This opens the editable MemoryDev scene, starts Play and focuses the Game view. Stop Play before editing.
4. Choose the environment order and sound on the panel, then click **START ROUND 1** or press **Space**.

No headset, Android modules, Blender, Python or paid Rider installation is needed for this desktop flow. Models, textures and recorded audio are included.

For the integrated project, open `Assets/Shared/Scenes/MainScene.unity` and press Play. This preview connects all three rooms. The earlier RoomBDev/RoomCDev shells are retained as historical starter files.

## Memory Room controls

| Input | Action |
| --- | --- |
| WASD / arrow keys | Move before or after a session |
| Hold right mouse and move the mouse | Look around before or after a session |
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

## Museum Room — Yuming ZHAI

### Research question

Does taking a photograph, compared with observing only, affect later memory accuracy and response time for virtual museum exhibits?

### Run the room

1. Choose **FXR → Integration → Play three rooms**.
2. Select **MUSEUM** in the top navigation bar and confirm.
3. Click **START EXPERIMENT**.
4. Follow the guide arrow and the on-screen instructions.

The standalone source scene is:

`Assets/Rooms/RoomB/Scenes/RoomB.unity`

### Museum controls

| Input | Action |
| --- | --- |
| WASD / arrow keys | Walk between exhibits outside observation periods |
| Mouse movement | Look around while the cursor is locked |
| Right mouse button | Resume mouse look after releasing the cursor |
| E | Start observing the current exhibit when nearby and prompted |
| P | Raise or lower the camera during a Photo trial |
| Left mouse button | Take a photo with the camera raised and the exhibit centered; select UI answers |
| E at the quiz station | Start the distraction task after completing all exhibits |
| Enter / numpad Enter | Submit an arithmetic answer during the distraction task |
| Esc | Release the Museum mouse cursor |
| Tab | Open the shared room menu in the integrated version |

### Experiment procedure

1. Visit **12 exhibits** in the indicated order.
2. Conditions are balanced between **six Photo trials and six Observe trials**.
3. Each exhibit has a **20-second observation period**.
4. Walking is disabled during observation. Mouse look remains available within the configured horizontal and vertical angle limits.
5. In a **Photo trial**, press **P** to raise the camera. Center the exhibit until the viewfinder turns green, then **left-click** to capture one photo.
6. If the countdown ends before a valid photo is taken, a valid capture is still required to finish that Photo trial.
7. In an **Observe trial**, observe the exhibit without taking a photograph.
8. After all exhibits, approach the quiz station and press **E**.
9. Complete the **60-second arithmetic distraction task**, followed by the memory quiz.

### Results and CSV export

Quiz results are exported automatically to:

`Application.persistentDataPath/MuseumResults`

The CSV records:

- Session ID and question order.
- Exhibit number and Photo/Observe condition.
- Question type.
- Selected answer and correct answer.
- Correctness and response time.
- Interruption flag.

Previously saved CSV files are retained when restarting. Do not commit participant results to the repository.

This is an exploratory course prototype. No participant study or psychological effect is claimed.

### Implementation and room layout

- Keyboard and mouse interactions use Unity's **Input System**.
- Exhibit guidance and on-screen prompts indicate the next available interaction.
- Photo trials include a camera viewfinder, framing validation and capture feedback.
- The experiment includes observation timing, an arithmetic distraction task, a memory quiz and CSV export.
- The room includes a closed decorative entrance door, a ceiling and two indoor point lights.
- The entrance door has no opening interaction.

### Source scene and integration

| Content | Location |
| --- | --- |
| Museum gameplay scripts | `Assets/Rooms/RoomB/Scripts` |
| Standalone source scene | `Assets/Rooms/RoomB/Scenes/RoomB.unity` |
| Integrated room prefab | `Assets/Integration/Prefabs/MuseumRoom.prefab` |
| Final integrated scene | `Assets/Shared/Scenes/MainScene.unity` |

Scripts are shared between the standalone and integrated versions. Do not duplicate the script files into the Integration folder.

Object, light and UI changes in the standalone RoomB scene do not automatically update the integrated prefab. Apply the corresponding changes to MuseumRoom when updating the integrated version.

Integrated environment-light settings are stored in MainScene under:

**Three room navigation → Rooms → MUSEUM → View**

Stop Play mode before editing and saving scenes or prefabs. Do not use **Rebuild generated prototype** or other regeneration commands to synchronize a manually edited room.

## Stroop Room — Aïda Sadul 

**Developer:** Aïda Sadul  
**Tools:** Unity 2022.3.62f3, Blender, C#  
**Development scene:** `Assets/Rooms/AidaRoom/Scenes/AidaExperiment.unity`  
**Final integration:** `Assets/Shared/Scenes/MainScene.unity`

### 1. Overview and Research Motivation

Stroop room is an interactive virtual psychological laboratory designed to explore how smartphone notifications may affect cognitive attention.

Participants perform a **Stroop task**, identifying the ink color of displayed words rather than their meaning, under two conditions: without notifications and with visual and auditory smartphone notifications.

**Research question:** *How do smartphone notifications influence reaction time and response accuracy during an attention-demanding task?*

The project combines cognitive psychology with interactive 3D development, demonstrating how a research-informed experiment can be implemented within a virtual environment.

### 2. Scientific Background and Design Rationale

The experimental design was informed by five academic studies:

| Research | Scientific contribution | Application to Room C |
|---|---|---|
| **Stroop (1935)** | Demonstrated cognitive interference between word meaning and ink color | Stroop color-identification task |
| **Stothart et al. (2015)** | Notifications can disrupt attention without direct phone interaction | Notifications appear without requiring phone interaction |
| **Ward et al. (2017)** | Smartphone presence may reduce available cognitive capacity | Smartphone remains visible in both conditions |
| **Upshaw et al. (2022)** | Notification sounds may slow cognitive responses | Auditory distractions and reaction-time measurement |
| **Fournier et al. (2026)** | Investigated notification-related disruption during a Stroop task | Main methodological inspiration for the experiment |

These studies motivated two hypotheses:

- **H1:** Notifications may increase reaction time.
- **H2:** Notifications may decrease response accuracy.

The hypotheses are research-informed predictions, not confirmed findings of this prototype.

### 3. Experimental Procedure

Participants complete two consecutive Stroop blocks:

| Phase | Duration | Task |
|---|---|---|
| Block 1 | 30 seconds | Stroop task without notifications |
| Transition | 3 seconds | Transition between conditions |
| Block 2 | 30 seconds | Stroop task with visual and auditory notifications |

Participants select one of three physical-style buttons (red, green, or blue) corresponding to the displayed **ink color**, not the written word.

The system evaluates response correctness and measures reaction time from stimulus presentation to response. A performance summary is presented upon completion.

The smartphone remains present in both blocks to maintain a consistent environment.

### 4. Implementation and Design Decisions

| Feature | Design rationale and implementation |
|---|---|
| **Virtual laboratory** | Modeled in Blender and imported into Unity to provide a realistic experimental setting with appropriate lighting and spatial organization |
| **Desktop navigation** | Keyboard and mouse interaction replaced the initially planned VR controls due to unavailable headsets; the integrated room uses the shared player |
| **Object selection** | Camera-based ray casting detects interactive objects through colliders |
| **Participant consent** | An interactive document opens a consent interface; the experiment remains locked until consent is accepted |
| **Physics interaction** | Rigidbody physics allows the consent paper to be raised during interaction and released under gravity afterward |
| **Stroop buttons** | Three color-coded interactive buttons register responses and provide visual feedback |
| **Notification system** | Visual and auditory smartphone alerts are introduced during the second block without requiring participant interaction |
| **Researcher guidance** | Three prerecorded voiceovers provide introduction, post-consent guidance, and completion feedback |
| **Dynamic lighting** | An interactive desk lamp demonstrates runtime light control with auditory feedback |
| **Experiment management** | C# scripts coordinate timing, task progression, notifications, measurements, completion, and reset |

The final integration uses the shared camera and player. The `DeskPointer` samples one ray hit per frame to prevent a Start click from unintentionally registering as a Stroop response.

### 5. Development Challenges and Solutions

Several technical issues required iterative debugging:

| Challenge | Solution and outcome |
|---|---|
| **Incorrect Blender import orientation** | Corrected transformations and camera alignment to position the laboratory properly |
| **Invisible Stroop display** | Adjusted the display's position and orientation to make stimuli visible |
| **Response buttons not functioning** | Corrected Inspector assignments from Start to Red/Green/Blue, restoring response processing |
| **Consent paper physics warnings** | Corrected transitions between kinematic and gravity-driven Rigidbody states |
| **Navigation and object accessibility** | Refined colliders, camera positioning, and interaction targets |
| **Experimental progression and reset** | Coordinated the two timed blocks, completion feedback, and state restoration |

These challenges demonstrated that reliable Unity interactions depend on both correct scripting and scene configuration, particularly colliders, object transformations, Inspector settings and interaction states.

### 6. Evaluation and Limitations

**Technical achievements:** Stroop room integrates navigation, ray casting, object manipulation, physics, audio, lighting, experimental state management, behavioral measurements and reset functionality into a coherent participant experience.

**Limitations:**

- Desktop interaction cannot fully reproduce immersive VR.
- Fixed condition order may introduce learning or fatigue effects.
- Mouse-based selection may influence reaction-time measurements.
- Short experimental blocks and simplified notifications limit ecological validity.
- No formal participant study or statistical comparison was conducted.

Therefore, the prototype demonstrates **functional experimental implementation rather than scientifically validated effects of smartphone notifications**.

Future improvements include counterbalancing conditions, exporting trial-level data, analyzing notification timing, conducting usability studies, and adapting the experiment to VR hardware.

### 7. Conclusion

Stroop room demonstrates how cognitive psychology research can inform the design of an interactive virtual experiment. By combining a Stroop task with smartphone notifications, consent procedures, behavioral measurements and 3D interactions, the project translates theoretical concepts into a functional experimental environment.

Its development also highlights the importance of research-informed design, iterative debugging, usability and experimental control.

### 8. Academic References

1. Stroop, J. R. (1935). *Studies of interference in serial verbal reactions.* Journal of Experimental Psychology, 18(6), 643–662. https://doi.org/10.1037/h0054651

2. Stothart, C., Mitchum, A., & Yehnert, C. (2015). *The attentional cost of receiving a cell phone notification.* Journal of Experimental Psychology: Human Perception and Performance, 41(4), 893–897. https://doi.org/10.1037/xhp0000100

3. Ward, A. F., Duke, K., Gneezy, A., & Bos, M. W. (2017). *Brain drain: The mere presence of one's own smartphone reduces available cognitive capacity.* Journal of the Association for Consumer Research, 2(2), 140–154. https://doi.org/10.1086/691462

4. Upshaw, J. D., Stevens, C. E., Jr., Ganis, G., & Zabelina, D. L. (2022). *The hidden cost of a smartphone: The effects of smartphone notifications on cognitive control from a behavioral and electrophysiological perspective.* PLOS ONE, 17(11), e0277220. https://doi.org/10.1371/journal.pone.0277220

5. Fournier, H., et al. (2026). *Attention hijacked: How social media notifications disrupt cognitive processing.* Computers in Human Behavior, 179, 108926. https://doi.org/10.1016/j.chb.2026.108926


### 4 November integration review

This branch incorporates Aïda's latest `52ab70f` and retains Yuming's `df270d7` / `760c23d` Museum updates. Shared bindings connect the new positioning/movement/audio logic to the common player. Museum menu clicks no longer recapture the cursor or take background photos; paused arithmetic input is suppressed. Stroop uses the same pointer system for its new Reset/lamp sound and keeps audio scheduling paused with the menu. Application version is **0.6.0**. Validation details and platform limits are in [the review](Docs/INTEGRATION_REVIEW_CN.md).

## Team workflow and contributions

Each person works in their own scene and folder. The integration owner combines the finished room roots in **MainScene**; the development scenes are retained. The teacher's 6 October email requires one project with the rooms connected through the same scene.

| Area | Owner / workflow |
| --- | --- |
| `Assets/Rooms/Memory/Scenes/MemoryDev.unity` | Yixiang Li: Memory Room source scene; edit its ordinary MemoryRoom root |
| `Assets/Rooms/RoomB/Scenes/RoomB.unity` | Yuming ZHAI: Museum source scene |
| `Assets/Rooms/AidaRoom/Scenes/AidaExperiment.unity` | Aïda Sadul: Stroop source scene |
| `Assets/Shared/Scenes/MainScene.unity` | Integration owner; final build starts here only |

- **Yixiang Li:** room concept, implementation, environment comparison and review, with AI coding/asset-preparation assistance.
- **Aïda Sadul** (`aidasdl`): Stroop room, furniture/lighting, consent and physical controls, researcher guidance/audio, and room/group-report documentation.
- **Yuming ZHAI** (`zhaiyuming283-rgb`): Museum concept and exhibit layout, Photo/Observe flow, camera interaction, distraction task, memory quiz, CSV export, Museum input fixes and room presentation updates.

Both teammates have accepted the repository invitations. The initial integration included Yuming’s `d562169` (24 October, PR #1) and Aïda’s `1b23eed` (29 October). Their original branches have not been rewritten. Subsequent Museum updates include keyboard/mouse input fixes and room layout, lighting and UI adjustments.

Stroop desk interactions use Input System coordinates consistently with the shared UI, fixing standalone Mac pointer offsets. Integration fixes and snapshots are confined to this preview branch; GitHub mergeability alone does not prove Unity compatibility.

Read [English joining/workflow guide](Docs/TEAM_SETUP_EN.md) or [中文协作说明](Docs/TEAM_SETUP_CN.md). Save/close Unity before branch changes. Commit assets with their `.meta` files, push your own branch and create a PR. Coordinate edits to shared scripts/settings/MainScene. Caches, local builds and user settings are ignored.

## Build and verify

- **FXR → Desktop → Build preview for this computer** builds MainScene as `Builds/Mac/FXRPreview.app` on Mac or `Builds/Windows/FXRMemory.exe` on Windows. Builds are local and ignored by Git. Both hosts use this same source and Editor version.
- **FXR → Check project references** checks all four scenes. **Window → General → Test Runner** contains EditMode scoring tests and PlayMode full-flow, real synthetic keyboard/mouse, drag, pause, rating and UI checks.
- **FXR → Desktop → Update Memory desktop setup** is an idempotent owner-only maintenance tool. The setup is already applied; daily development does not require this menu. It backs up the files and edits the existing Memory prefab in place to retain identifiers.
- Avoid the broad prototype/room regeneration menus after manual layout changes. Development scenes are working sources; the shared MainScene is an integration snapshot.

Yuming reports checking Museum movement, observation and photo interactions in the Windows Unity Editor. This integration review independently verifies the Mac player and automated flows; a complete exported Windows application check remains.

Before delivery, verify:

1. Navigation between all three rooms and each room's Restart.
2. Museum movement, cursor release and recovery, E observation, P camera toggle and left-click capture.
3. All 12 Museum trials, the arithmetic distraction task, the memory quiz and CSV output.
4. Readability of instructions and exhibit visibility in the integrated scene.
5. The exported application on the delivery computer.

Actual validation and platform limits are recorded in [STATUS](Docs/STATUS.md). Optional Quest instructions remain in [English](Docs/QUEST_SETUP_EN.md) / [中文](Docs/QUEST_SETUP_CN.md), with the earlier Quest build treated as historical.

## Report draft

Aïda’s new group report and detailed Stroop research/implementation section from `main` are preserved in [the English report draft](Docs/GROUP_REPORT_DRAFT_EN.md). It still contains placeholders and historical setup text; the runnable preview instructions and verified status are maintained in this README and STATUS. The report draft is not a submitted final report.

## Course requirements and final submission

The supplied [course brief](Docs/Assignment/2026-FXR-MiniProject.pdf) and [marking rubric](Docs/Assignment/Project-Assessment%20Criteria%20with%20Marking%20Rubric.pdf) are included for the private group.

Memory currently demonstrates glide/fly movement, distance ray selection/dragging, runtime activation, Canvas + 3D TextMeshPro, physics, interactive lighting, spatial audio, countdown and scripted experiment logic. The brief's literal **touch pad** input wording still needs classroom confirmation for the keyboard equivalent; the current desktop plan comes from the group conversation, not a new teacher email.

Before **midnight 4 November 2026**, as stated in the Ch5 timetable checked on 21 October:

1. Review the integration branch with both room owners and check connected navigation and each room's Restart on the delivery computer.
2. Confirm the keyboard navigation interpretation and complete a Windows application check; try the Memory flow with 1–2 people for usability.
3. Fill the full member names and actual contributions above; provide a compressed application demo video in `Submission/`.
4. Keep the submitted source under 2 GB, arrange teacher access to this private repository and email its link to `huyen.nguyen@universite-paris-saclay.fr`.

No final submission or email delivery has been performed. A usability trial checks the prototype; it does not establish a scientific effect.
