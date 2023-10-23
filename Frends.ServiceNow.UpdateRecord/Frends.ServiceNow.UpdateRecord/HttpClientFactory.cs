namespace Frends.ServiceNow.UpdateRecord;

using System.Net.Http;
using Frends.ServiceNow.UpdateRecord.Definitions;

internal class HttpClientFactory : IHttpClientFactory
{
    public HttpClient CreateClient(Options options)
    {
        var handler = new HttpClientHandler();
        handler.SetHandlerSettingsBasedOnOptions(options);
        return new HttpClient(handler);
    }
}
