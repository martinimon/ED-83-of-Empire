using Discord;

namespace HK47.Services;

/// <summary>
/// The message service.
/// </summary>
public class MessageService(IMessageChannel messageResponseChannel) : IMessageService
{
    /// <inheritdoc/>
    public async Task SendMessage(string messageResponse)
    {
        // Discord API prevents messages greater than 2000 characters from sending.
        await ValidateLength(messageResponse);
        await messageResponseChannel!.SendMessageAsync(messageResponse);
    }

    /// <inheritdoc/>
    public async Task ValidateLength(string message)
    {
        // Discord API prevents messages greater than 2000 characters from sending.
        if (message.Length > 2000)
        {
            message = "[Error]: Scumbags tried going higher than the character limit";
            await messageResponseChannel!.SendMessageAsync(message);
            throw new Exception("Character limit was reached");
        }
        if (message.Length == 1)
        {
            message = "[Error]: You're a bad Scallywag";
            await messageResponseChannel!.SendMessageAsync(message);
            throw new Exception("No Argument provided");
        }
    }
}