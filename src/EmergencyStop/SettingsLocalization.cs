using System.Globalization;

namespace EmergencyStop;

public static class SettingsLocalization
{
    private static readonly IReadOnlyDictionary<string, string> ChineseText = new Dictionary<string, string>
    {
        ["Emergency Stop Settings"] = "急停指示器设置",
        ["CONTROL"] = "控制",
        ["Language"] = "语言",
        ["System"] = "跟随系统",
        ["English"] = "English",
        ["Chinese"] = "中文",
        ["Crosshair"] = "准星",
        ["Overlay"] = "覆盖层",
        ["Timing"] = "时序",
        ["Keys"] = "键位",
        ["Quick presets"] = "快速预设",
        ["Pro Cyan"] = "职业青色",
        ["Dot Focus"] = "点式聚焦",
        ["State Adaptive"] = "状态自适应",
        ["Profile"] = "配置",
        ["Show center crosshair"] = "显示中心准星",
        ["Style"] = "样式",
        ["Color"] = "颜色",
        ["Red"] = "红色",
        ["Green"] = "绿色",
        ["Blue"] = "蓝色",
        ["Inner lines"] = "内线",
        ["Length"] = "长度",
        ["Thickness"] = "粗细",
        ["Center gap"] = "中心间隙",
        ["Opacity"] = "透明度",
        ["Outline thickness"] = "描边粗细",
        ["Outline opacity"] = "描边透明度",
        ["Dot and outer lines"] = "中心点和外线",
        ["Center dot"] = "中心点",
        ["Dot size"] = "点大小",
        ["Dot opacity"] = "点透明度",
        ["Outer lines"] = "外线",
        ["Outer length"] = "外线长度",
        ["Outer thickness"] = "外线粗细",
        ["Outer gap"] = "外线间隙",
        ["Outer opacity"] = "外线透明度",
        ["Movement fade"] = "移动淡化",
        ["Fade crosshair while moving"] = "移动时降低准星透明度",
        ["Moving opacity"] = "移动透明度",
        ["Window"] = "窗口",
        ["Show overlay"] = "显示覆盖层",
        ["Click through"] = "鼠标穿透",
        ["Open settings on startup"] = "启动时打开设置",
        ["Layout"] = "布局",
        ["Indicator size"] = "指示器大小",
        ["Overlay opacity"] = "覆盖层透明度",
        ["Horizontal offset"] = "水平偏移",
        ["Vertical offset"] = "垂直偏移",
        ["Movement cues"] = "移动提示",
        ["Movement bars"] = "移动条",
        ["State badge"] = "状态标识",
        ["Direction key labels"] = "方向键标签",
        ["Movement bar opacity"] = "移动条透明度",
        ["Outer ring"] = "外圈",
        ["Ring style"] = "外圈样式",
        ["Ring size"] = "外圈大小",
        ["Ring thickness"] = "外圈粗细",
        ["Ring opacity"] = "外圈透明度",
        ["Movement timing"] = "移动时序",
        ["Release stop estimate"] = "松键停止估计",
        ["Counter-strafe estimate"] = "反向急停估计",
        ["READY flash window"] = "READY 闪烁窗口",
        ["Movement keys"] = "移动键位",
        ["Click a direction button, then press a new movement key."] = "点击方向按钮，然后按下新的移动键。",
        ["Reset defaults"] = "恢复默认",
        ["Close"] = "关闭"
    };

    private static readonly IReadOnlyDictionary<string, string> AdditionalChineseText = new Dictionary<string, string>
    {
        ["Automatic updates"] = "自动更新",
        ["Check for updates"] = "检查更新",
        ["Emergency Stop Update"] = "Emergency Stop 更新",
        ["An update is already being checked."] = "正在检查更新。",
        ["Updates are available only after installing Emergency Stop with the Setup installer."] = "通过 Setup 安装 Emergency Stop 后才可使用更新。",
        ["Emergency Stop is up to date."] = "Emergency Stop 已是最新版本。",
        ["A new version is available. Download it now?"] = "发现新版本。现在下载吗？",
        ["Update downloaded. Restart Emergency Stop now to finish installing?"] = "更新已下载。现在重启 Emergency Stop 完成安装吗？",
        ["Unable to check for updates."] = "无法检查更新。",
        ["Unable to download the update."] = "无法下载更新。"
    };

