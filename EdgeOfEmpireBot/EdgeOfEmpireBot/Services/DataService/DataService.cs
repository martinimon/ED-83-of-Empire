using HK47.Models;
using Newtonsoft.Json;

namespace HK47.Services;

/// <summary>
/// The process message service.
/// </summary>
public class DataService : IDataService
{
    private readonly string gameFilePath;
    private readonly string rememberFilePath;
    public DataService()
    {
        gameFilePath = Path.Combine("Data/Games.json");
        rememberFilePath = Path.Combine("Data/Remember.json");
    }

    ///<inheritdoc/>
    public string AddCommand(string command)
    {
        // Parse the existing JSON file into a JObject
        var filePath = Path.Combine("Data/BasicCommands.json");
        var commands = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(filePath));

        // Split the command string into parts and remove any surrounding quotes
        string[] parts = command.Split('"', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 4 && parts[0].Trim() == "add")
        {
            //TODO Come up with a better method to ensure command is less than 1992 characters.
            //and is in the format .add "commandName" "commandText".
            var commandLabel = parts[1];
            var commandText = parts[3];

            commandLabel = commandLabel.ToLower();
            commandText = ModifyCommandTextToBeHK47(commandText);
            commands?.Add(commandLabel, commandText);

            // Write the updated commands back to the JSON file
            File.WriteAllText(filePath, JsonConvert.SerializeObject(commands, Formatting.Indented));

            return $"```[Statement]: .{commandLabel} command added to the list meatbag.```";
        }
        else
        {
            Console.WriteLine("Invalid command format.");
            return "[Statement]: It appears you have messed up the command format.\n[Theory]: Try it in the form\n.```add \"CommandLabel\" \"Command content\"```";
        }
    }

    ///<inheritdoc/>
    public async Task<string> GetGameByName(string name)
    {
        var games = JsonConvert.DeserializeObject<List<SteamGameDetails>>(await File.ReadAllTextAsync(gameFilePath)) ?? [];
        var game = games.Find(game => string.Equals(game.Name, name, StringComparison.OrdinalIgnoreCase));
        if (game == null) { return $"No game was found with the name {name}"; }

        return $"Name: {game.Name}\nId:{game.AppID}\nPrice:{game.Price}";
    }

    ///<inheritdoc/>
    public async Task UpdateGames(List<SteamGameDetails> UpdatedGames)
    {
        foreach (var game in UpdatedGames)
        {
            // Since this overwrites the older version of the game
            // We can leverage it here to update the game with the new values
            await WriteGameToFile(game);
        }
    }

    ///<inheritdoc/>
    public async Task WriteGameToFile(SteamGameDetails gameDetails)
    {
        var games = JsonConvert.DeserializeObject<List<SteamGameDetails>>(await File.ReadAllTextAsync(gameFilePath)) ?? [];
        var matchingGame = games.Find(game => game.AppID == gameDetails.AppID);

        if (matchingGame != null) { games.Remove(matchingGame); } // Prevents Duplicates

        games.Add(gameDetails);
        await File.WriteAllTextAsync(gameFilePath, JsonConvert.SerializeObject(games, Formatting.Indented));
    }

    /// <summary>
    /// Writes the phrase to the Remember.json file to recall it later
    /// </summary>
    public async Task AddRememberPhraseToFile(string command)
    {
        string[] parts = command.Split('"', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length > 3)
        {
            Console.WriteLine("Incorrect format for command");
            return;
        }

        var phrases = JsonConvert.DeserializeObject<Dictionary<int, string>>(await File.ReadAllTextAsync(rememberFilePath)) ?? [];
        phrases.Add(phrases.Count, parts[1]);
        await File.WriteAllTextAsync(rememberFilePath, JsonConvert.SerializeObject(phrases, Formatting.Indented));
    }

    /// <summary>
    /// Gets a random phrase from the saved phrases and returns is
    /// </summary>
    /// <returns>A random phrase</returns>
    public async Task<string> GetRememberWhenPhraseFromFile()
    {
        var random = new Random();
        var phrases = JsonConvert.DeserializeObject<Dictionary<int, string>>(await File.ReadAllTextAsync(rememberFilePath)) ?? [];
        return phrases[random.Next(0, phrases.Count)];
    }

    public T ReadFromFile<T>(string fileName)
    {
        var path = Path.Combine($"Data/{fileName}.json");
        return JsonConvert.DeserializeObject<T>(File.ReadAllText(path))!;
    }

    /// <summary>
    /// converts a string to a badly written version as if it was said by HK47.
    /// </summary>
    /// <param name="commandText"></param>
    /// <returns>A string badly formatted as if it was from HK47</returns>
    private static string ModifyCommandTextToBeHK47(string commandText)
    {
        string[] sentenceStructures = ["Exclamation! ", "Interrogative? ", "Imperative. ", "Declarative. "];

        // Define arrays of potential sentence components
        string[] subjects = ["meatbags", "organics", "biologicals", "fleshy beings"];
        string[] adjectives = ["pathetic", "inferior", "incompetent", "insignificant"];
        string[] verbs = ["destroy", "eliminate", "eradicate", "annihilate"];
        string[] objects = ["life forms", "inferior species", "sentient beings", "weaklings"];

        // Generate a random sentence structure and sentence components
        var random = new Random();
        var sentenceStructure = sentenceStructures[random.Next(0, sentenceStructures.Length)];
        var subject = subjects[random.Next(0, subjects.Length)];
        var adjective = adjectives[random.Next(0, adjectives.Length)];
        var verb = verbs[random.Next(0, verbs.Length)];
        var obj = objects[random.Next(0, objects.Length)];

        // Combine the sentence components into a single sentence
        var sentence = $"{subject} are {adjective}. {verb} the {obj}!";

        // Capitalize the first letter of the sentence
        sentence = char.ToUpper(sentence[0]) + sentence[1..];

        // Combine the sentence with the input text and the sentence structure
        return $"{sentence} {commandText.Trim()} {sentenceStructure}";
    }
}
