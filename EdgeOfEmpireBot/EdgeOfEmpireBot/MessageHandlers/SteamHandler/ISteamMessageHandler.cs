namespace HK47.MessageHandlers.Interfaces;

public interface ISteamMessageHandler
{
    Task ProcessCommand(string[] commandParams);
}