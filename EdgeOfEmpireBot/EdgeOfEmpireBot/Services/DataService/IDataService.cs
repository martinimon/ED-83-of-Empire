using HK47.Models;

namespace HK47.Services;

/// <summary>
/// The interface for the data service
/// </summary>
public interface IDataService
{
    /// <summary>
    /// Saves a basic command to the BasicCommands.json
    /// </summary>
    /// <remarks>
    /// The command format should be in the form .add "commandName" "CommandText"
    /// </remarks>
    string AddCommand(string command);

    /// <summary>
    /// Gets the game details from local storage
    /// </summary>
    /// <param name="name">The name of the game you are trying to retrieve</param>
    /// <returns>Returns the details of a game thats been stored in the json</returns>
    Task<string> GetGameByName(string name);

    /// <summary>
    /// Iterates through the provide list of details and updates them in the file
    /// </summary>
    /// <param name="UpdatedGames">List of games that have been updated</param>
    Task UpdateGames(List<SteamGameDetails> UpdatedGames);

    /// <summary>
    /// Gets the game details from local storage
    /// </summary>
    /// <param name="gameDetails">The game details that you want to add</param>
    Task WriteGameToFile(SteamGameDetails gameDetails);

    /// <summary>
    /// Adds a phrase to the File
    /// </summary>
    Task AddRememberPhraseToFile(string phrase);

    /// <summary>
    /// Retrieves a phrase from the Remember.Json file and return a random one
    /// </summary>
    Task<string> GetRememberWhenPhraseFromFile();

    /// <summary>
    /// Reads and Deserialises data type from a json file
    /// </summary>
    Task<T> ReadFromFile<T>(string fileName);
}