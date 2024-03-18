namespace HK47.MessageHandlers.Interfaces;

public interface IGeneralMessageHandler
{
    Task ProcessCommand(string[] commandParams);
}