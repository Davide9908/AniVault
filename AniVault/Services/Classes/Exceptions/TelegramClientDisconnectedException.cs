namespace AniVault.Services.Classes.Exceptions;

public class TelegramClientDisconnectedException : ApplicationException
{
    private new const string Message = "Telegram client is null or disconnected";
    public TelegramClientDisconnectedException() : base(Message) { }
    public TelegramClientDisconnectedException(Exception inner) : base(Message, inner) { }
}