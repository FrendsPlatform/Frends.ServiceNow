namespace Frends.ServiceNow.FetchAccessToken.Definitions;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

/// <summary>
/// Input class usually contains parameters that are required.
/// </summary>
public class Input
{
    /// <summary>
    /// The base URL to the ServiceNow instance.
    /// </summary>
    /// <example>https://dev123456.service-now.com</example>
    [DisplayFormat(DataFormatString = "Text")]
    public string Url { get; set; }

    /// <summary>
    /// Grant type determines how the access token is fetched for ServiceNow.
    /// </summary>
    /// <example>Frends.ServiceNow.Definitions.GrantType.Password</example>
    public GrantType GrantType { get; set; }

    /// <summary>
    /// ServiceNow instance Client Id.
    /// </summary>
    /// <example>1ab23456c789ded785dhd</example>
    [DisplayFormat(DataFormatString = "Text")]
    public string ClientId { get; set; }

    /// <summary>
    /// ServiceNow instance Client Secret.
    /// </summary>
    /// <example>HBF78cfbe6?</example>
    [PasswordPropertyText]
    [DisplayFormat(DataFormatString = "Text")]
    public string ClientSecret { get; set; }

    /// <summary>
    /// ServiceNow instance username.
    /// </summary>
    /// <example>integration.user</example>
    [UIHint(nameof(GrantType), "", GrantType.Password)]
    [DisplayFormat(DataFormatString = "Text")]
    public string Username { get; set; }

    /// <summary>
    /// ServiceNow instance password.
    /// </summary>
    /// <example>cvj#renivreb7tycv4e8!cv7reb8v47382?</example>
    [UIHint(nameof(GrantType), "", GrantType.Password)]
    [PasswordPropertyText]
    [DisplayFormat(DataFormatString = "Text")]
    public string Password { get; set; }

    /// <summary>
    /// RefreshToken for ServiceNow instance. You can get the refresh token when initially fetched the access token with password grant type.
    /// </summary>
    /// <example>5FDughf_ncuenUJbvcerubn67cbe-fjncer8c7efcbE88fcvbvc48vbncsifb_kbncUgbE9FCVBE</example>
    [UIHint(nameof(GrantType), "", GrantType.Refresh_token)]
    [PasswordPropertyText]
    [DisplayFormat(DataFormatString = "Text")]
    public string RefreshToken { get; set; }
}