using Snek.Settings;

namespace Snek;

class Program
{
    static void Main()
    {
        new Game(GameSettings.Default).Play();
    }
}