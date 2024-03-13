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
        string AddCommand(string command);

        /// <summary>
        /// Saves a requested game to the games.json file.
        /// </summary>
        string GameRequest(string command);
    }
}
