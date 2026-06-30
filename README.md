# Emergency Stop

一个面向 VALORANT / 无畏契约练习场景的 Windows 屏幕急停指示器和外置准星。它通过读取本机移动键状态，在屏幕中心显示可高度自定义的准星、移动提示和急停状态。

## 设计目标

- 在屏幕中心显示 `MOVE`、`STOP`、`BRAKE`、`READY`、`BOTH` 等移动状态
- 可以直接隐藏游戏内准星，使用本工具绘制的外置准星
- 仅监听本机键盘输入，不读取游戏进程、不注入、不抓包、不自动按键
- 通过系统托盘右键打开设置、显示/隐藏覆盖层、切换鼠标穿透
- 支持自定义准星形状、颜色、内线、外线、中心点、外圈、移动淡化、键位、尺寸、透明度、偏移和急停估算窗口

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

- 默认准星采用 VALORANT 常见职业小十字起点：青色、静态、内线 `1 / 4 / 2 / 2`、无中心点、无外线
- 支持准星样式：Classic、Dot、Circle、TShape、Box、Corners、Diamond、Chevron
- 支持颜色预设、状态色和自定义 RGB
- 支持中心点、外线、轮廓厚度、轮廓透明度、移动时自动降低准星透明度
- 支持外圈样式：Circle、Brackets、Corners、Ticks
- 四方向小条显示移动键激活状态，可独立显示/隐藏和调节透明度
- 设置页使用 WPF UI 风格外壳和简洁紧凑的黑白布局，跟随 Windows 亮色/暗色切换，并支持中文/英文
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
  - 检查更新
  - 显示/隐藏覆盖层
  - 鼠标穿透
  - 退出
- 设置自动保存到 `%AppData%\EmergencyStop\settings.json`
- 通过 Velopack 安装后支持启动时自动检查更新，也可以从托盘手动检查

## 运行

当前项目发布目标为 Windows x64。

推荐使用 GitHub Releases 中的 Velopack `EmergencyStop-win-Setup.exe` 安装。通过安装器安装后，自动更新才会生效。

开发构建的本地可执行文件位于：

```text
src\EmergencyStop\bin\Release\net9.0-windows\win-x64\publish\EmergencyStop.exe
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

NuGet 依赖：

- `WPF-UI`：设置页窗口外壳、主题资源和现代 WPF 基础设施
- `Velopack`：Windows 安装器、GitHub Releases 更新源和自动更新

构建：

```powershell
dotnet build EmergencyStop.sln
```

发布 Windows x64 自包含构建：

```powershell
dotnet publish src\EmergencyStop\EmergencyStop.csproj -c Release -r win-x64 --self-contained true -o artifacts\publish -p:PublishSingleFile=false
```

打包 Velopack 安装器：

```powershell
vpk --yes true --legacyConsole true pack --packId EmergencyStop --packVersion 0.1.0 --packDir artifacts\publish --outputDir artifacts\velopack --mainExe EmergencyStop.exe --packTitle "Emergency Stop" --packAuthors "Ninthless" --runtime win-x64 --channel win --shortcuts StartMenuRoot
```

完整发布流程见 [docs/release.md](docs/release.md)。

## 项目结构

```text
EmergencyStop.sln
README.md
.github/workflows/release.yml             GitHub Releases 自动发布流程
docs/release.md                           Velopack 发布和自动更新说明
src/
  EmergencyStop/
    EmergencyStop.csproj
    App.xaml / App.xaml.cs              应用启动、托盘、覆盖层生命周期
    OverlayWindow.xaml                  屏幕中心覆盖层 UI
    OverlayViewModel.cs                 覆盖层显示状态
    CrosshairElement.cs                 自绘准星
    OuterRingElement.cs                 自绘外圈
    MovementStateService.cs             移动键状态机
    RawInputKeyboardListener.cs         Raw Input 键盘监听
    KeyboardStatePoller.cs              物理按键状态轮询
    SettingsWindow.xaml                 设置窗口
    SettingsStore.cs                    本地配置读写
    TrayService.cs                      系统托盘菜单
    UpdateService.cs                    Velopack 更新检查、下载和重启安装
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

## 默认准星说明

VALORANT 没有唯一“最好”的准星。项目默认值选择的是主流职业玩家常见的小型静态十字方案：青色、高对比、关闭中心点、关闭外线、关闭移动淡化。它适合作为起点，再根据分辨率、准星缩放习惯和个人视力调整。

默认核心参数：

- 样式：`Classic`
- 颜色：`Cyan`
- 内线长度：`4`
- 内线厚度：`2`
- 中心间隙：`2`
- 轮廓厚度：`0`
- 中心点：关闭
- 外线：关闭
- 移动时淡化：关闭

## 许可证

当前未添加开源许可证。没有许可证时，默认保留所有权利。

## 免责声明

本项目与 Riot Games、腾讯或 VALORANT / 无畏契约官方没有关联。使用任何第三方工具都应自行确认平台规则和风险。
