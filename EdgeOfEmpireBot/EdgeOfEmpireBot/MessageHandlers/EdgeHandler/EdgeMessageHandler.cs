using HK47.MessageHandlers.Interfaces;

namespace HK47.MessageHandlers;

/// <summary>
/// Handles Commands that are for the EdgeOfEmpire bot
/// </summary>
public class EdgeMessageHandler : IEdgeMessageHandler
{
    /// <inheritdoc/>
    public Task Roll()
    {
        return Task.CompletedTask;
    }
}