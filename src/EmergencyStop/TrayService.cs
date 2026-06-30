using System.Drawing;
using Forms = System.Windows.Forms;

namespace EmergencyStop;

public sealed class TrayService : IDisposable
{
    private readonly AppSettings _settings;
    private readonly Action _saveNow;
    private readonly Forms.NotifyIcon _notifyIcon;
    private readonly Forms.ToolStripMenuItem _overlayItem;
    private readonly Forms.ToolStripMenuItem _clickThroughItem;

    public TrayService(
        AppSettings settings,
        Action openSettings,
        Action exitApplication,
        Action saveNow,
        Func<Task> checkForUpdates)
    {
        _settings = settings;
        _saveNow = saveNow;

        _overlayItem = new Forms.ToolStripMenuItem("显示覆盖层")
        {
            Checked = _settings.IsOverlayVisible,
            CheckOnClick = true
        };
        _overlayItem.CheckedChanged += (_, _) =>
        {
            _settings.IsOverlayVisible = _overlayItem.Checked;
            _saveNow();
        };

        _clickThroughItem = new Forms.ToolStripMenuItem("鼠标穿透")
        {
            Checked = _settings.IsClickThrough,
            CheckOnClick = true
        };
        _clickThroughItem.CheckedChanged += (_, _) =>
        {
            _settings.IsClickThrough = _clickThroughItem.Checked;
            _saveNow();
        };

        var menu = new Forms.ContextMenuStrip();
        menu.Items.Add("打开设置", null, (_, _) => openSettings());
        menu.Items.Add(SettingsLocalization.Text("Check for updates", _settings.SettingsLanguage), null, async (_, _) => await checkForUpdates());
        menu.Items.Add(_overlayItem);
        menu.Items.Add(_clickThroughItem);
        menu.Items.Add(new Forms.ToolStripSeparator());
        menu.Items.Add("退出", null, (_, _) => exitApplication());

        _notifyIcon = new Forms.NotifyIcon
        {
            Icon = SystemIcons.Application,
            Text = "Emergency Stop",
            ContextMenuStrip = menu,
            Visible = true
        };
        _notifyIcon.DoubleClick += (_, _) => openSettings();
        _settings.PropertyChanged += (_, _) => SyncMenuItems();
    }

    public void Dispose()
    {
        _notifyIcon.Visible = false;
        _notifyIcon.Dispose();
    }

    private void SyncMenuItems()
    {
        if (_overlayItem.Checked != _settings.IsOverlayVisible)
        {
            _overlayItem.Checked = _settings.IsOverlayVisible;
        }

        if (_clickThroughItem.Checked != _settings.IsClickThrough)
        {
            _clickThroughItem.Checked = _settings.IsClickThrough;
        }
    }
}
