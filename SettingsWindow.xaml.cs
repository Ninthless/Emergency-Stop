using System.Windows;
using System.Windows.Input;
using Button = System.Windows.Controls.Button;

namespace EmergencyStop;

public partial class SettingsWindow : Window
{
    private readonly AppSettings _settings;
    private readonly SettingsStore _settingsStore;
    private string? _captureTarget;

    public SettingsWindow(AppSettings settings, SettingsStore settingsStore)
    {
        _settings = settings;
        _settingsStore = settingsStore;
        DataContext = settings;
        InitializeComponent();
        UpdateKeyButtons();
    }

    protected override void OnClosed(EventArgs e)
    {
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
        CaptureHint.Text = $"按下新的 {TargetName(target)} 键，Esc 取消";
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
            CaptureHint.Text = "已取消";
            e.Handled = true;
            return;
        }

        if (key is Key.None or Key.Tab or Key.Enter)
        {
            return;
        }

        if (IsDuplicateKey(key, _captureTarget))
        {
            CaptureHint.Text = $"{KeyText.Display(key)} 已经被其他方向使用";
            e.Handled = true;
            return;
        }

        SetTargetKey(_captureTarget, key);
        CaptureHint.Text = $"{TargetName(_captureTarget)} 已设置为 {KeyText.Display(key)}";
        _captureTarget = null;
        UpdateKeyButtons();
        e.Handled = true;
    }

    private void ResetButton_Click(object sender, RoutedEventArgs e)
    {
        _captureTarget = null;
        _settings.CopyFrom(AppSettings.CreateDefault());
        CaptureHint.Text = "已恢复默认";
        UpdateKeyButtons();
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void UpdateKeyButtons()
    {
        ForwardKeyButton.Content = $"前进：{KeyText.Display(_settings.ForwardKey)}";
        BackwardKeyButton.Content = $"后退：{KeyText.Display(_settings.BackwardKey)}";
        LeftKeyButton.Content = $"左移：{KeyText.Display(_settings.LeftKey)}";
        RightKeyButton.Content = $"右移：{KeyText.Display(_settings.RightKey)}";
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

    private static string TargetName(string target)
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
}
