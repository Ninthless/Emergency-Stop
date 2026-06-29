namespace EmergencyStop;

public enum MovementIndicatorState
{
    Neutral,
    Moving,
    Stopping,
    CounterStrafing,
    Ready,
    Conflict
}

public sealed record MovementSnapshot(
    MovementIndicatorState State,
    string StatusText,
    string DetailText,
    string DirectionText,
    double Progress,
    bool ForwardActive,
    bool BackwardActive,
    bool LeftActive,
    bool RightActive)
{
    public static MovementSnapshot Initial { get; } = new(
        MovementIndicatorState.Neutral,
        "IDLE",
        "待机",
        string.Empty,
        0,
        false,
        false,
        false,
        false);
}
