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
    /// <example>
    /// <code>
    /// {
    ///      "access_token": "6mq79vy2XwgVp-vnrijbn7_JKfnbiebfjnube788743fYEiNltfjeOBbDTpMru664esGGvhJJxh2uJJ4EkDnKAkaA",
    ///      "refresh_token": "9zXaz_7cfbe7fbcvF76bvce72aAHylz-IKJDb7fhbee7bfe74Pwh77vv0tz815ptH_eFHr8zXzVA",
    ///      "scope": "useraccount",
    ///      "token_type": "Bearer",
    ///      "expires_in": 1799
    /// }
    /// </code>
    /// </example>
    public dynamic Body { get; private set; }

    /// <summary>
    /// Statuscode of response
    /// </summary>
    /// <example>200</example>
    public int StatusCode { get; private set; }
}
