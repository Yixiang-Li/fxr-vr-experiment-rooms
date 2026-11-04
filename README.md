# Psychological Experiment Room

### Fundamentals of Extended Reality (FXR) | Group Project

An interactive 3D environment for exploring psychological experiments through virtual interaction, experimental design, and behavioral measurement.

**Technologies:** Unity 2022.3.62f3 | C# | Blender  
**Platform:** Desktop (keyboard and mouse)  
**Project structure:** Three experimental rooms: Room A, Room B and Room C

---

## 1. Project Overview

The Psychological Experiment Room is a group project developed for the Fundamentals of Extended Reality (FXR) course. It explores how interactive 3D environments can support the design, implementation and demonstration of psychological experiments.

The project consists of three experimental rooms, each developed by a team member. Together, they form a collection of interactive environments that combine psychological research principles with technologies such as virtual navigation, object manipulation, ray casting, physics simulation, audio, lighting, and behavioral scripting.

The objective was not simply to construct visually convincing rooms but to design meaningful interactions in which technical choices serve a clear experimental purpose.

Each room was therefore approached through four questions:

- **WHY:** What problem or experimental requirement motivates the design?
- **WHICH:** Which interaction techniques, technologies or research findings inform the choice?
- **HOW:** How is the solution implemented?
- **WHAT:** What was achieved and what limitations or technical challenges remain?

Although the project was initially conceived for virtual reality, the unavailability of VR headsets required adaptation to a desktop environment. Keyboard and mouse interaction therefore replace headset- and controller-based interaction while preserving the central principles of virtual exploration and interactive experimentation.

## 2. Team Members and Contributions

The project was developed collaboratively by three members, with individual responsibility for each experimental room.

| Member | Responsibility | Experimental room |
|---|---|---|
| Yuming Zhai | [Contribution summary] | Room A |
| Yixiang Li | [Contribution summary] | Room B |
| Aïda Sadul | Environment design, Stroop experiment, consent interaction, notification system, object interactions, behavioral measurements, debugging and documentation | Room C |

Each member was responsible for designing, implementing, testing, and documenting their respective experimental environment.

Shared work, repository integration, and other collaborative contributions should be specified by the relevant members.

## 3. Development Tools and Technologies

| Technology | Purpose |
|---|---|
| Unity 2022.3.62f3 | Scene integration, rendering, interactive behavior and experimental execution |
| Blender | 3D modeling and environment preparation |
| C# | Interaction scripting, experimental logic and state management |
| Unity Physics | Collisions, ray casting, gravity and Rigidbody interactions |
| Unity UI | Participant instructions, consent interfaces and experiment feedback |
| Unity Audio | Researcher guidance, notifications and interaction feedback |
| Git / GitHub | Version control, collaboration and project submission |

The combination of Blender and Unity allowed the project to separate environment modeling from interactive behavior and experimental logic.

## 4. Project Structure and Installation

### 4.1 Repository Organization

The repository contains the Unity project and the three experimental environments.

**Repository:** [Insert GitHub repository link]

**Room A:** [Insert scene or folder path]

**Room B:** [Insert scene or folder path]

**Room C:** [Insert scene or folder path]

The exact directory structure and scene paths should be added from the final repository before submission.

### 4.2 Requirements

To open the project, install:

- Unity Hub
- Unity Editor 2022.3.62f3
- Required packages specified in the Unity project

A keyboard and mouse or compatible trackpad are required for interaction.

## Run my Memory Room

1. Clone the repository using GitHub Desktop and add the whole project folder to Unity Hub.
2. Open with **2022.3.62f3**. Wait for its first import.
3. Choose **FXR → Desktop → Play Memory Room**. This opens the editable MemoryDev scene, starts Play and focuses the Game view. Stop Play before editing.
4. Choose the environment order and sound on the panel, then click **START ROUND 1** or press **Space**.

No headset, Android modules, Blender, Python or paid Rider installation is needed for this desktop flow. Models, textures and recorded audio are included.

