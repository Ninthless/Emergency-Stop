using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using Brush = System.Windows.Media.Brush;
using Color = System.Windows.Media.Color;
using ColorConverter = System.Windows.Media.ColorConverter;
using SolidColorBrush = System.Windows.Media.SolidColorBrush;

namespace EmergencyStop;

public sealed class OverlayViewModel : INotifyPropertyChanged
{
    private const double CenterClearance = 34;

    private static readonly Brush ReadyBrush = FrozenBrush("#45F08A");
    private static readonly Brush MovingBrush = FrozenBrush("#FF4F68");
    private static readonly Brush StoppingBrush = FrozenBrush("#FFD166");
    private static readonly Brush CounterBrush = FrozenBrush("#59C8FF");
    private static readonly Brush ConflictBrush = FrozenBrush("#C084FC");
    private static readonly Brush NeutralBrush = FrozenBrush("#D7DEE8");
    private static readonly Brush CenterCrosshairOutlineBrush = FrozenBrush("#061018");
    private static readonly Brush CyanCrosshairBrush = FrozenBrush("#00E5FF");
    private static readonly Brush GreenCrosshairBrush = FrozenBrush("#45F08A");
    private static readonly Brush WhiteCrosshairBrush = FrozenBrush("#F7FBFF");
    private static readonly Brush YellowCrosshairBrush = FrozenBrush("#FFD84D");
    private static readonly Brush RedCrosshairBrush = FrozenBrush("#FF4F68");
    private static readonly Brush PurpleCrosshairBrush = FrozenBrush("#C084FC");

    private readonly AppSettings _settings;
    private readonly MovementStateService _movementState;
    private MovementSnapshot _snapshot;
    private double _inactiveBrushOpacity = -1;
    private Brush? _inactiveBrush;

