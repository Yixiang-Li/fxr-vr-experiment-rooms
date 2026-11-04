# FXR Psychological Experiment Rooms

**Unity Editor:** 2022.3.62f3  
**Platform:** Desktop (keyboard and mouse)  
**Repository:** https://github.com/Yixiang-Li/fxr-vr-experiment-rooms

**Final project branch:** [integration/three-rooms-2026-11-01](https://github.com/Yixiang-Li/fxr-vr-experiment-rooms/tree/integration/three-rooms-2026-11-01)

**Demonstration video:** [Full three-room demonstration](Submission/FXR_Three_Rooms_Demo.mp4) (8 min 32 sec). Shared UI and Memory Room first, followed by Museum Room and Stroop Room.

## Project Overview

This project presents three interactive virtual environments designed to explore psychological phenomena through research-informed experimental tasks. Each room investigates a different aspect of human cognition while demonstrating key concepts in extended reality development, including navigation, object interaction, physics, audio and behavioral measurement.

The project consists of three experiments:

- **Memory Room (Yixiang Li):** Investigates memory performance and perceived mental effort across office and nature-inspired environments.
- **Museum Room (Yuming Zhai):** Explores how photographing objects versus observing them affects subsequent memory performance.
- **Stroop Room (Aïda Sadul):** Investigates the potential influence of smartphone notifications on attention, reaction time, and response accuracy.

Although initially designed for virtual reality, the project was adapted for desktop interaction due to hardware constraints. All three rooms are integrated into a shared Unity scene, providing a connected experimental experience.

## Running the Integrated Project

1. On the **final project branch** linked above, choose **Code → Download ZIP** and extract it, or clone the repository and switch to `integration/three-rooms-2026-11-01`. Open the folder containing `Assets`, `Packages` and `ProjectSettings` in **Unity 2022.3.62f3**.
2. Open `Assets/Shared/Scenes/MainScene.unity` or select **FXR → Integration → Play three rooms**.
3. Enter Play Mode and select an experimental room using the navigation interface.
4. Follow the instructions provided within each experiment.

Press **Tab** to access the shared room menu. Switching rooms or restarting requires confirmation and resets the current room's progress. Previously exported CSV results are preserved.

Each experiment can also be opened independently through its development scene as described in the corresponding sections below.

## Memory Room — Yixiang Li

**Contribution:** Memory Room environments, matching-task logic, interaction and UI, and CSV logging; shared room navigation and integration.

### Research goal and inspiration

**Question:** Do office-like and nature-inspired settings affect memory-task performance or reported mental effort in a desktop 3D environment?

Orti et al. (2026), [Environmental Modulation of Perceived Cognitive Workload During a Memory Task: Evidence from Immersive Virtual Reality](https://doi.org/10.3390/jintelligence14080158), *Journal of Intelligence, 14*(8), 158, compared office/urban-park card matching in immersive VR. Behavioral performance did not differ; lower perceived workload in nature was primarily observed among highly efficient participants.

This desktop adaptation uses one exploratory 1–5 effort question instead of the paper's SIM-TLX questionnaire. Nature may feel less demanding; performance differences remain an open question. Card count and timing are prototype choices, not a replication of the original protocol.

### Experiment procedure

1. Preview both settings, choose **Office → Nature** or **Nature → Office**, and optionally practise dragging the orange cube. Previewing does not change the selected order.
2. Start round 1. Find **six matching pairs among 12 cards within 180 seconds** by clicking two cards at a time.
3. After completion or timeout, rate mental effort from **1 to 5**.
4. Complete round 2 in the other setting with the same symbols reshuffled, then submit the second rating.
5. View the final comparison and open **CSV FOLDER**. Live time, progress and mismatches remain visible during play; the final comparison appears only after both ratings.

### Environment and implementation

| Setting | Visual and audio environment |
| --- | --- |
| Office | Desks, chairs, screens, a city window view, recorded office/keyboard sounds and a cool background accent light |
| Nature | Plants, wood, stones, a forest window view, recorded forest/stream sounds and a green/warm background accent light |

Both rounds share card faces, grid, task lighting, camera and rules. Starting restores the task view; navigation and practice dragging stay locked through both rounds and rating/transition screens. Keep headphones and system volume consistent.

Features include mouse-ray selection, a draggable physics cube, matched cards falling into a tray, runtime environment/lighting changes, spatial audio and Canvas/TextMeshPro UI. Models, photographic materials and recorded sounds are credited in [asset provenance](Docs/ATTRIBUTIONS.md).

Pause, Reset confirmation and focus loss freeze timing and pair resolution. Reset restores the task, ratings, practice object and view, preserving order, sound settings and saved CSV files. New Session also requires confirmation.

**Development scene:** `Assets/Rooms/Memory/Scenes/MemoryDev.unity`. The final experience runs through the shared `MainScene` described above.

### Controls

| Input | Action |
| --- | --- |
| WASD / arrow keys | Move before or after a session |
| Hold right mouse and move the mouse | Look around before or after a session |
| G | Switch glide/fly navigation before or after a session |
| Q / E | Move down/up while flying |
| F | Return to the standard task view |
| Left mouse | Select cards/buttons; drag the orange practice cube before or after a session |
| Space | Start round 1 or continue to round 2 when ready |
| 1–5 | Submit mental-effort rating when prompted |
| Esc | Pause/resume a running round, cancel Reset or close Help |
| H / HELP | Read the guide outside the measured task |
| CSV FOLDER | Open saved results after completing both rounds |

Letter bindings use physical keyboard positions. The guide displays layout-specific names, including AZERTY; arrow keys provide an alternative.

### Data export

After the second effort rating, one CSV file containing both rounds is saved automatically to `Application.persistentDataPath/MemoryResults`. It records round/condition, completion or timeout, elapsed time, matched pairs, flips, pair attempts, mismatches, effort, input mode, app version, sound setting and interruption flag. Sessions with an interrupted round should be repeated before comparing conditions. Participant results should remain outside the repository.

### Evaluation and limitations

Automated functional checks covered both rounds, scoring, timeout, rating gates, pause/focus-loss handling, reset and CSV export. Desktop interaction was reviewed on macOS, and operation on Windows was confirmed by the group. These checks evaluate the application workflow.

- Each person completes both conditions, with manually selected order. Starting conditions can be alternated across participants to balance the order; reshuffling alone does not eliminate practice effects.
- Visual, audio and lighting changes are combined, so their individual effects cannot be isolated.
- The exploratory single-item effort rating does not measure stress or relaxation and is not equivalent to the paper's SIM-TLX questionnaire.
- Desktop interaction does not establish how the task would behave or feel in immersive headset VR.

### Conclusion

The Memory Room delivers a working desktop workflow for comparing a matching task across office-like and nature-inspired settings within the shared three-room application. Environment switching, consistent task rules, effort ratings and CSV export support the intended comparison. No participant study was conducted, so the prototype does not establish whether either setting changes memory performance or perceived effort.

## Museum Room — Yuming ZHAI

### Research question

Does taking a photograph, compared with observing only, affect later memory accuracy and response time for virtual museum exhibits?

### Research background and references

The Museum Room is inspired by research on the **photo-taking-impairment effect**.

Henkel (2014) compared photographing objects with observing them during a museum tour. Photographing whole objects was associated with poorer subsequent memory for some aspects of the experience. However, focused photography produced different results, suggesting that how people engage with an object matters.

Soares and Storm (2018) further investigated this effect, including conditions in which photographs were not retained.

Our project adapts the Photo-versus-Observe comparison to a keyboard-and-mouse virtual museum. It measures later quiz accuracy and response time. It is an exploratory course prototype, not a direct replication of either study. The exhibit count, observation duration, arithmetic distraction task and quiz design are implementation choices; they should not be interpreted as reproducing the original protocols.

**References**

- Henkel, L. A. (2014). Point-and-shoot memories: The influence of taking photos on memory for a museum tour. *Psychological Science, 25*(2), 396–402. https://doi.org/10.1177/0956797613504438
- Soares, J. S., & Storm, B. C. (2018). Forget in a flash: A further investigation of the photo-taking-impairment effect. *Journal of Applied Research in Memory and Cognition, 7*(1), 154–160. https://doi.org/10.1016/j.jarmac.2017.10.004

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

### Demonstration Video Timing

The demonstration video uses shortened timings: **5 seconds per exhibit** and **10 seconds for the arithmetic distraction task**.

The normal experiment uses **20 seconds per exhibit** and a **60-second arithmetic distraction task**. These shortened demonstration timings do not apply to the memory quiz and are used only to present the complete workflow more concisely.

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

## Conclusion

This project demonstrates how interactive 3D environments can be used to translate psychological research into functional experimental experiences. Through three distinct experiments, we explored the relationships between environmental context and memory, photography and memory retention and smartphone notifications and cognitive attention.

Beyond implementing individual experiments, the project required integrating navigation, object interaction, physics, audio, experimental state management and behavioral measurements into a unified Unity application. The development process highlighted the importance of research-informed design, iterative testing, technical problem-solving and collaboration.

Although these environments are educational prototypes rather than scientifically validated experiments, they demonstrate the potential of extended reality technologies to support experimental design and the study of human cognition. The project also provides a foundation for future improvements, including immersive VR implementation, controlled participant studies and more rigorous behavioral analysis.
