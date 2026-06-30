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
    private bool _showMovementBars = true;
    private bool _showDirectionPad;
    private bool _showOuterRing;
    private bool _showStatusBadge;
    private bool _showCenterCrosshair = true;
    private bool _showCenterDot;
    private bool _showOuterCrosshairLines;
    private bool _fadeCrosshairWhileMoving;
    private bool _openSettingsOnStartup;
    private bool _autoUpdateEnabled = true;
    private SettingsLanguage _settingsLanguage = SettingsLanguage.System;
    private double _indicatorSize = 136;
    private double _overlayOpacity = 0.82;
    private double _outerRingSize = 126;
    private double _outerRingThickness = 3;
    private double _outerRingOpacity = 0.9;
    private double _movementBarsOpacity = 0.95;
    private double _centerCrosshairOpacity = 1;
    private double _movingCrosshairOpacity = 0.45;
    private double _crosshairSize = 4;
    private double _crosshairThickness = 2;
    private double _crosshairGap = 2;
    private double _crosshairOutlineThickness;
    private double _crosshairOutlineOpacity = 0.65;
    private double _crosshairCenterDotSize = 2;
    private double _crosshairCenterDotOpacity = 1;
    private double _outerCrosshairSize = 2;
    private double _outerCrosshairThickness = 2;
    private double _outerCrosshairGap = 10;
    private double _outerCrosshairOpacity = 0.35;
    private int _crosshairRed;
    private int _crosshairGreen = 229;
    private int _crosshairBlue = 255;
    private CrosshairStyle _crosshairStyle = CrosshairStyle.Classic;
    private CrosshairColorPreset _crosshairColorPreset = CrosshairColorPreset.Cyan;
    private OuterRingStyle _outerRingStyle = OuterRingStyle.Brackets;
    private int _releaseStopMilliseconds = 105;
    private int _counterStopMilliseconds = 90;
    private int _readyFlashMilliseconds = 55;
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

    public bool ShowMovementBars
    {
        get => _showMovementBars;
        set => SetField(ref _showMovementBars, value);
    }

    public bool ShowDirectionPad
    {
        get => _showDirectionPad;
        set => SetField(ref _showDirectionPad, value);
    }

    public bool ShowOuterRing
    {
        get => _showOuterRing;
        set => SetField(ref _showOuterRing, value);
    }

    public bool ShowStatusBadge
    {
        get => _showStatusBadge;
        set => SetField(ref _showStatusBadge, value);
    }

    public bool ShowCenterCrosshair
    {
        get => _showCenterCrosshair;
        set => SetField(ref _showCenterCrosshair, value);
    }

    public bool ShowCenterDot
    {
        get => _showCenterDot;
        set => SetField(ref _showCenterDot, value);
    }

    public bool ShowOuterCrosshairLines
    {
        get => _showOuterCrosshairLines;
        set => SetField(ref _showOuterCrosshairLines, value);
    }

    public bool FadeCrosshairWhileMoving
    {
        get => _fadeCrosshairWhileMoving;
        set => SetField(ref _fadeCrosshairWhileMoving, value);
    }

    public bool OpenSettingsOnStartup
    {
        get => _openSettingsOnStartup;
        set => SetField(ref _openSettingsOnStartup, value);
    }

    public bool AutoUpdateEnabled
    {
        get => _autoUpdateEnabled;
        set => SetField(ref _autoUpdateEnabled, value);
    }

    public SettingsLanguage SettingsLanguage
    {
        get => _settingsLanguage;
        set => SetField(ref _settingsLanguage, value);
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

    public double OuterRingSize
    {
        get => _outerRingSize;
        set => SetField(ref _outerRingSize, Math.Clamp(value, 72, 170));
    }

    public double OuterRingThickness
    {
        get => _outerRingThickness;
        set => SetField(ref _outerRingThickness, Math.Clamp(value, 1, 8));
    }

    public double OuterRingOpacity
    {
        get => _outerRingOpacity;
        set => SetField(ref _outerRingOpacity, Math.Clamp(value, 0.15, 1));
    }

    public double MovementBarsOpacity
    {
        get => _movementBarsOpacity;
        set => SetField(ref _movementBarsOpacity, Math.Clamp(value, 0.05, 1));
    }

    public double CenterCrosshairOpacity
    {
        get => _centerCrosshairOpacity;
        set => SetField(ref _centerCrosshairOpacity, Math.Clamp(value, 0.05, 1));
    }

    public double MovingCrosshairOpacity
    {
        get => _movingCrosshairOpacity;
        set => SetField(ref _movingCrosshairOpacity, Math.Clamp(value, 0.05, 1));
    }

    public double CrosshairSize
    {
        get => _crosshairSize;
        set => SetField(ref _crosshairSize, Math.Clamp(value, 1, 24));
    }

    public double CrosshairThickness
    {
        get => _crosshairThickness;
        set => SetField(ref _crosshairThickness, Math.Clamp(value, 1, 6));
    }

    public double CrosshairGap
    {
        get => _crosshairGap;
        set => SetField(ref _crosshairGap, Math.Clamp(value, 0, 16));
    }

    public double CrosshairOutlineThickness
    {
        get => _crosshairOutlineThickness;
        set => SetField(ref _crosshairOutlineThickness, Math.Clamp(value, 0, 3));
    }

    public double CrosshairOutlineOpacity
    {
        get => _crosshairOutlineOpacity;
        set => SetField(ref _crosshairOutlineOpacity, Math.Clamp(value, 0, 1));
    }

    public double CrosshairCenterDotSize
    {
        get => _crosshairCenterDotSize;
        set => SetField(ref _crosshairCenterDotSize, Math.Clamp(value, 1, 8));
    }

    public double CrosshairCenterDotOpacity
    {
        get => _crosshairCenterDotOpacity;
        set => SetField(ref _crosshairCenterDotOpacity, Math.Clamp(value, 0.05, 1));
    }

    public double OuterCrosshairSize
    {
        get => _outerCrosshairSize;
        set => SetField(ref _outerCrosshairSize, Math.Clamp(value, 1, 12));
    }

    public double OuterCrosshairThickness
    {
        get => _outerCrosshairThickness;
        set => SetField(ref _outerCrosshairThickness, Math.Clamp(value, 1, 6));
    }

    public double OuterCrosshairGap
    {
        get => _outerCrosshairGap;
        set => SetField(ref _outerCrosshairGap, Math.Clamp(value, 4, 28));
    }

    public double OuterCrosshairOpacity
    {
        get => _outerCrosshairOpacity;
        set => SetField(ref _outerCrosshairOpacity, Math.Clamp(value, 0.05, 1));
    }

    public int CrosshairRed
    {
        get => _crosshairRed;
        set => SetField(ref _crosshairRed, Math.Clamp(value, 0, 255));
    }

    public int CrosshairGreen
    {
        get => _crosshairGreen;
        set => SetField(ref _crosshairGreen, Math.Clamp(value, 0, 255));
    }

    public int CrosshairBlue
    {
        get => _crosshairBlue;
        set => SetField(ref _crosshairBlue, Math.Clamp(value, 0, 255));
    }

    public CrosshairStyle CrosshairStyle
    {
        get => _crosshairStyle;
        set => SetField(ref _crosshairStyle, value);
    }

    public CrosshairColorPreset CrosshairColorPreset
    {
        get => _crosshairColorPreset;
        set => SetField(ref _crosshairColorPreset, value);
    }

    public OuterRingStyle OuterRingStyle
    {
        get => _outerRingStyle;
        set => SetField(ref _outerRingStyle, value);
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
        ShowMovementBars = settings.ShowMovementBars;
        ShowDirectionPad = settings.ShowDirectionPad;
        ShowOuterRing = settings.ShowOuterRing;
        ShowStatusBadge = settings.ShowStatusBadge;
        ShowCenterCrosshair = settings.ShowCenterCrosshair;
        ShowCenterDot = settings.ShowCenterDot;
        ShowOuterCrosshairLines = settings.ShowOuterCrosshairLines;
        FadeCrosshairWhileMoving = settings.FadeCrosshairWhileMoving;
        OpenSettingsOnStartup = settings.OpenSettingsOnStartup;
        AutoUpdateEnabled = settings.AutoUpdateEnabled;
        SettingsLanguage = settings.SettingsLanguage;
        IndicatorSize = settings.IndicatorSize;
        OverlayOpacity = settings.OverlayOpacity;
        OuterRingSize = settings.OuterRingSize;
        OuterRingThickness = settings.OuterRingThickness;
        OuterRingOpacity = settings.OuterRingOpacity;
        MovementBarsOpacity = settings.MovementBarsOpacity;
        CenterCrosshairOpacity = settings.CenterCrosshairOpacity;
        MovingCrosshairOpacity = settings.MovingCrosshairOpacity;
        CrosshairSize = settings.CrosshairSize;
        CrosshairThickness = settings.CrosshairThickness;
        CrosshairGap = settings.CrosshairGap;
        CrosshairOutlineThickness = settings.CrosshairOutlineThickness;
        CrosshairOutlineOpacity = settings.CrosshairOutlineOpacity;
        CrosshairCenterDotSize = settings.CrosshairCenterDotSize;
        CrosshairCenterDotOpacity = settings.CrosshairCenterDotOpacity;
        OuterCrosshairSize = settings.OuterCrosshairSize;
        OuterCrosshairThickness = settings.OuterCrosshairThickness;
        OuterCrosshairGap = settings.OuterCrosshairGap;
        OuterCrosshairOpacity = settings.OuterCrosshairOpacity;
        CrosshairRed = settings.CrosshairRed;
        CrosshairGreen = settings.CrosshairGreen;
        CrosshairBlue = settings.CrosshairBlue;
        CrosshairStyle = settings.CrosshairStyle;
        CrosshairColorPreset = settings.CrosshairColorPreset;
        OuterRingStyle = settings.OuterRingStyle;
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
