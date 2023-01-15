using static Snek.Grid;

namespace Snek;

/// <summary>
/// Service responsible for rendering game information
/// </summary>
public class Display
{
    /// <summary>
    /// The overall width of the display.
    /// </summary>
    private readonly int _width;

    /// <summary>
    /// The overall height of the display.
    /// </summary>
    private readonly int _height;

    /// <summary>
    /// The multiplier to be applied when drawing along the `X` axis. 
    /// </summary>
    /// <example>
    /// When set to `2`, a cell will be drawn twice along the `X` axis.
    /// </example>
    private readonly int _widthMultiplier;

    /// <summary>
    /// The multiplier to be applied when drawing along the `Y` axis. 
    /// </summary>
    /// <example>
    /// When set to `2`, a cell will be drawn twice along the `Y` axis.
    /// </example>
    private readonly int _heightMultiplier;

    /// <summary>
    /// The grid that sits behind the display.
    /// </summary>
    private readonly Grid _grid;

    /// <summary>
    /// Constructs the display and initializes the console by drawing the grid.
    /// </summary>
    /// <param name="grid">The grid that sits behind the display. All cells associated with the grid will be drawn to the display.</param>
    /// <param name="widthMultiplier">The number of times to repeat a cell along the `X` axis when drawing it to the display.</param>
    /// <param name="heightMultiplier">The number of times to repeat a cell along the `Y` axis when drawing it to the display.</param>
    public Display(Grid grid, int widthMultiplier, int heightMultiplier)
    {
        _grid = grid;
        _widthMultiplier = widthMultiplier;
        _heightMultiplier = heightMultiplier;
        _width = _grid.Width * _widthMultiplier;
        _height = _grid.Height * _heightMultiplier;

        InitializeConsole();

        foreach (var cell in _grid.Cells) Draw(cell);

        _grid.CellUpdated += OnCellUpdated;
    }

    /// <summary>
    /// Draws the specified <paramref name="cell"/> to the console.
    /// </summary>
    /// <param name="cell">The data to be drawn</param>
    public void Draw(Cell cell)
    {
        var _bgCopy = Console.BackgroundColor;
        var _fgCopy = Console.ForegroundColor;

        Console.BackgroundColor = cell.BackgroundColor;
        Console.ForegroundColor = cell.SpriteColor;

        // the grid cell may take up more than one display cell. 
        for (int r = 0; r < _heightMultiplier; r++)
        {
            for (int c = 0; c < _widthMultiplier; c++)
            {
                var x = (cell.Position.X * _widthMultiplier) + c;
                var y = (cell.Position.Y * _heightMultiplier) + r;
                Console.SetCursorPosition(x, y);
                Console.Write(cell.Sprite);
            }
        }

        Console.BackgroundColor = _bgCopy;
        Console.ForegroundColor = _fgCopy;
    }

    /// <summary>
    /// Handles cells that have been updated on the grid, drawing them to the console.
    /// </summary>
    public void OnCellUpdated(object sender, CellUpdatedEventArgs e)
    {
        Draw(e.Cell);
    }

    /// <summary>
    /// Initializes the console dimensions, if possible. 
    /// Otherwise, verifies that the current console dimensions will support the game size.
    /// </summary>
    private void InitializeConsole()
    {
        Console.CursorVisible = false;
        if (OperatingSystem.IsWindows())
        {
            InitializeWindowsConsole();
        }
        else
        {
            InitializeNonWindowsConsole();
        }
    }

    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    private void InitializeWindowsConsole()
    {
        // In Windows OS, we can control the size of the terminal at runtime, 
        // so lets set the dimensions based on the size of the game.
        Console.SetWindowSize(_width, _height);
        Console.SetBufferSize(_width, _height);
    }

    private void InitializeNonWindowsConsole()
    {
        // In non-windows OS, we cant control the console size.
        // Instead, verify that its size will support the game dimensions and throw an error if not.
        var badWidth = Console.WindowWidth < _width;
        var badHeight = Console.WindowHeight < _height;

        if (badWidth || badHeight)
        {
            var axes = badWidth && badHeight ? "width and height" : badWidth ? "width" : "height";
            throw new NotSupportedException($"The console dimensions are too small to support the game size. Try increasing the console {axes}.");
        }
    }
}