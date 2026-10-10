# Quest 安装与实测：最少手动步骤

使用指定的 **Unity 2022.3.62f3**。电脑侧已整合房间，配置 Android / ARM64 / IL2CPP / Vulkan / OpenXR，并用安装好的 Unity OpenXR 校验规则检查项目。当前是控制器 VR，使用 OpenXR 自带的 Meta Quest Support 与 Oculus Touch 配置。[Unity 官方说明](https://docs.unity3d.com/Packages/com.unity.xr.openxr@1.16/manual/index.html)。

## 拿到学校 Quest 后

1. 确认学校允许安装课程 APK，并确认型号、开发者模式是否已开启。学校设备的账号和权限设置由学校处理。
2. USB 连接 Mac，在头显中允许这台电脑进行 USB 调试。
3. 双击 `tools/Quest_Install.command`。脚本会安装本机 `Builds/Quest/FXRMemory.apk` 并启动应用，无需你手动配置 Unity 打包参数。
4. 戴头显完成两轮，检查控制器射线、文字大小、移动舒适度、音量、Reset、暂停恢复和帧率；再试一次自然环境先开始的顺序。

安装包只保存在本机，不随 Git 上传。队友可在 Unity 中用 **FXR → Quest → Validate and build updated APK** 自动构建自己的安装包。安装辅助脚本在设备未连接、未授权或多台设备选择不明时会停止，不修改头显账号／开发者设置。

我可以继续处理安装、修复和数据取回。必须在头显里点击授权、实际佩戴判断舒适度这两部分，需要你或学校现场完成。

CSV 数据取回：`python3 tools/quest_device.py results`；应用日志取回：`python3 tools/quest_device.py logs`。仅收集本项目的文件或进程日志。

电脑测试和 APK 构建成功不代表头显实测完成。老师要求的 touchpad gliding/flying 与 Quest 摇杆移动是否等价，仍需确认。
