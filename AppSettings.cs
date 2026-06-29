using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace EmergencyStop;

public sealed class AppSettings : INotifyPropertyChanged
{
    private Key _forwardKey = Key.W;
    private Key _backwardKey = Key.S;
    private Key _leftKey = Key.A;
    private Key _rightKey = Key.D;
    private bool _isOverlayVisible = true;
    private bool _isClickThrough = true;
    private bool _showDirectionPad;
    private bool _openSettingsOnStartup;
    private double _indicatorSize = 136;
    private double _overlayOpacity = 0.82;
    private int _releaseStopMilliseconds = 105;
    private int _counterStopMilliseconds = 45;
    private int _readyFlashMilliseconds = 90;
    private double _offsetX;
    private double _offsetY;

    public event PropertyChangedEventHandler? PropertyChanged;

    public Key ForwardKey
    {
        get => _forwardKey;
        set => SetField(ref _forwardKey, value);
    }

    public Key BackwardKey
    {
        get => _backwardKey;
        set => SetField(ref _backwardKey, value);
    }

    public Key LeftKey
    {
        get => _leftKey;
        set => SetField(ref _leftKey, value);
    }

    public Key RightKey
    {
        get => _rightKey;
        set => SetField(ref _rightKey, value);
    }

    public bool IsOverlayVisible
    {
        get => _isOverlayVisible;
        set => SetField(ref _isOverlayVisible, value);
    }

    public bool IsClickThrough
    {
        get => _isClickThrough;
        set => SetField(ref _isClickThrough, value);
    }

    public bool ShowDirectionPad
    {
        get => _showDirectionPad;
        set => SetField(ref _showDirectionPad, value);
    }

    public bool OpenSettingsOnStartup
    {
        get => _openSettingsOnStartup;
        set => SetField(ref _openSettingsOnStartup, value);
    }

    public double IndicatorSize
    {
        get => _indicatorSize;
        set => SetField(ref _indicatorSize, Math.Clamp(value, 96, 260));
    }

    public double OverlayOpacity
    {
        get => _overlayOpacity;
        set => SetField(ref _overlayOpacity, Math.Clamp(value, 0.25, 1));
    }

    public int ReleaseStopMilliseconds
    {
        get => _releaseStopMilliseconds;
        set => SetField(ref _releaseStopMilliseconds, Math.Clamp(value, 35, 220));
    }

    public int CounterStopMilliseconds
    {
        get => _counterStopMilliseconds;
        set => SetField(ref _counterStopMilliseconds, Math.Clamp(value, 15, 140));
    }

    public int ReadyFlashMilliseconds
    {
        get => _readyFlashMilliseconds;
        set => SetField(ref _readyFlashMilliseconds, Math.Clamp(value, 25, 180));
    }

    public double OffsetX
    {
        get => _offsetX;
        set => SetField(ref _offsetX, Math.Clamp(value, -640, 640));
    }

    public double OffsetY
    {
        get => _offsetY;
        set => SetField(ref _offsetY, Math.Clamp(value, -360, 360));
    }

    public static AppSettings CreateDefault()
    {
        return new AppSettings();
    }

    public void CopyFrom(AppSettings settings)
    {
        ForwardKey = settings.ForwardKey;
        BackwardKey = settings.BackwardKey;
        LeftKey = settings.LeftKey;
        RightKey = settings.RightKey;
        IsOverlayVisible = settings.IsOverlayVisible;
        IsClickThrough = settings.IsClickThrough;
        ShowDirectionPad = settings.ShowDirectionPad;
        OpenSettingsOnStartup = settings.OpenSettingsOnStartup;
        IndicatorSize = settings.IndicatorSize;
        OverlayOpacity = settings.OverlayOpacity;
        ReleaseStopMilliseconds = settings.ReleaseStopMilliseconds;
        CounterStopMilliseconds = settings.CounterStopMilliseconds;
        ReadyFlashMilliseconds = settings.ReadyFlashMilliseconds;
        OffsetX = settings.OffsetX;
        OffsetY = settings.OffsetY;
    }

    private bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        return true;
    }
}
