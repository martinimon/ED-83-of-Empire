using HK47.Models;
using HK47.Wrappers;
using Newtonsoft.Json;

namespace HK47.Services;
/// <summary>
/// The Steam Service.
/// </summary>
public class SteamService(ISteamStoreApi steamApi) : ISteamService
{
    private readonly ISteamStoreApi steamApi = steamApi;
    private readonly string filePath = Path.Combine("Data/Games.json");

    public SteamService() : this(new SteamStoreApi()) { }

    /// <summary>
    /// Iterates through the list of games stored and queries the steam api
    /// to get the prices for the games
    /// </summary>
    /// <returns>This will return the Details in the form of a string.
    /// If there is any changes to the prices of the games stored we will return
    /// a list of games that need to be updated</returns>
    public async Task<(string Message, List<SteamGameDetails> GamesWithNewPrice)> GetGamePrices()
    {
        var games = JsonConvert.DeserializeObject<List<SteamGameDetails>>(await File.ReadAllTextAsync(filePath)) ?? [];
        var updatedGames = new List<SteamGameDetails>();
        var result = string.Empty;
        foreach (var game in games)
        {
            try
            {
                var gameDetails = await steamApi.GetAppDetails(int.Parse(game.AppID), "AU");
                var priceOverview = gameDetails.PriceOverview;
                var currentPrice = priceOverview.Initial;
                var discountPercent = priceOverview.DiscountPercent;
                var discountedPrice = priceOverview.Final;
                if (!Equals(game.Price, priceOverview.Initial))
                {
                    game.Name = gameDetails.Name;
                    game.Price = priceOverview.Initial.ToString();
                    updatedGames.Add(game);
                }

                if (currentPrice != 0)
                {
                    result += $"{game.Name} - ${game.Price} (${currentPrice})";

                    if (discountPercent > 0)
                    {
                        result += $" {discountPercent}% off, now ${discountedPrice}";
                    }

                    result += "\n";
                }
            }
            catch (Exception ex)
            {
                result += $"\nCouldn't receive price for {game.Name} due to {ex.Message}";
            }
        }

        return (result, updatedGames);
    }

    /// <summary>
    /// Uses the Steam API to retrieve a game with the appId
    /// </summary>
    /// <param name="appId">App Id of the game you wish to retrieve</param>
    /// <returns>Returns a transformed version of the result</returns>
    public async Task<SteamGameDetails> RetrieveGameFromSteam(string appId)
    {
        var game = await steamApi.GetAppDetails(int.Parse(appId), "AU");

        // The SteamGameDetails Model can be extended to have more information if we want it but this is good for now.
        if (game.PriceOverview == null) { throw new Exception("Game not available on steam"); }
        return new SteamGameDetails
        {
            AppID = appId.Trim(),
            Name = game.Name.Trim(),
            Price = game.PriceOverview.Final.ToString()
        };
    }
}
