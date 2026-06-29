using System.Runtime.InteropServices;
using System.Windows.Input;
using System.Windows.Interop;

namespace EmergencyStop;

public sealed class RawInputKeyboardListener : IDisposable
{
    private const int WmInput = 0x00FF;
    private const int WsPopup = unchecked((int)0x80000000);
    private const int WsExToolWindow = 0x00000080;
    private const int WsExNoActivate = 0x08000000;
    private const uint RidInput = 0x10000003;
    private const uint RidevInputSink = 0x00000100;
    private const uint RimTypeKeyboard = 1;
    private const uint WmKeyDown = 0x0100;
    private const uint WmKeyUp = 0x0101;
    private const uint WmSysKeyDown = 0x0104;
    private const uint WmSysKeyUp = 0x0105;

    private HwndSource? _source;

    public event EventHandler<KeyboardInputEventArgs>? KeyChanged;

    public void Start()
    {
        if (_source is not null)
        {
            return;
        }

        var parameters = new HwndSourceParameters("EmergencyStopRawInput")
        {
            Width = 1,
            Height = 1,
            PositionX = -32000,
            PositionY = -32000,
            WindowStyle = WsPopup,
            ExtendedWindowStyle = WsExToolWindow | WsExNoActivate
        };

        _source = new HwndSource(parameters);
        _source.AddHook(WndProc);

        var devices = new[]
        {
            new RawInputDevice
            {
                UsagePage = 0x01,
                Usage = 0x06,
                Flags = RidevInputSink,
                Target = _source.Handle
            }
        };

        if (!RegisterRawInputDevices(devices, (uint)devices.Length, (uint)Marshal.SizeOf<RawInputDevice>()))
        {
            throw new InvalidOperationException("无法注册键盘 Raw Input。");
        }
    }

    public void Dispose()
    {
        if (_source is null)
        {
            return;
        }

        _source.RemoveHook(WndProc);
        _source.Dispose();
        _source = null;
    }

    private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        if (msg != WmInput)
        {
            return IntPtr.Zero;
        }

        var size = 0u;
        GetRawInputData(lParam, RidInput, IntPtr.Zero, ref size, (uint)Marshal.SizeOf<RawInputHeader>());

        if (size == 0)
        {
            return IntPtr.Zero;
        }

        var buffer = Marshal.AllocHGlobal((int)size);

        try
        {
            var read = GetRawInputData(lParam, RidInput, buffer, ref size, (uint)Marshal.SizeOf<RawInputHeader>());
            if (read != size)
            {
                return IntPtr.Zero;
            }

            var raw = Marshal.PtrToStructure<RawInput>(buffer);
            if (raw.Header.Type != RimTypeKeyboard || raw.Keyboard.VKey == 255)
            {
                return IntPtr.Zero;
            }

            var isDown = raw.Keyboard.Message switch
            {
                WmKeyDown or WmSysKeyDown => true,
                WmKeyUp or WmSysKeyUp => false,
                _ => (bool?)null
            };

            if (isDown is null)
            {
                return IntPtr.Zero;
            }

            var key = KeyInterop.KeyFromVirtualKey(raw.Keyboard.VKey);
            if (key == Key.None)
            {
                return IntPtr.Zero;
            }

            KeyChanged?.Invoke(this, new KeyboardInputEventArgs(key, isDown.Value));
        }
        finally
        {
            Marshal.FreeHGlobal(buffer);
        }

        return IntPtr.Zero;
    }

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool RegisterRawInputDevices(
        [In] RawInputDevice[] rawInputDevices,
        uint deviceCount,
        uint size);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern uint GetRawInputData(
        IntPtr rawInput,
        uint command,
        IntPtr data,
        ref uint size,
        uint headerSize);

    [StructLayout(LayoutKind.Sequential)]
    private struct RawInputDevice
    {
        public ushort UsagePage;
        public ushort Usage;
        public uint Flags;
        public IntPtr Target;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct RawInputHeader
    {
        public uint Type;
        public uint Size;
        public IntPtr Device;
        public IntPtr WParam;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct RawKeyboard
    {
        public ushort MakeCode;
        public ushort Flags;
        public ushort Reserved;
        public ushort VKey;
        public uint Message;
        public uint ExtraInformation;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct RawInput
    {
        public RawInputHeader Header;
        public RawKeyboard Keyboard;
    }
}
