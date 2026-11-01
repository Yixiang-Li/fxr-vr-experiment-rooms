# Joining and working together

## Integration preview — 1 November

The new preview branch is `integration/three-rooms-2026-11-01`. Your existing branches and local work are preserved. To review it, save/close Unity, commit your current work, Fetch in GitHub Desktop and switch to this branch, or clone it into a separate folder. Open with 2022.3.62f3 and choose **FXR → Integration → Play three rooms**. Use the top buttons, Tab and Restart. Source-scene editing remains in your own scene below; a layout change needs a new integration snapshot before it appears in MainScene.

Integration adds shared navigation, source-specific player binding and whole-room restart. It keeps the two owners’ experiment rules; Museum CSV now uses `Application.persistentDataPath/MuseumResults`, and pauses are marked. The misspelled Exhibit script filename was corrected while retaining its `.meta`.  Stroop desk interactions now use Input System coordinates consistently with the shared UI, fixing standalone Mac pointer offsets. Original owner branches are unchanged. Review integration fixes before merging them back into an owner branch; keep asset `.meta` files.


Private repository: [fxr-vr-experiment-rooms](https://github.com/Yixiang-Li/fxr-vr-experiment-rooms).

Use the exact course version **2022.3.62f3**, including `f3`. If it is absent from Hub, install from the [official Unity archive](https://unity.com/releases/editor/whats-new/2022.3.62f3). Apple Silicon Macs also need Rosetta 2 for this Editor's Intel helper tools.

## Desktop delivery — 14 October

The primary target is a desktop keyboard/mouse application. Android/Quest modules are optional. For the Memory demo, **FXR → Desktop → Play Memory Room** opens the source scene, starts Play and focuses the Game view. For your own contribution, continue to open RoomB.unity / AidaExperiment.unity normally. **FXR → Desktop → Build preview for this computer** produces a local Mac app or Windows executable.

The Memory task has Help, fixed task view, pause/focus-loss handling and an automatic CSV-folder button. Movement uses physical WASD positions or arrow keys; the HUD displays layout-specific letters. G switches glide/fly; F returns to the task. Refer to the root README for the complete controls. The course brief retains touch-pad navigation wording; the group's desktop plan has not been independently confirmed in a new teacher reply.

## Updating an existing Unity 6 checkout

1. Save your work and close Unity. Commit any local changes on your own branch before updating.
2. In GitHub Desktop, switch to `main`, fetch and pull the migration update.
3. Open the project with **2022.3.62f3** in Hub. Allow the first import to finish, then open your existing development scene and press Play.

The room folders and scenes are retained. If your branch still has Unity 6 changes, coordinate with the integration owner before merging it so old package/settings files do not replace the course version.

## Quick overview

- We share **one Unity project and one GitHub repository**, using **Unity 2022.3.62f3**. Everyone works on their own local copy.
- Each person develops their own room in **MemoryDev**, **RoomB**, or **AidaExperiment**. Edit normally, save, and press **Play** to test. Prefab Mode is not required for daily work.
- When a room is ready, the integration owner saves its room objects as a prefab and places it in **MainScene**, which connects all three rooms for the final experience.
- Keep the development scenes. The final build starts from **MainScene only**. The integration preview now contains all three submitted rooms. The old RoomBDev/RoomCDev shells remain historical files.

## Access status — 14 October

Both `aidasdl` and `zhaiyuming283-rgb` have accepted the invitations and have write access. No invitation needs to be sent again. Sign into that invited GitHub account before cloning the private repository.

## First setup for each teammate

1. Install [GitHub Desktop](https://desktop.github.com/download/) and sign into the invited account.
2. Accept the invitation. Use **File → Clone repository** in Desktop and clone this repository.
3. Install **Unity 2022.3.62f3** through Unity Hub. Install Android Build Support / SDK / NDK / OpenJDK if you will build for Quest.
4. In Unity Hub, use **Add project from disk** and select the cloned project folder.
5. Open your development scene from the table below and press **Play**. Rider is optional; Unity Version Control is not used here.

Everyone has a local copy of the same project. Git synchronizes committed changes.

## Windows and macOS

- Both use this same repository and **2022.3.62f3**. Clone to a short local folder, for example `C:\Projects\FXR` on Windows; each computer creates its own ignored `Library` cache on first import.
- Open the folder containing `Assets`, `Packages`, and `ProjectSettings`. The included models, textures and recorded audio are ready to import; Blender, Python and Rider are not required to edit a room or press Play.
- Windows desktop Play uses the same WASD / right-mouse look / left-click controls. The `.app` preview and `.command` launcher are Mac-specific; Windows uses the Unity Editor, or builds its own Windows player through Build Settings.
- Quest APK building uses **FXR → Quest → Validate and build updated APK** on either OS, after installing this Editor's Android modules. Python 3 is only needed for the optional install/data-collection helper; Windows has `tools/Quest_Install.bat`.
- After the first import, run **FXR → Check project references**, open your development scene and press Play. A repository audit and fresh Mac import have been completed; Windows Editor/graphics and Quest hardware still need a real device check. See [compatibility review](CROSS_PLATFORM_REVIEW.md).
- Include asset `.meta` files in commits. Keep the existing line-ending rules; do not commit caches, generated builds or your personal editor settings. Coordinate shared `Packages` / `ProjectSettings` changes with the integration owner.

## Ownership

| Folder | Contribution |
| --- | --- |
| `Assets/Rooms/Memory/` | Yixiang's Memory Room |
| `Assets/Rooms/RoomB/` | Yuming's Museum room |
| `Assets/Rooms/AidaRoom/` | Aïda's Stroop room |
| `Assets/Shared/Scenes/MainScene.unity` | One integration owner |

## Work in your own scene

| Person | Development scene | Your Hierarchy root |
| --- | --- | --- |
| Yixiang | `Assets/Rooms/Memory/Scenes/MemoryDev.unity` | `MemoryRoom` |
| Yuming | `Assets/Rooms/RoomB/Scenes/RoomB.unity` | Museum scene objects |
| Aïda | `Assets/Rooms/AidaRoom/Scenes/AidaExperiment.unity` | Stroop scene objects |

Double-click your `.unity` file in Unity's Project window. Each listed source scene contains its owner’s experiment. The old RoomBDev/RoomCDev files are starter shells, not the submitted room sources.

1. Put your models, lights, audio sources, UI, and interactive objects under your room root in the Hierarchy.
2. Edit normally in the scene, save, then press **Play** to test. Stop Play before editing; runtime object edits are usually discarded when Play stops.
3. Keep scripts, materials, models, and audio in your own room folder. Use distinct script namespaces.

You do not need Prefab Mode for daily work. Your scene stores room layout/component edits. When the room is ready, the integration owner packages the room root as a prefab and integrates it into MainScene. Development-scene layout edits do not automatically update MainScene; scripts/materials/audio remain separate referenced assets in the same project.

Each source scene keeps its owner’s test setup. Integration snapshots remove the personal player and EventSystem and bind to the shared player in MainScene. The off-screen Stroop monitor camera is retained. Keep all three development scenes in the project, but enable **MainScene only** in final build settings. On the integration branch, MainScene connects the three submitted room snapshots using the top navigation and shared player.

Coordinate before editing MainScene, another person's Prefab, Packages, or ProjectSettings. Avoid simultaneous edits to the same scene or prefab.


If you already have a room branch (for example `aida-room`), save/close Unity and commit your local work first. Fetch the repository, then merge the updated `main` into your room branch before reopening Unity. Keep your room changes; coordinate any shared-file conflicts with the integration owner.

## Daily workflow in GitHub Desktop

1. Save and close Unity. Switch to `main`, then **Fetch origin → Pull origin**.
2. **Branch → New branch**, using a descriptive name such as `room-b-first-version`. **Publish branch**.
3. Open Unity, open your own development scene, edit your room, and save.
4. Review changed files in Desktop, including asset `.meta` files. Add a short summary → **Commit to… → Push origin**.
5. **Create Pull Request** and ask the integration owner to review and merge it.
6. Pull main again before the next work session. Close Unity before switching branches or pulling so it does not overwrite changes from disk.

Do not upload a ZIP of the whole project or just a `.unity` file. Library, Temp, Logs, and Builds are ignored. Move and rename assets in Unity's Project window so their metadata follows.

## Conflicts and submission

Preserve your branch and tell the integration owner which files conflict. Do not blindly choose all local or all remote changes. The prefab owner resolves room changes; the integration owner handles MainScene.

The integration owner works on a separate branch, preserves source scenes, and packages the Museum/Stroop snapshots under `Assets/Integration/Prefabs/`. Memory keeps its existing prefab. Imported players and EventSystems are removed and references bind to MainScene’s shared player; the off-screen Stroop monitor camera stays. Do not rerun initial generation over the current integration. Check navigation, cancel/confirm, whole-room Restart and CSV persistence after changes.

**FXR → Create missing development scenes** creates absent starter scenes only; it never replaces an existing development scene. Do not delete your scene to regenerate it: it may contain newer changes than the integrated prefab.

Before submission, fill in the three names/contributions in README, check room switching and Restart on the delivery computer, complete a native Windows first-open/run check, and provide the teacher with access according to the course instructions. Put the compressed demo video in `Submission/`. Standard GitHub Git limits individual files to 100 MiB; compress or discuss Git LFS before adding a larger video.

References: [GitHub collaborator invitations](https://docs.github.com/en/repositories/managing-your-repositorys-settings-and-features/repository-access-and-collaboration/inviting-collaborators-to-a-personal-repository), [Unity Version Control](https://docs.unity.com/en-us/unity-version-control).