For the integrated project, open `Assets/Shared/Scenes/MainScene.unity` and press Play. Room B and C currently remain starter slots; the full three-room submission is not complete.

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

Control schemes may differ between experimental rooms. Each member should document any room-specific controls.

---

# 5. Experimental Rooms

## 5.1 Room A: [Experiment Title]

**Developed by:** [Member 1]

### Overview

[Describe the psychological experiment, its purpose and how the participant interacts with the virtual environment.]

### Research Question

[Insert the research question.]

### Academic Background

[Explain the scientific principles supporting the experiment and cite the relevant research papers.]

### Experimental Design

[Describe the experimental procedure, conditions, independent and dependent variables, and measurements.]

### Design and Technical Implementation

[Explain WHY each major feature was needed, WHICH technique was selected, HOW it was implemented and WHAT was achieved.]

### Development Challenges

[Describe significant technical difficulties, debugging decisions, solutions and outcomes.]

### Results and Limitations

[Explain what the prototype demonstrates, what was tested and what limitations remain.]

### Screenshots and Demonstration

[Insert screenshots and demonstration video link.]

### References

[Insert academic and technical references.]

---

## 5.2 Room B: [Experiment Title]

**Developed by:** [Member 2]

### Overview

[Describe the psychological experiment, its purpose and how the participant interacts with the virtual environment.]

### Research Question

[Insert the research question.]

### Academic Background

[Explain the scientific principles supporting the experiment and cite the relevant research papers.]

### Experimental Design

[Describe the experimental procedure, conditions, independent and dependent variables, and measurements.]

### Design and Technical Implementation

[Explain WHY each major feature was needed, WHICH technique was selected, HOW it was implemented and WHAT was achieved.]

### Development Challenges

[Describe significant technical difficulties, debugging decisions, solutions and outcomes.]

### Results and Limitations

[Explain what the prototype demonstrates, what was tested and what limitations remain.]

### Screenshots and Demonstration

[Insert screenshots and demonstration video link.]

### References

[Insert academic and technical references.]

---

# 5.3 Room C: The Impact of Smartphone Notifications on Cognitive Attention

**Developed by:** Aïda Sadul  
**Experimental task:** Stroop-Based Attention Assessment

## 5.3.1 Introduction and Research Motivation

Smartphones have become an integral part of daily life, providing continuous access to communication and information. However, their constant presence and frequent notifications raise questions about their potential impact on attention and cognitive performance.

Students and professionals often perform demanding activities while smartphones remain nearby. Even when notifications are ignored, their visual or auditory signals may interrupt ongoing cognitive processes.

Room C investigates this issue through a virtual psychological laboratory in which participants complete a Stroop task under two experimental conditions: one without smartphone notifications and another with visual and auditory notifications.

The environment combines a research-informed cognitive task with interactive 3D elements, including a researcher character, consent procedure, physical-style response buttons, a virtual smartphone, performance measurements and environmental interactions.

**Research question:**

How do visual and auditory smartphone notifications affect reaction time and response accuracy during a Stroop task in an interactive virtual environment?

The objective is to demonstrate how a virtual environment can support a structured psychological experiment, rather than to establish scientifically conclusive findings about the effect of smartphone notifications.

## 5.3.2 Academic Background and Related Research

The experimental design was informed by established findings in cognitive psychology and research on smartphone-related distraction.

Five publications provide the theoretical and methodological foundation.

### A. Stroop (1935): Cognitive Interference

The Stroop effect describes the interference that occurs when the meaning of a written color word conflicts with the color of the ink in which it appears.

For example, when the word RED appears in blue, identifying the ink color requires suppressing the automatic tendency to read the word.

This makes the Stroop task particularly relevant for studying selective attention and cognitive control.

**Application to Room C:** Participants must identify the displayed ink color rather than the written word, using three corresponding response buttons.

The task introduces a cognitively demanding activity against which notification-related interference can be investigated.

