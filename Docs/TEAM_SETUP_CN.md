> 11 月 4 日：已完成整合复查 Aïda 两阶段 Stroop 和 Yuming 博物馆新版。组员继续推个人分支；源 Scene 布局修改仍需同步整合快照。不要重新运行初次三房间生成器。

# 组员加入与协作

## 整合预览：11 月 1 日

预览分支是 `integration/three-rooms-2026-11-01`。队友原分支和各自电脑上的文件保持原样。想查看时，先保存／关闭 Unity 并提交自己的工作，在 GitHub Desktop Fetch 后切到这个分支，或另克隆一份；用 2022.3.62f3 打开，点 **FXR → Integration → Play three rooms**。顶部按钮切房间，Tab 打开菜单，Restart 重置整间房。日常仍编辑下面表里的个人源 Scene，布局改动需要更新整合快照才会出现在 MainScene。

整合加入公共导航、共享玩家绑定与完整房间重启；博物馆 CSV 改存用户数据目录 MuseumResults，菜单中断会标记；修正 Exhibit 脚本拼写并保留 `.meta`。这些改动只在整合分支，合回个人分支前先审阅，避免覆盖尚未提交的本地工作。


私有仓库：[fxr-vr-experiment-rooms](https://github.com/Yixiang-Li/fxr-vr-experiment-rooms)。

版本已按课堂要求调整为 **2022.3.62f3**，包括 `f3`。若 Hub 列表没有此版本，从 [Unity 官方归档页](https://unity.com/releases/editor/whats-new/2022.3.62f3)安装。Apple Silicon Mac 还需要 Rosetta 2，因为此编辑器的部分内置工具为 Intel 程序。

## 电脑键鼠交付：10 月 14 日

当前主要交付电脑键盘／鼠标版本，Android／Quest 组件只作为可选项。**FXR → Desktop → Play Memory Room** 会自动打开记忆房开发场景、开始 Play 并聚焦 Game 窗口；组员仍正常打开自己的 RoomB.unity／AidaExperiment.unity。**FXR → Desktop → Build preview for this computer** 一键生成本机 Mac／Windows 程序。

记忆房已加帮助、固定任务视角、暂停／失焦处理与打开 CSV 目录。移动支持物理 WASD 位置或方向键，提示会随键盘布局显示对应字母；G 切滑行／飞行，F 回任务视角。完整操作见根 README。PDF 的 touch pad 字面要求如何对应键鼠，仍须按课堂说明确认；目前桌面计划来自组内沟通。

## 已经下载 Unity 6 项目的组员如何更新

1. 保存并关闭 Unity；有本地修改时，先在自己的分支提交。
2. 在 GitHub Desktop 切到 `main`，Fetch 后 Pull。
3. 在 Hub 中选择 **2022.3.62f3** 打开项目，等首次导入结束，再打开自己的开发 Scene 点 Play。

房间文件夹和开发 Scene 都保留。如果自己的分支仍包含 Unity 6 的改动，合并前与整合负责人协调，避免旧的插件和设置覆盖指定版本。

## 访问状态：10 月 14 日

`aidasdl` 和 `zhaiyuming283-rgb` 都已接受邀请，已有 write 权限，无需再次邀请。克隆私有仓库时登录对应的 GitHub 账号。

## 组员第一次加入

1. 安装 [GitHub Desktop](https://desktop.github.com/download/)，登录收到邀请的 GitHub 账号。
2. 接受 GitHub 邀请，然后在 Desktop 中 **File → Clone repository**，选择本项目，存到自己电脑。
3. Unity Hub 安装 **2022.3.62f3**。需要 Quest 打包的人额外安装 Android Build Support / SDK / NDK / OpenJDK。
4. Unity Hub → **Add project from disk**，选刚才克隆的整个项目文件夹。
5. 打开下面表格中自己的开发 Scene，点 **Play**。无需购买 Rider，也无需启用 Unity Version Control。

每个人电脑上都有完整项目的一份副本，Git 同步大家提交的改动。

## Windows 与 Mac 一起开发

- 两种电脑使用当前同一个仓库、同一个 **2022.3.62f3**。Windows 建议下载到较短的本地路径，例如 `C:\Projects\FXR`；首次打开时，各自生成不上传 Git 的 `Library` 缓存。
- 在 Hub 中打开包含 `Assets`、`Packages`、`ProjectSettings` 的整个项目目录。模型、材质、录音已经包含，无需安装 Blender、Python 或 Rider 就能编辑房间、点 Play。
- Windows 的桌面预览同样使用 WASD、按住鼠标右键看方向、左键点击。`.app` 预览和 `.command` 启动器是 Mac 用的；Windows 直接用 Unity，或在 Build Settings 中生成自己的 Windows 程序。
- 两边都可用 **FXR → Quest → Validate and build updated APK**，前提是安装对应版本的 Android 组件。可选的安装／数据取回脚本需要 Python 3；Windows 已有 `tools/Quest_Install.bat`。
- 首次导入后，运行 **FXR → Check project references**，再打开自己的开发 Scene 点 Play。已完成仓库审查及 Mac 全新导入验证；Windows 图形／编辑器和 Quest 仍需要真机检查。详见[兼容性审查](CROSS_PLATFORM_REVIEW.md)。
- 资源与 `.meta` 一起提交，保留仓库的换行规则。缓存、打包结果和个人设置不提交；共享 `Packages`／`ProjectSettings` 的修改先协调。

## 每个人改自己的房间

| 人员 | 打开的开发 Scene | Hierarchy 中自己的房间 |
| --- | --- | --- |
| 你 | `Assets/Rooms/Memory/Scenes/MemoryDev.unity` | `MemoryRoom` |
| Yuming | `Assets/Rooms/RoomB/Scenes/RoomB.unity` | 博物馆场景物体 |
| Aïda | `Assets/Rooms/AidaRoom/Scenes/AidaExperiment.unity` | Stroop 场景物体 |

在 Unity 的 Project 窗口里双击自己的 `.unity` 文件，就能打开 Scene。表中三个源 Scene 各有对应房间；RoomBDev/RoomCDev 是保留的旧模板，不是队友这次上传的房间。

1. 展开 Hierarchy 中自己的房间物体，把模型、灯光、音源、交互物体等放在它下面。
2. 在 Scene 中直接编辑，保存后点 **Play** 测试。停止 Play 后再改；Play 时的物体修改通常不会保留。
3. 模型、音频、材质和脚本保存在自己的房间文件夹里；脚本使用不同命名空间，避免同名类。

**平时不用进入 Prefab Mode。** 各自 Scene 保存房间布局与组件设置，整合负责人在房间准备好后，将房间总物体打包为 Prefab，放进 `MainScene`。开发 Scene 中的布局修改不会自动同步到 MainScene；脚本、材质、音频仍然是同一项目中的资源，通过引用使用。

各自源 Scene 保留原有测试玩家与环境设置。整合快照移除个人玩家和 EventSystem，再绑定 MainScene 的共享玩家；Stroop 显示器专用的离屏摄像机仍保留。布局只存在于源 Scene 时，不会自动出现在 MainScene。

三个开发 Scene 都可以保留在项目里。**最终打包只启用 MainScene**，从里面连续进入三个房间；无需删除开发 Scene。整合分支的 MainScene 现通过顶部导航和共享玩家连接三个已上传房间。

**不要三个人同时改 MainScene、同一个 Prefab、Packages 或 ProjectSettings。** 想改共享配置先在群里说明。


已有自己的分支（例如 `aida-room`）时：保存／关闭 Unity，先提交本地修改，Fetch 后把更新的 `main` 合入自己的房间分支，再打开 Unity。保留自己的房间工作；共享文件冲突由整合负责人协助处理。

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

1. 在独立整合分支接入组员提交，保留各自源 Scene 和分支。
2. Memory 使用原有 Prefab；博物馆和 Stroop 的整合快照位于 `Assets/Integration/Prefabs/`，不带个人玩家或 EventSystem。保留 Stroop 显示器的离屏摄像机。
3. 更新 MainScene 对应快照与玩家绑定；不要重复运行初次生成工具覆盖已整合场景。
4. 从 MainScene 检查三个房间的实际输入、菜单、取消、确认重启和 CSV；个人 Scene 继续保留。

`FXR → Create missing development scenes` 只补建不存在的初始模板，不覆盖已有开发 Scene。不要删除自己的 Scene 来重新生成。

## 交付前

- 填写 README 的三个人名字、贡献、功能和操作说明。
- 三个房间都能从 MainScene 进入，Reset 与共同玩家不冲突。
- 在最终交付电脑检查键鼠、文字、音量、切房与 Restart。Windows 尚需实际首次打开与运行检查；Quest 仅为保留的可选历史配置。
- 仓库保持可供老师访问，按提交要求提供权限 / 链接；不要放个人账号资料。
- 压缩演示视频后放 `Submission/`。GitHub 普通 Git 的单文件限制为 100 MiB；超过时先讨论压缩或 Git LFS，不强行提交。

参考：[GitHub 官方邀请说明](https://docs.github.com/en/repositories/managing-your-repositorys-settings-and-features/repository-access-and-collaboration/inviting-collaborators-to-a-personal-repository)、[Unity Version Control](https://docs.unity.com/en-us/unity-version-control)。
