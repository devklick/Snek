using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Snek.Core;
using Snek.Core.Settings;

namespace Snek.Cli.Args;

public class CliGameSettings
{
    public static readonly CliGameSettings Default = new()
    {
        Width = 15,
        Height = 15,
        InitialTicksPerSecond = 8,
        IncreaseSpeedOnEnemyDestroyed = false,
        WallCollisionBehavior = WallCollisionBehavior.Portal,
        AudioEnabled = true,
        DebugLogging = false,
    };

    [CliArg("width", "x"), Range(13, 80)]
    [Description("The number of cells along the horizontal axis")]
    public int Width { get; set; }

    [CliArg("height", "y"), Range(6, 80)]
    [Description("The number of cells along the vertical axis")]
    public int Height { get; set; }

    [CliArg("speed", "s"), Range(1, 50)]
    [Description("The starting number of times per second the snake will move")]
    public int InitialTicksPerSecond { get; set; }

    [CliArg("increase-speed", "i")]
    [Description("Whether or not the snake should get faster every time it destroys an enemy")]
    public bool IncreaseSpeedOnEnemyDestroyed { get; set; }

    [CliArg("collision", "c")]
    [Description("How the game should behave when the snake collides with a wall")]
    public WallCollisionBehavior WallCollisionBehavior { get; set; }

    [CliArg("debug", "d")]
    [Description("Whether or not to log events to file")]
    public bool DebugLogging { get; set; }

    [CliArg("audio", "a")]
    [Description("Whether or not sound effects should play")]
    public bool AudioEnabled { get; set; }

    public static implicit operator GameSettings(CliGameSettings cliGameSettings)
    {
        return new()
        {
            AudioEnabled = cliGameSettings.AudioEnabled,
            DebugLogging = cliGameSettings.DebugLogging,
            Height = cliGameSettings.Height,
            IncreaseSpeedOnEnemyDestroyed = cliGameSettings.IncreaseSpeedOnEnemyDestroyed,
            InitialTicksPerSecond = cliGameSettings.InitialTicksPerSecond,
            Width = cliGameSettings.Width,
            WallCollisionBehavior = cliGameSettings.WallCollisionBehavior
        };
    }
}