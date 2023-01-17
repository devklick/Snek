namespace Snek.Events;

/// <summary>
/// The signature of the method that consumes Cell update events.
/// </summary>
/// <param name="sender">The thing that sent the event</param>
/// <param name="e">The event arguments</param>
public delegate void CellUpdatedEventHandler(object? sender, CellUpdatedEventArgs e);