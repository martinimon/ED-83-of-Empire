using SteamStorefrontAPI;
using SteamStorefrontAPI.Classes;

namespace HK47.Wrappers;

/// <summary>
/// A wrapper thats used to handle these pesky static calls to a third party
/// </summary>
/// <remarks>
/// I called it SteamStoreApi because when putting it into a class and using it as "SteamApiWrapper" doesnt make sense to call
/// as its not totally indicative of what it does.
/// </remarks>
public class SteamStoreApi : ISteamStoreApi
{
    /// <inheritdoc/>
    public Task<SteamApp> GetAppDetails(int appId, string countryCode)
    {
        return AppDetails.GetAsync(appId, countryCode);
    }
}

/// <summary>
/// An interface for the wrapper thats used to handle these pesky static calls to a third party
/// </summary>
public interface ISteamStoreApi
{
    /// <summary>
    /// Retrieves the Application Details from the Public facing API
    /// </summary>
    Task<SteamApp> GetAppDetails(int appId, string countryCode);
}