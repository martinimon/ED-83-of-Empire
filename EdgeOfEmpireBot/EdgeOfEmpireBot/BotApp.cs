﻿using Discord.WebSocket;
using Discord;
using System.Reflection;

public class BotApp
{
    private DiscordSocketClient? client;

    /// <summary>
    /// The bot itself.
    /// </summary>
    public BotApp()
    {
        this.client = new DiscordSocketClient();
    }
    static void Main(string[] args) => new BotApp().MainAsync().GetAwaiter().GetResult();

    /// <summary>
    /// The main function that starts the bot.
    /// </summary>
    public async Task MainAsync()
    {
        await InitBot();
        // Block this task until the program is closed.
        await Task.Delay(-1);

    }

    /// <summary>
    /// Initialize and starts the bot.
    /// </summary>
    private async Task InitBot()
    {
        var token = getToken();

        var config = new DiscordSocketConfig { MessageCacheSize = 100 };
        this.client = new DiscordSocketClient(config);

        await this.client.LoginAsync(TokenType.Bot, token);
        //Starts the bot
        await this.client.StartAsync();

        this.client.MessageUpdated += MessageUpdated;
        this.client.Ready += () =>
        {
            Console.WriteLine("Bot is connected!");
            return Task.CompletedTask;
        };

        this.client.Log += Log;
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
    private string getToken()
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
    /// <param name="msg"></param>
    private Task Log(LogMessage msg)
    {
        Console.WriteLine(msg.ToString());
        return Task.CompletedTask;
    }


}