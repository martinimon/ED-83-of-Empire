
using HK47.MessageHandlers.Interfaces;
using HK47.Services;
using Newtonsoft.Json;

namespace HK47.MessageHandlers;

public class GeneralMessageHandler(IMessageService messageService, IDataService dataService) : IGeneralMessageHandler
{
    private readonly string[] ListOfRpgs = ["edge", "masks", "sprawl"]; // This contains example rpgs

    /// </inheritdoc>
    public async Task ProcessCommand(string userInput)
    {
        // !!!! DO NOT REMOVE THIS COMMENT IT IS VITAL:
        // Elliotts a silly goose
        // ----------------------------------------------
        // Check if the command is a custom command that requires functionality, ie a roll.
        // If not a custom command try processing it as a simple command.
        // TODO: Consider the order of this once the scope of overall commands have been established. For efficency.

        var commandParams = userInput.Split(' ');
        var command = commandParams![0].ToLower();
        switch (command)
        {
            case "add":
                {
                    var msg = dataService.AddCommand(command);
                    await messageService.SendMessage(msg);
                    break;
                }

            case "remember":
                if (commandParams[1] != "when")
                {
                    // TODO: Improve processing of remembering phrases.
                    // Add safety checks and what not
                    var phrase = string.Join(" ", commandParams[1..]);
                    await dataService.AddRememberPhraseToFile(phrase);
                    await messageService.SendMessage("[Statement] I will remember that meatbag...");
                    break;
                }

                var remember = await dataService.GetRememberWhenPhraseFromFile();
                await messageService.SendMessage("[Sarcasm]: I dont remember...\n" +
                $"[Statement]:That was a joke human.\n[Recalling]: I remember {remember}");
                break;
            case "changerpg":
                await ChangeRpgs(commandParams[1]);
                break;
            default:
                // Not a custom command so try processing as a simple command.
                await ProcessSimpleCommand(command);
                break;
        }
    }

    /// <summary>
    /// Updates the commands that are shared across rpgs to the rpg that is provided
    /// </summary>
    private async Task ChangeRpgs(string rpg)
    {
        if (!ListOfRpgs.Contains(rpg))
        {
            await messageService.SendMessage("[Sad]: That RPG is not available...");
            return;
        }

        await dataService.UpsertToJsonFile("Commands", ("roll", rpg));
        await messageService.SendMessage("[Ecstatic]: How the turn tables...");
    }

    /// <summary>
    /// Process a simple command that can simply be displayed as a text response.
    /// </summary>
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