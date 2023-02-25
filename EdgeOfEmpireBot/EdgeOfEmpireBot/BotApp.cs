﻿using Discord.WebSocket;
using Discord;
using Microsoft.Extensions.DependencyInjection;
using EdgeOfEmpireBot.Service;
using EdgeOfEmpireBot.IService;

public class BotApp
{
    private readonly IServiceProvider serviceProvider;
    private readonly DiscordSocketClient client;
    private readonly IMessageService messageService;
    private readonly IProcessMessageService processMessageService;

    /// <summary>
    /// The bot itself.
    /// </summary>
    public BotApp()
    {
        serviceProvider = CreateProvider();
        this.client = serviceProvider.GetRequiredService<DiscordSocketClient>();
        this.messageService = serviceProvider.GetRequiredService<IMessageService>();
        this.processMessageService = serviceProvider.GetRequiredService<IProcessMessageService>();
    }


    static void Main(string[] args) => new BotApp().MainAsync(args).GetAwaiter().GetResult();

    /// <summary>
    /// The main function that starts the bot.
    /// </summary>
    public async Task MainAsync(string[] args)
    {
        await InitializeBot();
        await VerifyBotIsConnected();

        // Block this task until the program is closed.
        await Task.Delay(Timeout.Infinite);
    }

    /// <summary>
    /// Configures and registers services. 
    /// </summary>
    private IServiceProvider CreateProvider()
    {
        var discordConfig = new DiscordSocketConfig
        {
            MessageCacheSize = 100,
            GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent
        };
        var collection = new ServiceCollection();

        collection.AddSingleton(discordConfig).AddSingleton<DiscordSocketClient>();
        
        collection.AddScoped<IMessageService, MessageService>();
        collection.AddScoped<IProcessMessageService, ProcessMessageService>();

        return collection.BuildServiceProvider();
    }

    /// <summary>
    /// Starts and configures the bot.
    /// </summary>
    private async Task InitializeBot()
    {
        var token = GetToken();

        this.client.Log += Log;
        /* TODO: 
         * Woah we can do message Updated? Need to look into this once some more key functionality is completed.
         * Have turned off for the moment to prevent any potential issue occurring for the moment. 
        */
        //this.client.MessageUpdated += MessageUpdated;
        this.client.MessageReceived += MessageReceived;

        await this.client.LoginAsync(TokenType.Bot, token);
        //Starts the bot
        await this.client.StartAsync();

        this.client.Ready += () =>
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
       await messageService.ProcessMessage(msg);
    }

    /// <summary>
    /// Verifies the Bot can send messages to Discord.
    /// </summary>
    private async Task VerifyBotIsConnected()
    {
        // TODO: Consider replacing this to data. 
        var channelId = 1062707370809098291;

        /// !! URGENT DO NOT REMOVE !!
        /// SENDS VERY IMPORTANT START UP MESSAGE
        var channel = await this.client.GetChannelAsync((ulong)channelId) as IMessageChannel;
        await channel!.SendMessageAsync("ScurvyDog");
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