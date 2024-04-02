using HK47.MessageHandlers.Interfaces;
using HK47.Services;

namespace HK47.MessageHandlers;

public class SteamMessageHandler(ISteamService steamService, IDataService dataService, IMessageService messageService) : ISteamMessageHandler
{
    /// </inheritdoc>
    public async Task ProcessCommand(string userInput)
    {
        var commandParams = userInput.Split(' ');
        var command = commandParams!.First().ToLower();
        switch (command)
        {
            case "requestgame":
            case "gr":
                try
                {
                    var appId = commandParams[1];
                    var game = await steamService.RetrieveGameFromSteam(appId);
                    await dataService.WriteGameToFile(game);
                    await messageService.SendMessage($"```[Statement]: {game.Name} was added to the list.```");
                }
                catch (Exception ex)
                {
                    await messageService.SendMessage($"[Error]: Game not added due to the following:\n{ex.Message}");
                }
                break;
            case "games":
                var (msg, updatedGames) = await steamService.GetGamePrices();
                if (updatedGames.Count != 0) { await dataService.UpdateGames(updatedGames); }
                await messageService.SendMessage($"{msg}");
                break;
            default:
                await messageService.SendMessage("[Error]: Invalid command detected...");
                break;
        }
    }
}