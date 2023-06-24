using System.Diagnostics.CodeAnalysis;
using Snek.Core.Cli;
using Snek.Core;

namespace Snek.Console;
using Console = System.Console;

class Program
{
    async static Task Main(string[] args)
    {
        var cliArgs = new CliArgs(args);

        HandleArgs(cliArgs);

        var console = new CliConsole();
        await new Game(cliArgs.GameSettings, console).Play();
    }

    private static void HandleArgs(CliArgs cliArgs)
    {
        if (cliArgs.RequiresHelp)
        {
            HandleHelp(cliArgs);
        }
        if (cliArgs.HasError)
        {
            HandleError(cliArgs);
        }
    }

    [DoesNotReturn]
    private static void HandleHelp(CliArgs cliArgs)
    {
        Console.WriteLine(cliArgs.HelpInfo.ToString());
        Environment.Exit(0);
    }

    [DoesNotReturn]
    private static void HandleError(CliArgs cliArgs)
    {
        Console.WriteLine(string.Join(Environment.NewLine, cliArgs.Errors));
        Console.WriteLine(Environment.NewLine);
        Console.WriteLine(cliArgs.HelpInfo.ToString());
        Environment.Exit(1);
    }
}