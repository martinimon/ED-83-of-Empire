﻿using EdgeOfEmpireBot.IService;
using EdgeOfEmpireBot.Models;
using Newtonsoft.Json;

namespace EdgeOfEmpireBot.Service
{
    /// <summary>
    /// The process message service.
    /// </summary>
    public class DataService : IDataService
    {
        private readonly string gameFilePath;
        public DataService()
        {
            gameFilePath = Path.Combine("Data/games.json");
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

        public async Task<string> GetGameByName(string name)
        {
            var games = JsonConvert.DeserializeObject<List<Game>>(await File.ReadAllTextAsync(gameFilePath)) ?? new List<Game>();
            var game = games.Find(game => string.Equals(game.Name, name, StringComparison.OrdinalIgnoreCase));
            if (game == null) { return $"No game was found with the name {name}"; }

            return $"Name: {game.Name}\nId:{game.AppID}\nPrice:{game.Price}";
        }

        public async Task WriteGameToFile(Game gameDetails)
        {
            var games = JsonConvert.DeserializeObject<List<Game>>(await File.ReadAllTextAsync(gameFilePath)) ?? new List<Game>();
            var matchingGame = games.Find(game => game.AppID == gameDetails.AppID);

            if(matchingGame != null) { games.Remove(matchingGame); } // Prevents Duplicates

            games.Add(gameDetails);
            await File.WriteAllTextAsync(gameFilePath, JsonConvert.SerializeObject(games, Formatting.Indented));
        }

        /// <summary>
        /// converts a string to a badly written version as if it was said by HK-47.
        /// </summary>
        /// <param name="commandText"></param>
        /// <returns>A string badly formatted as if it was from HK-47</returns>
        private static string ModifyCommandTextToBeHK47(string commandText)
        {
            string[] sentenceStructures = { "Exclamation! ", "Interrogative? ", "Imperative. ", "Declarative. " };

            // Define arrays of potential sentence components
            string[] subjects = { "meatbags", "organics", "biologicals", "fleshy beings" };
            string[] adjectives = { "pathetic", "inferior", "incompetent", "insignificant" };
            string[] verbs = { "destroy", "eliminate", "eradicate", "annihilate" };
            string[] objects = { "life forms", "inferior species", "sentient beings", "weaklings" };

            // Generate a random sentence structure and sentence components
            var random = new Random();
            var sentenceStructure = sentenceStructures[random.Next(sentenceStructures.Length)];
            var subject = subjects[random.Next(subjects.Length)];
            var adjective = adjectives[random.Next(adjectives.Length)];
            var verb = verbs[random.Next(verbs.Length)];
            var obj = objects[random.Next(objects.Length)];

            // Combine the sentence components into a single sentence
            var sentence = $"{subject} are {adjective}. {verb} the {obj}!";

            // Capitalize the first letter of the sentence
            sentence = char.ToUpper(sentence[0]) + sentence[1..];

            // Combine the sentence with the input text and the sentence structure
            return $"{sentence} {commandText.Trim()} {sentenceStructure}";
        }
    }
}
