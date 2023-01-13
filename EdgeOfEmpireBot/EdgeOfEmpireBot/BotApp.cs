using Discord.WebSocket;
using Discord;
using static EdgeOfEmpireBot.SetTokenVariable;

namespace EdgeOfEmpireBot
{
    public class BotApp
    {
        private DiscordSocketClient? client;

        public static void Main(string[] args)
            => new BotApp().Main().GetAwaiter().GetResult();


        /// <summary>
        /// 
        /// </summary>
        public async Task Main()
        {
            await InitBot();

            // Block this task until the program is closed.
            await Task.Delay(-1);
        }

        /// <summary>
        /// Initializes the bot 
        /// </summary>
        private async Task InitBot()
        {
            var token = Environment.GetEnvironmentVariable("EdgeOfEmpireBotToken");

            if (token == null)
            {
                // Need to implement a better way of storing/generating the token. 
                // Can't use Token itself as git bots look for discord tokens to be malicious. 
                Console.WriteLine("No token found.. Adding token now...");
                SetTokenVariable.SetTokenEnvironmentVariable();
                token = Environment.GetEnvironmentVariable("EdgeOfEmpireBotToken");
            }


            this.client = new DiscordSocketClient();
            this.client.Log += Log;

            await this.client.LoginAsync(TokenType.Bot, token);

            //Starts the bot
            await this.client.StartAsync();
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
}