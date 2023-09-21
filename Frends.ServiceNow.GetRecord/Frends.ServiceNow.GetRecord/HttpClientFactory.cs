namespace Frends.ServiceNow.GetRecord;

using System.Net.Http;
using Frends.ServiceNow.GetRecord.Definitions;

internal class HttpClientFactory : IHttpClientFactory
{
    public HttpClient CreateClient(Options options)
    {
        var handler = new HttpClientHandler();
        handler.SetHandlerSettingsBasedOnOptions(options);
        return new HttpClient(handler);
    }
}
