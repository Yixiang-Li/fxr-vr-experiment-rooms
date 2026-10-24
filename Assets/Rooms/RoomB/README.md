# RoomB ¡ª Photo-Taking and Visual Memory Experiment

## 1. Overview

RoomB is an interactive 3D museum environment designed to explore how taking photographs may influence visual memory.

Participants examine 12 exhibits under two experimental conditions:

- **Photo:** Observe the exhibit and take one photograph using a virtual camera.
- **Observe:** Observe the exhibit without taking a photograph.

After completing the exhibition, participants perform a short distraction task followed by a memory test.

The room is designed for desktop interaction using a keyboard and mouse.

## 2. Experiment Flow

1. **Start Experiment:** The participant enters the museum and begins the experiment.
2. **Exhibit Observation:** Visit 12 exhibits in the guided order.
   - 6 exhibits are assigned to the Photo condition.
   - 6 exhibits are assigned to the Observe condition.
   - Each exhibit has a 20-second observation period.
   - In the Photo condition, one photograph is required.
3. **Distraction Task:** Complete a 60-second mental arithmetic task.
4. **Memory Test:** Answer 12 questions about the exhibits, including image recognition and detail memory questions.
5. **Results:** View the overall accuracy and number of correct answers.
6. **Finish:** Choose between:
   - **Confirm & Exit:** Leave the exhibition.
   - **Explore Again:** Freely explore the museum without experimental tasks.

## 3. Controls

| Input | Action |
|---|---|
| WASD | Move |
| Mouse | Look around |
| E | Start observing an exhibit |
| P | Raise or lower the virtual camera (Photo condition) |
| Left Mouse Button | Take a photograph when the exhibit is correctly framed |

During observation, player movement is disabled, but limited camera rotation is allowed.

A yellow navigation arrow guides participants to the next exhibit and, after all exhibits are completed, to the quiz station.

## 4. How to Run

1. Open the Unity project using **Unity 2022.3.62f3**.
2. Open the scene at `Assets/Rooms/RoomB/Scenes/Room2.unity`.
3. Enter Play mode.
4. Click **START EXPERIMENT**.
5. Follow the on-screen instructions to complete the experiment.

### Project Requirements

- Universal Render Pipeline (URP)
- Active Input Handling: **Both**
- TextMesh Pro
- Keyboard and mouse

The experiment does not require a VR headset.

## 5. Experimental Settings

| Parameter | Value |
|---|---|
| Number of exhibits | 12 |
| Photo condition | 6 exhibits |
| Observe condition | 6 exhibits |
| Observation duration | 20 seconds |
| Distraction duration | 60 seconds |
| Memory questions | 12 |
| Evaluation | Accuracy and correct answer count |

The Photo and Observe conditions are balanced across question types. Exhibit conditions and memory-question order are randomized.

**Before final testing, ensure that Test Mode is disabled in ExperimentManager.**

## 6. Data Collection

The experiment exports participant-level trial results to CSV files.

Results