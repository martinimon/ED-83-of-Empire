using EdgeOfEmpireBot.Models;

namespace EdgeOfEmpireBot.IService
{
    /// <summary>
    /// The interface for the Steam service
    /// </summary>
    public interface ISteamService
    {
        /// <summary>
        /// Iterates through the list of games stored and queries the steam api
        /// to get the prices for the games
        /// </summary>
        /// <returns>This will return the Details in the form of a string.
        /// If there is any changes to the prices of the games stored we will return
        /// a list of games that need to be updated</returns>
        Task<(string Message, List<SteamGameDetails> GamesWithNewPrice)> GetGamePrices();

        /// <summary>
        /// Uses the Steam API to retrieve a game with the appId
        /// </summary>
        /// <param name="appId">App Id of the game you wish to retrieve</param>
        /// <returns>Returns a transformed version of the result</returns>
        Task<SteamGameDetails> RetrieveGameFromSteam(string appId);
    }
}
