namespace Frends.ServiceNow.FetchAccessToken.Definitions;

/// <summary>
/// Custom property class for customly made url encoded message.
/// </summary>
public class CustomProperty
{
    /// <summary>
    /// Name of the custom property.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Value of the custom property.
    /// </summary>
    public string Value { get; set; }
}