### B. Stothart et al. (2015): The Attentional Cost of Receiving a Cell Phone Notification

Stothart, Mitchum and Yehnert investigated whether smartphone notifications could interfere with attention even when individuals did not directly interact with their phones.

Their findings suggested that receiving notifications alone could disrupt performance on an ongoing attention-demanding activity.

**Application to Room C:** Notifications appear during the Stroop task without requiring participants to interact with the virtual smartphone.

This distinction is important because interacting with a phone would introduce an additional task. The implementation instead focuses on distraction associated with receiving a notification.

### C. Ward et al. (2017): Brain Drain

Ward and colleagues investigated whether the mere presence of a smartphone could reduce available cognitive capacity.

Their findings suggested that the presence of one's own smartphone could impair certain measures of cognitive capacity even without direct interaction.

**Application to Room C:** A smartphone remains visible on the experimental desk throughout both conditions.

By keeping the phone physically present in both blocks, the prototype maintains a consistent environmental arrangement. The intended comparison concerns notifications rather than smartphone presence.

The experiment does not independently evaluate the effect of the phone's presence.

### D. Upshaw et al. (2022): The Hidden Cost of a Smartphone

Upshaw and colleagues examined the impact of smartphone notification sounds on cognitive control using behavioral and electrophysiological measurements.

Their research reported slower responses during trials associated with smartphone notification sounds, highlighting a potential cognitive cost of auditory alerts.

**Application to Room C:** Auditory notifications are incorporated into the distraction condition, while the experiment records reaction time and response correctness.

Both measurements are necessary because notification-related interference may influence response speed without producing an equivalent change in accuracy.

### E. Fournier et al. (2026): Attention Hijacked

Fournier and colleagues investigated how social media notifications disrupt cognitive processing during a Stroop task.

Their research reported a temporary slowdown in cognitive processing following notifications and examined how notification relevance and smartphone-use habits influence distraction.

**Application to Room C:** This research provided the closest methodological inspiration for the experiment.

Room C combines a Stroop task with smartphone-style notifications while recording behavioral measures of performance.

Unlike the original research, the prototype does not reproduce its physiological measurements, participant protocol or detailed analysis of notification onset.

### Research-Informed Design Decisions

| Research | Finding or principle | Implementation |
|---|---|---|
| Stroop (1935) | Conflict between automatic reading and color identification | Stroop color-identification task |
| Stothart et al. (2015) | Notifications may disrupt attention without direct interaction | Notifications do not require phone interaction |
| Ward et al. (2017) | Smartphone presence may influence cognitive capacity | Smartphone remains visible in both conditions |
| Upshaw et al. (2022) | Auditory notifications may affect cognitive response speed | Notification sounds and reaction-time measurements |
| Fournier et al. (2026) | Notifications can temporarily disrupt Stroop-task performance | Stroop experiment with notification and no-notification conditions |

These studies informed the experimental task, notification design, environmental arrangement and performance measurements.

## 5.3.3 Research Objectives and Hypotheses

The experiment was designed around four objectives:

1. Investigate the potential influence of notifications on reaction time.
2. Compare response accuracy across notification and no-notification conditions.
3. Reproduce smartphone-related distractions within an interactive virtual environment.
4. Implement a coherent psychological experiment combining consent, interaction, experimental control, timing and behavioral measurements.

Two hypotheses guided the implementation:

**H1:** Reaction times may increase when smartphone notifications are present.

**H2:** Response accuracy may decrease when smartphone notifications are present.

These hypotheses are derived from the literature and represent predictions rather than confirmed results from Room C.

## 5.3.4 Experimental Design

### The Stroop Task

The experiment requires participants to identify the ink color of a displayed word rather than its semantic meaning.

Three response colors are used:

- Red
- Green
- Blue

Three large physical-style buttons are positioned on the experimental desk. Participants respond by selecting the button corresponding to the stimulus ink color.

This approach embeds a conventional computerized cognitive task within an interactive 3D environment.

