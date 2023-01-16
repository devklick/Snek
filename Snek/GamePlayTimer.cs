using Snek.Events;

namespace Snek;

public class GamePlayTimer
{
    public GamePlayTimerUpdatedEventHandler? Updated { get; set; }
    private readonly System.Timers.Timer _timer;
    private readonly System.Diagnostics.Stopwatch _stopwatch;

    public GamePlayTimer(int intervalMs)
    {
        _timer = new(intervalMs);
        _stopwatch = new();
        _timer.Elapsed += (s, e) => Updated?.Invoke(this, new(_stopwatch.Elapsed));
    }

    public void Start()
    {
        _timer.Start();
        _stopwatch.Start();
    }

    public void Stop()
    {
        _timer.Start();
        _stopwatch.Stop();
    }

    public void Reset()
    {
        _stopwatch.Reset();
        Updated?.Invoke(this, new(_stopwatch.Elapsed));
    }
}