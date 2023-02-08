using Discord.WebSocket;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;

public class BotApp
{
    private readonly IServiceProvider serviceProvider;
    private readonly DiscordSocketClient client;
    private readonly ulong channelId = 1062707370809098291;
    /// <summary>
    /// The bot itself.
    /// </summary>
    public BotApp()
    {
        serviceProvider = CreateProvider();
        client = serviceProvider.GetRequiredService<DiscordSocketClient>();
    }

    private IServiceProvider CreateProvider()
    {
        var config = new DiscordSocketConfig { MessageCacheSize = 100 };
        var collection = new ServiceCollection().AddSingleton(config).AddSingleton<DiscordSocketClient>();
        return collection.BuildServiceProvider();
    }

    static void Main(string[] args) => new BotApp().MainAsync(args).GetAwaiter().GetResult();

    /// <summary>
    /// The main function that starts the bot.
    /// </summary>
    public async Task MainAsync(string[] args)
    {

        var token = GetToken();

        client.Log += Log;
        client.MessageUpdated += MessageUpdated;
        client.MessageReceived += MessageReceived;
        await client.LoginAsync(TokenType.Bot, token);
        //Starts the bot
        await client.StartAsync();

        client.Ready += () =>
        {
            Console.WriteLine("Bot is connected!");
            return Task.CompletedTask;
        };

        /// !! URGENT DO NOT REMOVE !!
        /// SENDS VERY IMPORTANT START UP MESSAGE
        var channel = await client.GetChannelAsync(channelId) as IMessageChannel;
        await channel!.SendMessageAsync("ScurvyDog");

        // Block this task until the program is closed.
        await Task.Delay(Timeout.Infinite);

    }

    private Task MessageReceived(SocketMessage msg)
    {
        Console.WriteLine(msg.Author+ ": " + msg.Content);
        return Task.CompletedTask;
    }

    /// <summary>
    /// TODO
    /// </summary>
    /// <param name="before"></param>
    /// <param name="after"></param>
    /// <param name="channel"></param>
    /// <returns>Task</returns>
    private async Task MessageUpdated(Cacheable<IMessage, ulong> before, SocketMessage after, ISocketMessageChannel channel)
    {
        // If the message was not in the cache, downloading it will result in getting a copy of `after`.
        var message = await before.GetOrDownloadAsync();
        Console.WriteLine($"{message} -> {after}");
    }

    /// <summary>
    /// Gets the Discord Token
    /// </summary>
    /// <returns>the token string</returns>
    private string GetToken()
    {
        var token = string.Empty;

        try
        {
            string path = Path.Combine(@"Data\token.txt");
            token = File.ReadAllText(path);
        }
        catch (Exception ex) 
        { 
            Console.WriteLine(ex.ToString());
        }
        return token;
    }

    /// <summary>
    /// Logs a message to console.
    /// </summary>
    /// <param name="message"></param>

    private async Task Log(LogMessage message)
    {
        Console.WriteLine(message);
        await Task.CompletedTask;
    }

}