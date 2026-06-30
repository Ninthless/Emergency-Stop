using System.IO;
using System.Text.Json;
using System.Windows.Input;

namespace EmergencyStop;

public sealed class SettingsStore
{
    private const int CurrentSettingsVersion = 10;

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
        var version = dto.SettingsVersion ?? 0;

        settings.ForwardKey = ParseKey(dto.ForwardKey, settings.ForwardKey);
        settings.BackwardKey = ParseKey(dto.BackwardKey, settings.BackwardKey);
        settings.LeftKey = ParseKey(dto.LeftKey, settings.LeftKey);
        settings.RightKey = ParseKey(dto.RightKey, settings.RightKey);
        settings.IsOverlayVisible = dto.IsOverlayVisible ?? settings.IsOverlayVisible;
        settings.IsClickThrough = dto.IsClickThrough ?? settings.IsClickThrough;
        settings.ShowMovementBars = dto.ShowMovementBars ?? settings.ShowMovementBars;
        settings.ShowDirectionPad = version >= 5
            ? dto.ShowDirectionPad ?? settings.ShowDirectionPad
            : false;
        settings.ShowOuterRing = dto.ShowOuterRing ?? settings.ShowOuterRing;
        settings.ShowStatusBadge = dto.ShowStatusBadge ?? settings.ShowStatusBadge;
        settings.ShowCenterCrosshair = version >= 5
            ? dto.ShowCenterCrosshair ?? settings.ShowCenterCrosshair
            : true;
        settings.ShowCenterDot = dto.ShowCenterDot ?? settings.ShowCenterDot;
        settings.ShowOuterCrosshairLines = dto.ShowOuterCrosshairLines ?? settings.ShowOuterCrosshairLines;
        settings.FadeCrosshairWhileMoving = dto.FadeCrosshairWhileMoving ?? settings.FadeCrosshairWhileMoving;
        settings.OpenSettingsOnStartup = version >= 5
            ? dto.OpenSettingsOnStartup ?? settings.OpenSettingsOnStartup
            : false;
        settings.AutoUpdateEnabled = version >= 9
            ? dto.AutoUpdateEnabled ?? settings.AutoUpdateEnabled
            : true;
        settings.SettingsLanguage = ParseEnum(dto.SettingsLanguage, settings.SettingsLanguage);
        settings.IndicatorSize = version >= 5
            ? dto.IndicatorSize ?? settings.IndicatorSize
            : Math.Min(dto.IndicatorSize ?? settings.IndicatorSize, 136);
        settings.OverlayOpacity = version >= 5
            ? dto.OverlayOpacity ?? settings.OverlayOpacity
            : Math.Min(dto.OverlayOpacity ?? settings.OverlayOpacity, 0.82);
        settings.OuterRingSize = dto.OuterRingSize ?? settings.OuterRingSize;
        settings.OuterRingThickness = dto.OuterRingThickness ?? settings.OuterRingThickness;
        settings.OuterRingOpacity = dto.OuterRingOpacity ?? settings.OuterRingOpacity;
        settings.MovementBarsOpacity = dto.MovementBarsOpacity ?? settings.MovementBarsOpacity;
        settings.MovementBarLength = dto.MovementBarLength ?? settings.MovementBarLength;
        settings.MovementBarThickness = dto.MovementBarThickness ?? settings.MovementBarThickness;
        settings.MovementBarDistance = dto.MovementBarDistance ?? settings.MovementBarDistance;
        settings.MovementBarInactiveOpacity = dto.MovementBarInactiveOpacity ?? settings.MovementBarInactiveOpacity;
        settings.StatusBadgeOpacity = dto.StatusBadgeOpacity ?? settings.StatusBadgeOpacity;
        settings.StatusBadgeWidth = dto.StatusBadgeWidth ?? settings.StatusBadgeWidth;
        settings.StatusBadgeFontSize = dto.StatusBadgeFontSize ?? settings.StatusBadgeFontSize;
        settings.StatusBadgeOffset = dto.StatusBadgeOffset ?? settings.StatusBadgeOffset;
        settings.DirectionLabelOpacity = dto.DirectionLabelOpacity ?? settings.DirectionLabelOpacity;
        settings.DirectionLabelWidth = dto.DirectionLabelWidth ?? settings.DirectionLabelWidth;
        settings.DirectionLabelFontSize = dto.DirectionLabelFontSize ?? settings.DirectionLabelFontSize;
        settings.DirectionLabelDistance = dto.DirectionLabelDistance ?? settings.DirectionLabelDistance;
        settings.CenterCrosshairOpacity = version >= 5
            ? dto.CenterCrosshairOpacity ?? settings.CenterCrosshairOpacity
            : settings.CenterCrosshairOpacity;
        settings.MovingCrosshairOpacity = dto.MovingCrosshairOpacity ?? settings.MovingCrosshairOpacity;
        settings.CrosshairSize = dto.CrosshairSize ?? settings.CrosshairSize;
        settings.CrosshairThickness = dto.CrosshairThickness ?? settings.CrosshairThickness;
        settings.CrosshairGap = dto.CrosshairGap ?? settings.CrosshairGap;
        settings.CrosshairOutlineThickness = dto.CrosshairOutlineThickness ?? settings.CrosshairOutlineThickness;
        settings.CrosshairOutlineOpacity = dto.CrosshairOutlineOpacity ?? settings.CrosshairOutlineOpacity;
        settings.CrosshairCenterDotSize = dto.CrosshairCenterDotSize ?? settings.CrosshairCenterDotSize;
        settings.CrosshairCenterDotOpacity = dto.CrosshairCenterDotOpacity ?? settings.CrosshairCenterDotOpacity;
        settings.OuterCrosshairSize = dto.OuterCrosshairSize ?? settings.OuterCrosshairSize;
        settings.OuterCrosshairThickness = dto.OuterCrosshairThickness ?? settings.OuterCrosshairThickness;
        settings.OuterCrosshairGap = dto.OuterCrosshairGap ?? settings.OuterCrosshairGap;
        settings.OuterCrosshairOpacity = dto.OuterCrosshairOpacity ?? settings.OuterCrosshairOpacity;
        settings.CrosshairRed = dto.CrosshairRed ?? settings.CrosshairRed;
        settings.CrosshairGreen = dto.CrosshairGreen ?? settings.CrosshairGreen;
        settings.CrosshairBlue = dto.CrosshairBlue ?? settings.CrosshairBlue;
        settings.CrosshairStyle = ParseEnum(dto.CrosshairStyle, settings.CrosshairStyle);
        settings.CrosshairColorPreset = ParseEnum(dto.CrosshairColorPreset, settings.CrosshairColorPreset);
        settings.OuterRingStyle = ParseEnum(dto.OuterRingStyle, settings.OuterRingStyle);
        settings.MovementBarShape = ParseEnum(dto.MovementBarShape, settings.MovementBarShape);
        settings.StatusBadgeContent = ParseEnum(dto.StatusBadgeContent, settings.StatusBadgeContent);
        settings.StatusBadgePosition = ParseEnum(dto.StatusBadgePosition, settings.StatusBadgePosition);
        settings.DirectionLabelContent = ParseEnum(dto.DirectionLabelContent, settings.DirectionLabelContent);
        settings.ReleaseStopMilliseconds = dto.ReleaseStopMilliseconds ?? settings.ReleaseStopMilliseconds;
        settings.CounterStopMilliseconds = ResolveVersionedDefault(
            dto.CounterStopMilliseconds,
            settings.CounterStopMilliseconds,
            45,
            version < 8);
        settings.ReadyFlashMilliseconds = ResolveVersionedDefault(
            dto.ReadyFlashMilliseconds,
            settings.ReadyFlashMilliseconds,
            90,
            version < 8);
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
            ShowMovementBars = settings.ShowMovementBars,
            ShowDirectionPad = settings.ShowDirectionPad,
            ShowOuterRing = settings.ShowOuterRing,
            ShowStatusBadge = settings.ShowStatusBadge,
            ShowCenterCrosshair = settings.ShowCenterCrosshair,
            ShowCenterDot = settings.ShowCenterDot,
            ShowOuterCrosshairLines = settings.ShowOuterCrosshairLines,
            FadeCrosshairWhileMoving = settings.FadeCrosshairWhileMoving,
            OpenSettingsOnStartup = settings.OpenSettingsOnStartup,
            AutoUpdateEnabled = settings.AutoUpdateEnabled,
            SettingsLanguage = settings.SettingsLanguage.ToString(),
            IndicatorSize = settings.IndicatorSize,
            OverlayOpacity = settings.OverlayOpacity,
            OuterRingSize = settings.OuterRingSize,
            OuterRingThickness = settings.OuterRingThickness,
            OuterRingOpacity = settings.OuterRingOpacity,
            MovementBarsOpacity = settings.MovementBarsOpacity,
            MovementBarLength = settings.MovementBarLength,
            MovementBarThickness = settings.MovementBarThickness,
            MovementBarDistance = settings.MovementBarDistance,
            MovementBarInactiveOpacity = settings.MovementBarInactiveOpacity,
            StatusBadgeOpacity = settings.StatusBadgeOpacity,
            StatusBadgeWidth = settings.StatusBadgeWidth,
            StatusBadgeFontSize = settings.StatusBadgeFontSize,
            StatusBadgeOffset = settings.StatusBadgeOffset,
            DirectionLabelOpacity = settings.DirectionLabelOpacity,
            DirectionLabelWidth = settings.DirectionLabelWidth,
            DirectionLabelFontSize = settings.DirectionLabelFontSize,
            DirectionLabelDistance = settings.DirectionLabelDistance,
            CenterCrosshairOpacity = settings.CenterCrosshairOpacity,
            MovingCrosshairOpacity = settings.MovingCrosshairOpacity,
            CrosshairSize = settings.CrosshairSize,
            CrosshairThickness = settings.CrosshairThickness,
            CrosshairGap = settings.CrosshairGap,
            CrosshairOutlineThickness = settings.CrosshairOutlineThickness,
            CrosshairOutlineOpacity = settings.CrosshairOutlineOpacity,
            CrosshairCenterDotSize = settings.CrosshairCenterDotSize,
            CrosshairCenterDotOpacity = settings.CrosshairCenterDotOpacity,
            OuterCrosshairSize = settings.OuterCrosshairSize,
            OuterCrosshairThickness = settings.OuterCrosshairThickness,
            OuterCrosshairGap = settings.OuterCrosshairGap,
            OuterCrosshairOpacity = settings.OuterCrosshairOpacity,
            CrosshairRed = settings.CrosshairRed,
            CrosshairGreen = settings.CrosshairGreen,
            CrosshairBlue = settings.CrosshairBlue,
            CrosshairStyle = settings.CrosshairStyle.ToString(),
            CrosshairColorPreset = settings.CrosshairColorPreset.ToString(),
            OuterRingStyle = settings.OuterRingStyle.ToString(),
            MovementBarShape = settings.MovementBarShape.ToString(),
            StatusBadgeContent = settings.StatusBadgeContent.ToString(),
            StatusBadgePosition = settings.StatusBadgePosition.ToString(),
            DirectionLabelContent = settings.DirectionLabelContent.ToString(),
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

