# Joining and working together

Private repository: [fxr-vr-experiment-rooms](https://github.com/Yixiang-Li/fxr-vr-experiment-rooms).

Use the exact course version **2022.3.62f3**, including `f3`. If it is absent from Hub, install from the [official Unity archive](https://unity.com/releases/editor/whats-new/2022.3.62f3). Apple Silicon Macs also need Rosetta 2 for this Editor's Intel helper tools.

## Quick overview

- We share **one Unity project and one GitHub repository**, using **Unity 2022.3.62f3**. Everyone works on their own local copy.
- Each person develops their own room in **MemoryDev**, **RoomBDev**, or **RoomCDev**. Edit normally, save, and press **Play** to test. Prefab Mode is not required for daily work.
- When a room is ready, the integration owner saves its room objects as a prefab and places it in **MainScene**, which connects all three rooms for the final experience.
- Keep the development scenes. The final build starts from **MainScene only**. Room B and Room C currently contain empty room shells; their tasks are still to be built.

## Repository owner: invite the two teammates

1. Ask each teammate for their **GitHub username or GitHub account email address**.
2. Open the repository on GitHub → **Settings → Collaborators → Add people**.
3. Search by username or email, verify the account/address, send an invitation, and ask them to accept it.

The repository is private. A link alone does not grant access. Invitations can be sent directly to an email address; the teammate must sign into GitHub and accept the invitation.

## First setup for each teammate

1. Install [GitHub Desktop](https://desktop.github.com/download/) and sign into the invited account.
2. Accept the invitation. Use **File → Clone repository** in Desktop and clone this repository.
3. Install **Unity 2022.3.62f3** through Unity Hub. Install Android Build Support / SDK / NDK / OpenJDK if you will build for Quest.
4. In Unity Hub, use **Add project from disk** and select the cloned project folder.
5. Open your development scene from the table below and press **Play**. Rider is optional; Unity Version Control is not used here.

Everyone has a local copy of the same project. Git synchronizes committed changes.

## Ownership

| Folder | Contribution |
| --- | --- |
| `Assets/Rooms/Memory/` | Yixiang's Memory Room |
| `Assets/Rooms/RoomB/` | Teammate B's room |
| `Assets/Rooms/RoomC/` | Teammate C's room |
| `Assets/Shared/Scenes/MainScene.unity` | One integration owner |

## Work in your own scene

| Person | Development scene | Your Hierarchy root |
| --- | --- | --- |
| Yixiang | `Assets/Rooms/Memory/Scenes/MemoryDev.unity` | `MemoryRoom` |
| Teammate B | `Assets/Rooms/RoomB/Scenes/RoomBDev.unity` | `RoomB` |
| Teammate C | `Assets/Rooms/RoomC/Scenes/RoomCDev.unity` | `RoomC` |

Double-click your `.unity` file in Unity's Project window. The Memory scene contains the working memory task; B and C start with a floor and walls.

1. Put your models, lights, audio sources, UI, and interactive objects under your room root in the Hierarchy.
2. Edit normally in the scene, save, then press **Play** to test. Stop Play before editing; runtime object edits are usually discarded when Play stops.
3. Keep scripts, materials, models, and audio in your own room folder. Use distinct script namespaces.

You do not need Prefab Mode for daily work. Your scene stores room layout/component edits. When the room is ready, the integration owner packages the room root as a prefab and integrates it into MainScene. Development-scene layout edits do not automatically update MainScene; scripts/materials/audio remain separate referenced assets in the same project.

Each development scene already includes a testing player, XR Interaction Manager, EventSystem, and shared daylight outside your room root. Keep those out of your room prefab: MainScene supplies the shared player and interaction setup. Keep all three development scenes in the project, but enable **MainScene only** in final build settings. MainScene currently contains the Memory Room and two entrance placeholders; B/C still need their own tasks.

Coordinate before editing MainScene, another person's Prefab, Packages, or ProjectSettings. Avoid simultaneous edits to the same scene or prefab.

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

The integration owner opens each owner's development scene, verifies the room, and saves only the room root into that owner's `Prefabs/` folder. Update the existing MemoryRoom prefab; create RoomB/RoomC prefabs on first integration. Leave the testing player, EventSystem, XR Interaction Manager, and shared daylight outside the prefab. Place the finished prefabs in MainScene, connect entrances, save, and test the shared player and Reset. Keep the development scenes.

**FXR → Create missing development scenes** creates absent starter scenes only; it never replaces an existing development scene. Do not delete your scene to regenerate it: it may contain newer changes than the integrated prefab.

Before submission, fill in the three names/contributions in README, check connected entrances and Reset, test on the school Quest, and provide the teacher with access according to the course instructions. Put the compressed demo video in `Submission/`. Standard GitHub Git limits individual files to 100 MiB; compress or discuss Git LFS before adding a larger video.

References: [GitHub collaborator invitations](https://docs.github.com/en/repositories/managing-your-repositorys-settings-and-features/repository-access-and-collaboration/inviting-collaborators-to-a-personal-repository), [Unity Version Control](https://docs.unity.com/en-us/unity-version-control).
