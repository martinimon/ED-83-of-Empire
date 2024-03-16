using EdgeOfEmpireBot.IService;
using EdgeOfEmpireBot.Models;
using Newtonsoft.Json;
using SteamWebAPI2.Interfaces;
using SteamWebAPI2.Utilities;

namespace EdgeOfEmpireBot.Service
{
    /// <summary>
    /// The Steam Service.
    /// </summary>
    public class SteamService : ISteamService
    {
        private readonly SteamStore steamInterface;
        private readonly string filePath;

        public SteamService()
        {
            steamInterface = new SteamWebInterfaceFactory(GetToken()).CreateSteamStoreInterface();
            filePath = Path.Combine("Data/Games.json");
        }

        public async Task<(string Message, List<SteamGameDetails> GamesWithNewPrice)> GetGamePrices()
        {
            var games = JsonConvert.DeserializeObject<List<SteamGameDetails>>(await File.ReadAllTextAsync(filePath)) ?? new List<SteamGameDetails>();
            var updatedGames = new List<SteamGameDetails>();
            var result = "";
            foreach (var game in games)
            {
                try
                {
                    var gameDetails = await steamInterface.GetStoreAppDetailsAsync(uint.Parse(game.AppID), "AU");
                    var priceOverview = gameDetails.PriceOverview;
                    var currentPrice = !string.IsNullOrEmpty(priceOverview.InitialFormatted) ? priceOverview.InitialFormatted : priceOverview.FinalFormatted;
                    var discountPercent = priceOverview.DiscountPercent;
                    var discountedPrice = priceOverview.FinalFormatted;
                    if (!string.Equals(game.Price, priceOverview.InitialFormatted))
                    {
                        game.Name = gameDetails.Name;
                        game.Price = priceOverview.InitialFormatted;
                        updatedGames.Add(game);
                    };
                    if (!string.IsNullOrEmpty(currentPrice))
                    {
                        result += $"{game.Name} - {game.Price} ({currentPrice})";

                        if (discountPercent > 0)
                        {
                            result += $" {discountPercent}% off, now {discountedPrice}";
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

        public async Task<SteamGameDetails> RetrieveGameFromSteam(string appId)
        {
            var game = await steamInterface.GetStoreAppDetailsAsync(uint.Parse(appId), "AU");

            // The SteamGameDetails Model can be extended to have more information if we want it but this is good for now.
            return new SteamGameDetails
            {
                AppID = appId.Trim(),
                Name = game.Name.Trim(),
                Price = game.PriceOverview.FinalFormatted.Trim()
            };
        }

        /// <summary>
        /// Gets the Steam Token
        /// </summary>
        /// <returns>the token string</returns>
        private static string GetToken()
        {
            var token = string.Empty;

            try
            {
                string path = Path.Combine("Data/steamToken.txt");
                token = File.ReadAllText(path);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return token;
        }
    }
}
