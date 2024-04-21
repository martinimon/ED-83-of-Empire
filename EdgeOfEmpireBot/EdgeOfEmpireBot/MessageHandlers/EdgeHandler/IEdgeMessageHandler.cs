namespace HK47.MessageHandlers.Interfaces;

/// <summary>
/// The Interface for the EdgeMessageHandler.
/// </summary>
public interface IEdgeMessageHandler
{
    /// <summary>
    /// Rolls the dice based off the EdgeOfEmpire Rules
    /// </summary>
    Task Roll();
}