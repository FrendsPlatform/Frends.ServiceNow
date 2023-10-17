namespace Frends.ServiceNow.DeleteRecord.Definitions;

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
    /// <example>https://dev123456.service-now.com/api/now/table/{tableName}/{sys_id}</example>
    [DefaultValue("https://dev123456.service-now.com/api/now/table/{tableName}/{sys_id}")]
    [DisplayFormat(DataFormatString = "Text")]
    public string Url { get; set; }

    /// <summary>
    /// List of HTTP headers to be added to the request.
    /// </summary>
    /// <example>Name: Content-Type, Value: application/json</example>
    public Header[] Headers { get; set; }
}