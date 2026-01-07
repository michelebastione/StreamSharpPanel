using System.Net;

namespace StreamSharpPanel.Models.Http;

public class InvalidTokenResponse 
{ 
    public HttpStatusCode Status { get; init; }
    public string? Message { get; init; }
}

public class ValidTokenResponse
{
    public required string ClientId { get; init; }
    public required string Login  { get; init; }
    public required string UserId  { get; init; }
    public string[] Scopes { get; init; } = [];
    public int ExpiresIn { get; init; }

    public DateTimeOffset ExpirationTimestamp =>  DateTimeOffset.UtcNow.AddSeconds(ExpiresIn);
}