    public OverlayViewModel(AppSettings settings, MovementStateService movementState)
    {
        _settings = settings;
        _movementState = movementState;
        _snapshot = movementState.Current;

        _settings.PropertyChanged += (_, _) => RaiseAll();
        _movementState.SnapshotChanged += (_, snapshot) =>
        {
            _snapshot = snapshot;
            RaiseAll();
        };
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public double IndicatorSize => _settings.IndicatorSize;

    public double OverlayOpacity => _settings.OverlayOpacity;

    public double OuterRingSize => _settings.OuterRingSize;

    public double OuterRingThickness => _settings.OuterRingThickness;

    public double OuterRingOpacity => _settings.OuterRingOpacity;

    public double MovementBarsOpacity => _settings.MovementBarsOpacity;

    public double MovementBarLength => _settings.MovementBarLength;

    public double MovementBarThickness => _settings.MovementBarThickness;

    public Thickness ForwardBarMargin => new(0, MovementBarOuterMargin, 0, 0);

    public Thickness BackwardBarMargin => new(0, 0, 0, MovementBarOuterMargin);

    public Thickness LeftBarMargin => new(MovementBarOuterMargin, 0, 0, 0);

    public Thickness RightBarMargin => new(0, 0, MovementBarOuterMargin, 0);

    public CornerRadius MovementBarCornerRadius => _settings.MovementBarShape switch
    {
        MovementBarShape.Square => new CornerRadius(0),
        MovementBarShape.Soft => new CornerRadius(2),
        _ => new CornerRadius(_settings.MovementBarThickness * 0.5)
    };

    public double CenterCrosshairOpacity => _settings.CenterCrosshairOpacity;

    public double CrosshairEffectiveOpacity => ShouldFadeCrosshair ? _settings.MovingCrosshairOpacity : _settings.CenterCrosshairOpacity;

    public double CrosshairSize => _settings.CrosshairSize;

    public double CrosshairThickness => _settings.CrosshairThickness;

    public double CrosshairGap => _settings.CrosshairGap;

    public double CrosshairOutlineThickness => _settings.CrosshairOutlineThickness;

    public double CrosshairOutlineOpacity => _settings.CrosshairOutlineOpacity;

    public bool ShowCenterDot => _settings.ShowCenterDot;

    public double CrosshairCenterDotSize => _settings.CrosshairCenterDotSize;

    public double CrosshairCenterDotOpacity => _settings.CrosshairCenterDotOpacity;

    public bool ShowOuterCrosshairLines => _settings.ShowOuterCrosshairLines;

    public double OuterCrosshairSize => _settings.OuterCrosshairSize;

    public double OuterCrosshairThickness => _settings.OuterCrosshairThickness;

    public double OuterCrosshairGap => _settings.OuterCrosshairGap;

    public double OuterCrosshairOpacity => _settings.OuterCrosshairOpacity;

    public CrosshairStyle CrosshairStyle => _settings.CrosshairStyle;

    public OuterRingStyle OuterRingStyle => _settings.OuterRingStyle;

    public double OffsetX => _settings.OffsetX;

    public double OffsetY => _settings.OffsetY;

    public string StatusText => _snapshot.StatusText;

    public string DetailText => _snapshot.DetailText;

    public string StatusBadgeText => _settings.StatusBadgeContent switch
    {
        StatusBadgeContent.Detail => _snapshot.DetailText,
        StatusBadgeContent.StatusAndDetail => $"{_snapshot.StatusText}  {_snapshot.DetailText}",
        _ => _snapshot.StatusText
    };

    public double StatusBadgeOpacity => _settings.StatusBadgeOpacity;

    public double StatusBadgeWidth => _settings.StatusBadgeWidth;

    public double StatusBadgeHeight => Math.Clamp(_settings.StatusBadgeFontSize + 12, 20, 34);

    public double StatusBadgeFontSize => _settings.StatusBadgeFontSize;

    public VerticalAlignment StatusBadgeVerticalAlignment =>
        _settings.StatusBadgePosition == StatusBadgePosition.Top
            ? VerticalAlignment.Top
            : VerticalAlignment.Bottom;

    public Thickness StatusBadgeMargin =>
        _settings.StatusBadgePosition == StatusBadgePosition.Top
            ? new Thickness(0, ProtectedStatusBadgeOffset, 0, 0)
            : new Thickness(0, 0, 0, ProtectedStatusBadgeOffset);

    public double ProgressValue => _snapshot.Progress;

    public Visibility DirectionPadVisibility => _settings.ShowDirectionPad ? Visibility.Visible : Visibility.Collapsed;

    public Visibility MovementBarsVisibility => _settings.ShowMovementBars ? Visibility.Visible : Visibility.Collapsed;

    public Visibility OuterRingVisibility => _settings.ShowOuterRing ? Visibility.Visible : Visibility.Collapsed;

    public Visibility StatusBadgeVisibility => _settings.ShowStatusBadge ? Visibility.Visible : Visibility.Collapsed;

    public Visibility CenterCrosshairVisibility => _settings.ShowCenterCrosshair ? Visibility.Visible : Visibility.Collapsed;

    public double DirectionLabelOpacity => _settings.DirectionLabelOpacity;

    public double DirectionLabelWidth => _settings.DirectionLabelWidth;

    public double DirectionLabelHeight => Math.Clamp(_settings.DirectionLabelFontSize + 11, 19, 30);

    public double DirectionLabelFontSize => _settings.DirectionLabelFontSize;

    public Thickness ForwardLabelMargin => new(0, DirectionLabelVerticalMargin, 0, 0);

    public Thickness BackwardLabelMargin => new(0, 0, 0, DirectionLabelVerticalMargin);

    public Thickness LeftLabelMargin => new(DirectionLabelHorizontalMargin, 0, 0, 0);

    public Thickness RightLabelMargin => new(0, 0, DirectionLabelHorizontalMargin, 0);

    public string ForwardLabel => DirectionLabel("Forward", KeyText.Display(_settings.ForwardKey), "\u2191");

    public string BackwardLabel => DirectionLabel("Backward", KeyText.Display(_settings.BackwardKey), "\u2193");

    public string LeftLabel => DirectionLabel("Left", KeyText.Display(_settings.LeftKey), "\u2190");

    public string RightLabel => DirectionLabel("Right", KeyText.Display(_settings.RightKey), "\u2192");

    public Brush AccentBrush => _snapshot.State switch
    {
        MovementIndicatorState.Moving => MovingBrush,
        MovementIndicatorState.Stopping => StoppingBrush,
        MovementIndicatorState.CounterStrafing => CounterBrush,
        MovementIndicatorState.Ready => ReadyBrush,
        MovementIndicatorState.Conflict => ConflictBrush,
        _ => NeutralBrush
    };

    public Brush ForwardBrush => _snapshot.ForwardActive ? AccentBrush : MovementInactiveBrush;

    public Brush BackwardBrush => _snapshot.BackwardActive ? AccentBrush : MovementInactiveBrush;

    public Brush LeftBrush => _snapshot.LeftActive ? AccentBrush : MovementInactiveBrush;

    public Brush RightBrush => _snapshot.RightActive ? AccentBrush : MovementInactiveBrush;

    public Brush CrosshairBrush => _settings.CrosshairColorPreset switch
    {
        CrosshairColorPreset.Green => GreenCrosshairBrush,
        CrosshairColorPreset.White => WhiteCrosshairBrush,
        CrosshairColorPreset.Yellow => YellowCrosshairBrush,
        CrosshairColorPreset.Red => RedCrosshairBrush,
        CrosshairColorPreset.Purple => PurpleCrosshairBrush,
        CrosshairColorPreset.State => AccentBrush,
        CrosshairColorPreset.Custom => FrozenBrush(Color.FromRgb(
            (byte)_settings.CrosshairRed,
            (byte)_settings.CrosshairGreen,
            (byte)_settings.CrosshairBlue)),
        _ => CyanCrosshairBrush
    };

    public Brush CrosshairOutlineBrush => CenterCrosshairOutlineBrush;

    private bool ShouldFadeCrosshair => _settings.FadeCrosshairWhileMoving
        && _snapshot.State is MovementIndicatorState.Moving
            or MovementIndicatorState.Stopping
            or MovementIndicatorState.CounterStrafing
            or MovementIndicatorState.Conflict;

    private double MovementBarOuterMargin => Math.Clamp(
        Math.Min(
            90 - _settings.MovementBarDistance - _settings.MovementBarThickness * 0.5,
            90 - CenterClearance - _settings.MovementBarThickness),
        0,
        90);

    private double ProtectedStatusBadgeOffset => Math.Clamp(
        _settings.StatusBadgeOffset,
        0,
        Math.Max(0, 90 - CenterClearance - StatusBadgeHeight));

    private double DirectionLabelVerticalMargin => Math.Clamp(
        Math.Min(
            90 - _settings.DirectionLabelDistance - DirectionLabelHeight * 0.5,
            90 - CenterClearance - DirectionLabelHeight),
        0,
        90);

    private double DirectionLabelHorizontalMargin => Math.Clamp(
        Math.Min(
            90 - _settings.DirectionLabelDistance - _settings.DirectionLabelWidth * 0.5,
            90 - CenterClearance - _settings.DirectionLabelWidth),
        0,
        90);

    private Brush MovementInactiveBrush
    {
        get
        {
            if (_inactiveBrush is not null && Math.Abs(_inactiveBrushOpacity - _settings.MovementBarInactiveOpacity) < 0.001)
            {
                return _inactiveBrush;
            }

            _inactiveBrushOpacity = _settings.MovementBarInactiveOpacity;
            _inactiveBrush = FrozenBrush(Color.FromArgb(
                (byte)Math.Round(_settings.MovementBarInactiveOpacity * 255),
                32,
                48,
                58));
            return _inactiveBrush;
        }
    }

    private string DirectionLabel(string target, string key, string arrow)
    {
        return _settings.DirectionLabelContent switch
        {
            DirectionLabelContent.Arrows => arrow,
            DirectionLabelContent.DirectionNames => target switch
            {
                "Forward" => SettingsLocalization.Resolve(_settings.SettingsLanguage) == SettingsLanguage.Chinese ? "前" : "FWD",
                "Backward" => SettingsLocalization.Resolve(_settings.SettingsLanguage) == SettingsLanguage.Chinese ? "后" : "BACK",
                "Left" => SettingsLocalization.Resolve(_settings.SettingsLanguage) == SettingsLanguage.Chinese ? "左" : "LEFT",
                "Right" => SettingsLocalization.Resolve(_settings.SettingsLanguage) == SettingsLanguage.Chinese ? "右" : "RIGHT",
                _ => target
            },
            _ => key
        };
    }

    private static Brush FrozenBrush(string color)
    {
        var brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(color));
        brush.Freeze();
        return brush;
    }

    private static Brush FrozenBrush(Color color)
    {
        var brush = new SolidColorBrush(color);
        brush.Freeze();
        return brush;
    }

    private void RaiseAll()
    {
        OnPropertyChanged(string.Empty);
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
