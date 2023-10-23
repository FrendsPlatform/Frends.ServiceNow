namespace Frends.ServiceNow.CreateRecord.Definitions;

/// <summary>
/// Request authentication.
/// </summary>
public enum Authentication
{
    /// <summary>
    /// No authentication.
    /// </summary>
    None,

    /// <summary>
    /// Basic authentication.
    /// </summary>
    Basic,

    /// <summary>
    /// OAuth authentication.
    /// </summary>
    OAuth,
}
