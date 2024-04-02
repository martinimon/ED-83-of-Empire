using Discord.WebSocket;
using HK47.MessageHandlers.Interfaces;
using HK47.Services;

namespace HK47.Router;

public class MessageRouter(IDataService dataService, ISteamMessageHandler steamHandler, IGeneralMessageHandler generalHandler, IMessageService messageService) : IMessageRouter
{
    public async Task ProcessMessage(SocketMessage message)
    {
        try
        {
            Console.WriteLine("Processing Message...");
            if (!message.Content.StartsWith('.'))
            {
                // Ignore message.
                Console.WriteLine("Ignoring Message....");
                return;
            }
            await messageService.ValidateLength(message.Content);

            // Removes the '.' prefix to indicate a bot command.
            var messageCommand = message.Content.Remove(0, 1);
            var messageChannel = message.Channel;
            await RouteCommand(messageCommand);

            Console.WriteLine(messageCommand + "  command processed. ");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error Processing Command");
            Console.WriteLine(ex.ToString());
        }
    }

    /// <summary>
    /// Processes a Command
    /// </summary>
    /// <param name="command"></param>
    private async Task RouteCommand(string userInput)
    {
        // Check if the command is a custom command that requires functionality, ie a roll.
        // If not a custom command try processing it as a simple command.
        // TODO: Consider the order of this once the scope of overall commands have been established. For efficency.
        var command = userInput.Split(' ').First();

        var commands = await dataService.ReadFromFile<Dictionary<string,string>>("Commands");

        if (!commands.TryGetValue(command!.ToLower(), out var process))
        {
            await messageService.SendMessage("[Error]: Invalid command detected...");
            return;
        }

        switch(process)
        {
            case "steam":
                await steamHandler.ProcessCommand(userInput);
                break;
            case "general":
                await generalHandler.ProcessCommand(userInput);
                break;
            case "edge":
                break;
            default:
                break;
        }
    }
}