using EdgeOfEmpireBot.Models;

namespace EdgeOfEmpireBot.IService
{
    /// <summary>
    /// The interface for the Steam service
    /// </summary>
    public interface ISteamService
    {
        Task<(string Message, List<SteamGameDetails> GamesWithNewPrice)> GetGamePrices();
        Task<SteamGameDetails> RetrieveGameFromSteam(string appId);
    }
}
