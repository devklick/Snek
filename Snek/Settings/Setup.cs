using Snek.UI;

namespace Snek.Settings;

public class Setup
{
    private readonly GameGrid _grid;
    private readonly Display _display;


    public Setup()
    {
        _grid = new(20, 20);
        _display = new(20, 20, 0, 0, _grid);

        var form = new Form(

        );
    }
}