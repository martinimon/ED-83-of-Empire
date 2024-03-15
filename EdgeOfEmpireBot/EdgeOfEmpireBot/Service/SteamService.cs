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
        private readonly HttpClient client;
        private readonly SteamStore steamInterface;
        private readonly string filePath;

        public SteamService()
        {
            client = new HttpClient();
            steamInterface = new SteamWebInterfaceFactory(GetToken()).CreateSteamStoreInterface();
            filePath = Path.Combine("Data/games.json");
        }

        public async Task<string> GetGamePrices()
        {
            var games = JsonConvert.DeserializeObject<List<Game>>(await File.ReadAllTextAsync(filePath)) ?? new List<Game>();
            var result = "";
            foreach (var game in games)
            {
                var gameDetails = await steamInterface.GetStoreAppDetailsAsync(uint.Parse(game.AppID));
                var priceOverview = gameDetails.PriceOverview;
                var currentPrice = priceOverview.InitialFormatted;
                var discountPercent = priceOverview.DiscountPercent;
                var discountedPrice = priceOverview.FinalFormatted;

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

            return result;
        }

        public async Task<string> GetGameByName(string name)
        {
            var games = JsonConvert.DeserializeObject<List<Game>>(await File.ReadAllTextAsync(filePath)) ?? new List<Game>();
            var game = games.Where(game => string.Equals(game.Name, name, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            if (game == null) { return $"No game was found with the name {name}"; }

            return $"Name: {game.Name}\nId:{game.AppID}\nPrice:{game.Price}";
        }

        public async Task AddGameToList(string appId)
        {
            var game = await steamInterface.GetStoreAppDetailsAsync(uint.Parse(appId));

            // The Game Model can be extended to have more information if we want it but this is good for now.
            var gameDetails = new Game
            {
                AppID = appId,
                Name = game.Name,
                Price = game.PriceOverview.FinalFormatted
            };

            var games = JsonConvert.DeserializeObject<List<Game>>(await File.ReadAllTextAsync(filePath)) ?? new List<Game>();
            games.Add(gameDetails);
            await File.WriteAllTextAsync(filePath, JsonConvert.SerializeObject(games, Formatting.Indented));
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
