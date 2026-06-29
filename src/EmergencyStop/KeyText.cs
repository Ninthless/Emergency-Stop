using System.Windows.Input;

namespace EmergencyStop;

public static class KeyText
{
    public static string Display(Key key)
    {
        return key switch
        {
            Key.D0 => "0",
            Key.D1 => "1",
            Key.D2 => "2",
            Key.D3 => "3",
            Key.D4 => "4",
            Key.D5 => "5",
            Key.D6 => "6",
            Key.D7 => "7",
            Key.D8 => "8",
            Key.D9 => "9",
            Key.OemMinus => "-",
            Key.OemPlus => "=",
            Key.OemOpenBrackets => "[",
            Key.OemCloseBrackets => "]",
            Key.OemSemicolon => ";",
            Key.OemQuotes => "'",
            Key.OemComma => ",",
            Key.OemPeriod => ".",
            Key.OemQuestion => "/",
            Key.OemBackslash => "\\",
            Key.Space => "SPACE",
            Key.LeftShift => "L-SHIFT",
            Key.RightShift => "R-SHIFT",
            Key.LeftCtrl => "L-CTRL",
            Key.RightCtrl => "R-CTRL",
            Key.LeftAlt => "L-ALT",
            Key.RightAlt => "R-ALT",
            _ => key.ToString().ToUpperInvariant()
        };
    }
}
