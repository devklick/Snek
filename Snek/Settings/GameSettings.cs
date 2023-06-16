using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Snek.Settings;

public class GameSettings
{
    public static readonly GameSettings Default = new()
    {
        Width = 15,
        Height = 15,
        InitialTicksPerSecond = 8,
        IncreaseSpeedOnEnemyDestroyed = false,
        WallCollisionBehavior = WallCollisionBehavior.Portal,
        AudioEnabled = true
    };

    [CliArg("width", "w"), Range(13, 80)]
    [Description("The number of cells along the horizontal axis")]
    public int Width { get; set; }

    [CliArg("height", "h"), Range(6, 80)]
    [Description("The number of cells along the vertical axis")]
    public int Height { get; set; }

    [CliArg("initialTicksPerSecond", "s"), Range(1, 50)]
    [Description("The starting number of times per second the snake will move")]
    public int InitialTicksPerSecond { get; set; }

    [CliArg("increaseSpeedOnEnemyDestroyed", "i")]
    [Description("Whether or not the snake should get faster every time it destroys an enemy")]
    public bool IncreaseSpeedOnEnemyDestroyed { get; set; }

    [CliArg("wallCollisionBehavior", "c")]
    [Description("How the game should behave when the snake collides with a wall")]
    public WallCollisionBehavior WallCollisionBehavior { get; set; }

    [CliArg("audioEnabled", "a")]
    [Description("Whether or not sound effects should play")]
    public bool AudioEnabled { get; set; }

    public int DisplayWidthMultiplier => 2;
    public int DisplayHeightMultiplier => 1;
    public int DisplayWidth => DisplayWidthMultiplier * Width;
    public int DisplayHeight => (DisplayHeightMultiplier * Height) + HudHeight;
    public int HudWidth => DisplayWidth;
    public int HudHeight => DisplayHeightMultiplier * 5;
}