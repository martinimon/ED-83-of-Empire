namespace EdgeOfEmpireBot.IService
{
    /// <summary>
    /// The interface for the Steam service
    /// </summary>
    public interface ISteamService
    {
        Task<string> GetGamePrices();
        Task<string> GetGameByName(string name);
        Task AddGameToList(string appId);
    }
}
