using Discord.WebSocket;
using Newtonsoft.Json;

namespace EdgeOfEmpireBot.Services;

/// <summary>
/// The message service.
/// </summary>
/// <param name="dataService">The data service.</param>
/// <param name="steamService">The Steam service</param>
public class MessageService(IDataService dataService, ISteamService steamService) : IMessageService
{
    private readonly IDataService dataService = dataService;
    private readonly ISteamService steamService = steamService;
    private readonly bool messageIsSafe = false;

    /// <inheritdoc/>
    public async Task SendMessage(string messageResponse, ISocketMessageChannel messageResponseChannel)
    {
        string message;

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

            if (!message.Content.StartsWith('.'))
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
        var commandParams = command.Split(' ');
        var commandNoParam = commandParams[0];
        switch (commandNoParam?.ToLower())
        {
            case "roll":
                {
                    // TODO:
                    //RollCommand()
                    const string msg = "```[Statement]: Roll command is not ready yet so I need you to calm down and either wait or implement it yourself Meatbag!```";
                    Console.WriteLine(msg);
                    await SendMessage(msg, channel);
                    break;
                }
            case "add":
                {
                    var msg = dataService.AddCommand(command);
                    await SendMessage(msg, channel);
                    break;
                }
            case "gamerequest":
            case "gamesrequest":
            case "gr":
                {
                    try
                    {
                        var appId = commandParams[1];
                        var game = await steamService.RetrieveGameFromSteam(appId);
                        await dataService.WriteGameToFile(game);
                        var msg = $"```[Statement]: {game.Name} was added to the list.```";
                        await SendMessage(msg, channel);
                    }
                    catch (Exception ex)
                    {
                        var msg = $"[Error]: Game not added due to the following:\n{ex.Message}";
                        await SendMessage(msg, channel);
                    }
                    break;
                }
            case "steam":
            case "games":
                {
                    var (msg, updatedGames) = await steamService.GetGamePrices();
                    if (updatedGames.Count != 0) { await dataService.UpdateGames(updatedGames); }
                    await SendMessage($"{msg}", channel);
                    break;
                }
            case "remember":
                if (commandParams[1] == "when")
                {
                    var phrase = await dataService.GetRememberWhenPhraseFromFile();
                    await SendMessage("[Sarcasm]: I dont remember...\n" +
                    $"[Statement]:That was a joke human.\n[Recalling]: I remember {phrase}", channel);
                    break;
                }
                // Write command param[1] to Remember.json
                await dataService.AddRememberPhraseToFile(command);
                await SendMessage("[Statement] I will remember that meatbag...", channel);
                break;
            default:
                // Not a custom command so try processing as a simple command.
                await ProcessSimpleCommand(command, channel);
                break;
        }
    }

    /// <summary>
    /// Process a simple command that can simply be displayed as a text response.
    /// </summary>
    /// <param name="command"></param>
    private async Task ProcessSimpleCommand(string command, ISocketMessageChannel channel)
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
            await SendMessage(msg, channel);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            msg = "```[Statement] it appears the developer fucked up and broke something.\n[Observation] The error is\n" + ex.Message + "```";
            await SendMessage(msg, channel);
        }
    }
}