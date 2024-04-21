namespace HK47.MessageHandlers.Interfaces;

public interface IGeneralMessageHandler
{
    /// <summary>
    /// Processes General Commands
    /// </summary>
    Task ProcessCommand(string userInput);
}