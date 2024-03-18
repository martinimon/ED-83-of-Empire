using Discord.WebSocket;

namespace HK47.Router;

public interface IMessageRouter
{
    Task ProcessMessage(SocketMessage message);
}