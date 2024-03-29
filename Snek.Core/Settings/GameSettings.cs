using System.Linq.Expressions;
using System.Diagnostics.CodeAnalysis;
using System.ComponentModel;
using Snek.Core.Cli;
using System.ComponentModel.DataAnnotations;

namespace Snek.Core.Settings;

public class GameSettings
{
    public static readonly GameSettings Default = new()
    {
        Width = 15,
        Height = 15,
        InitialTicksPerSecond = 8,
        IncreaseSpeedOnEnemyDestroyed = false,
        WallCollisionBehavior = WallCollisionBehavior.GameOver,
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
    public int DisplayWidthMultiplier => 2;
    public int DisplayHeightMultiplier => 1;
    public int DisplayWidth => DisplayWidthMultiplier * Width;
    public int DisplayHeight => (DisplayHeightMultiplier * Height) + HudHeight;
    public int HudWidth => DisplayWidth;
    public int HudHeight => DisplayHeightMultiplier * 5;

    public override string ToString()
    {
        var parts = new[]
        {
            GetProp(x => x.Width),
            GetProp(x => x.Height),
            GetProp(x => x.InitialTicksPerSecond),
            GetProp(x => x.IncreaseSpeedOnEnemyDestroyed),
            GetProp(x => x.WallCollisionBehavior),
            GetProp(x => x.AudioEnabled),
            GetProp(x => x.DisplayWidthMultiplier),
            GetProp(x => x.DisplayHeightMultiplier),
            GetProp(x => x.DisplayWidth),
            GetProp(x => x.DisplayHeight),
            GetProp(x => x.HudWidth),
            GetProp(x => x.HudHeight),
        };
        return string.Join(",", parts);
    }

    private string GetProp(Expression<Func<GameSettings, object>> exp)
    {
        var name = exp.Body switch
        {
            MemberExpression m => GetMemberName(m),
            UnaryExpression u => GetMemberName(u),
            _ => throw CannotGetMember(exp)
        };

        var value = exp.Compile().Invoke(this);
        return $"{name}={value}";
    }

    private static string GetMemberName(MemberExpression exp)
        => exp.Member.Name;
    private static string GetMemberName(UnaryExpression exp) => exp.Operand switch
    {
        MemberExpression m => GetMemberName(m),
        _ => throw CannotGetMember(exp)
    };

    [DoesNotReturn]
    private static Exception CannotGetMember(Expression exp)
        => throw new NotImplementedException($"Cannot get member name from a ${exp.GetType().Name}");
}