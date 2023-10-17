namespace Frends.ServiceNow.DeleteRecord;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net.Http;
using System.Runtime.Caching;
using Frends.ServiceNow.DeleteRecord.Definitions;

/// <summary>
/// Main class of the Task.
/// </summary>
public static class ServiceNow
{
    private static readonly IHttpClientFactory ClientFactory = new HttpClientFactory();
    private static readonly ObjectCache ClientCache = MemoryCache.Default;
    private static readonly CacheItemPolicy CachePolicy = new CacheItemPolicy() { SlidingExpiration = TimeSpan.FromHours(1) };

    /// <summary>
    /// Frends Task for deleting a record from ServiceNow instance.
    /// [Documentation](https://tasks.frends.com/tasks/frends-tasks/Frends.ServiceNow.DeleteRecord).
    /// </summary>
    /// <param name="input">Input parameters.</param>
    /// <param name="options">Options parameters.</param>
    /// <param name="cancellationToken">Cancellation token given by Frends.</param>
    /// <returns>Object { dynamic Body, Dictionary&lt;string, string&gt; Headers, int StatusCode }</returns>
    public static async Task<Result> DeleteRecord([PropertyTab] Input input, [PropertyTab] Options options, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(input.Url)) throw new ArgumentNullException("Parameter Url can not be empty.");

        var httpClient = GetHttpClientForOptions(options);

        using var responseMessage = await GetHttpRequestResponseAsync(
                httpClient,
                input.Url,
                cancellationToken)
            .ConfigureAwait(false);

        dynamic response;

        var rbody = TryParseRequestStringResultAsJToken(await responseMessage.Content
            .ReadAsStringAsync(cancellationToken)
            .ConfigureAwait(false));
        var rstatusCode = (int)responseMessage.StatusCode;
        var rheaders = GetResponseHeaderDictionary(responseMessage.Headers, responseMessage.Content.Headers);
        response = new Result(rbody, rheaders, rstatusCode);

        if (!responseMessage.IsSuccessStatusCode && options.ThrowExceptionOnErrorResponse)
        {
            throw new WebException(
                $"Request to '{input.Url}' failed with status code {(int)responseMessage.StatusCode}. Response body: {response.Body}");
        }

        return response;
    }

    internal static async Task<HttpResponseMessage> GetHttpRequestResponseAsync(
            HttpClient httpClient,
            string url,
            CancellationToken cancellationToken,
            string method = "DELETE")
    {
        cancellationToken.ThrowIfCancellationRequested();

        using var request = new HttpRequestMessage(new HttpMethod(method), new Uri(url));

        HttpResponseMessage response;
        try
        {
            response = await httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }
        catch (TaskCanceledException canceledException)
        {
            if (cancellationToken.IsCancellationRequested)
                throw;

            // Cancellation is from inside of the request, mostly likely a timeout
            throw new ArgumentException("HttpRequest was canceled, most likely due to a timeout.", canceledException);
        }

        return response;
    }

    internal static object TryParseRequestStringResultAsJToken(string response)
    {
        try
        {
            return string.IsNullOrWhiteSpace(response) ? new JValue(string.Empty) : JToken.Parse(response);
        }
        catch (JsonReaderException)
        {
            throw new JsonReaderException($"Unable to read response message as json: {response}");
        }
    }

    private static Dictionary<string, string> GetResponseHeaderDictionary(HttpResponseHeaders responseMessageHeaders, HttpContentHeaders contentHeaders)
    {
        var responseHeaders = responseMessageHeaders.ToDictionary(h => h.Key, h => string.Join(";", h.Value));
        var allHeaders = contentHeaders?.ToDictionary(h => h.Key, h => string.Join(";", h.Value)) ?? new Dictionary<string, string>();
        responseHeaders.ToList().ForEach(x => allHeaders[x.Key] = x.Value);
        return allHeaders;
    }

    private static HttpClient GetHttpClientForOptions(Options options)
    {
        var cacheKey = GetHttpClientCacheKey(options);

        if (ClientCache.Get(cacheKey) is HttpClient httpClient)
            return httpClient;

        httpClient = ClientFactory.CreateClient(options);
        httpClient.SetDefaultRequestHeadersBasedOnOptions(options);

        ClientCache.Add(cacheKey, httpClient, CachePolicy);

        return httpClient;
    }

    private static string GetHttpClientCacheKey(Options options)
    {
        // Includes everything except for options.Token, which is used on request level, not http client level
        return $"{options.Authentication}:{options.Username}:{options.Password}:{options.ConnectionTimeoutSeconds}"
               + $":{options.FollowRedirects}:{options.AllowInvalidCertificate}:{options.AllowInvalidResponseContentTypeCharSet}"
               + $":{options.ThrowExceptionOnErrorResponse}:{options.AutomaticCookieHandling}";
    }
}
