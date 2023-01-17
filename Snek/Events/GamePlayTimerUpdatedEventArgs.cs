namespace Snek.Events;

public class GamePlayTimerUpdatedEventArgs
{
    public TimeSpan Elapsed { get; }
    public GamePlayTimerUpdatedEventArgs(TimeSpan elapsed)
    {
        Elapsed = elapsed;
    }
}