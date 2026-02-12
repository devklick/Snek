namespace Snek.Core.Events;

/// <summary>
/// Object that exposes the relevant information that's published when a <see cref="Snek.Cell"/> is updated.
/// </summary>
/// <param name="Cell">The cell that's been updated</param>
public class CellUpdatedEventArgs(Cell cell, bool preserveExact = false)
{
    /// <summary>
    /// The cell that's been updated
    /// </summary>
    public Cell Cell { get; } = cell;

    /// <summary>
    /// Whether or not this cell should be preserved exactly as-is, should it be drawn to console. 
    /// This should typically be set to true if the cell represents a single character in a string of human-readable text. 
    /// If set to false, the Display will decide whether or not the character will be repeated.
    /// </summary>
    public bool PreserveExact { get; } = preserveExact;
}