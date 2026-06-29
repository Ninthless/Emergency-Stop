using System.IO;
using System.Text.Json;
using System.Windows.Input;

namespace EmergencyStop;

public sealed class SettingsStore
{
    private const int CurrentSettingsVersion = 3;

    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true
    };

    private readonly string _settingsPath;

    public SettingsStore()
    {
        var directory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "EmergencyStop");
        Directory.CreateDirectory(directory);
        _settingsPath = Path.Combine(directory, "settings.json");
    }

    public AppSettings Load()
    {
        if (!File.Exists(_settingsPath))
        {
            return AppSettings.CreateDefault();
        }

        try
        {
            var json = File.ReadAllText(_settingsPath);
            var dto = JsonSerializer.Deserialize<SettingsDto>(json, Options);
            return dto is null ? AppSettings.CreateDefault() : FromDto(dto);
        }
        catch
        {
            return AppSettings.CreateDefault();
        }
    }

    public void Save(AppSettings settings)
    {
        var json = JsonSerializer.Serialize(ToDto(settings), Options);
        File.WriteAllText(_settingsPath, json);
    }

    private static AppSettings FromDto(SettingsDto dto)
    {
        var settings = AppSettings.CreateDefault();
        settings.ForwardKey = ParseKey(dto.ForwardKey, settings.ForwardKey);
        settings.BackwardKey = ParseKey(dto.BackwardKey, settings.BackwardKey);
        settings.LeftKey = ParseKey(dto.LeftKey, settings.LeftKey);
        settings.RightKey = ParseKey(dto.RightKey, settings.RightKey);
        settings.IsOverlayVisible = dto.IsOverlayVisible ?? settings.IsOverlayVisible;
        settings.IsClickThrough = dto.IsClickThrough ?? settings.IsClickThrough;
        settings.ShowDirectionPad = dto.SettingsVersion >= CurrentSettingsVersion
            ? dto.ShowDirectionPad ?? settings.ShowDirectionPad
            : false;
        settings.OpenSettingsOnStartup = dto.SettingsVersion >= CurrentSettingsVersion
            ? dto.OpenSettingsOnStartup ?? settings.OpenSettingsOnStartup
            : false;
        settings.IndicatorSize = dto.SettingsVersion >= CurrentSettingsVersion
            ? dto.IndicatorSize ?? settings.IndicatorSize
            : Math.Min(dto.IndicatorSize ?? settings.IndicatorSize, 136);
        settings.OverlayOpacity = dto.SettingsVersion >= CurrentSettingsVersion
            ? dto.OverlayOpacity ?? settings.OverlayOpacity
            : Math.Min(dto.OverlayOpacity ?? settings.OverlayOpacity, 0.82);
        settings.ReleaseStopMilliseconds = dto.ReleaseStopMilliseconds ?? settings.ReleaseStopMilliseconds;
        settings.CounterStopMilliseconds = dto.CounterStopMilliseconds ?? settings.CounterStopMilliseconds;
        settings.ReadyFlashMilliseconds = dto.ReadyFlashMilliseconds ?? settings.ReadyFlashMilliseconds;
        settings.OffsetX = dto.OffsetX ?? settings.OffsetX;
        settings.OffsetY = dto.OffsetY ?? settings.OffsetY;
        return settings;
    }

    private static SettingsDto ToDto(AppSettings settings)
    {
        return new SettingsDto
        {
            SettingsVersion = CurrentSettingsVersion,
            ForwardKey = settings.ForwardKey.ToString(),
            BackwardKey = settings.BackwardKey.ToString(),
            LeftKey = settings.LeftKey.ToString(),
            RightKey = settings.RightKey.ToString(),
            IsOverlayVisible = settings.IsOverlayVisible,
            IsClickThrough = settings.IsClickThrough,
            ShowDirectionPad = settings.ShowDirectionPad,
            OpenSettingsOnStartup = settings.OpenSettingsOnStartup,
            IndicatorSize = settings.IndicatorSize,
            OverlayOpacity = settings.OverlayOpacity,
            ReleaseStopMilliseconds = settings.ReleaseStopMilliseconds,
            CounterStopMilliseconds = settings.CounterStopMilliseconds,
            ReadyFlashMilliseconds = settings.ReadyFlashMilliseconds,
            OffsetX = settings.OffsetX,
            OffsetY = settings.OffsetY
        };
    }

    private static Key ParseKey(string? value, Key fallback)
    {
        return Enum.TryParse(value, true, out Key key) ? key : fallback;
    }

    private sealed class SettingsDto
    {
        public int? SettingsVersion { get; set; }
        public string? ForwardKey { get; set; }
        public string? BackwardKey { get; set; }
        public string? LeftKey { get; set; }
        public string? RightKey { get; set; }
        public bool? IsOverlayVisible { get; set; }
        public bool? IsClickThrough { get; set; }
        public bool? ShowDirectionPad { get; set; }
        public bool? OpenSettingsOnStartup { get; set; }
        public double? IndicatorSize { get; set; }
        public double? OverlayOpacity { get; set; }
        public int? ReleaseStopMilliseconds { get; set; }
        public int? CounterStopMilliseconds { get; set; }
        public int? ReadyFlashMilliseconds { get; set; }
        public double? OffsetX { get; set; }
        public double? OffsetY { get; set; }
    }
}
