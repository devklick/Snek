namespace Snek.Events;

public class ScoreUpdatedEventArgs
{
    public int Score { get; }

    public ScoreUpdatedEventArgs(int score)
    {
        Score = score;
    }
}