### Experimental Conditions

**Condition 1: Without Notifications**

Participants complete the Stroop task without receiving smartphone notifications.

This condition provides a baseline against which performance during the distraction condition can be considered.

**Condition 2: With Notifications**

Participants perform the same type of task while a smartphone on the desk produces visual and auditory notifications.

They are instructed to ignore the alerts and continue responding to the Stroop stimuli.

The principal experimental manipulation is the presence or absence of notifications.

### Experimental Variables

| Variable | Description |
|---|---|
| Independent variable | Smartphone notifications: absent or present |
| Dependent variable 1 | Reaction time |
| Dependent variable 2 | Response accuracy |
| Additional measure | Number of incorrect responses |
| Controlled environmental element | Smartphone remains present in both conditions |
| Experimental sequence | No-notification block followed by notification block |

Reaction time is measured from stimulus presentation to participant response.

Accuracy is determined by whether the selected response matches the displayed ink color.

The C# experiment logic records response-related information, while Unity provides debugging output and a final performance summary.

**Configuration details to verify in the final build:** Block duration, number of trials, notification frequency and whether performance statistics are separated by condition.

## 5.3.5 Virtual Laboratory and Participant Experience

The environment was designed to resemble a welcoming psychological research room rather than an abstract testing interface.

It includes a researcher character, experimental desk, computer display, colored response buttons, consent document, smartphone, clock, interactive lamp and environmental furniture.

These objects support a structured participant journey.

### 1. Entering the Laboratory

Participants explore the environment using first-person desktop navigation.


### 2. Researcher Introduction

A virtual researcher welcomes participants and introduces the activity through prerecorded audio.

The researcher remains stationary, prioritizing instructional clarity over unnecessary animation complexity.

### 3. Participant Consent

Before beginning the experiment, participants interact with a consent document positioned on the desk.

Selecting the document raises it slightly and opens an on-screen interface through which participation can be accepted or declined.

The consent state is recorded, and the experiment remains inaccessible until consent has been accepted.

After the interaction, the paper is released and responds to gravity using Unity's physics system.

The consent procedure is a simulated interface interaction rather than a legally authenticated signature.

### 4. Beginning the Stroop Task

Following consent, the participant can start the experiment.

Stroop stimuli appear on the display, and responses are provided through the three colored buttons.

Each response is evaluated according to its correctness and reaction time.

### 5. Notification Condition

During the second block, the smartphone generates visual and auditory notifications.

The participant continues performing the task without interacting with the smartphone.

### 6. Experiment Completion

At the end of the experimental sequence, the system displays a performance summary including accuracy and average reaction time.

A concluding researcher voiceover communicates the end of the session.

### 7. Reset and Repetition

The experiment can be reset to allow another session.

The reset logic restores relevant experimental states, while the consent system can return the document and its controls to their initial state.

This supports repeated demonstrations and testing without rebuilding the Unity scene.

## 5.3.6 Technical Implementation and Design Rationale

The implementation was guided by the principle that technical features should serve experimental functionality, participant understanding or usability.

### A. Environment Modeling and Integration

**WHY:** A dedicated laboratory was needed to provide a coherent setting for the psychological experiment.

**WHICH:** Blender was selected for modeling and Unity for interactive implementation.

**HOW:** The room was modeled in Blender, exported as an FBX asset and imported into Unity. Orientation, positioning, colliders, lighting and interactive components were configured after import.

**WHAT:** The final scene provides an explorable experimental environment containing the objects required for the participant journey.

### B. First-Person Navigation

**WHY:** Participants must navigate the room and approach the experimental equipment.

**WHICH:** Keyboard and mouse navigation replaced the initially intended VR controller interaction.

**HOW:** A first-person controller manages movement and camera orientation, with environmental colliders restricting movement through room boundaries.

**WHAT:** The laboratory can be explored using desktop equipment, although this cannot fully reproduce the embodied interaction of immersive VR.