    private static T ParseEnum<T>(string? value, T fallback)
        where T : struct, Enum
    {
        return Enum.TryParse(value, true, out T result) ? result : fallback;
    }

    private static int ResolveVersionedDefault(int? value, int currentDefault, int previousDefault, bool shouldMigrate)
    {
        if (value is null)
        {
            return currentDefault;
        }

        return shouldMigrate && value == previousDefault ? currentDefault : value.Value;
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
        public bool? ShowMovementBars { get; set; }
        public bool? ShowDirectionPad { get; set; }
        public bool? ShowOuterRing { get; set; }
        public bool? ShowStatusBadge { get; set; }
        public bool? ShowCenterCrosshair { get; set; }
        public bool? ShowCenterDot { get; set; }
        public bool? ShowOuterCrosshairLines { get; set; }
        public bool? FadeCrosshairWhileMoving { get; set; }
        public bool? OpenSettingsOnStartup { get; set; }
        public bool? AutoUpdateEnabled { get; set; }
        public string? SettingsLanguage { get; set; }
        public double? IndicatorSize { get; set; }
        public double? OverlayOpacity { get; set; }
        public double? OuterRingSize { get; set; }
        public double? OuterRingThickness { get; set; }
        public double? OuterRingOpacity { get; set; }
        public double? MovementBarsOpacity { get; set; }
        public double? MovementBarLength { get; set; }
        public double? MovementBarThickness { get; set; }
        public double? MovementBarDistance { get; set; }
        public double? MovementBarInactiveOpacity { get; set; }
        public double? StatusBadgeOpacity { get; set; }
        public double? StatusBadgeWidth { get; set; }
        public double? StatusBadgeFontSize { get; set; }
        public double? StatusBadgeOffset { get; set; }
        public double? DirectionLabelOpacity { get; set; }
        public double? DirectionLabelWidth { get; set; }
        public double? DirectionLabelFontSize { get; set; }
        public double? DirectionLabelDistance { get; set; }
        public double? CenterCrosshairOpacity { get; set; }
        public double? MovingCrosshairOpacity { get; set; }
        public double? CrosshairSize { get; set; }
        public double? CrosshairThickness { get; set; }
        public double? CrosshairGap { get; set; }
        public double? CrosshairOutlineThickness { get; set; }
        public double? CrosshairOutlineOpacity { get; set; }
        public double? CrosshairCenterDotSize { get; set; }
        public double? CrosshairCenterDotOpacity { get; set; }
        public double? OuterCrosshairSize { get; set; }
        public double? OuterCrosshairThickness { get; set; }
        public double? OuterCrosshairGap { get; set; }
        public double? OuterCrosshairOpacity { get; set; }
        public int? CrosshairRed { get; set; }
        public int? CrosshairGreen { get; set; }
        public int? CrosshairBlue { get; set; }
        public string? CrosshairStyle { get; set; }
        public string? CrosshairColorPreset { get; set; }
        public string? OuterRingStyle { get; set; }
        public string? MovementBarShape { get; set; }
        public string? StatusBadgeContent { get; set; }
        public string? StatusBadgePosition { get; set; }
        public string? DirectionLabelContent { get; set; }
        public int? ReleaseStopMilliseconds { get; set; }
        public int? CounterStopMilliseconds { get; set; }
        public int? ReadyFlashMilliseconds { get; set; }
        public double? OffsetX { get; set; }
        public double? OffsetY { get; set; }
    }
}
