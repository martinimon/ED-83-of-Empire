using HK47.Models;

namespace HK47.Services;

/// <summary>
/// The interface for the Steam service
/// </summary>
public interface ISteamService
{
    Task<(string Message, List<SteamGameDetails> GamesWithNewPrice)> GetGamePrices();

    Task<SteamGameDetails> RetrieveGameFromSteam(string appId);
}
