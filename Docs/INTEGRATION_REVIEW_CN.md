# 三房间整合审查 — 2026-11-01

## 本次结果

预览位于独立分支 `integration/three-rooms-2026-11-01`，本机工作目录是 `/Users/yixiangli/Unity/FXR-Integration`。原 `/Users/yixiangli/Unity/FXR-Group-Project`、main、aida-room、yuming-room2 保留；不是在队友分支上重写。整合前已保存本地 Git bundle 备份。

用 **Unity 2022.3.62f3** 打开整合项目，选择 **FXR → Integration → Play three rooms**。Stroop 接受同意书并等它关闭后，可用下方 **START TASK** 或桌面实体 Start 开始。顶部切房；博物馆锁定鼠标时按 **Tab** 释放并打开菜单。Restart 重置当前房间，切房／重启都须确认；已保存 CSV 不删除。

## 如何满足共同 Scene 要求

老师 10 月 6 日邮件要求 one Unity project，三个房间由同一 Scene 连接。最终只打包 `Assets/Shared/Scenes/MainScene.unity`，运行中始终一个 Scene；三个房间通过快照 Prefab 在该 Scene 内启停和重建。采用 Prefab 是实施选择，不是老师指定的文件格式。

- Memory 保留原有源 Scene／Prefab，办公室与自然环境实验不重建。
- Yuming 源：`Assets/Rooms/RoomB/Scenes/RoomB.unity`；Aïda 源：`Assets/Rooms/AidaRoom/Scenes/AidaExperiment.unity`。
- 两个新快照在 `Assets/Integration/Prefabs/`。个人源 Scene 的布局修改需要重新同步快照，日常无需进入 Prefab Mode。
- 只有一个共享玩家、面向玩家的摄像机、AudioListener 和 EventSystem。Stroop 另保留显示器专用的离屏摄像机。

## 修复和边界

- 修正 Exhibit 脚本文件名与类名不一致，保留原 `.meta` GUID；导入房间重新绑定共享玩家与摄像机，保留原实验规则。
- 原 Standard 材质转换为现有 URP；未升级包或更换 Editor。
- 共享玩家保留物理碰撞，但放入 Ignore Raycast 层，避免源相机偏移时点击先命中自己；显示器离屏摄像机也关闭物理点击事件。
- 全局菜单冻结计时／音频并释放鼠标，取消恢复原状态；顶部导航保持可点击。切房重建整个实例，恢复 consent、控制器和物体。
- Memory 菜单中断沿用原记录；Museum 增加 interruption 列，取消菜单补偿 quiz 实时时钟。中断会话不宜直接与连续会话比较。
- Museum CSV 改用用户数据目录的 `MuseumResults`，避免写入只读安装目录。Memory CSV 保留原路径。Stroop 目前显示成绩／平均反应时间，未新增 CSV。
- Mac 独立程序诊断确认旧鼠标位置与 Input System 位置不一致；Stroop 纸面、实体按钮和灯具改用与共享 UI 相同的 Input System 坐标，保留原实验规则。
- Stroop 显示器的离屏 Canvas 关闭输入射线，避免看不见的显示界面吞掉同意书／桌面点击。保留 consent 门槛、实体 Start／颜色按钮和灯具控制；共享提示显示当前试次与完成成绩；增加清楚的 START TASK 入口，并区分接受后等待关闭／拒绝后需重启的状态。原模型的少量导入警告没有阻断运行，视觉仍应请房间作者复核。

## 已做检查

| 范围 | 结果 |
| --- | --- |
| 完整 PlayMode | 8/8 通过，包含记忆房原有完整流程／输入检查与三房间测试 |
| EditMode | 2/2 通过 |
| 三房间最终定向测试 | 3/3 通过，含纸面、实体 Start 及共享 START TASK 的实际 Input System 指针事件 |
| 点击遮挡 | 切房按钮第一命中、纸面物理射线与无离屏 UI 遮挡均通过 |
| Museum | 12 次观察、干扰、12 题、CSV、重启后文件保留 |
| Stroop | 未 consent 无法 Start；接受后完成 10 次响应；成绩与全房重置 |
| 共同运行环境 | 单 Scene／玩家输入／面向玩家摄像机／AudioListener／EventSystem |

自动化测试使用短时长和自动回答，仅证明流程。截图是 Unity 实际渲染，数值是测试数据，不能作为参与者研究结果。XML／日志在本机忽略目录 `Logs/integration-*`；渲染在 `Builds/Review/integration-*`。

Mac 独立程序 **0.5.0** 已成功构建并实际启动：公共菜单、Memory → Museum → Stroop、博物馆 Start／Tab 释放鼠标、Stroop 纸面／接受／START TASK／实体颜色响应均已操作检查。Stroop 取消 Restart 保留 Trial 2，确认 Restart 恢复未 consent 的初始状态。原实体 Start 的近距离指针检查由自动化覆盖；本机演示使用清楚的 START TASK。检查的 Player.log 没有异常行。本次没有手动走完 12×20 秒博物馆或 10 次 Stroop，完整流程由上述自动化测试覆盖。

资源静态审查：既有元数据 GUID 均保留，无重复 GUID／Windows 非法文件名；Memory 原源 Scene／Prefab 没有删除物体标识。源文件约 **651 MiB**，低于 2 GB；Packages 版本保持原样。

### 实际 Unity 渲染（自动化测试画面）

![Memory](Images/integration-memory.png)

![Museum welcome](Images/integration-museum-welcome.png)

![Stroop trial; generated response fixture, not participant data](Images/integration-stroop-trial.png)

## 还需完成

1. 两位房间作者在整合分支确认自己的房间没有行为或视觉偏差；至少一位 Windows 组员用指定版本实际打开并运行一遍。
2. 在最终展示电脑检查字体、鼠标操作、音量和切房／Restart。记忆房可找 1–2 人做简短可用性试用；鼠标拖拽物体需要真人复核。
3. 填全三人英文姓名，确认实际贡献，准备压缩演示视频。演示视频／正式提交尚未完成。
4. 源项目小于 2 GB；安排老师访问私有仓库，按要求邮件提交链接。Ch5 p.2 的最新已核验截止为 **11 月 4 日午夜前**；本次没有重查后续公告。
5. PDF 的 touch-pad 原文与课堂键鼠替代方式仍有待课堂口径确认。现有主要交付按组内键鼠约定进行；没有 Windows 实机或头显测试，也没有证实环境会产生心理效应。
