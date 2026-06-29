using System.Runtime.InteropServices;
using System.Windows.Input;

namespace EmergencyStop;

public sealed class KeyboardStatePoller
{
    private readonly AppSettings _settings;
    private readonly MovementStateService _movementState;
    private readonly Dictionary<Key, bool> _lastStates = [];

    public KeyboardStatePoller(AppSettings settings, MovementStateService movementState)
    {
        _settings = settings;
        _movementState = movementState;
    }

    public void Poll()
    {
        PollKey(_settings.ForwardKey);
        PollKey(_settings.BackwardKey);
        PollKey(_settings.LeftKey);
        PollKey(_settings.RightKey);
    }

    private void PollKey(Key key)
    {
        var virtualKey = KeyInterop.VirtualKeyFromKey(key);
        if (virtualKey == 0)
        {
            return;
        }

        var isDown = (GetAsyncKeyState(virtualKey) & 0x8000) != 0;

        if (_lastStates.TryGetValue(key, out var previous) && previous == isDown)
        {
            return;
        }

        _lastStates[key] = isDown;
        _movementState.SetKeyState(key, isDown);
    }

    [DllImport("user32.dll")]
    private static extern short GetAsyncKeyState(int virtualKey);
}
