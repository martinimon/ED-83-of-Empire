using Discord.WebSocket;
using EdgeOfEmpireBot.IService;
using Newtonsoft.Json.Linq;

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
            JObject jsonObj = JObject.Parse(File.ReadAllText(filePath));

            // Split the command string into parts and remove any surrounding quotes
            string[] parts = command.Split('"', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 4 && parts[0].Trim() == "add")
            {
                //TODO Come up with a better method to ensure command is less than 1992 characters.
                //and is in the format .add "commandName" "commandText".
                var commandLabel = parts[1];
                var commandText = parts[3];

                commandText = ModifyCommandTextToBeHK47(commandText);

                // Update the JObject with the new command
                jsonObj["commands"][commandLabel] = commandText;

                // Write the updated JObject back to the JSON file
                File.WriteAllText(filePath, jsonObj.ToString());

                var msg = "```[Statement]: ." + commandLabel + " command added to the list meatbag.```";

                return msg;
            }
            else
            {
                Console.WriteLine("Invalid command format.");
                var msg = "[Statement]: It appears you have messed up the command format.\n[Theory]: Try it in the form\n.```add \"CommandLabel\" \"Command content\"```";
                return msg;
            }
        }

        /// <inheritdoc/>
        public string GameRequest(string command)
        {
            // "game|appID|Price" is the expected format. 
            var commandClean = command.Split('|');
            var msg = string.Empty;

            if (commandClean.Length != 4)
            {
                msg = "[Observation]: This is not in the correct format.\n[Mockery]: If you Meatbag are capable of processing it, try in the following format.\n```.gameRequest |GameName|GameID|GamePrice```";
                Console.WriteLine(msg);
                return msg;
            }

            // Parse the existing JSON file into a JObject
            var filePath = Path.Combine(@"Data\games.json");
            JObject jsonObj = JObject.Parse(File.ReadAllText(filePath));

            var gameName = commandClean[1];
            var gameID = commandClean[2];
            var gamePrice = commandClean[3];

            // Create a new JObject to hold the game information
            JObject newGame = new JObject();
            newGame["AppID"] = gameID;
            newGame["Price"] = gamePrice;

            // Add the new game information to the existing JArray of games
            JArray gamesArray = (JArray)jsonObj["Games"];
            gamesArray.Add(new JObject(new JProperty(gameName, newGame)));

            // Write the updated JObject back to the JSON file
            File.WriteAllText(filePath, jsonObj.ToString());

            msg = "```[Statement]: "+gameName+" was added to the list.```";
            Console.WriteLine(msg);

            return msg;
        }

        /// <summary>
        /// converts a string to a badly written version as if it was said by HK-47.
        /// </summary>
        /// <param name="commandText"></param>
        /// <returns>A string badly formatted as if it was from HK-47</returns>
        private string ModifyCommandTextToBeHK47(string commandText)
        {
            string[] sentenceStructures = { "Exclamation! ", "Interrogative? ", "Imperative. ", "Declarative. " };

            // Define arrays of potential sentence components
            string[] subjects = { "meatbags", "organics", "biologicals", "fleshy beings" };
            string[] adjectives = { "pathetic", "inferior", "incompetent", "insignificant" };
            string[] verbs = { "destroy", "eliminate", "eradicate", "annihilate" };
            string[] objects = { "all life forms", "inferior species", "sentient beings", "weaklings" };

            // Generate a random sentence structure and sentence components
            Random random = new Random();
            var sentenceStructure = sentenceStructures[random.Next(sentenceStructures.Length)];
            var subject = subjects[random.Next(subjects.Length)];
            var adjective = adjectives[random.Next(adjectives.Length)];
            var verb = verbs[random.Next(verbs.Length)];
            var obj = objects[random.Next(objects.Length)];

            // Combine the sentence components into a single sentence
            var sentence = subject + " are " + adjective + ". " + verb + " the " + obj + "!";

            // Capitalize the first letter of the sentence
            sentence = char.ToUpper(sentence[0]) + sentence.Substring(1);

            // Combine the sentence with the input text and the sentence structure
            var outputText = sentence + " " + commandText.Trim() + " " + sentenceStructure;

            return outputText;
        }
    }     
}

