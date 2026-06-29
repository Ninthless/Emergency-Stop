using System.Windows.Input;

namespace EmergencyStop;

public sealed class KeyboardInputEventArgs : EventArgs
{
    public KeyboardInputEventArgs(Key key, bool isDown)
    {
        Key = key;
        IsDown = isDown;
    }

    public Key Key { get; }

    public bool IsDown { get; }
}
