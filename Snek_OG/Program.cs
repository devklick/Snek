using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snek_OG
{
    class Program
    {
        static void Main(string[] args)
        {
            Engine engine = new Engine(30, 30);
            engine.Initialise();
            engine.Run();
        }
    }
}
