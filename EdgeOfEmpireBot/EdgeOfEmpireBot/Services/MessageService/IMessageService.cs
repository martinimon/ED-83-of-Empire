namespace HK47.Services;

public interface IMessageService
{
    /// <summary>
    /// Sends a message to Discord.
    /// </summary>
    Task SendMessage(string messageResponse);

    Task ValidateLength(string message);
}
