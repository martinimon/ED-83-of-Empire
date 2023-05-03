using Discord.WebSocket;
using EdgeOfEmpireBot.IService;
using System.Text.Json;

namespace EdgeOfEmpireBot.Service
{
    public class MessageService : IMessageService
    {
        private bool messageIsSafe = false;

        /// <inheritdoc/>
        public async Task SendMessage(string messageResponse, ISocketMessageChannel messageResponseChannel)
        {
            var message = string.Empty;
  
            // Discord API prevents messages greater than 2000 characters from sending.
            if (messageResponse.Length > 2000)
            {
                message = "```ERROR 2319 - Message is greater than the maximum 2000 character limit.```";
                Console.WriteLine(message);
            }
            else
            {
                message = messageResponse;
            }

            await messageResponseChannel!.SendMessageAsync(message);

        }

        /// <inheritdoc/>
        public async Task ProcessMessage(SocketMessage message)
        {
            try
            {
                Console.WriteLine("Processing Message...");

                if (!message.Content.StartsWith("."))
                {
                    // Ignore message.
                    Console.WriteLine("Ignoring Message....");
                    return;
                }

                // Removes the '.' prefix to indicate a bot command. 
                var messageCommand = message.Content.Remove(0, 1);
                var messageChannel = message.Channel;

                await ProcessCommand(messageCommand, messageChannel);

                Console.WriteLine(messageCommand + "  command processed. ");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Processing Command");
                Console.WriteLine(ex.ToString());
            }
        }


        ///<inheritdoc/>
        public bool MessageIsSafe()
        {
            return messageIsSafe;
        }

        /// <summary>
        /// Processes a Command
        /// </summary>
        /// <param name="command"></param>
        private async Task ProcessCommand(string command, ISocketMessageChannel channel)
        {
            // Check if the command is a custom command that requires functionality, ie a roll. 
            // If not a custom command try processing it as a simple command. 
            // TODO: Consider the order of this once the scope of overall commands have been established. For efficency. 
            switch (command)
            {

                case "roll":
                    {
                        // TODO:
                        //RollCommand()
                        var msg = "```[Statement]: Roll command is not ready yet so I need you to calm down and either wait or implement it yourself Meatbag!```";
                        Console.WriteLine(msg);
                        await SendMessage(msg, channel);
                        break;
                    }

                default:
                    {
                        // Not a custom command so try processing as a simple command.
                        await ProcessSimpleCommand(command, channel);
                        break;
                    }
            }

        }


       /// <summary>
       /// Process a simple command that can simply be displayed as a text response.
       /// </summary>
       /// <param name="command"></param>
        private async Task ProcessSimpleCommand(string command, ISocketMessageChannel channel)
        {
            Console.WriteLine($"Command: {command}");
            var filePath = Path.Combine(@"Data\BasicCommands.json");
            var msg = string.Empty;
            var commandNotFoundMsg = "```[Statement]: " + command + " command does not exist or is not implemented. Please speak to your local dev about " + command + " command toady! \n[Sarcasm:] You could try the same command again and see if it works now.```";

            try
            {
                // Read the entire JSON file into a string
                var jsonString = File.ReadAllText(filePath);

                // Parse the JSON string into a JsonDocument
                using JsonDocument doc = JsonDocument.Parse(jsonString);

                // Get the "commands" property from the JSON document
                if (doc.RootElement.TryGetProperty("commands", out JsonElement commands))
                {
                    Console.WriteLine("command property exists.");

                    // Check if the specified command exists in the "commands" object
                    if (commands.TryGetProperty(command, out JsonElement commandResponse))
                    {
                        // If the command exists, return the corresponding response string
                        Console.WriteLine("command exists.");
                        msg = commandResponse.GetString();
                    }
                }

                // If message is still an empty string, set it to commandNotFoundMsg
                msg = !string.IsNullOrEmpty(msg) ? $"```{msg}```" : commandNotFoundMsg;

                Console.WriteLine(msg);
                await SendMessage(msg, channel);
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                msg = "```[Statement] it appears the developer fucked up and broke something.\n[Observation] The error is\n" + ex.Message + "```";
                await SendMessage(msg, channel);
            }
        }
    }
}