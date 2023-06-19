using Snek.Core.Events;

namespace Snek.Core.Interfaces;

public interface IGrid
{
    /// <summary>
    /// The number of cells along the horizontal axis.
    /// </summary>
    int Width { get; }

    /// <summary>
    /// The number of cells along the vertical axis.
    /// </summary>
    int Height { get; }

    /// <summary>
    /// The cells that make up the grid. 
    /// </summary>
    /// <remarks>
    /// There may be a requirement in future for these to be index by position (e.g. multi-dimensional array, dictionary, etc)
    /// </remarks>
    List<Cell> Cells { get; }

    /// <summary>
    /// The offset is used when drawing the grids cells to the display. 
    /// The position of the cells will be relative to the offset.
    /// </summary>
    Position Offset { get; }

    /// <summary>
    /// The event that is fired whenever a <see cref="Cell"/> has been updated.
    /// </summary>
    event CellUpdatedEventHandler? CellUpdated;
}