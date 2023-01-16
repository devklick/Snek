using Snek.Events;

namespace Snek.Interfaces;

public interface IGrid
{
    List<Cell> Cells { get; }

    /// <summary>
    /// The event that is fired whenever a <see cref="Cell"/> has been updated.
    /// </summary>
    event CellUpdatedEventHandler? CellUpdated;
}