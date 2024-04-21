namespace HK47.Services;

public interface IMessageService
{
    /// <summary>
    /// Sends a message to Discord.
    /// </summary>
    Task SendMessage(string messageResponse);

    /// <summary>
    /// Ensures the length of the commands do not exceed the limit and are not below the minimum
    /// </summary>
    Task ValidateLength(string message);
}
