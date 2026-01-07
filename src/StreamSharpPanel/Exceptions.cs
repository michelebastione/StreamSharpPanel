namespace StreamSharpPanel;

public abstract class TwitchException : Exception;

public class RevocationException(string message) : TwitchException
{
    public override string Message => message;
}