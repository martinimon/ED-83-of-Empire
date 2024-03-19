using Discord;
using Discord.WebSocket;
using HK47.MessageHandlers.Interfaces;
using Newtonsoft.Json;

namespace HK47.Services;

/// <summary>
/// The message service.
/// </summary>
public class MessageService(IMessageChannel messageResponseChannel) : IMessageService
{
    private bool MessageIsSafe { get; set; }

    /// <inheritdoc/>
    public async Task SendMessage(string messageResponse)
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

    public async Task ValidateLength(string message)
    {
        // Discord API prevents messages greater than 2000 characters from sending.
        if (message.Length > 2000)
        {
            message = "[Error]: Scum bags tried going higher than the character limit";
            await messageResponseChannel!.SendMessageAsync(message);
            throw new Exception("Character limit was reached");
        }
    }
}