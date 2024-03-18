
using HK47.MessageHandlers.Interfaces;
using HK47.Services;
using Newtonsoft.Json;

namespace HK47.MessageHandlers;

public class GeneralMessageHandler(IMessageService messageService, IDataService dataService) : IGeneralMessageHandler
{
    /// <summary>
    /// Processes a Command
    /// </summary>
    /// <param name="command"></param>
    public async Task ProcessCommand(string[] commandParams)
    {
        // Check if the command is a custom command that requires functionality, ie a roll.
        // If not a custom command try processing it as a simple command.
        // TODO: Consider the order of this once the scope of overall commands have been established. For efficency.
        var commandNoParam = commandParams[0];
        var command = commandParams[1];
        switch (commandNoParam?.ToLower())
        {
            case "roll":
                {
                    // TODO:
                    //RollCommand()
                    const string msg = "```[Statement]: Roll command is not ready yet so I need you to calm down and either wait or implement it yourself Meatbag!```";
                    Console.WriteLine(msg);
                    await messageService.SendMessage(msg);
                    break;
                }
            case "add":
                {
                    var msg = dataService.AddCommand(command);
                    await messageService.SendMessage(msg);
                    break;
                }

            case "remember":
                if (commandParams[1] == "when")
                {
                    var phrase = await dataService.GetRememberWhenPhraseFromFile();
                    await messageService.SendMessage("[Sarcasm]: I dont remember...\n" +
                    $"[Statement]:That was a joke human.\n[Recalling]: I remember {phrase}");
                    break;
                }
                // Write command param[1] to Remember.json
                await dataService.AddRememberPhraseToFile(command);
                await messageService.SendMessage("[Statement] I will remember that meatbag...");
                break;
            default:
                // Not a custom command so try processing as a simple command.
                await ProcessSimpleCommand(command);
                break;
        }
    }

        /// <summary>
    /// Process a simple command that can simply be displayed as a text response.
    /// </summary>
    /// <param name="command"></param>
    private async Task ProcessSimpleCommand(string command)
    {
        var msg = string.Empty;

        try
        {
            // Ensure command is case insensive
            Console.WriteLine($"Command: {command}");
            command = command.ToLower();

            var filePath = Path.Combine("Data/BasicCommands.json");
            var commandNotFoundMsg = "```[Statement]: " + command + " command does not exist or is not implemented. Please speak to your local dev about " + command + " command toady! \n[Sarcasm:] You could try the same command again and see if it works now.```";

            // Read the entire JSON file
            var commands = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(filePath));

            // Check if the specified command exists in the "commands" object
            if (commands?.ContainsKey(command) == true)
            {
                // If the command exists, return the corresponding response string
                Console.WriteLine("command exists.");
                msg = commands[command];
            }

            // If message is still an empty string, set it to commandNotFoundMsg
            msg = !string.IsNullOrEmpty(msg) ? $"```{msg}```" : commandNotFoundMsg;

            Console.WriteLine(msg);
            await messageService.SendMessage(msg);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            msg = "```[Statement] it appears the developer fucked up and broke something.\n[Observation] The error is\n" + ex.Message + "```";
            await messageService.SendMessage(msg);
        }
    }
}