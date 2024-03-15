using EdgeOfEmpireBot.Models;

namespace EdgeOfEmpireBot.IService
{
    /// <summary>
    /// The interface for the Steam service
    /// </summary>
    public interface ISteamService
    {
        Task<(string Message, List<Game> GamesWithNewPrice)> GetGamePrices();
        Task<Game> RetrieveGameFromSteam(string appId);
    }
}
