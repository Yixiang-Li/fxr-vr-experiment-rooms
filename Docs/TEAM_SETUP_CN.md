# 组员加入与协作

私有仓库：[fxr-vr-experiment-rooms](https://github.com/Yixiang-Li/fxr-vr-experiment-rooms)。

版本已按课堂要求调整为 **2022.3.62f3**，包括 `f3`。若 Hub 列表没有此版本，从 [Unity 官方归档页](https://unity.com/releases/editor/whats-new/2022.3.62f3)安装。Apple Silicon Mac 还需要 Rosetta 2，因为此编辑器的部分内置工具为 Intel 程序。

## 你先做：邀请两位组员

1. 让他们注册 / 登录 GitHub，把 **GitHub 用户名或 GitHub 账号邮箱**发给你。
2. 打开项目的 GitHub 页面 → **Settings → Collaborators → Add people**。
3. 搜索用户名或邮箱，核对账号 / 邮箱，发送邀请。他们接受后就能访问这个私有仓库。

仓库链接本身不会给别人访问权限。邀请可以直接发到邮箱，组员仍需登录 GitHub 并接受邀请。

## 组员第一次加入

1. 安装 [GitHub Desktop](https://desktop.github.com/download/)，登录收到邀请的 GitHub 账号。
2. 接受 GitHub 邀请，然后在 Desktop 中 **File → Clone repository**，选择本项目，存到自己电脑。
3. Unity Hub 安装 **2022.3.62f3**。需要 Quest 打包的人额外安装 Android Build Support / SDK / NDK / OpenJDK。
4. Unity Hub → **Add project from disk**，选刚才克隆的整个项目文件夹。
5. 打开下面表格中自己的开发 Scene，点 **Play**。无需购买 Rider，也无需启用 Unity Version Control。

每个人电脑上都有完整项目的一份副本，Git 同步大家提交的改动。

## 每个人改自己的房间

| 人员 | 打开的开发 Scene | Hierarchy 中自己的房间 |
| --- | --- | --- |
| 你 | `Assets/Rooms/Memory/Scenes/MemoryDev.unity` | `MemoryRoom` |
| 第二位 | `Assets/Rooms/RoomB/Scenes/RoomBDev.unity` | `RoomB` |
| 第三位 | `Assets/Rooms/RoomC/Scenes/RoomCDev.unity` | `RoomC` |

在 Unity 的 Project 窗口里双击自己的 `.unity` 文件，就能打开 Scene。你的 Scene 已包含完整记忆任务；B / C 当前有地板和墙壁，供组员开始搭建。

1. 展开 Hierarchy 中自己的房间物体，把模型、灯光、音源、交互物体等放在它下面。
2. 在 Scene 中直接编辑，保存后点 **Play** 测试。停止 Play 后再改；Play 时的物体修改通常不会保留。
3. 模型、音频、材质和脚本保存在自己的房间文件夹里；脚本使用不同命名空间，避免同名类。

**平时不用进入 Prefab Mode。** 各自 Scene 保存房间布局与组件设置，整合负责人在房间准备好后，将房间总物体打包为 Prefab，放进 `MainScene`。开发 Scene 中的布局修改不会自动同步到 MainScene；脚本、材质、音频仍然是同一项目中的资源，通过引用使用。

每个开发 Scene 已带测试玩家、XR Interaction Manager、EventSystem 和公共日光。这些是测试环境，放在房间总物体外。整合时只打包房间物体，三个房间共同使用 MainScene 的玩家和交互设置。

三个开发 Scene 都可以保留在项目里。**最终打包只启用 MainScene**，从里面连续进入三个房间；无需删除开发 Scene。MainScene 当前只有记忆房和另两个房间的入口占位，B / C 的具体任务仍由组员开发。

**不要三个人同时改 MainScene、同一个 Prefab、Packages 或 ProjectSettings。** 想改共享配置先在群里说明。

## 每次工作的最简单流程

1. 保存并关闭 Unity。GitHub Desktop 切到 `main`，点 **Fetch origin → Pull origin**。
2. **Branch → New branch**，用自己的房间名，例如 `room-b-first-version`；点击 **Publish branch**。
3. 打开 Unity，双击自己的开发 Scene，改自己的房间并保存。
4. 回到 Desktop 检查改动；资源与 `.meta` 要一起提交。写一句修改说明 → **Commit to… → Push origin**。
5. 点 **Create Pull Request**，请负责整合的人检查并合并。一个分支对应一批改动即可。
6. 下一次工作前再拉取 main；切分支、拉取或合并时先保存并关闭 Unity，避免编辑器覆盖刚更新的文件。

`Library`、`Temp`、`Logs` 和 `Builds` 已自动忽略。不要重新打包整个文件夹上传，也不要只上传 `.unity` 文件。新资源尽量在 Unity 的 Project 窗口移动 / 改名，让 `.meta` 跟随。

## 发生冲突

不要直接选“全部用我的”。先保留各自分支，告诉整合负责人冲突文件名。Prefab 或 Scene 冲突通常由文件所有者处理；共享场景由整合负责人重新放入已完成的房间 Prefab。

## 整合负责人做什么

1. 合并组员的提交，打开对应开发 Scene，确认房间可以运行。
2. 只把 `MemoryRoom` / `RoomB` / `RoomC` 总物体保存成各自 `Prefabs/` 文件夹里的 Prefab；不带测试玩家、EventSystem、XR Interaction Manager 或公共日光。
3. 记忆房整合时更新已有 `MemoryRoom.prefab`；B / C 初次整合时创建各自的 Prefab。打开 MainScene，放好房间、连接入口并保存。
4. 从 MainScene 测试三个房间，确认共同玩家、Reset 和交互正常。个人开发 Scene 继续保留。

`FXR → Create missing development scenes` 只补建不存在的初始 Scene，不覆盖已有开发 Scene。不要为重新生成而删除自己的 Scene；它可能包含尚未整合的最新修改。

## 交付前

- 填写 README 的三个人名字、贡献、功能和操作说明。
- 三个房间都能从 MainScene 进入，Reset 与共同玩家不冲突。
- 在学校 Quest 实测；Mac Play Mode 通过不等于 VR 已通过。
- 仓库保持可供老师访问，按提交要求提供权限 / 链接；不要放个人账号资料。
- 压缩演示视频后放 `Submission/`。GitHub 普通 Git 的单文件限制为 100 MiB；超过时先讨论压缩或 Git LFS，不强行提交。

参考：[GitHub 官方邀请说明](https://docs.github.com/en/repositories/managing-your-repositorys-settings-and-features/repository-access-and-collaboration/inviting-collaborators-to-a-personal-repository)、[Unity Version Control](https://docs.unity.com/en-us/unity-version-control)。
