using Discord.WebSocket;
using EdgeOfEmpireBot.IService;

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
                // Indicate the message is for the bot by starting it with a '.'
                if (message.Content.StartsWith("."))
                {
                    Console.WriteLine("Processing Message...");

                    // Removes the '.' prefix to indicate a bot command. 
                    var messageCommand = message.Content.Remove(0, 1);
                    var messageChannel = message.Channel;

                   await ProcessCommand(messageCommand, messageChannel);

                    Console.WriteLine(messageCommand + "  command processed. ");

                }
                else
                {
                    // Ignore the message. 
                }
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
            // TODO: ADD Functionality to check a json file (or something TBD) and display 
            var msg = "```[Statement]: " + command + " command does not exist or is not implemented. Please speak to your local dev about " + command + " command toady! \n[Sarcasm:] You could try the same command again and see if it works now.```";
            Console.WriteLine(msg);
            await SendMessage(msg, channel);
        }
    }
}