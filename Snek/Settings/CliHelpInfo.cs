using System.Text;

namespace Snek.Settings;

public class CliHelpInfo
{
    public static readonly string Description = " A terminal version of the classic mobile game, Snake.";
    public static readonly string FullName = "--help";
    public static readonly string ShortName = "-h";
    public List<CliArgHelpInfo> ArgsInfo = new()
    {
        new CliArgHelpInfo(FullName, ShortName, "Shows this help information")
    };

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine(Description);
        sb.AppendLine();

        sb.AppendLine("Supported arguments:");

        foreach (var arg in ArgsInfo)
        {
            sb.AppendLine(arg.ToString());
        }

        return sb.ToString().TrimEnd('\n').TrimEnd('\r');
    }

    public void Add(CliArgHelpInfo cliArg)
        => ArgsInfo.Add(cliArg);
}