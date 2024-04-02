using Discord.WebSocket;

namespace HK47.Router;

public interface IMessageRouter
{
    /// <summary>
    /// Routes the commands to the appropriate message handlers
    /// </summary>
    Task ProcessMessage(SocketMessage message);
}