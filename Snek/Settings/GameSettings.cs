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
    public int Width { get; set; }

    [CliArg("height", "h"), Range(6, 80)]
    public int Height { get; set; }

    [CliArg("initialTicksPerSecond", "s"), Range(1, 50)]
    public int InitialTicksPerSecond { get; set; }

    [CliArg("increaseSpeedOnEnemyDestroyed", "i")]
    public bool IncreaseSpeedOnEnemyDestroyed { get; set; }

    [CliArg("wallCollisionBehavior", "c")]
    public WallCollisionBehavior WallCollisionBehavior { get; set; }

    [CliArg("audioEnabled", "a")]
    public bool AudioEnabled { get; set; }

    public int DisplayWidthMultiplier => 2;
    public int DisplayHeightMultiplier => 1;
    public int DisplayWidth => DisplayWidthMultiplier * Width;
    public int DisplayHeight => (DisplayHeightMultiplier * Height) + HudHeight;
    public int HudWidth => DisplayWidth;
    public int HudHeight => DisplayHeightMultiplier * 5;
}