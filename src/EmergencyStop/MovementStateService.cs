using System.ComponentModel;
using System.Windows.Input;

namespace EmergencyStop;

public sealed class MovementStateService
{
    private readonly AppSettings _settings;
    private readonly HashSet<Key> _pressedKeys = [];
    private readonly AxisRuntime _horizontal = new();
    private readonly AxisRuntime _vertical = new();
    private MovementSnapshot _current = MovementSnapshot.Initial;

    public MovementStateService(AppSettings settings)
    {
        _settings = settings;
        _settings.PropertyChanged += HandleSettingsChanged;
    }

    public event EventHandler<MovementSnapshot>? SnapshotChanged;

    public MovementSnapshot Current => _current;

    public void SetKeyState(Key key, bool isDown)
    {
        if (!IsTrackedKey(key))
        {
            return;
        }

        var changed = isDown ? _pressedKeys.Add(key) : _pressedKeys.Remove(key);
        if (changed)
        {
            Update(DateTimeOffset.UtcNow);
        }
    }

    public void Update(DateTimeOffset now)
    {
        var horizontal = _horizontal.Update(
            ReadAxis(_settings.LeftKey, _settings.RightKey),
            now,
            _settings);

        var vertical = _vertical.Update(
            ReadAxis(_settings.BackwardKey, _settings.ForwardKey),
            now,
            _settings);

        var state = CombineState(horizontal.State, vertical.State);
        var progress = PickProgress(state, horizontal, vertical);
        var directionText = BuildDirectionText();
        var snapshot = new MovementSnapshot(
            state,
            BuildStatusText(state),
            BuildDetailText(state, directionText),
            directionText,
            progress,
            _pressedKeys.Contains(_settings.ForwardKey),
            _pressedKeys.Contains(_settings.BackwardKey),
            _pressedKeys.Contains(_settings.LeftKey),
            _pressedKeys.Contains(_settings.RightKey));

        Publish(snapshot);
    }

