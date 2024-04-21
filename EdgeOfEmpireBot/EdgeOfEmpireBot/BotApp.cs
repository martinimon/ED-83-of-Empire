using Discord;
using Discord.WebSocket;
using HK47.MessageHandlers;
using HK47.MessageHandlers.Interfaces;
using HK47.Router;
using HK47.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HK47.Api;

public class BotApp
{
    private readonly IServiceProvider serviceProvider;
    private readonly IMessageRouter messageRouter;
    private readonly IMessageService messageService;
    private readonly DiscordSocketClient client;

    /// <summary>
    /// The bot itself.
    /// </summary>
    public BotApp()
    {
        const long channelId = 1062707370809098291;

        serviceProvider = CreateProvider();
        this.client = serviceProvider.GetRequiredService<DiscordSocketClient>();
        InitializeBot().GetAwaiter().GetResult();

        var steamService = serviceProvider.GetRequiredService<ISteamService>();
        var dataService = serviceProvider.GetRequiredService<IDataService>();

        var channel = client.GetChannelAsync(channelId).GetAwaiter().GetResult() as IMessageChannel;

        messageService = new MessageService(channel!);
        var steamMessageHandler = new SteamMessageHandler(steamService, dataService, messageService);
        var generalMessageHandler = new GeneralMessageHandler(messageService, dataService);
        messageRouter = new MessageRouter(dataService, steamMessageHandler, generalMessageHandler, messageService);
    }

    static void Main() => new BotApp().MainAsync().GetAwaiter().GetResult();

    /// <summary>
    /// The main function that starts the bot.
    /// </summary>
    public async Task MainAsync()
    {
        await VerifyBotIsConnected();

        // Block this task until the program is closed.
        await Task.Delay(Timeout.Infinite);
    }

    /// <summary>
    /// Configures and registers services.
    /// </summary>
    private static ServiceProvider CreateProvider()
    {
        var discordConfig = new DiscordSocketConfig
        {
            MessageCacheSize = 100,
            GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent
        };
        var collection = new ServiceCollection();

        collection.AddSingleton(discordConfig).AddSingleton<DiscordSocketClient>();
        collection.AddScoped<IMessageService, MessageService>();
        collection.AddScoped<IDataService, DataService>();
        collection.AddScoped<ISteamService, SteamService>();
        collection.AddScoped<ISteamMessageHandler, SteamMessageHandler>();
        collection.AddScoped<IGeneralMessageHandler, GeneralMessageHandler>();
        collection.AddScoped<IMessageRouter, MessageRouter>();

        return collection.BuildServiceProvider();
    }

    /// <summary>
    /// Starts and configures the bot.
    /// </summary>
    private async Task InitializeBot()
    {
        var token = GetToken();

        client.Log += Log;
        /* TODO:
         * Woah we can do message Updated? Need to look into this once some more key functionality is completed.
         * Have turned off for the moment to prevent any potential issue occurring for the moment.
        */
        // this.client.MessageUpdated += MessageUpdated;
        client.MessageReceived += MessageReceived;

        await client.LoginAsync(TokenType.Bot, token);
        //Starts the bot
        await client.StartAsync();

        client.Ready += () =>
        {
            Console.WriteLine("Bot is connected!");
            return Task.CompletedTask;
        };
    }

    /// <summary>
    /// Logs a user message the Bot can read to Discord.
    /// </summary>
    /// <param name="msg"></param>
    //private Task MessageReceived(SocketMessage msg)
    private async Task MessageReceived(SocketMessage msg)
    {
        await messageRouter.ProcessMessage(msg);
    }

    /// <summary>
    /// Verifies the Bot can send messages to Discord.
    /// </summary>
    private async Task VerifyBotIsConnected()
    {
        /// !! URGENT DO NOT REMOVE !!
        /// SENDS VERY IMPORTANT START UP MESSAGE
        await messageService.SendMessage("ScurvyDog");
    }

    /// <summary>
    /// TODO
    /// </summary>
    /// <param name="before"></param>
    /// <param name="after"></param>
    /// <param name="channel"></param>
    /// <returns>Task</returns>
    private static async Task MessageUpdated(Cacheable<IMessage, ulong> before, SocketMessage after, ISocketMessageChannel channel)
    {
        // If the message was not in the cache, downloading it will result in getting a copy of `after`.
        var message = await before.GetOrDownloadAsync();
        Console.WriteLine($"{message} -> {after}");
    }

    /// <summary>
    /// Gets the Discord Token
    /// </summary>
    /// <returns>the token string</returns>
    private static string GetToken()
    {
        var token = string.Empty;

        try
        {
            string path = Path.Combine("Data/token.txt");
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