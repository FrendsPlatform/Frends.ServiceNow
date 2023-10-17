namespace Frends.ServiceNow.FetchAccessToken.Definitions;

/// <summary>
/// Result class usually contains properties of the return object.
/// </summary>
public class Result
{
    internal Result(object body, int statusCode)
    {
        Body = body;
        StatusCode = statusCode;
    }

    /// <summary>
    /// Contains the access token to a certain ServiceNow instance.
    /// </summary>
    /// <example>Example of the output.</example>
    public dynamic Body { get; private set; }

    /// <summary>
    /// Statuscode of response
    /// </summary>
    /// <example>200</example>
    public int StatusCode { get; private set; }
}
