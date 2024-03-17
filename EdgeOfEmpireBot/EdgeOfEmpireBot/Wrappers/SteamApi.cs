using SteamStorefrontAPI;
using SteamStorefrontAPI.Classes;

namespace EdgeOfEmpireBot.Wrappers;

/// <summary>
/// A wrapper thats used to handle these pesky static calls to a third party
/// </summary>
/// <remarks>
/// I called it SteamStoreApi because when putting it into a class and using it as "SteamApiWrapper" doesnt make sense to call
/// as its not totally indicative of what it does.
/// </remarks>
public class SteamStoreApi : ISteamStoreApi
{
    /// <summary>
    /// Retrieves the Application Details from the Public facing API
    /// </summary>
    public Task<SteamApp> GetAppDetails(int appId, string countryCode)
    {
        return AppDetails.GetAsync(appId, countryCode);
    }
}

public interface ISteamStoreApi
{
    Task<SteamApp> GetAppDetails(int appId, string countryCode);
}