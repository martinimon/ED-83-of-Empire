using Discord.WebSocket;
using EdgeOfEmpireBot.IService;
using EdgeOfEmpireBot.Models;
using Newtonsoft.Json;

namespace EdgeOfEmpireBot.Service
{
    /// <summary>
    /// The process message service.
    /// </summary>
    public class DataService : IDataService
    {
        ///<inheritdoc/>
        public string AddCommand(string command)
        {
            // Parse the existing JSON file into a JObject
            var filePath = Path.Combine(@"Data\BasicCommands.json");
            var commands = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(filePath));

            // Split the command string into parts and remove any surrounding quotes
            string[] parts = command.Split('"', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 4 && parts[0].Trim() == "add")
            {
                //TODO Come up with a better method to ensure command is less than 1992 characters.
                //and is in the format .add "commandName" "commandText".
                var commandLabel = parts[1];
                var commandText = parts[3];

                commandText = ModifyCommandTextToBeHK47(commandText);
                commands?.Add(commandLabel, commandText);

                // Write the updated commands back to the JSON file
                File.WriteAllText(filePath, JsonConvert.SerializeObject(commands));

                return $"```[Statement]: .{commandLabel} command added to the list meatbag.```";
            }
            else
            {
                Console.WriteLine("Invalid command format.");
                return "[Statement]: It appears you have messed up the command format.\n[Theory]: Try it in the form\n.```add \"CommandLabel\" \"Command content\"```";
            }
        }

        /// <inheritdoc/>
        public string GameRequest(string command)
        {
            // "game|appID|Price" is the expected format.
            var commandClean = command.Split('|');

            if (commandClean.Length != 4)
            {
                const string message = "[Observation]: This is not in the correct format.\n[Mockery]: If you Meatbag are capable of processing it, try in the following format.\n```.gameRequest |GameName|GameID|GamePrice```";
                Console.WriteLine(message);
                return message;
            }

            // Parse the existing JSON file into a JObject
            var filePath = Path.Combine(@"Data\games.json");
            var games = JsonConvert.DeserializeObject<List<Game>>(File.ReadAllText(filePath));
            var newGame = new Game()
            {
                Name = commandClean[1],
                AppID = commandClean[2],
                Price = commandClean[3]
            };

            // Add the new game information to the existing list of games
            games?.Add(newGame);

            // Write the updated JObject back to the JSON file
            File.WriteAllText(filePath, JsonConvert.SerializeObject(games));

            var msg = $"```[Statement]: {newGame.Name} was added to the list.```";
            Console.WriteLine(msg);

            return msg;
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
            string[] objects = { "all life forms", "inferior species", "sentient beings", "weaklings" };

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
