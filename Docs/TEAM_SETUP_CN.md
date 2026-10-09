# 组员加入与协作

私有仓库：[fxr-vr-experiment-rooms](https://github.com/Yixiang-Li/fxr-vr-experiment-rooms)。

## 你先做：邀请两位组员

1. 让他们注册 / 登录 GitHub，把 **GitHub 用户名**发给你。
2. 打开项目的 GitHub 页面 → **Settings → Collaborators → Add people**。
3. 搜索用户名，核对账号，发送邀请。他们接受后就能访问这个私有仓库。

仓库链接本身不会给别人访问权限。还没有邀请任何组员；必须先知道准确的账号。

## 组员第一次加入

1. 安装 [GitHub Desktop](https://desktop.github.com/download/)，登录收到邀请的 GitHub 账号。
2. 接受 GitHub 邀请，然后在 Desktop 中 **File → Clone repository**，选择本项目，存到自己电脑。
3. Unity Hub 安装 **6000.3.24f1**。需要 Quest 打包的人额外安装 Android Build Support / SDK / NDK / OpenJDK。
4. Unity Hub → **Add project from disk**，选刚才克隆的整个项目文件夹。
5. 打开 `Assets/Shared/Scenes/MainScene.unity`，Play。无需购买 Rider，也无需启用 Unity Version Control。

每个人电脑上都有完整项目的一份副本，Git 同步大家提交的改动。

## 每个人改自己的房间

- 你的记忆房：`Assets/Rooms/Memory/`。
- 第二位：`Assets/Rooms/RoomB/`。
- 第三位：`Assets/Rooms/RoomC/`。
- 每个房间做成自己的 **Prefab**。使用各自文件夹保存模型、音频、材质和脚本；脚本使用不同命名空间，避免同名类。
- `MainScene` 由一个人负责整合：把三个 Prefab 放入同一个连通场景，留好入口，统一使用现有 XR Rig 和 EventSystem。
- 可以用个人临时 Scene 搭房间，但最终要从 MainScene 连续进入三个房间。

**不要三个人同时改 MainScene、同一个 Prefab、Packages 或 ProjectSettings。** 想改共享配置先在群里说明。

## 每次工作的最简单流程

1. 保存并关闭 Unity。GitHub Desktop 切到 `main`，点 **Fetch origin → Pull origin**。
2. **Branch → New branch**，用自己的房间名，例如 `room-b-first-version`；点击 **Publish branch**。
3. 打开 Unity，改自己的文件夹，保存。
4. 回到 Desktop 检查改动；资源与 `.meta` 要一起提交。写一句修改说明 → **Commit to… → Push origin**。
5. 点 **Create Pull Request**，请负责整合的人检查并合并。一个分支对应一批改动即可。
6. 下一次工作前再拉取 main；切分支、拉取或合并时先保存并关闭 Unity，避免编辑器覆盖刚更新的文件。

`Library`、`Temp`、`Logs` 和 `Builds` 已自动忽略。不要重新打包整个文件夹上传，也不要只上传 `.unity` 文件。新资源尽量在 Unity 的 Project 窗口移动 / 改名，让 `.meta` 跟随。

## 发生冲突

不要直接选“全部用我的”。先保留各自分支，告诉整合负责人冲突文件名。Prefab 或 Scene 冲突通常由文件所有者处理；共享场景由整合负责人重新放入已完成的房间 Prefab。

## 交付前

- 填写 README 的三个人名字、贡献、功能和操作说明。
- 三个房间都能从 MainScene 进入，Reset 与共同玩家不冲突。
- 在学校 Quest 实测；Mac Play Mode 通过不等于 VR 已通过。
- 仓库保持可供老师访问，按提交要求提供权限 / 链接；不要放个人账号资料。
- 压缩演示视频后放 `Submission/`。GitHub 普通 Git 的单文件限制为 100 MiB；超过时先讨论压缩或 Git LFS，不强行提交。

参考：[GitHub 官方邀请说明](https://docs.github.com/en/repositories/managing-your-repositorys-settings-and-features/repository-access-and-collaboration/inviting-collaborators-to-a-personal-repository)、[Unity Version Control](https://docs.unity.com/en-us/unity-version-control)。