    private static readonly IReadOnlyDictionary<string, string> EnglishText =
        ChineseText.Keys
            .Concat(AdditionalChineseText.Keys)
            .ToDictionary(key => key, key => key);

    private static readonly IReadOnlyDictionary<string, string> ChineseToEnglish =
        ChineseText
            .Concat(AdditionalChineseText)
            .ToDictionary(pair => pair.Value, pair => pair.Key);

    public static SettingsLanguage Resolve(SettingsLanguage language)
    {
        if (language != SettingsLanguage.System)
        {
            return language;
        }

        return CultureInfo.CurrentUICulture.TwoLetterISOLanguageName.Equals("zh", StringComparison.OrdinalIgnoreCase)
            ? SettingsLanguage.Chinese
            : SettingsLanguage.English;
    }

    public static string Text(string english, SettingsLanguage language)
    {
        var resolved = Resolve(language);
        var source = ChineseToEnglish.TryGetValue(english, out var reverse) ? reverse : english;
        return resolved == SettingsLanguage.Chinese
            && (ChineseText.TryGetValue(source, out var chinese)
                || AdditionalChineseText.TryGetValue(source, out chinese))
            ? chinese
            : EnglishText.TryGetValue(source, out var text) ? text : source;
    }

    public static bool TryText(string value, SettingsLanguage language, out string text)
    {
        if (EnglishText.ContainsKey(value) || ChineseToEnglish.ContainsKey(value))
        {
            text = Text(value, language);
            return true;
        }

        text = value;
        return false;
    }

    public static string LanguageName(SettingsLanguage option, SettingsLanguage language)
    {
        return option switch
        {
            SettingsLanguage.Chinese => Text("Chinese", language),
            SettingsLanguage.English => Text("English", language),
            _ => Text("System", language)
        };
    }

    public static string DirectionName(string target, SettingsLanguage language)
    {
        if (Resolve(language) == SettingsLanguage.Chinese)
        {
            return target switch
            {
                "Forward" => "前进",
                "Backward" => "后退",
                "Left" => "左移",
                "Right" => "右移",
                _ => "移动"
            };
        }

        return target switch
        {
            "Forward" => "forward",
            "Backward" => "backward",
            "Left" => "left",
            "Right" => "right",
            _ => "movement"
        };
    }

    public static string KeyButton(string target, string key, SettingsLanguage language)
    {
        if (Resolve(language) == SettingsLanguage.Chinese)
        {
            var direction = target switch
            {
                "Forward" => "前进",
                "Backward" => "后退",
                "Left" => "左移",
                "Right" => "右移",
                _ => "移动"
            };
            return $"{direction}：{key}";
        }

        return $"{target}: {key}";
    }

    public static string CapturePrompt(string target, SettingsLanguage language)
    {
        return Resolve(language) == SettingsLanguage.Chinese
            ? $"按下新的{DirectionName(target, language)}键，或按 Esc 取消。"
            : $"Press a new {DirectionName(target, language)} key, or Esc to cancel.";
    }

    public static string CaptureCanceled(SettingsLanguage language)
    {
        return Resolve(language) == SettingsLanguage.Chinese ? "已取消键位捕获。" : "Capture canceled.";
    }

    public static string DuplicateKey(string key, SettingsLanguage language)
    {
        return Resolve(language) == SettingsLanguage.Chinese ? $"{key} 已经被使用。" : $"{key} is already assigned.";
    }

    public static string KeyAssigned(string target, string key, SettingsLanguage language)
    {
        return Resolve(language) == SettingsLanguage.Chinese
            ? $"{DirectionName(target, language)} 已设置为 {key}。"
            : $"{DirectionName(target, language)} is now {key}.";
    }

    public static string DefaultsRestored(SettingsLanguage language)
    {
        return Resolve(language) == SettingsLanguage.Chinese ? "已恢复默认设置。" : "Defaults restored.";
    }

    public static string PresetApplied(string presetName, SettingsLanguage language)
    {
        return Resolve(language) == SettingsLanguage.Chinese ? $"已应用{presetName}预设。" : $"{presetName} preset applied.";
    }
}
