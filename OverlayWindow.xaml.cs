using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace EmergencyStop;

public partial class OverlayWindow : Window
{
    private const int GwlExStyle = -20;
    private const long WsExTransparent = 0x00000020L;
    private const long WsExLayered = 0x00080000L;
    private const long WsExToolWindow = 0x00000080L;
    private const long WsExNoActivate = 0x08000000L;

    private readonly AppSettings _settings;
    private IntPtr _handle;

    public OverlayWindow(AppSettings settings, OverlayViewModel viewModel)
    {
        _settings = settings;
        DataContext = viewModel;
        InitializeComponent();
        _settings.PropertyChanged += (_, _) => ApplySettings();
    }

    public void ApplySettings()
    {
        FitToVirtualScreen();
        Topmost = true;
        ApplyWindowStyles();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        ApplySettings();
    }

    private void Window_SourceInitialized(object sender, EventArgs e)
    {
        _handle = new WindowInteropHelper(this).Handle;
        ApplyWindowStyles();
    }

    private void FitToVirtualScreen()
    {
        Left = SystemParameters.VirtualScreenLeft;
        Top = SystemParameters.VirtualScreenTop;
        Width = SystemParameters.VirtualScreenWidth;
        Height = SystemParameters.VirtualScreenHeight;
    }

    private void ApplyWindowStyles()
    {
        if (_handle == IntPtr.Zero)
        {
            return;
        }

        var style = GetWindowLongPtr(_handle, GwlExStyle).ToInt64();
        style |= WsExLayered | WsExToolWindow | WsExNoActivate;

        if (_settings.IsClickThrough)
        {
            style |= WsExTransparent;
        }
        else
        {
            style &= ~WsExTransparent;
        }

        SetWindowLongPtr(_handle, GwlExStyle, new IntPtr(style));
    }

    [DllImport("user32.dll", EntryPoint = "GetWindowLongPtrW", SetLastError = true)]
    private static extern IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll", EntryPoint = "SetWindowLongPtrW", SetLastError = true)]
    private static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);
}
