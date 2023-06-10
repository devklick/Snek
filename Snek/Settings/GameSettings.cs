namespace Snek;

public class GameSettings
{
    public static readonly GameSettings Default = new()
    {
        Width = 15,
        Height = 15,
        InitialTicksPerSecond = 8,
        IncreaseSpeedOnEnemyDestroyed = false,
        WallCollisionBehavior = WallCollisionBehavior.Portal
    };

    public int Width { get; set; }
    public int Height { get; set; }
    public int InitialTicksPerSecond { get; set; }
    public bool IncreaseSpeedOnEnemyDestroyed { get; set; }
    public WallCollisionBehavior WallCollisionBehavior { get; set; }

    public int DisplayWidthMultiplier => 2;
    public int DisplayHeightMultiplier => 1;
    public int DisplayWidth => DisplayWidthMultiplier * Width;
    public int DisplayHeight => DisplayHeightMultiplier * Height;
    public int HudWidth => DisplayWidth;
    public int HudHeight => DisplayHeightMultiplier * 5;
}