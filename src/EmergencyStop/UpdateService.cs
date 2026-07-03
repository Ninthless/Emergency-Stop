using System.Diagnostics;
using System.Windows;
using Velopack;
using Velopack.Sources;
using MessageBox = System.Windows.MessageBox;

namespace EmergencyStop;

public sealed class UpdateService
{
    private const string RepositoryUrl = "https://github.com/Ninthless/Emergency-Stop";
    private const string ReleasesUrl = $"{RepositoryUrl}/releases";

    private readonly AppSettings _settings;
    private readonly Action _saveNow;
    private readonly UpdateManager _manager;
    private bool _isChecking;

    public UpdateService(AppSettings settings, Action saveNow)
    {
        _settings = settings;
        _saveNow = saveNow;
        _manager = new UpdateManager(
            new GithubSource(RepositoryUrl, null, false, null),
            new UpdateOptions(),
            null);
    }

    public async Task CheckForUpdatesAsync(bool userInitiated)
    {
        if (_isChecking)
        {
            if (userInitiated)
            {
                ShowInfo("An update is already being checked.");
            }

            return;
        }

        if (!_manager.IsInstalled)
        {
            if (userInitiated)
            {
                ShowInfo("Updates are available only after installing Emergency Stop with the Setup installer.");
            }

            return;
        }

        _isChecking = true;

        try
        {
            var pendingUpdate = _manager.UpdatePendingRestart;
            if (pendingUpdate is not null)
            {
                PromptRestart(pendingUpdate);
                return;
            }

            var update = await CheckRemoteUpdateAsync(userInitiated);
            if (update is null)
            {
                return;
            }

            if (userInitiated && !ConfirmDownload(update.TargetFullRelease))
            {
                return;
            }

            if (!await DownloadUpdateAsync(update, userInitiated))
            {
                return;
            }

            PromptRestart(update.TargetFullRelease);
        }
        finally
        {
            _isChecking = false;
        }
    }

    private async Task<UpdateInfo?> CheckRemoteUpdateAsync(bool userInitiated)
    {
        try
        {
            var update = await _manager.CheckForUpdatesAsync();
            if (update is null && userInitiated)
            {
                ShowInfo("Emergency Stop is up to date.");
            }

            return update;
        }
        catch (Exception exception)
        {
            Debug.WriteLine(exception);

            if (userInitiated)
            {
                ShowUpdateFailure("Unable to check for updates.", exception);
            }

            return null;
        }
    }

    private async Task<bool> DownloadUpdateAsync(UpdateInfo update, bool userInitiated)
    {
        try
        {
            await _manager.DownloadUpdatesAsync(update, null, CancellationToken.None);
            return true;
        }
        catch (Exception exception)
        {
            Debug.WriteLine(exception);

            if (userInitiated)
            {
                ShowUpdateFailure("Unable to download the update.", exception);
            }

            return false;
        }
    }

    private bool ConfirmDownload(VelopackAsset update)
    {
        var message = $"{SettingsLocalization.Text("A new version is available. Download it now?", _settings.SettingsLanguage)}\n{update.Version}";
        return MessageBox.Show(
            message,
            SettingsLocalization.Text("Emergency Stop Update", _settings.SettingsLanguage),
            MessageBoxButton.YesNo,
            MessageBoxImage.Information) == MessageBoxResult.Yes;
    }

    private void PromptRestart(VelopackAsset update)
    {
        var message = $"{SettingsLocalization.Text("Update downloaded. Restart Emergency Stop now to finish installing?", _settings.SettingsLanguage)}\n{update.Version}";
        var result = MessageBox.Show(
            message,
            SettingsLocalization.Text("Emergency Stop Update", _settings.SettingsLanguage),
            MessageBoxButton.YesNo,
            MessageBoxImage.Information);

        if (result != MessageBoxResult.Yes)
        {
            return;
        }

        _saveNow();
        _manager.ApplyUpdatesAndRestart(update, []);
    }

    private void ShowInfo(string text)
    {
        MessageBox.Show(
            SettingsLocalization.Text(text, _settings.SettingsLanguage),
            SettingsLocalization.Text("Emergency Stop Update", _settings.SettingsLanguage),
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }

    private void ShowUpdateFailure(string text, Exception exception)
    {
        var baseException = exception.GetBaseException();
        var message = string.Join(
            Environment.NewLine + Environment.NewLine,
            SettingsLocalization.Text(text, _settings.SettingsLanguage),
            $"{SettingsLocalization.Text("Reason", _settings.SettingsLanguage)}: {baseException.Message}",
            SettingsLocalization.Text("Open the GitHub releases page?", _settings.SettingsLanguage));

        var result = MessageBox.Show(
            message,
            SettingsLocalization.Text("Emergency Stop Update", _settings.SettingsLanguage),
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);

        if (result == MessageBoxResult.Yes)
        {
            OpenReleasesPage();
        }
    }

    private void OpenReleasesPage()
    {
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = ReleasesUrl,
                UseShellExecute = true
            });
        }
        catch (Exception exception)
        {
            Debug.WriteLine(exception);
            ShowInfo("Unable to open the GitHub releases page.");
        }
    }
}