### C. Ray Casting and Object Selection

**WHY:** Participants need a direct way to select interactive objects within the 3D environment.

**WHICH:** Camera-based ray casting was chosen.

**HOW:** A ray is generated through the mouse position when the participant clicks. Unity's physics system checks whether it intersects the collider of an interactive object and activates the corresponding interaction logic.

**WHAT:** Objects such as the consent document can be selected without requiring an additional selection interface.

This technique also highlighted the importance of object visibility, camera positioning and collider accessibility.

### D. Consent and Rigidbody Physics

**WHY:** Consent needed to be integrated into the laboratory experience rather than implemented as an unrelated menu.

**WHICH:** Unity UI, ray casting, colliders and Rigidbody physics were combined.

**HOW:** Selecting the document suspends its physical simulation while the paper is raised and the consent interface appears. When the interaction concludes, physics is re-enabled to produce gravity-driven movement.

**WHAT:** The procedure combines ethical participation requirements with virtual object manipulation and physical feedback.

### E. Physical Response Buttons

**WHY:** The Stroop task requires a clear and consistent response mechanism.

**WHICH:** Three physical-style buttons were selected instead of relying solely on keyboard responses.

**HOW:** Each button is assigned a color-specific response type. The experiment compares the selected color with the displayed ink color, records the response and advances the task.

**WHAT:** Participants respond through direct interaction with objects in the laboratory rather than through a conventional two-dimensional interface.

### F. Experimental State Management

**WHY:** Consent, trial progression, experimental conditions and completion must occur in a controlled sequence.

**WHICH:** Script-based state management using C#.

**HOW:** The experiment logic coordinates consent status, initiation, stimulus progression, notification conditions, timing, response evaluation, completion and reset.

**WHAT:** The system operates as a structured experimental procedure rather than a collection of unrelated interactions.

### G. Smartphone Notifications

**WHY:** Notifications are the independent variable central to the research question.

**WHICH:** Recognizable visual and auditory smartphone alerts were chosen as potential distractors.

**HOW:** The notification mechanism operates during the second experimental block while the Stroop task continues.

**WHAT:** The participant experiences smartphone-related distractions without needing to abandon the cognitive task.

The notifications are intentionally simplified rather than implemented as a complete smartphone interface.

### H. Audio and Researcher Guidance

**WHY:** Instructions and feedback should be accessible without excessive on-screen text.

**WHICH:** Unity's audio system and prerecorded researcher messages.

**HOW:** Three researcher voiceovers are associated with the introduction, consent progression and experiment completion. Additional sounds accompany notifications and environmental interactions.

**WHAT:** Audio supports the participant journey while also providing an experimental distraction through smartphone notification sounds.

### I. Dynamic Lighting

**WHY:** Lighting contributes to environmental readability and demonstrates runtime control over the virtual scene.

**WHICH:** An interactive desk lamp with a sound effect.

**HOW:** A scripted interaction activates or deactivates the lamp's light source and produces auditory feedback.

**WHAT:** Participants can manipulate a local environmental property during runtime.

Lighting is not an experimental independent variable and should remain consistent across both Stroop conditions.

### J. Timing and Response Evaluation

**WHY:** Reaction time and accuracy are central to the experiment's research objectives.

**WHICH:** C# timing and response-processing logic.

**HOW:** The system compares each response with the expected ink color and measures the time elapsed since stimulus presentation. A separate countdown controls the experimental blocks.

**WHAT:** The experiment provides behavioral measurements and a final performance summary.

## 5.3.7 Development Challenges and Problem Solving

Developing Room C required several iterations of modeling, testing, debugging and refinement.

The most significant challenges involved ensuring that independently functioning components behaved correctly when integrated into the experimental workflow.

