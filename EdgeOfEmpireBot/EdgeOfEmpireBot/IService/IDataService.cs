using Discord.WebSocket;

namespace EdgeOfEmpireBot.IService
{
    /// <summary>
    /// The interface for the data service
    /// </summary>
    public interface IDataService
    {
        /// <summary>
        /// Saves a basic command to the BasicCommands.json
        /// <remarks>
        /// The command format should be in the form .add "commandName" "CommandText"
        /// </remarks>
        /// </summary>
        /// <param name="command"></param>
        /// <param name="channel"></param>
        string AddCommand(string command);

        /// <summary>
        /// Saves a requested game to the games.json file.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        string GameRequest(string command);
    }
}
