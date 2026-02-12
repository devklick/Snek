namespace Snek.Core.Events;

public class ScoreUpdatedEventArgs(int score)
{
    public int Score { get; } = score;
}
