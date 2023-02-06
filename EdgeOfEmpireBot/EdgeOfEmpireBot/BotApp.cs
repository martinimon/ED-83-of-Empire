using Discord.WebSocket;
using Discord;

public class BotApp
{
    private DiscordSocketClient? _client;

    public BotApp()
    {
        _client = new DiscordSocketClient();
    }
    static void Main(string[] args) => new BotApp().MainAsync().GetAwaiter().GetResult();

    public async Task MainAsync()
    {
        await InitBot();
        // Block this task until the program is closed.
        await Task.Delay(-1);

    }
    private async Task InitBot()
    {
        var token = "Insert Token Here";
        var _config = new DiscordSocketConfig { MessageCacheSize = 100 };
        this._client = new DiscordSocketClient(_config);

        await this._client.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable(token));

        //  You can assign your bot token to a string, and pass that in to connect.
        //  This is, however, insecure, particularly if you plan to have your code hosted in a public repository.

        // Some alternative options would be to keep your token in an Environment Variable or a standalone file.
        // var token = Environment.GetEnvironmentVariable("NameOfYourEnvironmentVariable");
        // var token = File.ReadAllText("token.txt");
        // var token = JsonConvert.DeserializeObject<AConfigurationClass>(File.ReadAllText("config.json")).Token;

        this._client.MessageUpdated += MessageUpdated;
        this._client.Ready += () =>
        {
            Console.WriteLine("Bot is connected!");
            return Task.CompletedTask;
        };

        //Environment.GetEnvironmentVariable("EdgeOfEmpireBotToken");

        if (token == null)
        {
            // Need to implement a better way of storing/generating the token. 
            // Can't use Token itself as git bots look for discord tokens to be malicious. 
            Console.WriteLine("No token found.. Adding token now...");
            token = Environment.GetEnvironmentVariable("EdgeOfEmpireBotToken");
        }


        this._client = new DiscordSocketClient();
        this._client.Log += Log;

        await this._client.LoginAsync(TokenType.Bot, token);

        //Starts the bot
        await this._client.StartAsync();
    }
    private async Task MessageUpdated(Cacheable<IMessage, ulong> before, SocketMessage after, ISocketMessageChannel channel)
    {
        // If the message was not in the cache, downloading it will result in getting a copy of `after`.
        var message = await before.GetOrDownloadAsync();
        Console.WriteLine($"{message} -> {after}");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="msg"></param>
    private Task Log(LogMessage msg)
    {
        Console.WriteLine(msg.ToString());
        return Task.CompletedTask;
    }


}