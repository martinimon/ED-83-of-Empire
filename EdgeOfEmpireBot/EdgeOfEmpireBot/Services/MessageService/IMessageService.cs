using Discord.WebSocket;

namespace EdgeOfEmpireBot.Services;

public interface IMessageService
{
    /// <summary>
    /// Sends a message to Discord.
    /// </summary>
    Task SendMessage(string messageResponse, ISocketMessageChannel messageResponseChannel);

    /// <summary>
    /// Processes a message.
    /// </summary>
    Task ProcessMessage(SocketMessage message);

    /// <summary>
    /// A bool to indicate if the message is safe.
    /// </summary>
    /// <returns>bool</returns>
    bool MessageIsSafe();
}
