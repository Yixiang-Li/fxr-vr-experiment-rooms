# FXR Experiment Rooms

One Unity project with a connected main scene and three individually owned rooms. The Memory Room is Yixiang's contribution; Room B and Room C are teammate slots to be filled.

Private repository: https://github.com/Yixiang-Li/fxr-vr-experiment-rooms

## Open and play

1. Clone this repository. Install **Unity 6000.3.24f1** through Unity Hub.
2. In Unity Hub, **Add project from disk**, selecting the folder containing `Assets`, `Packages`, and `ProjectSettings`.
3. Open `Assets/Shared/Scenes/MainScene.unity` and press **Play**.
4. Desktop: **WASD** to move, hold **right mouse** to look, **left mouse** to select a card or panel button. Start on the panel to the right of the cards.

The default editor mode needs no headset. The shared player also contains the XR Interaction Toolkit rig and optional XR Interaction Simulator. Enable `simulateXR` on **Shared Player Setup / PlayerModeBootstrap** to use the simulator instead of desktop controls.

## Memory Room

- Two atmospheres: an office with synthetic hum/keyboard/phone sounds, and a room with plants and synthetic nature sounds.
- The same 12-card / 6-pair matching task in each atmosphere; the deck is reshuffled between rounds.
- A provisional 180-second limit; toggle Office → Nature / Nature → Office before starting.
- Select two cards. A wrong pair turns back; a matched pair drops into a physical tray.
- Rate mental effort from 1 to 5 after each round; the panel shows completion time, errors, and ratings.
- **Reset Room** restores cards, physics, environment, countdown, and current results. Previously exported CSV files remain on disk.
- Completed sessions save anonymous CSV files in `Application.persistentDataPath/MemoryResults`. On Quest, files are in the app's Android data folder.

Inspiration: Orti et al. (2026), [Environmental Modulation of Perceived Cognitive Workload During a Memory Task: Evidence from Immersive Virtual Reality](https://doi.org/10.3390/jintelligence14080158). This prototype adapts the environment comparison; it is not a replication of the paper's workload instrument. The synthetic audio and minimal geometry are initial placeholders.

See the [Chinese PRD](Docs/Memory_Room_Mini_PRD_CN.md) and [English PRD](Docs/Memory_Room_Mini_PRD_EN.md).

## Team workflow

Read [中文加入与协作指南](Docs/TEAM_SETUP_CN.md) or [English team setup](Docs/TEAM_SETUP_EN.md).

| Area | Owner / rule |
| --- | --- |
| `Assets/Rooms/Memory/` | Yixiang: memory task and environment comparison |
| `Assets/Rooms/RoomB/` | Teammate B: own scripts, materials, audio, prefab |
| `Assets/Rooms/RoomC/` | Teammate C: own scripts, materials, audio, prefab |
| `Assets/Shared/Scenes/MainScene.unity` | One integration owner; places all room prefabs and connects entrances |
| `Assets/Shared/`, `Packages/`, `ProjectSettings/` | Shared; coordinate before changing |

Develop your room as a **Prefab** in your folder. Temporary personal scenes can be used for building, but the submitted experience runs from **MainScene**. Keep a single shared player and EventSystem when integrating rooms. Do not commit `Library`, `Temp`, or local builds; do commit each asset's `.meta` file.

### Contributions — complete before submission

- Yixiang Li: Memory Room concept and implementation, with AI coding assistance; name/features to be checked against the final version.
- Teammate B: name and features to be filled.
- Teammate C: name and features to be filled.

## Meta Quest build

Install **Android Build Support**, its SDK/NDK tools, and OpenJDK for this editor in Unity Hub. This project configures Android / ARM64 / IL2CPP / OpenXR with the Oculus Touch interaction profile and Quest support.

1. Use **FXR → Configure project for Quest**.
2. Open **File → Build Profiles**, choose Android, and switch the active platform.
3. The enabled build scene must be MainScene. Build an APK, then install it on the reserved school Quest with the school's approved method.
4. Check controller rays, UI clicks, movement, tracking height, card readability, audio level, frame rate, Reset, and CSV retrieval on the device.

Do not enable a second XR provider or add another player rig. The school Quest model and developer-mode permissions still need confirmation. Editor and build checks do not establish headset compatibility or comfort. Confirm with the teacher whether Quest joystick movement satisfies the assignment's touchpad gliding/flying wording.

## Development tools and checks

Rider is optional. Scripts are ordinary C# files; Unity compiles them. GitHub Desktop is the recommended team interface. Unity Version Control (formerly Plastic SCM) is a separate system from Git; this repository uses Git/GitHub.

- **Window → General → Test Runner → EditMode → Run All** checks session order, scoring, timeout, rating, and Reset. **PlayMode → Run All** runs both rounds in MainScene and checks physical card reset and CSV export.
- **FXR → Rebuild generated prototype** regenerates MainScene and MemoryRoom.prefab. It can overwrite team edits: commit first and coordinate before using it.
- `tools/generate_ambience.py` regenerates the original synthetic sound placeholders.
- Development builds belong in the ignored `Builds/` folder. Put the final compressed demo video in `Submission/` and keep the submitted Unity source project below the course's 2 GB limit.

Third-party asset information is in [Attributions](Docs/ATTRIBUTIONS.md). Validation status is in [Development status](Docs/STATUS.md).
