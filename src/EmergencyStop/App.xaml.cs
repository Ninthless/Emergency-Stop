using System.Windows;
using System.Windows.Threading;
using Velopack;

namespace EmergencyStop;

public partial class App : System.Windows.Application
{
    private SettingsStore? _settingsStore;
    private AppSettings? _settings;
    private MovementStateService? _movementState;
    private RawInputKeyboardListener? _keyboardListener;
    private KeyboardStatePoller? _keyboardStatePoller;
    private TrayService? _trayService;
    private UpdateService? _updateService;
    private OverlayWindow? _overlayWindow;
    private SettingsWindow? _settingsWindow;
    private DispatcherTimer? _movementTimer;
    private DispatcherTimer? _saveTimer;

    [STAThread]
    public static void Main(string[] args)
    {
        VelopackApp.Build()
            .SetArgs(args)
            .SetAutoApplyOnStartup(false)
            .Run();

        var app = new App();
        app.InitializeComponent();
        app.Run();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        try
        {
            StartApplication();
        }
        catch (Exception exception)
        {
            System.Windows.MessageBox.Show(
                exception.ToString(),
                "Emergency Stop 启动失败",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            Shutdown(-1);
        }
    }

    private void StartApplication()
    {
        ShutdownMode = ShutdownMode.OnExplicitShutdown;

        _settingsStore = new SettingsStore();
        _settings = _settingsStore.Load();

        _movementState = new MovementStateService(_settings);
        _overlayWindow = new OverlayWindow(_settings, new OverlayViewModel(_settings, _movementState));
        _overlayWindow.Show();
        ApplyOverlayVisibility();

        _keyboardListener = new RawInputKeyboardListener();
        _keyboardListener.KeyChanged += HandleKeyChanged;
        _keyboardListener.Start();
        _keyboardStatePoller = new KeyboardStatePoller(_settings, _movementState);

        _movementTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(8)
        };
        _movementTimer.Tick += (_, _) =>
        {
            _keyboardStatePoller.Poll();
            _movementState.Update(DateTimeOffset.UtcNow);
        };
        _movementTimer.Start();

        _saveTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(350)
        };
        _saveTimer.Tick += (_, _) => SaveNow();

        _settings.PropertyChanged += (_, _) =>
        {
            ApplyOverlayVisibility();
            QueueSave();
        };

        _updateService = new UpdateService(_settings, SaveNow);
        _trayService = new TrayService(_settings, OpenSettings, ExitApplication, SaveNow, CheckForUpdates);
        Dispatcher.BeginInvoke(CheckForUpdatesOnStartup);

        if (_settings.OpenSettingsOnStartup)
        {
            Dispatcher.BeginInvoke(OpenSettings);
        }
    }

    protected override void OnExit(ExitEventArgs e)
    {
        SaveNow();
        _movementTimer?.Stop();
        _saveTimer?.Stop();
        _keyboardListener?.Dispose();
        _trayService?.Dispose();
        base.OnExit(e);
    }

    private async void CheckForUpdatesOnStartup()
    {
        if (_updateService is not null && _settings?.AutoUpdateEnabled == true)
        {
            await _updateService.CheckForUpdatesAsync(false);
        }
    }

    private async Task CheckForUpdates()
    {
        if (_updateService is not null)
        {
            await _updateService.CheckForUpdatesAsync(true);
        }
    }

    private void HandleKeyChanged(object? sender, KeyboardInputEventArgs e)
    {
        _movementState?.SetKeyState(e.Key, e.IsDown);
    }

    private void OpenSettings()
    {
        if (_settings is null || _settingsStore is null)
        {
            return;
        }

        if (_settingsWindow is { IsVisible: true })
        {
            _settingsWindow.Activate();
            return;
        }

        _settingsWindow = new SettingsWindow(_settings, _settingsStore);
        _settingsWindow.Closed += (_, _) => _settingsWindow = null;
        _settingsWindow.Show();
        _settingsWindow.Activate();
    }

    private void ApplyOverlayVisibility()
    {
        if (_settings is null || _overlayWindow is null)
        {
            return;
        }

        if (_settings.IsOverlayVisible)
        {
            if (!_overlayWindow.IsVisible)
            {
                _overlayWindow.Show();
            }

            _overlayWindow.ApplySettings();
        }
        else
        {
            _overlayWindow.Hide();
        }
    }

    private void QueueSave()
    {
        if (_saveTimer is null)
        {
            return;
        }

        _saveTimer.Stop();
        _saveTimer.Start();
    }

    private void SaveNow()
    {
        if (_settingsStore is null || _settings is null)
        {
            return;
        }

        _saveTimer?.Stop();
        _settingsStore.Save(_settings);
    }

    private void ExitApplication()
    {
        SaveNow();
        Shutdown();
    }
}
