namespace Snek.Core.Events;

public class GamePlayTimerUpdatedEventArgs(TimeSpan elapsed)
{
    public TimeSpan Elapsed { get; } = elapsed;
}