| Challenge | Investigation and solution | Outcome and lesson |
|---|---|---|
| Incorrect room orientation after import | Inspected and corrected the model transformation and camera alignment | Demonstrated the importance of coordinate-system compatibility between Blender and Unity |
| Stroop display activated but not visible | Adjusted the display's position and orientation | Showed that successful GameObject activation does not guarantee visibility |
| Response buttons failed to advance trials | Identified incorrect button types in the Unity Inspector and reassigned them to Red, Green and Blue | Restored response processing and highlighted the impact of scene configuration |
| Consent document produced Rigidbody warnings | Separated controlled movement from physics-driven movement and corrected kinematic-state handling | Enabled document pickup, release and restoration without the original physics warnings |
| Navigation and object accessibility required refinement | Configured environmental colliders and adjusted scene positioning | Improved the consistency of desktop exploration and object interaction |
| Multiple experimental stages required coordinated completion and reset | Integrated completion logic, concluding audio and reset behavior into the experiment workflow | Supported repeated task execution and a clearer participant journey |

### Key Lessons from Development

**Implementation involves more than writing functional code.**

The response-button issue was particularly instructive because the underlying response-processing logic was not the primary problem. Incorrect Inspector assignments prevented the scene objects from communicating their intended responses.

Similarly, the Stroop display problem demonstrated the distinction between internal application state and what the participant can actually see.

The consent-document interaction highlighted the importance of managing transitions between scripted object movement and physics simulation.

Together, these challenges reinforced the value of iterative testing across scripts, scene objects, colliders, transformations and participant interactions.

## 5.3.8 Functional Evaluation

The implemented prototype demonstrates several fundamental concepts covered in FXR.

| Technical concept | Room C implementation |
|---|---|
| Virtual navigation | First-person keyboard and mouse navigation |
| Object selection | Camera-based ray casting |
| Object manipulation | Interactive consent document |
| Physics simulation | Rigidbody, gravity and collisions |
| Runtime activation and deactivation | Consent interface, notifications and interaction controls |
| Environmental interaction | Interactive desk lamp |
| Audio integration | Researcher guidance and sound feedback |
| Experimental logic | C# state management and Stroop task control |
| Behavioral measurement | Reaction time and response correctness |
| Timing | Countdown-based experimental blocks |
| Reset | Restoration of relevant experimental and interaction states |

The main technical achievement is the integration of these elements into a coherent experimental sequence.

The prototype was designed so that consent authorizes participation, participant responses drive trial progression, notifications alter the experimental condition and the completion stage provides feedback.

## 5.3.9 Experimental and Usability Limitations

Although the prototype demonstrates the intended interactive workflow, several limitations must be considered.

**Desktop interaction:** The absence of VR hardware required keyboard and mouse navigation. This limits immersive interaction and introduces additional motor demands.

**Fixed condition order:** The no-notification condition precedes the notification condition. Practice or fatigue effects may influence performance independently of notifications.

**Response interaction:** Selecting 3D buttons requires visual targeting and mouse movement. Recorded reaction times may therefore reflect both cognitive processing and interaction performance.

**Simplified notifications:** Virtual alerts cannot necessarily reproduce the personal relevance of notifications received on a participant's own smartphone.

**Limited experimental duration:** Short blocks are appropriate for demonstration but provide limited data for rigorous behavioral analysis.

**Data analysis:** The prototype provides performance measurements but does not establish a validated research dataset or formal statistical comparison.

**Participant evaluation:** No controlled participant study is documented. The system therefore does not establish whether notifications significantly influence response accuracy or reaction time.

These limitations distinguish successful technical implementation from scientific validation.

## 5.3.10 Future Improvements

Several extensions would improve the scientific validity and usability of the prototype.

**Counterbalanced experimental conditions:** Varying the order of the two conditions would help reduce practice and fatigue effects.

**Structured trial-level data export:** Recording stimulus information, responses, accuracy, reaction times and experimental conditions would enable more detailed analysis.

**Notification-onset measurements:** Recording precise notification timestamps would allow investigation of short-term response slowing immediately after alerts.

**Participant usability evaluation:** A pilot study could assess instruction clarity, navigation comfort and interaction reliability.

