# FXR Experiment Rooms

One Unity project with a connected main scene and three individually owned rooms. The Memory Room is Yixiang's contribution; Room B and Room C are teammate slots to be filled.

Private repository: https://github.com/Yixiang-Li/fxr-vr-experiment-rooms

## Open and play

1. Clone this repository. Install **Unity 6000.3.24f1** through Unity Hub.
2. In Unity Hub, **Add project from disk**, selecting the folder containing `Assets`, `Packages`, and `ProjectSettings`.
3. To work on your room, open its development scene from the table below and press **Play**. Open `Assets/Shared/Scenes/MainScene.unity` to try the integrated experience.
4. Desktop: **WASD** to move, hold **right mouse** to look, **left mouse** to select a card or panel button. Start on the panel to the right of the cards.

The default editor mode needs no headset. The shared player also contains the XR Interaction Toolkit rig and optional XR Interaction Simulator. Enable `simulateXR` on **Shared Player Setup / PlayerModeBootstrap** to use the simulator instead of desktop controls.

## Memory Room

- Office and indoor garden atmospheres now use free CC0 furniture and plants from Poly Haven, photographic wood/tile/plaster materials, and photographed distant window views. Supplementary props and architecture are original. The task geometry, card faces and main task light remain the same.
- Real recorded office conversations/keyboard, forest birds and running water from CC0 Freesound recordings, with spatial detail, loop editing and fades. Source RMS is matched; actual perceived volume still needs headset tuning. Short task cues remain original.
- The same 12-card / 6-pair matching task in each atmosphere; the deck is reshuffled between rounds.
- A provisional 180-second limit; toggle Office → Nature / Nature → Office before starting.
- Select two cards. A wrong pair turns back; a matched pair drops into a physical tray.
- Rate mental effort from 1 to 5 after each round. Results are shown after both ratings, so the first result does not cue the second rating.
- Preview, sound and order controls are available before starting. Running rounds show time, pair progress and selection feedback; cards highlight on hover.
- **Reset Session** asks for confirmation during a session and pauses the timer while the prompt is open. Cancel returns to the task; previously exported CSV files remain on disk. Rounds interrupted by this prompt or application pause are marked in CSV and should be repeated for a fair comparison.
- Completed sessions save anonymous CSV files in `Application.persistentDataPath/MemoryResults`. On Quest, files are in the app's Android data folder.

Inspiration: Orti et al. (2026), [Environmental Modulation of Perceived Cognitive Workload During a Memory Task: Evidence from Immersive Virtual Reality](https://doi.org/10.3390/jintelligence14080158). This prototype adapts the environment comparison; it is not a replication of the paper's workload instrument. Asset sources and edits are recorded in [Attributions](Docs/ATTRIBUTIONS.md). The scene is ready for hardware testing; environmental workload effects have not been established.

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

### Your daily development scene

| Person | Open this scene | Edit this Hierarchy root |
| --- | --- | --- |
| Yixiang | `Assets/Rooms/Memory/Scenes/MemoryDev.unity` | `MemoryRoom` |
| Teammate B | `Assets/Rooms/RoomB/Scenes/RoomBDev.unity` | `RoomB` |
| Teammate C | `Assets/Rooms/RoomC/Scenes/RoomCDev.unity` | `RoomC` |

The Memory development scene contains the working memory experiment. Room B and Room C start as empty rooms with floors/walls. All three contain a ready desktop player and XR testing setup. Add your objects under your room root, save the scene, and press Play. You do not need to edit in Prefab Mode.

Room layout/component edits saved in a development scene are packaged into a room prefab by the integration owner when ready. MainScene uses those integration snapshots; it does not automatically receive development-scene layout edits. Scripts, materials, and audio remain separate referenced assets in the same project. Keep the development scenes as working files; the final build uses **MainScene only**. Keep the testing player, EventSystem, interaction manager, and shared daylight outside the room prefab. Do not commit `Library`, `Temp`, or local builds; do commit each asset's `.meta` file.

### Contributions — complete before submission

- Yixiang Li: Memory Room concept and implementation, with AI coding assistance; name/features to be checked against the final version.
- Teammate B: name and features to be filled.
- Teammate C: name and features to be filled.

## Meta Quest build

Install **Android Build Support**, its SDK/NDK tools, and OpenJDK for this editor in Unity Hub. This project configures Android / ARM64 / IL2CPP / OpenXR with the Oculus Touch interaction profile and Quest support.

1. Use **FXR → Quest → Validate and build updated APK**. This configures mobile rendering, checks OpenXR validation and builds `Builds/Quest/FXRMemory.apk` from MainScene.
2. Confirm the school's installation/developer-mode permission, connect USB, and authorize the computer inside the headset.
3. On Yixiang's Mac, double-click `tools/Quest_Install.command` to install and launch the local APK. Builds are ignored by Git; teammates build their own APK with the menu above.
4. Follow the [English Quest guide](Docs/QUEST_SETUP_EN.md) or [中文 Quest 指南](Docs/QUEST_SETUP_CN.md) for device checks and automated CSV/log collection.

Do not enable a second XR provider or add another player rig. The school Quest model and developer-mode permissions still need confirmation. Editor and build checks do not establish headset compatibility or comfort. Confirm with the teacher whether Quest joystick movement satisfies the assignment's touchpad gliding/flying wording.

## Development tools and checks

Rider is optional. Scripts are ordinary C# files; Unity compiles them. GitHub Desktop is the recommended team interface. Unity Version Control (formerly Plastic SCM) is a separate system from Git; this repository uses Git/GitHub.

- **Window → General → Test Runner → EditMode → Run All** checks session order, scoring, timeout, rating, and Reset. **PlayMode → Run All** runs MainScene and MemoryDev through both rounds, checks card physics and CSV, and reviews preview controls, reset confirmation, settings locks, timeout, button targeting and text layout.
- **FXR → Rebuild generated prototype** regenerates MainScene and MemoryRoom.prefab. It can overwrite team edits: commit first and coordinate before using it.
- **FXR → Create missing development scenes** restores absent starter scenes only. It leaves existing development scenes and MainScene untouched. Do not delete your development scene to regenerate it: its layout edits may be newer than the integrated prefab.
- **FXR → Polish and integrate Memory Room** replaces generated furniture/planting/finish/UI in MemoryDev and updates the MemoryRoom prefab used by the existing MainScene. It keeps a local backup, but coordinate before running it after manually editing these generated areas.
- `tools/fetch_real_assets.py` prepares the selected CC0 models, textures and recorded ambience. Assets are already included: teammates do not need to run it. `tools/generate_room_audio.py` regenerates only short neutral task cues; the legacy placeholder ambience generator is retired.
- Development builds belong in the ignored `Builds/` folder. Put the final compressed demo video in `Submission/` and keep the submitted Unity source project below the course's 2 GB limit.

Third-party asset information is in [Attributions](Docs/ATTRIBUTIONS.md). Validation status is in [Development status](Docs/STATUS.md).
