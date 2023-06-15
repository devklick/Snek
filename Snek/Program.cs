using Snek.Settings;

namespace Snek;

class Program
{
    static void Main(string[] args)
    {
        var cliArgs = new CliArgs(args);

        if (cliArgs.Help)
        {
            cliArgs.HelpInfo.ForEach(Console.WriteLine);
            Environment.Exit(0);
        }
        if (cliArgs.Errors.Any())
        {
            cliArgs.Errors.ForEach(Console.WriteLine);
            Environment.Exit(1);
        }

        new Game(cliArgs.GameSettings).Play();
    }
}