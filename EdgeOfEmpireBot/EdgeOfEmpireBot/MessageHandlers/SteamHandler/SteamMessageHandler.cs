using HK47.MessageHandlers.Interfaces;
using HK47.Services;

namespace HK47.MessageHandlers;

public class SteamMessageHandler(ISteamService steamService, IDataService dataService, IMessageService messageService) : ISteamMessageHandler
{
    /// <inheritdoc/>
    public async Task ProcessCommand(string userInput)
    {
        var commandParams = userInput.Split(' ');
        var command = commandParams[0].ToLower();
        switch (command)
        {
            case "requestGame":
                await RecordGameRequest(commandParams[1]);
                break;
            case "displayGames":
                await DisplayCurrentGames();
                break;
            default:
                await messageService.SendMessage("[Error]: You broke the router so badly that you ended up in a place thats unreachable good job jack ass!");
                break;
        }
    }

    private async Task RecordGameRequest(string appId)
    {
        try
        {
            var game = await steamService.RetrieveGameFromSteam(appId);
            await dataService.WriteGameToFile(game);
            await messageService.SendMessage($"```[Statement]: {game.Name} was added to the list.```");
        }
        catch (Exception ex)
        {
            await messageService.SendMessage($"[Error]: Game not added due to the following:\n{ex.Message}");
        }
    }

    private async Task DisplayCurrentGames()
    {
        var (msg, updatedGames) = await steamService.GetGamePrices();
        if (updatedGames.Count != 0) { await dataService.UpdateGames(updatedGames); }
        await messageService.SendMessage($"{msg}");
    }
}