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

    public double OffsetX => _settings.OffsetX;

    public double OffsetY => _settings.OffsetY;

    public string StatusText => _snapshot.StatusText;

    public string DetailText => _snapshot.DetailText;

    public double ProgressValue => _snapshot.Progress;

    public Visibility DirectionPadVisibility => _settings.ShowDirectionPad ? Visibility.Visible : Visibility.Collapsed;

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

    private static Brush FrozenBrush(string color)
    {
        var brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(color));
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
