namespace HK47.MessageHandlers.Interfaces;

public interface ISteamMessageHandler
{
    /// <summary>
    /// Processes Steam Commands
    /// </summary>
    Task ProcessCommand(string userInput);
}