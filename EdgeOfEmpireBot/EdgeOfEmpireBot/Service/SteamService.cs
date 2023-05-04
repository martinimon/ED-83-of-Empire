using EdgeOfEmpireBot.IService;
using System.Text.Json;

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
            string json = await File.ReadAllTextAsync(filePath);
            var games = JsonSerializer.Deserialize<JsonElement>(json);
            var steamToken = GetToken();

            var result = "";
            using var client = new HttpClient();
            foreach (var game in games.GetProperty("Games").EnumerateArray())
            {
                var gameDetails = game.GetProperty(game.EnumerateObject().First().Name);
                var appId = gameDetails.GetProperty("AppID").GetString().Trim();
                var price = gameDetails.GetProperty("Price").GetString().Trim();

                var response = await client.GetAsync($"https://store.steampowered.com/api/appdetails?appids={appId}&cc=au&key={steamToken}");
                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    var appDetails = JsonSerializer.Deserialize<JsonElement>(responseJson);// [int.Parse(appId)];
                    var data = appDetails.GetProperty(appId).GetProperty("data");
                    var priceOverview = data.GetProperty("price_overview");
                    var currentPrice = priceOverview.GetProperty("final_formatted").GetString();
                    var discountPercent = data.GetProperty("price_overview").GetProperty("discount_percent").GetInt32();
                    var discountedPrice = data.GetProperty("price_overview").GetProperty("final_formatted").GetString() ?? "";

                    if (!string.IsNullOrEmpty(currentPrice))
                    {
                        result += $"{game.EnumerateObject().First().Name} - {price} ({currentPrice})";

                        if (discountPercent > 0)
                        {
                            result += $" {discountPercent}% off, now {discountedPrice}";
                        }

                        result += "\n";
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the Steam Token
        /// </summary>
        /// <returns>the token string</returns>
        private string GetToken()
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

