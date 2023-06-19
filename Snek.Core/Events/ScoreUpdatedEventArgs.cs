namespace Snek.Core.Events;

public class ScoreUpdatedEventArgs
{
    public int Score { get; }

    public ScoreUpdatedEventArgs(int score)
    {
        Score = score;
    }
}
