namespace Frends.ServiceNow.CreateRecord.Definitions;

using System.Net.Http;

/// <summary>
/// Http Client Factory Interface.
/// </summary>
public interface IHttpClientFactory
{
    /// <summary>
    /// Create client.
    /// </summary>
    /// <returns>HttpClient object</returns>
    HttpClient CreateClient(Options options);
}
