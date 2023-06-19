namespace Snek.Core.Events;

public class GamePlayTimerUpdatedEventArgs
{
    public TimeSpan Elapsed { get; }
    public GamePlayTimerUpdatedEventArgs(TimeSpan elapsed)
    {
        Elapsed = elapsed;
    }
}