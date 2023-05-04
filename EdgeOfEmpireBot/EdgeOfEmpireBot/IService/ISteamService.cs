namespace EdgeOfEmpireBot.IService
{
    /// <summary>
    /// The interface for the Steam service
    /// </summary>
    public interface ISteamService
    {
        Task<string> GetGamePrices();
    }
}
