using Snek.Events;
using Snek.Interfaces;

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

    private readonly Position _hudOffset = Position.Default;

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
    /// A simple object intended to be used for locking the <see cref="Draw(Cell, Position?, bool)"/> method.
    /// </summary>
    private readonly object drawLock = new();

    /// <summary>
    /// Constructs the display and initializes the console by drawing the grid.
    /// </summary>
    /// <param name="gameGrid">The grid that sits behind the display. All cells associated with the grid will be drawn to the display.</param>
    /// <param name="widthMultiplier">The number of times to repeat a cell along the `X` axis when drawing it to the display.</param>
    /// <param name="heightMultiplier">The number of times to repeat a cell along the `Y` axis when drawing it to the display.</param>
    public Display(IGrid gameGrid, IGrid hud, int widthMultiplier, int heightMultiplier)
    {
        _widthMultiplier = widthMultiplier;
        _heightMultiplier = heightMultiplier;
        _width = gameGrid.Width * _widthMultiplier;
        _height = gameGrid.Height * _heightMultiplier + hud.Height * _heightMultiplier;

        InitializeConsole();

        foreach (var cell in gameGrid.Cells) Draw(cell);
        foreach (var cell in hud.Cells) Draw(cell, hud.Offset, true);

        gameGrid.CellUpdated += OnGameGridCellUpdated;
        hud.CellUpdated += OnHudCellUpdated;
        _hudOffset = hud.Offset;
    }

    /// <summary>
    /// Draws the specified <paramref name="cell"/> to the console.
    /// </summary>
    /// <param name="cell">The data to be drawn</param>
    /// <param name="offset">The position that the cell should be drawn relative to. Defaults to `0,0`</param>
    /// <param name="disableMultipliers">Whether or not multiplication (or rather repetition) of cells should be disabled.</param>
    private void Draw(Cell cell, Position? offset = null, bool disableMultipliers = false)
    {
        lock (drawLock)
        {
            offset ??= Position.Default;
            var _bgCopy = Console.BackgroundColor;
            var _fgCopy = Console.ForegroundColor;

            Console.BackgroundColor = cell.BackgroundColor;
            Console.ForegroundColor = cell.SpriteColor;

            var heightMultiplier = disableMultipliers ? 1 : _heightMultiplier;
            var widthMultiplier = disableMultipliers ? 1 : _widthMultiplier;

            // the grid cell may take up more than one display cell. 
            for (int r = 0; r < heightMultiplier; r++)
            {
                for (int c = 0; c < widthMultiplier; c++)
                {
                    var x = (cell.Position.X * widthMultiplier) + c + offset.Value.X;
                    var y = (cell.Position.Y * heightMultiplier) + r + offset.Value.Y;
                    Console.SetCursorPosition(x, y);
                    Console.Write(cell.Sprite);
                }
            }

            Console.BackgroundColor = _bgCopy;
            Console.ForegroundColor = _fgCopy;
        }
    }

    /// <summary>
    /// Handles cells that have been updated on the grid, drawing them to the console.
    /// </summary>
    private void OnGameGridCellUpdated(object? sender, CellUpdatedEventArgs e)
        => Draw(e.Cell, null, e.PreserveExact);

    /// <summary>
    /// Handles cells on the HUD that have been updated, drawing them to the console.
    /// </summary>
    private void OnHudCellUpdated(object? sender, CellUpdatedEventArgs e)
        => Draw(e.Cell, _hudOffset, e.PreserveExact);

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

        // We add 1 to the buffer and window height to accommodate for overflow.
        // TODO: Verify this works (run on windows)
        Console.SetWindowSize(_width, _height + 1);
        Console.SetBufferSize(_width, _height + 1);
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