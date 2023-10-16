namespace Frends.ServiceNow.CreateRecord.Definitions;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

/// <summary>
/// Options class
/// </summary>
public class Options
{
    /// <summary>
    /// Method of authenticating request. By default, ServiceNow REST APIs use basic authentication or OAuth.
    /// </summary>
    /// <example>OAuth</example>
    public Authentication Authentication { get; set; }

    /// <summary>
    /// Username for basic authentication.
    /// </summary>
    /// <example>Username</example>
    [UIHint(nameof(Authentication), "", Authentication.Basic)]
    public string Username { get; set; }

    /// <summary>
    /// Password for the user.
    /// </summary>
    /// <example>Password123</example>
    [PasswordPropertyText]
    [UIHint(nameof(Authentication), "", Authentication.Basic)]
    public string Password { get; set; }

    /// <summary>
    /// Bearer token to be used for request. Token will be added as Authorization header.
    /// </summary>
    /// <example>Token123</example>
    [PasswordPropertyText]
    [UIHint(nameof(Authentication), "", Authentication.OAuth)]
    public string Token { get; set; }

    /// <summary>
    /// Timeout in seconds to be used for the connection and operation.
    /// </summary>
    /// <example>30</example>
    [DefaultValue(30)]
    public int ConnectionTimeoutSeconds { get; set; }

    /// <summary>
    /// If FollowRedirects is set to false, all responses with an HTTP status code from 300 to 399 is returned to the application.
    /// </summary>
    /// <example>true</example>
    [DefaultValue(true)]
    public bool FollowRedirects { get; set; }

    /// <summary>
    /// Do not throw an exception on certificate error.
    /// </summary>
    /// <example>true</example>
    public bool AllowInvalidCertificate { get; set; }

    /// <summary>
    /// Some Api's return faulty content-type charset header. This setting overrides the returned charset.
    /// </summary>
    /// <example>true</example>
    public bool AllowInvalidResponseContentTypeCharSet { get; set; }

    /// <summary>
    /// Throw exception if return code of request is not successfull.
    /// </summary>
    /// <example>true</example>
    public bool ThrowExceptionOnErrorResponse { get; set; }

    /// <summary>
    /// If set to false, cookies must be handled manually. Defaults to true.
    /// </summary>
    /// <example>true</example>
    [DefaultValue(true)]
    public bool AutomaticCookieHandling { get; set; } = true;
}
