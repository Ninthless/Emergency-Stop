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
    private static readonly Brush ReadyBrush = FrozenBrush("#45F08A");
    private static readonly Brush MovingBrush = FrozenBrush("#FF4F68");
    private static readonly Brush StoppingBrush = FrozenBrush("#FFD166");
    private static readonly Brush CounterBrush = FrozenBrush("#59C8FF");
    private static readonly Brush ConflictBrush = FrozenBrush("#C084FC");
    private static readonly Brush NeutralBrush = FrozenBrush("#D7DEE8");
    private static readonly Brush InactiveBrush = FrozenBrush("#20303A");
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

    public double ProgressValue => _snapshot.Progress;

    public Visibility DirectionPadVisibility => _settings.ShowDirectionPad ? Visibility.Visible : Visibility.Collapsed;

    public Visibility MovementBarsVisibility => _settings.ShowMovementBars ? Visibility.Visible : Visibility.Collapsed;

    public Visibility OuterRingVisibility => _settings.ShowOuterRing ? Visibility.Visible : Visibility.Collapsed;

    public Visibility StatusBadgeVisibility => _settings.ShowStatusBadge ? Visibility.Visible : Visibility.Collapsed;

    public Visibility CenterCrosshairVisibility => _settings.ShowCenterCrosshair ? Visibility.Visible : Visibility.Collapsed;

    public string ForwardLabel => KeyText.Display(_settings.ForwardKey);

    public string BackwardLabel => KeyText.Display(_settings.BackwardKey);

    public string LeftLabel => KeyText.Display(_settings.LeftKey);

    public string RightLabel => KeyText.Display(_settings.RightKey);

    public Brush AccentBrush => _snapshot.State switch
    {
        MovementIndicatorState.Moving => MovingBrush,
        MovementIndicatorState.Stopping => StoppingBrush,
        MovementIndicatorState.CounterStrafing => CounterBrush,
        MovementIndicatorState.Ready => ReadyBrush,
        MovementIndicatorState.Conflict => ConflictBrush,
        _ => NeutralBrush
    };

    public Brush ForwardBrush => _snapshot.ForwardActive ? AccentBrush : InactiveBrush;

    public Brush BackwardBrush => _snapshot.BackwardActive ? AccentBrush : InactiveBrush;

    public Brush LeftBrush => _snapshot.LeftActive ? AccentBrush : InactiveBrush;

    public Brush RightBrush => _snapshot.RightActive ? AccentBrush : InactiveBrush;

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
