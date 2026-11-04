# Project conventions

- Unity editor is pinned to 2022.3.62f3. Keep package versions and .meta GUIDs stable.
- Final connected experience uses Assets/Shared/Scenes/MainScene.unity; rooms are separate prefabs with one owner each.
- Yixiang owns Assets/Rooms/Memory, Yuming owns Assets/Rooms/RoomB, Aïda owns Assets/Rooms/AidaRoom. Shared scene/settings changes require coordination.
- Daily sources: Memory/Scenes/MemoryDev.unity, RoomB/Scenes/RoomB.unity, AidaRoom/Scenes/AidaExperiment.unity. Edit normally and press Play; Prefab Mode is not required. RoomBDev/RoomCDev are historical starter shells.
- Owner scenes are layout sources. MainScene contains their integration snapshots. Imported Museum/Stroop prefabs are under Assets/Integration/Prefabs; strip owner players/EventSystems, rebind to the shared player, and preserve the off-screen Stroop RenderTexture camera. Keep development scenes out of final build settings. MainScene has desktopOnly enabled.
- Primary delivery is desktop keyboard/mouse, explicitly requested by the owner on 2026-10-14. Keep one shared active player and EventSystem; retain the optional XR rig/simulator without claiming headset validation.
- Memory rounds use a fixed task view. Preserve pause/focus-loss handling, interruption logging, rating gates and result withholding. Practice manipulation and free navigation are outside the measured task.
- FXR/Desktop/Update Memory desktop setup adds known desktop features incrementally in the existing owner scene/prefab. Preserve existing object IDs; do not replace the prefab wholesale from an unpacked scene to apply small updates.
- Preserve asset .meta files. Exclude Library, Temp, Logs, Builds, and user settings from Git.
- Rebuilding the generated prototype can overwrite MainScene/MemoryRoom; inspect and preserve team changes first.
- FXR/Create missing development scenes only creates absent scenes. Never overwrite an existing development scene from MainScene, which may contain an older integration snapshot.
- Use existing session tests for scoring/timeouts/reset; add tests only for real behavioral risks. Test on Quest before calling VR verified.
- Keep research inspiration separate from implemented evidence. 12 cards/180 seconds are prototype parameters.

- Review integration on integration/three-rooms-2026-11-01 without rewriting owner branches or main. ThreeRoomBuilder.Build is initial-only: never regenerate over the existing integration. Update snapshots deliberately and retain source scenes.
- Global room navigation pauses timing/audio, releases the cursor and confirms destructive progress reset. Preserve CSV files; Museum results use Application.persistentDataPath/MuseumResults. Test actual UI ray hits as well as button handlers.

- Stroop now uses two 30-second blocks and a 3-second transition. Preserve latest source physics/audio/positioning; do not restore the old fixed-trial protocol. Use the targeted Refresh Aida snapshot menu when updating this owner scene, not the initial three-room generator.
- DeskPointer samples one hit per frame so automatic Start positioning cannot turn the same click into an answer. Keep task buttons and the consent paper inside the shared camera view; retain shared pitch/yaw when movement resumes.
