using Snek.Settings;

namespace Snek;

class Program
{
    static void Main(string[] args)
    {
        var cliArgs = new CliArgs(args);

        if (cliArgs.Help)
        {
            Console.WriteLine(cliArgs.HelpInfo.ToString());
            Environment.Exit(cliArgs.Error ? 1 : 0);
        }

        new Game(cliArgs.GameSettings).Play();
    }
}