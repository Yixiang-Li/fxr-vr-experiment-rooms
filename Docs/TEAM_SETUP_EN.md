# Joining and working together

Private repository: [fxr-vr-experiment-rooms](https://github.com/Yixiang-Li/fxr-vr-experiment-rooms).

## Repository owner: invite the two teammates

1. Ask each teammate for their **GitHub username**.
2. Open the repository on GitHub → **Settings → Collaborators → Add people**.
3. Find and verify their account, send an invitation, and ask them to accept it.

The repository is private. A link alone does not grant access. No teammate invitations have been sent; exact usernames are needed first.

## First setup for each teammate

1. Install [GitHub Desktop](https://desktop.github.com/download/) and sign into the invited account.
2. Accept the invitation. Use **File → Clone repository** in Desktop and clone this repository.
3. Install **Unity 6000.3.24f1** through Unity Hub. Install Android Build Support / SDK / NDK / OpenJDK if you will build for Quest.
4. In Unity Hub, use **Add project from disk** and select the cloned project folder.
5. Open `Assets/Shared/Scenes/MainScene.unity` and press Play. Rider is optional; Unity Version Control is not used here.

Everyone has a local copy of the same project. Git synchronizes committed changes.

## Ownership

| Folder | Contribution |
| --- | --- |
| `Assets/Rooms/Memory/` | Yixiang's Memory Room |
| `Assets/Rooms/RoomB/` | Teammate B's room |
| `Assets/Rooms/RoomC/` | Teammate C's room |
| `Assets/Shared/Scenes/MainScene.unity` | One integration owner |

Build each room as its own **Prefab**, with scripts, materials, models, and audio in its own folder. Use distinct script namespaces. Personal test scenes are fine during development; the final experience starts in MainScene and connects all three rooms. Use the existing shared XR player and EventSystem when integrating.

Coordinate before editing MainScene, another person's Prefab, Packages, or ProjectSettings. Avoid simultaneous edits to the same scene or prefab.

## Daily workflow in GitHub Desktop

1. Save and close Unity. Switch to `main`, then **Fetch origin → Pull origin**.
2. **Branch → New branch**, using a descriptive name such as `room-b-first-version`. **Publish branch**.
3. Open Unity, work in your own folder, and save.
4. Review changed files in Desktop, including asset `.meta` files. Add a short summary → **Commit to… → Push origin**.
5. **Create Pull Request** and ask the integration owner to review and merge it.
6. Pull main again before the next work session. Close Unity before switching branches or pulling so it does not overwrite changes from disk.

Do not upload a ZIP of the whole project or just a `.unity` file. Library, Temp, Logs, and Builds are ignored. Move and rename assets in Unity's Project window so their metadata follows.

## Conflicts and submission

Preserve your branch and tell the integration owner which files conflict. Do not blindly choose all local or all remote changes. The prefab owner resolves room changes; the integration owner handles MainScene.

Before submission, fill in the three names/contributions in README, check connected entrances and Reset, test on the school Quest, and provide the teacher with access according to the course instructions. Put the compressed demo video in `Submission/`. Standard GitHub Git limits individual files to 100 MiB; compress or discuss Git LFS before adding a larger video.

References: [GitHub collaborator invitations](https://docs.github.com/en/repositories/managing-your-repositorys-settings-and-features/repository-access-and-collaboration/inviting-collaborators-to-a-personal-repository), [Unity Version Control](https://docs.unity.com/en-us/unity-version-control).