**VR adaptation:** Compatible headsets and controllers could enable more immersive navigation and object manipulation.

## 5.3.11 Room C Conclusion

Room C demonstrates how an interactive virtual environment can translate a research-informed psychological task into a structured experimental experience.

The design combines the Stroop paradigm with smartphone notifications while integrating participant consent, object interactions, audio guidance, environmental control and behavioral measurements.

Academic research guided the choice of cognitive task, experimental conditions and dependent measures. Unity and Blender provided the technical foundation for transforming these principles into an interactive laboratory.

The development process also emphasized the importance of iterative debugging and the relationship between script logic, scene configuration, physics and usability.

Although the prototype does not provide conclusive evidence about notification-related attention effects, it demonstrates how psychological research principles can inform the design and implementation of interactive experimental systems.

---

# 6. Group-Level Evaluation and Reflection

The three experimental rooms illustrate how virtual environments can be used to explore psychological concepts through interaction and behavioral experimentation.

Each room contributes its own experimental approach while sharing the broader objective of translating psychological procedures into interactive 3D experiences.

The project also highlights the relationship between technical implementation and experimental validity. Successful interaction design requires not only functioning scripts and virtual objects but also understandable participant instructions, reliable task progression and meaningful measurements.

**Room A contribution to the overall project:** [Member 1 to complete]

**Room B contribution to the overall project:** [Member 2 to complete]

**Room C contribution to the overall project:** Room C contributes a research-informed attention experiment in which smartphone notifications are introduced during a Stroop task, supported by consent, response evaluation and behavioral measurements.

**Shared challenges and lessons:** [Complete collaboratively with the team's integration, testing and coordination experience.]

---

# 7. Academic References

The following references support the scientific foundations of Room C.

Stroop, J. R. (1935). Studies of interference in serial verbal reactions. *Journal of Experimental Psychology, 18*(6), 643–662.  
https://doi.org/10.1037/h0054651

Stothart, C., Mitchum, A., & Yehnert, C. (2015). The attentional cost of receiving a cell phone notification. *Journal of Experimental Psychology: Human Perception and Performance, 41*(4), 893–897.  
https://doi.org/10.1037/xhp0000100

Ward, A. F., Duke, K., Gneezy, A., & Bos, M. W. (2017). Brain drain: The mere presence of one's own smartphone reduces available cognitive capacity. *Journal of the Association for Consumer Research, 2*(2), 140–154.  
https://doi.org/10.1086/691462

Upshaw, J. D., Stevens, C. E., Jr., Ganis, G., & Zabelina, D. L. (2022). The hidden cost of a smartphone: The effects of smartphone notifications on cognitive control from a behavioral and electrophysiological perspective. *PLOS ONE, 17*(11), e0277220.  
https://doi.org/10.1371/journal.pone.0277220

Fournier, H., Fournel, A., Osiurak, F., Koenig, O., Pâris, F., Gaujoux, V., & Ringeval, F. (2026). Attention hijacked: How social media notifications disrupt cognitive processing. *Computers in Human Behavior, 179*, 108926.  
https://doi.org/10.1016/j.chb.2026.108926

**Room A academic references:** [Member 1 to complete]

**Room B academic references:** [Member 2 to complete]

---

# 8. Final Conclusion

The Psychological Experiment Room project demonstrates the potential of interactive 3D environments to support research-informed psychological experimentation.

Across three experimental rooms, the project brings together virtual environment development, interaction techniques and experimental design.

Room C specifically explores the potential influence of smartphone notifications on cognitive attention through a Stroop-based task. Its design was informed by academic research and implemented through a combination of 3D modeling, interaction scripting, physics, audio, behavioral measurements and iterative testing.

The project illustrates how psychological concepts can be translated into interactive systems while also revealing the practical challenges of experimental control, usability and reliable implementation.

Ultimately, its contribution lies in demonstrating how research, design reasoning and technical development can be combined to produce meaningful interactive experimental experiences.
