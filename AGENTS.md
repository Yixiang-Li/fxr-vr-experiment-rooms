# Project conventions

- Unity editor is pinned to 6000.3.24f1. Keep package versions and .meta GUIDs stable.
- Final connected experience uses Assets/Shared/Scenes/MainScene.unity; rooms are separate prefabs with one owner each.
- Yixiang owns Assets/Rooms/Memory. Teammates own RoomB and RoomC. Shared scene/settings changes require coordination.
- Daily development uses Memory/Scenes/MemoryDev.unity, RoomB/Scenes/RoomBDev.unity, and RoomC/Scenes/RoomCDev.unity. Edit the ordinary room root in the owner's scene and press Play; Prefab Mode is not required.
- The owner's development scene is the source for room hierarchy/layout edits. The integration owner packages only the room root into its prefab and updates MainScene; shared players, EventSystem, interaction manager, and development daylight stay outside room prefabs. Keep development scenes out of final build settings.
- Keep a single shared XR rig and EventSystem. Desktop preview is a development mode.
- Preserve asset .meta files. Exclude Library, Temp, Logs, Builds, and user settings from Git.
- Rebuilding the generated prototype can overwrite MainScene/MemoryRoom; inspect and preserve team changes first.
- FXR/Create missing development scenes only creates absent scenes. Never overwrite an existing development scene from MainScene, which may contain an older integration snapshot.
- Use existing session tests for scoring/timeouts/reset; add tests only for real behavioral risks. Test on Quest before calling VR verified.
- Keep research inspiration separate from implemented evidence. 12 cards/180 seconds are prototype parameters.
