using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;
using Button = System.Windows.Controls.Button;
using RadioButton = System.Windows.Controls.RadioButton;
using WpfTextBlock = System.Windows.Controls.TextBlock;

namespace EmergencyStop;

public partial class SettingsWindow : FluentWindow
{
    private readonly AppSettings _settings;
    private readonly SettingsStore _settingsStore;
    private string? _captureTarget;
    private string _currentSection = "Crosshair";
    private bool _isUpdatingLanguageCombo;

    public SettingsWindow(AppSettings settings, SettingsStore settingsStore)
    {
        _settings = settings;
        _settingsStore = settingsStore;
        DataContext = settings;
        InitializeComponent();
        ApplySystemTheme();
        SystemEvents.UserPreferenceChanged += SystemEvents_UserPreferenceChanged;
        CrosshairStyleComboBox.ItemsSource = Enum.GetValues<CrosshairStyle>();
        CrosshairColorComboBox.ItemsSource = Enum.GetValues<CrosshairColorPreset>();
        OuterRingStyleComboBox.ItemsSource = Enum.GetValues<OuterRingStyle>();
        LanguageComboBox.DisplayMemberPath = nameof(SettingsLanguageOption.DisplayName);
        LanguageComboBox.SelectedValuePath = nameof(SettingsLanguageOption.Language);
        UpdateLanguageCombo();
        ApplyLanguage();
        ShowSection("Crosshair");
        UpdateKeyButtons();
    }

    protected override void OnClosed(EventArgs e)
    {
        SystemEvents.UserPreferenceChanged -= SystemEvents_UserPreferenceChanged;
        _settingsStore.Save(_settings);
        base.OnClosed(e);
    }

