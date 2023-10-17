namespace Frends.ServiceNow.CreateRecord.Definitions;

using System.Collections.Generic;

/// <summary>
/// Result class
/// </summary>
public class Result
{
    internal Result(object body, Dictionary<string, string> headers, int statusCode)
    {
        Body = body;
        Headers = headers;
        StatusCode = statusCode;
    }

    /// <summary>
    /// Body of response
    /// </summary>
    /// <example>{"id": "abcdefghijkl123456789",  "success": true,  "errors": []}</example>
    public dynamic Body { get; private set; }

    /// <summary>
    /// Headers of response
    /// </summary>
    /// <example>{[ "content-type": "application/json", ... ]}</example>
    public Dictionary<string, string> Headers { get; private set; }

    /// <summary>
    /// Statuscode of response
    /// </summary>
    /// <example>200</example>
    public int StatusCode { get; private set; }
}
