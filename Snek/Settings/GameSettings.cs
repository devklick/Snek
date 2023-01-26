namespace Snek;

public class GameSettings
{
    public static readonly GameSettings Default = new()
    {
        Width = 15,
        Height = 15,
        InitialTicksPerSecond = 3,
        IncreaseSpeedOnEnemyDestroyed = true,
        WallCollisionBehavior = WallCollisionBehavior.Rebound
    };

    public int Width { get; set; }
    public int Height { get; set; }
    public int InitialTicksPerSecond { get; set; }
    public bool IncreaseSpeedOnEnemyDestroyed { get; set; }
    public WallCollisionBehavior WallCollisionBehavior { get; set; }
}