    private void KeyButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button || button.Tag is not string target)
        {
            return;
        }

        _captureTarget = target;
        CaptureHint.Text = SettingsLocalization.CapturePrompt(target, _settings.SettingsLanguage);
        Focus();
    }

    private void Window_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
        if (_captureTarget is null)
        {
            return;
        }

        var key = e.Key == Key.System ? e.SystemKey : e.Key;

        if (key == Key.Escape)
        {
            _captureTarget = null;
            CaptureHint.Text = SettingsLocalization.CaptureCanceled(_settings.SettingsLanguage);
            e.Handled = true;
            return;
        }

        if (key is Key.None or Key.Tab or Key.Enter)
        {
            return;
        }

        if (IsDuplicateKey(key, _captureTarget))
        {
            CaptureHint.Text = SettingsLocalization.DuplicateKey(KeyText.Display(key), _settings.SettingsLanguage);
            e.Handled = true;
            return;
        }

        SetTargetKey(_captureTarget, key);
        CaptureHint.Text = SettingsLocalization.KeyAssigned(_captureTarget, KeyText.Display(key), _settings.SettingsLanguage);
        _captureTarget = null;
        UpdateKeyButtons();
        e.Handled = true;
    }

    private void ResetButton_Click(object sender, RoutedEventArgs e)
    {
        _captureTarget = null;
        var language = _settings.SettingsLanguage;
        _settings.CopyFrom(AppSettings.CreateDefault());
        _settings.SettingsLanguage = language;
        UpdateLanguageCombo();
        ApplyLanguage();
        CaptureHint.Text = SettingsLocalization.DefaultsRestored(_settings.SettingsLanguage);
        UpdateKeyButtons();
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void NavigationButton_Checked(object sender, RoutedEventArgs e)
    {
        if (sender is RadioButton { Tag: string section })
        {
            ShowSection(section);
        }
    }

    private void PresetButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button || button.Tag is not string preset)
        {
            return;
        }

        _captureTarget = null;

        switch (preset)
        {
            case "DotFocus":
                ApplyDotFocusPreset();
                break;
            case "StateAdaptive":
                ApplyStateAdaptivePreset();
                break;
            default:
                ApplyProCyanPreset();
                break;
        }

        var presetName = button.Content?.ToString() ?? SettingsLocalization.Text("Pro Cyan", _settings.SettingsLanguage);
        CaptureHint.Text = SettingsLocalization.PresetApplied(presetName, _settings.SettingsLanguage);
    }

    private void ShowSection(string section)
    {
        if (CrosshairPage is null || OverlayPage is null || TimingPage is null || KeysPage is null)
        {
            return;
        }

        CrosshairPage.Visibility = section == "Crosshair" ? Visibility.Visible : Visibility.Collapsed;
        OverlayPage.Visibility = section == "Overlay" ? Visibility.Visible : Visibility.Collapsed;
        TimingPage.Visibility = section == "Timing" ? Visibility.Visible : Visibility.Collapsed;
        KeysPage.Visibility = section == "Keys" ? Visibility.Visible : Visibility.Collapsed;

        _currentSection = section;
        PageTitle.Text = section switch
        {
            "Overlay" => SettingsLocalization.Text("Overlay", _settings.SettingsLanguage),
            "Timing" => SettingsLocalization.Text("Timing", _settings.SettingsLanguage),
            "Keys" => SettingsLocalization.Text("Keys", _settings.SettingsLanguage),
            _ => SettingsLocalization.Text("Crosshair", _settings.SettingsLanguage)
        };
    }

    private void UpdateKeyButtons()
    {
        ForwardKeyButton.Content = SettingsLocalization.KeyButton("Forward", KeyText.Display(_settings.ForwardKey), _settings.SettingsLanguage);
        BackwardKeyButton.Content = SettingsLocalization.KeyButton("Backward", KeyText.Display(_settings.BackwardKey), _settings.SettingsLanguage);
        LeftKeyButton.Content = SettingsLocalization.KeyButton("Left", KeyText.Display(_settings.LeftKey), _settings.SettingsLanguage);
        RightKeyButton.Content = SettingsLocalization.KeyButton("Right", KeyText.Display(_settings.RightKey), _settings.SettingsLanguage);
    }

    private void LanguageComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_isUpdatingLanguageCombo || LanguageComboBox.SelectedValue is not SettingsLanguage language)
        {
            return;
        }

        if (_settings.SettingsLanguage == language)
        {
            return;
        }

        _settings.SettingsLanguage = language;
        ApplyLanguage();
    }

    private bool IsDuplicateKey(Key key, string target)
    {
        return target != "Forward" && key == _settings.ForwardKey
            || target != "Backward" && key == _settings.BackwardKey
            || target != "Left" && key == _settings.LeftKey
            || target != "Right" && key == _settings.RightKey;
    }

    private void SetTargetKey(string target, Key key)
    {
        switch (target)
        {
            case "Forward":
                _settings.ForwardKey = key;
                break;
            case "Backward":
                _settings.BackwardKey = key;
                break;
            case "Left":
                _settings.LeftKey = key;
                break;
            case "Right":
                _settings.RightKey = key;
                break;
        }
    }

    private void ApplyLanguage()
    {
        var title = SettingsLocalization.Text("Emergency Stop Settings", _settings.SettingsLanguage);
        Title = title;
        SettingsTitleBar.Title = title;
        UpdateLanguageCombo();
        ApplyLocalizedText(this);
        ShowSection(_currentSection);
        UpdateKeyButtons();
        CaptureHint.Text = _captureTarget is null
            ? SettingsLocalization.Text("Click a direction button, then press a new movement key.", _settings.SettingsLanguage)
            : SettingsLocalization.CapturePrompt(_captureTarget, _settings.SettingsLanguage);
    }

    private void UpdateLanguageCombo()
    {
        _isUpdatingLanguageCombo = true;
        LanguageComboBox.ItemsSource = Enum.GetValues<SettingsLanguage>()
            .Select(language => new SettingsLanguageOption(
                language,
                SettingsLocalization.LanguageName(language, _settings.SettingsLanguage)))
            .ToArray();
        LanguageComboBox.SelectedValue = _settings.SettingsLanguage;
        _isUpdatingLanguageCombo = false;
    }

    private void ApplyLocalizedText(DependencyObject root)
    {
        switch (root)
        {
            case WpfTextBlock textBlock when SettingsLocalization.TryText(textBlock.Text, _settings.SettingsLanguage, out var text):
                textBlock.Text = text;
                break;
            case ContentControl { Content: string content } contentControl
                when SettingsLocalization.TryText(content, _settings.SettingsLanguage, out var text):
                contentControl.Content = text;
                break;
        }

        foreach (var child in LogicalTreeHelper.GetChildren(root))
        {
            if (child is DependencyObject dependencyObject)
            {
                ApplyLocalizedText(dependencyObject);
            }
        }
    }

    private void SystemEvents_UserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
    {
        if (e.Category is UserPreferenceCategory.Color or UserPreferenceCategory.General or UserPreferenceCategory.VisualStyle)
        {
            Dispatcher.Invoke(ApplySystemTheme);
        }
    }

    private void ApplySystemTheme()
    {
        var systemTheme = ApplicationThemeManager.GetSystemTheme();
        var useDark = systemTheme switch
        {
            SystemTheme.Dark or SystemTheme.HCBlack or SystemTheme.HC1 or SystemTheme.HC2 => true,
            SystemTheme.Light or SystemTheme.HCWhite => false,
            _ => !IsWindowsAppsLightTheme()
        };
        var appTheme = useDark ? ApplicationTheme.Dark : ApplicationTheme.Light;

        SettingsTitleBar.ApplicationTheme = appTheme;
        ApplyPalette(useDark);
    }

    private void ApplyPalette(bool useDark)
    {
        if (useDark)
        {
            SetBrush("WindowBackgroundBrush", 8, 8, 8);
            SetBrush("PanelBrush", 18, 18, 18);
            SetBrush("PanelAltBrush", 12, 12, 12);
            SetBrush("PanelBorderBrush", 43, 43, 43);
            SetBrush("ForegroundBrush", 244, 244, 244);
            SetBrush("MutedBrush", 176, 176, 176);
            SetBrush("SubtleTextBrush", 126, 126, 126);
            SetBrush("AccentBrush", 248, 248, 248);
            SetBrush("AccentTextBrush", 8, 8, 8);
            SetBrush("ControlBrush", 23, 23, 23);
            SetBrush("ControlHoverBrush", 36, 36, 36);
            SetBrush("ControlPressedBrush", 48, 48, 48);
            SetBrush("ControlBorderBrush", 54, 54, 54);
            SetBrush("ControlBorderHoverBrush", 102, 102, 102);
            SetBrush("InputBrush", 16, 16, 16);
            SetBrush("SelectedBrush", 37, 37, 37);
            SetBrush("ToggleTrackBrush", 36, 36, 36);
            SetBrush("ToggleBorderBrush", 74, 74, 74);
            SetBrush("ToggleThumbBrush", 237, 237, 237);
            SetBrush("SliderTrackBrush", 42, 42, 42);
            SetBrush("ScrollThumbBrush", 120, 120, 120);
            return;
        }

        SetBrush("WindowBackgroundBrush", 247, 247, 247);
        SetBrush("PanelBrush", 255, 255, 255);
        SetBrush("PanelAltBrush", 241, 241, 241);
        SetBrush("PanelBorderBrush", 208, 208, 208);
        SetBrush("ForegroundBrush", 17, 17, 17);
        SetBrush("MutedBrush", 89, 89, 89);
        SetBrush("SubtleTextBrush", 116, 116, 116);
        SetBrush("AccentBrush", 17, 17, 17);
        SetBrush("AccentTextBrush", 255, 255, 255);
        SetBrush("ControlBrush", 255, 255, 255);
        SetBrush("ControlHoverBrush", 240, 240, 240);
        SetBrush("ControlPressedBrush", 227, 227, 227);
        SetBrush("ControlBorderBrush", 200, 200, 200);
        SetBrush("ControlBorderHoverBrush", 138, 138, 138);
        SetBrush("InputBrush", 255, 255, 255);
        SetBrush("SelectedBrush", 233, 233, 233);
        SetBrush("ToggleTrackBrush", 217, 217, 217);
        SetBrush("ToggleBorderBrush", 160, 160, 160);
        SetBrush("ToggleThumbBrush", 255, 255, 255);
        SetBrush("SliderTrackBrush", 221, 221, 221);
        SetBrush("ScrollThumbBrush", 154, 154, 154);
    }

    private void SetBrush(string key, byte red, byte green, byte blue)
    {
        Resources[key] = new SolidColorBrush(System.Windows.Media.Color.FromRgb(red, green, blue));
    }

    private static bool IsWindowsAppsLightTheme()
    {
        using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");
        return key?.GetValue("AppsUseLightTheme") is not int value || value != 0;
    }

    private sealed record SettingsLanguageOption(SettingsLanguage Language, string DisplayName)
    {
        public override string ToString()
        {
            return DisplayName;
        }
    }

    private void ApplyProCyanPreset()
    {
        _settings.ShowCenterCrosshair = true;
        _settings.CrosshairStyle = CrosshairStyle.Classic;
        _settings.CrosshairColorPreset = CrosshairColorPreset.Cyan;
        _settings.CrosshairRed = 0;
        _settings.CrosshairGreen = 229;
        _settings.CrosshairBlue = 255;
        _settings.CrosshairSize = 4;
        _settings.CrosshairThickness = 2;
        _settings.CrosshairGap = 2;
        _settings.CrosshairOutlineThickness = 0;
        _settings.CrosshairOutlineOpacity = 0.65;
        _settings.CenterCrosshairOpacity = 1;
        _settings.ShowCenterDot = false;
        _settings.CrosshairCenterDotSize = 2;
        _settings.CrosshairCenterDotOpacity = 1;
        _settings.ShowOuterCrosshairLines = false;
        _settings.OuterCrosshairSize = 2;
        _settings.OuterCrosshairThickness = 2;
        _settings.OuterCrosshairGap = 10;
        _settings.OuterCrosshairOpacity = 0.35;
        _settings.FadeCrosshairWhileMoving = false;
        _settings.MovingCrosshairOpacity = 0.45;
    }

    private void ApplyDotFocusPreset()
    {
        _settings.ShowCenterCrosshair = true;
        _settings.CrosshairStyle = CrosshairStyle.Dot;
        _settings.CrosshairColorPreset = CrosshairColorPreset.Green;
        _settings.CrosshairSize = 3;
        _settings.CrosshairThickness = 2;
        _settings.CrosshairGap = 0;
        _settings.CrosshairOutlineThickness = 1;
        _settings.CrosshairOutlineOpacity = 0.8;
        _settings.CenterCrosshairOpacity = 1;
        _settings.ShowCenterDot = false;
        _settings.ShowOuterCrosshairLines = false;
        _settings.FadeCrosshairWhileMoving = false;
    }

    private void ApplyStateAdaptivePreset()
    {
        _settings.ShowCenterCrosshair = true;
        _settings.CrosshairStyle = CrosshairStyle.Classic;
        _settings.CrosshairColorPreset = CrosshairColorPreset.State;
        _settings.CrosshairSize = 5;
        _settings.CrosshairThickness = 2;
        _settings.CrosshairGap = 2;
        _settings.CrosshairOutlineThickness = 1;
        _settings.CrosshairOutlineOpacity = 0.45;
        _settings.CenterCrosshairOpacity = 1;
        _settings.ShowCenterDot = false;
        _settings.ShowOuterCrosshairLines = true;
        _settings.OuterCrosshairSize = 2;
        _settings.OuterCrosshairThickness = 2;
        _settings.OuterCrosshairGap = 11;
        _settings.OuterCrosshairOpacity = 0.28;
        _settings.FadeCrosshairWhileMoving = true;
        _settings.MovingCrosshairOpacity = 0.42;
    }
}
