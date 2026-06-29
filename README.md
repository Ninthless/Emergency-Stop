# Emergency Stop

一个面向 VALORANT / 无畏契约练习场景的 Windows 屏幕急停指示器。它通过读取本机移动键状态，在屏幕中心显示一个轻量、空心、不遮挡准星的急停状态提示。

## 设计目标

- 在屏幕中心显示 `MOVE`、`STOP`、`BRAKE`、`READY`、`BOTH` 等移动状态
- 中心区域保持留白，让游戏内准星露出来
- 仅监听本机键盘输入，不读取游戏进程、不注入、不抓包、不自动按键
- 通过系统托盘右键打开设置、显示/隐藏覆盖层、切换鼠标穿透
- 支持自定义移动键位、尺寸、透明度、偏移和急停估算窗口

## 合规边界

这个项目被设计成外部训练辅助显示器，而不是游戏自动化工具。

它不会做这些事：

- 不读取 VALORANT / 无畏契约进程内存
- 不注入 DirectX、Vulkan 或游戏进程
- 不抓包、不读取游戏日志、不识别敌人或准星
- 不模拟按键、不自动反向急停、不做宏
- 不隐藏进程、不绕过 Vanguard 或其他反作弊

它会做这些事：

- 监听 Windows 键盘输入状态
- 绘制一个透明置顶 WPF 覆盖层
- 根据移动键状态显示估算的急停提示

Riot / 腾讯 / Vanguard 的规则和检测策略可能变化，最终解释权属于平台方。请优先用于靶场、自定义训练和个人练习环境。

## 功能

- 空心环形中心指示器，默认不遮挡游戏准星
- 四方向小条显示移动键激活状态
- Raw Input + `GetAsyncKeyState` 双通道键盘状态读取
- 默认键位：`W`、`A`、`S`、`D`
- 默认状态：
  - `IDLE`：无移动输入
  - `MOVE`：正在移动
  - `STOP`：松键自然减速估算中
  - `BRAKE`：反向急停估算中
  - `READY`：估算可开枪窗口
  - `BOTH`：同轴双键冲突
- 系统托盘菜单：
  - 打开设置
  - 显示/隐藏覆盖层
  - 鼠标穿透
  - 退出
- 设置自动保存到 `%AppData%\EmergencyStop\settings.json`

## 运行

当前项目发布目标为 Windows x64。

已发布的本地可执行文件位于：

```text
bin\Release\net9.0-windows\win-x64\publish\EmergencyStop.exe
```

运行后程序默认进入托盘，并显示屏幕中心覆盖层。右键托盘图标可以打开设置窗口。

建议游戏显示模式：

- 优先使用无边框窗口或窗口化全屏
- 独占全屏下 Windows 覆盖层可能无法稳定显示

如果游戏内收不到 WASD：

- 先确认旧的 `EmergencyStop.exe` 进程已经退出
- 尝试右键 `EmergencyStop.exe`，选择以管理员身份运行
- 确认游戏不是独占全屏
- 确认设置窗口已经关闭，托盘菜单里的鼠标穿透保持勾选

## 从源码构建

需要：

- Windows
- .NET 9 SDK

构建：

```powershell
dotnet build
```

发布 Windows x64 自包含版本：

```powershell
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:EnableCompressionInSingleFile=true
```

## 项目结构

```text
App.xaml / App.xaml.cs              应用启动、托盘、覆盖层生命周期
OverlayWindow.xaml                  屏幕中心覆盖层 UI
OverlayViewModel.cs                 覆盖层显示状态
MovementStateService.cs             移动键状态机
RawInputKeyboardListener.cs         Raw Input 键盘监听
KeyboardStatePoller.cs              物理按键状态轮询
SettingsWindow.xaml                 设置窗口
SettingsStore.cs                    本地配置读写
TrayService.cs                      系统托盘菜单
```

## 急停估算说明

VALORANT 没有公开完整的玩家移动速度曲线。本项目不会声称精确模拟游戏物理，而是根据键位输入变化做可调的时序估算：

- 松开移动键后进入 `STOP`
- 反向输入时进入 `BRAKE`
- 计时窗口结束后显示 `READY`

你可以在靶场里根据手感调整：

- 松键急停估算
- 反向急停估算
- `READY` 闪烁窗口

## 许可证

当前未添加开源许可证。没有许可证时，默认保留所有权利。

## 免责声明

本项目与 Riot Games、腾讯或 VALORANT / 无畏契约官方没有关联。使用任何第三方工具都应自行确认平台规则和风险。