    private void HandleSettingsChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(AppSettings.ForwardKey)
            or nameof(AppSettings.BackwardKey)
            or nameof(AppSettings.LeftKey)
            or nameof(AppSettings.RightKey))
        {
            _pressedKeys.Clear();
            _horizontal.Reset();
            _vertical.Reset();
            Publish(MovementSnapshot.Initial);
        }
    }

    private AxisInput ReadAxis(Key negativeKey, Key positiveKey)
    {
        var negative = _pressedKeys.Contains(negativeKey);
        var positive = _pressedKeys.Contains(positiveKey);

        if (negative && positive)
        {
            return new AxisInput(0, true);
        }

        if (negative)
        {
            return new AxisInput(-1, false);
        }

        if (positive)
        {
            return new AxisInput(1, false);
        }

        return new AxisInput(0, false);
    }

    private bool IsTrackedKey(Key key)
    {
        return key == _settings.ForwardKey
            || key == _settings.BackwardKey
            || key == _settings.LeftKey
            || key == _settings.RightKey;
    }

    private string BuildDirectionText()
    {
        var keys = new List<string>(4);

        if (_pressedKeys.Contains(_settings.ForwardKey))
        {
            keys.Add(KeyText.Display(_settings.ForwardKey));
        }

        if (_pressedKeys.Contains(_settings.BackwardKey))
        {
            keys.Add(KeyText.Display(_settings.BackwardKey));
        }

        if (_pressedKeys.Contains(_settings.LeftKey))
        {
            keys.Add(KeyText.Display(_settings.LeftKey));
        }

        if (_pressedKeys.Contains(_settings.RightKey))
        {
            keys.Add(KeyText.Display(_settings.RightKey));
        }

        return string.Join(" + ", keys);
    }

    private static MovementIndicatorState CombineState(
        MovementIndicatorState horizontal,
        MovementIndicatorState vertical)
    {
        if (horizontal == MovementIndicatorState.Conflict || vertical == MovementIndicatorState.Conflict)
        {
            return MovementIndicatorState.Conflict;
        }

        if (horizontal == MovementIndicatorState.CounterStrafing || vertical == MovementIndicatorState.CounterStrafing)
        {
            return MovementIndicatorState.CounterStrafing;
        }

        if (horizontal == MovementIndicatorState.Stopping || vertical == MovementIndicatorState.Stopping)
        {
            return MovementIndicatorState.Stopping;
        }

        if (horizontal == MovementIndicatorState.Moving || vertical == MovementIndicatorState.Moving)
        {
            return MovementIndicatorState.Moving;
        }

        if (horizontal == MovementIndicatorState.Ready || vertical == MovementIndicatorState.Ready)
        {
            return MovementIndicatorState.Ready;
        }

        return MovementIndicatorState.Neutral;
    }

    private static double PickProgress(MovementIndicatorState state, AxisSnapshot horizontal, AxisSnapshot vertical)
    {
        if (state is MovementIndicatorState.Ready)
        {
            return 1;
        }

        return Math.Max(
            horizontal.State == state ? horizontal.Progress : 0,
            vertical.State == state ? vertical.Progress : 0);
    }

    private static string BuildStatusText(MovementIndicatorState state)
    {
        return state switch
        {
            MovementIndicatorState.Moving => "MOVE",
            MovementIndicatorState.Stopping => "STOP",
            MovementIndicatorState.CounterStrafing => "BRAKE",
            MovementIndicatorState.Ready => "READY",
            MovementIndicatorState.Conflict => "BOTH",
            _ => "IDLE"
        };
    }

    private static string BuildDetailText(MovementIndicatorState state, string directionText)
    {
        return state switch
        {
            MovementIndicatorState.Moving => string.IsNullOrWhiteSpace(directionText) ? "移动中" : directionText,
            MovementIndicatorState.Stopping => "自然减速",
            MovementIndicatorState.CounterStrafing => "反向急停",
            MovementIndicatorState.Ready => "可开枪",
            MovementIndicatorState.Conflict => "按键冲突",
            _ => "待机"
        };
    }

    private void Publish(MovementSnapshot snapshot)
    {
        if (snapshot == _current)
        {
            return;
        }

        _current = snapshot;
        SnapshotChanged?.Invoke(this, snapshot);
    }

    private readonly record struct AxisInput(int Direction, bool Conflict);

    private readonly record struct AxisSnapshot(MovementIndicatorState State, double Progress);

    private sealed class AxisRuntime
    {
        private int _currentInput;
        private int _lastDirection;
        private int _counterDirection;
        private bool _wasConflict;
        private DateTimeOffset? _releaseStartedAt;
        private DateTimeOffset? _counterStartedAt;

        public AxisSnapshot Update(AxisInput input, DateTimeOffset now, AppSettings settings)
        {
            if (input.Conflict)
            {
                _wasConflict = true;
                _currentInput = 0;
                _releaseStartedAt = null;
                _counterStartedAt = null;
                return new AxisSnapshot(MovementIndicatorState.Conflict, 0);
            }

            var direction = input.Direction;

            if (direction == 0)
            {
                if (_currentInput != 0)
                {
                    _lastDirection = _currentInput;
                    _releaseStartedAt = now;
                    _counterStartedAt = null;
                }

                _currentInput = 0;
                _wasConflict = false;

                if (_releaseStartedAt is null)
                {
                    return _lastDirection == 0
                        ? new AxisSnapshot(MovementIndicatorState.Neutral, 0)
                        : new AxisSnapshot(MovementIndicatorState.Ready, 1);
                }

                var elapsed = (now - _releaseStartedAt.Value).TotalMilliseconds;
                if (elapsed < settings.ReleaseStopMilliseconds)
                {
                    return new AxisSnapshot(
                        MovementIndicatorState.Stopping,
                        Math.Clamp(elapsed / settings.ReleaseStopMilliseconds, 0, 1));
                }

                return new AxisSnapshot(MovementIndicatorState.Ready, 1);
            }

            if (direction != _currentInput)
            {
                var shouldCounter = _lastDirection != 0 && direction == -_lastDirection;
                shouldCounter = shouldCounter || (_currentInput != 0 && direction == -_currentInput);
                shouldCounter = shouldCounter || (_wasConflict && _lastDirection != 0 && direction == -_lastDirection);

                _releaseStartedAt = null;
                _counterStartedAt = shouldCounter ? now : null;
                _counterDirection = shouldCounter ? direction : 0;
            }

            _currentInput = direction;
            _lastDirection = direction;
            _wasConflict = false;

            if (_counterStartedAt is not null && _counterDirection == direction)
            {
                var elapsed = (now - _counterStartedAt.Value).TotalMilliseconds;

                if (elapsed < settings.CounterStopMilliseconds)
                {
                    return new AxisSnapshot(
                        MovementIndicatorState.CounterStrafing,
                        Math.Clamp(elapsed / settings.CounterStopMilliseconds, 0, 1));
                }

                if (elapsed < settings.CounterStopMilliseconds + settings.ReadyFlashMilliseconds)
                {
                    return new AxisSnapshot(MovementIndicatorState.Ready, 1);
                }

                _counterStartedAt = null;
                _counterDirection = 0;
            }

            return new AxisSnapshot(MovementIndicatorState.Moving, 0);
        }

        public void Reset()
        {
            _currentInput = 0;
            _lastDirection = 0;
            _counterDirection = 0;
            _wasConflict = false;
            _releaseStartedAt = null;
            _counterStartedAt = null;
        }
    }
}
