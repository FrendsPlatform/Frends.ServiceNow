namespace Frends.ServiceNow.FetchAccessToken.Definitions;

/// <summary>
/// Possible grant type options for fetching access token to ServiceNow.
/// </summary>
public enum GrantType
{
    /// <summary>
    /// Password grant type enables to get an access token using user credentials.
    /// </summary>
    Password,

    /// <summary>
    /// Refresh_token grant type enables to get an access token using alive refresh token.
    /// </summary>
    Refresh_token,
}
