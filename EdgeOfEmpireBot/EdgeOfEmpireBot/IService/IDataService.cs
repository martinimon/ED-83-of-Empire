﻿using EdgeOfEmpireBot.Models;

namespace EdgeOfEmpireBot.IService;

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

    /// <summary>Gets the game details from local storage</summary>
    Task<string> GetGameByName(string name);

    /// <summary>Iterates through the provide list of details and updates them in the file</summary>
    Task UpdateGames(List<SteamGameDetails> UpdatedGames);

    /// <summary>Gets the game details from local storage</summary>
    Task WriteGameToFile(SteamGameDetails gameDetails);
}