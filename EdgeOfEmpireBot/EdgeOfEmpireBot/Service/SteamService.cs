using System.Text.Json;
using EdgeOfEmpireBot.IService;
using EdgeOfEmpireBot.Models;
using Newtonsoft.Json;

namespace EdgeOfEmpireBot.Service
{
    /// <summary>
    /// The Steam Service.
    /// </summary>
    public class SteamService : ISteamService
    {
        public async Task<string> GetGamePrices()
        {
            var filePath = Path.Combine(@"Data\games.json");
            var games = JsonConvert.DeserializeObject<List<Game>>(await File.ReadAllTextAsync(filePath));
            var steamToken = GetToken();

            var result = "";
            using var client = new HttpClient();
            foreach (var game in games!)
            {
                var response = await client.GetAsync($"https://store.steampowered.com/api/appdetails?appids={game.AppID}&cc=au&key={steamToken}");

                if (!response.IsSuccessStatusCode) { throw new Exception("Ooft failed to get success response from Steam"); }

                var responseJson = await response.Content.ReadAsStringAsync();
                var appDetails = JsonConvert.DeserializeObject<JsonElement>(responseJson);// [int.Parse(appId)];
                var data = appDetails.GetProperty(game.AppID).GetProperty("data");
                var priceOverview = data.GetProperty("price_overview");
                var currentPrice = priceOverview.GetProperty("final_formatted").GetString();
                var discountPercent = data.GetProperty("price_overview").GetProperty("discount_percent").GetInt32();
                var discountedPrice = data.GetProperty("price_overview").GetProperty("final_formatted").GetString() ?? "";

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

        /// <summary>
        /// Gets the Steam Token
        /// </summary>
        /// <returns>the token string</returns>
        private static string GetToken()
        {
            var token = string.Empty;

            try
            {
                string path = Path.Combine(@"Data\steamToken.txt");
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
