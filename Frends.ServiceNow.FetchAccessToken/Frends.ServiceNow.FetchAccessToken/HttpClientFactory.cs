namespace Frends.ServiceNow.FetchAccessToken;

using System.Net.Http;
using Frends.ServiceNow.FetchAccessToken.Definitions;

internal class HttpClientFactory : IHttpClientFactory
{
    public HttpClient CreateClient(Options options)
    {
        var handler = new HttpClientHandler();
        handler.SetHandlerSettingsBasedOnOptions(options);
        return new HttpClient(handler);
    }
}
