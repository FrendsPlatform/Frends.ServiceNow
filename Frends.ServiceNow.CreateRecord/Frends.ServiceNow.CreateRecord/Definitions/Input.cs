namespace Frends.ServiceNow.CreateRecord.Definitions;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

/// <summary>
/// Input class
/// </summary>
public class Input
{
    /// <summary>
    /// The URL with protocol and path to the ServiceNow instance. You can include query parameters directly in the url.
    /// </summary>
    /// <example>https://dev123456.service-now.com/api/now/table/{tableName}</example>
    [DefaultValue("https://dev123456.service-now.com/api/now/table/{tableName}")]
    [DisplayFormat(DataFormatString = "Text")]
    public string Url { get; set; }

    /// <summary>
    /// Format of the message. Possible values: JSON, XML
    /// </summary>
    public MessageFormat Format { get; set; }

    /// <summary>
    /// Body of the request message in JSON format.
    /// Content-Type: application/json header is passed automatically to the HttpContent.
    /// </summary>
    /// <example>
    /// {
    ///     "short_description":"Hello World",
    ///     "caller_id":"oauth.user"
    /// }
    /// </example>
    [UIHint(nameof(Format), "", MessageFormat.JSON)]
    [DisplayFormat(DataFormatString = "Json")]
    public string Json { get; set; }

    /// <summary>
    /// Body of the request message in XML format.
    /// Content-Type: application/xml header is passed automatically to the HttpContent.
    /// </summary>
    /// <example>
    /// &lt;request&gt;
    ///    &lt;entry&gt;
    ///        &lt;short_description&gt;Hello World&lt;/short_description&gt;
    ///        &lt;caller_id&gt;oauth.user&lt;/caller_id&gt;
    ///    &lt;/entry&gt;
    /// &lt;/request&gt;
    /// </example>
    [UIHint(nameof(Format), "", MessageFormat.XML)]
    [DisplayFormat(DataFormatString = "Xml")]
    public string Xml { get; set; }

    /// <summary>
    /// List of HTTP headers to be added to the request.
    /// Content-Type headers are passed automatically depending on the message format.
    /// </summary>
    /// <example>Name: Accept, Value: application/json</example>
    public Header[] Headers { get; set; }
}