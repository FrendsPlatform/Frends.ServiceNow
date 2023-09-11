namespace Frends.ServiceNow.GetRecord;

using System.Collections.Generic;
using System;
using System.ComponentModel;
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
using System.Diagnostics.CodeAnalysis;
using Frends.ServiceNow.GetRecord.Definitions;

/// <summary>
/// Main class of the Task.
/// </summary>
public static class ServiceNow
{
    internal static IHttpClientFactory ClientFactory = new HttpClientFactory();
    internal static readonly ObjectCache ClientCache = MemoryCache.Default;
    private static readonly CacheItemPolicy _cachePolicy = new CacheItemPolicy() { SlidingExpiration = TimeSpan.FromHours(1) };

    /// <summary>
    /// Frends Task for retrieving records from ServiceNow.
    /// [Documentation](https://tasks.frends.com/tasks/frends-tasks/Frends.ServiceNow.GetRecord).
    /// </summary>
    /// <param name="input">What to repeat.</param>
    /// <param name="options">Define if repeated multiple times. </param>
    /// <param name="cancellationToken">Cancellation token given by Frends.</param>
    /// <returns>Object { dynamic Body, Dictionary(string, string) Headers, int StatusCode }</returns>
    public static async Task<Result> GetRecord([PropertyTab] Input input, [PropertyTab] Options options, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(input.Url)) throw new ArgumentNullException("Url can not be empty.");

        var httpClient = GetHttpClientForOptions(options);
        var headers = GetHeaderDictionary(input.Headers, options);

        using var responseMessage = await GetHttpRequestResponseAsync(
                httpClient,
                input.Url,
                headers,
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

    internal static void ClearClientCache()
    {
        var cacheKeys = ClientCache.Select(kvp => kvp.Key).ToList();
        foreach (var cacheKey in cacheKeys)
            ClientCache.Remove(cacheKey);
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

    private static IDictionary<string, string> GetHeaderDictionary(Header[] headers, Options options)
    {
        if (!headers.Any(header => header.Name.ToLower().Equals("authorization")))
        {
            var authHeader = new Header { Name = "Authorization" };
            switch (options.Authentication)
            {
                case Authentication.Basic:
                    authHeader.Value = $"Basic {Convert.ToBase64String(Encoding.ASCII.GetBytes($"{options.Username}:{options.Password}"))}";
                    headers = headers.Concat(new[] { authHeader }).ToArray();
                    break;
                case Authentication.OAuth:
                    authHeader.Value = $"Bearer {options.Token}";
                    headers = headers.Concat(new[] { authHeader }).ToArray();
                    break;
            }
        }

        return headers.ToDictionary(key => key.Name, value => value.Value, StringComparer.InvariantCultureIgnoreCase);
    }

    private static HttpClient GetHttpClientForOptions(Options options)
    {
        var cacheKey = GetHttpClientCacheKey(options);

        if (ClientCache.Get(cacheKey) is HttpClient httpClient)
            return httpClient;

        httpClient = ClientFactory.CreateClient(options);
        httpClient.SetDefaultRequestHeadersBasedOnOptions(options);

        ClientCache.Add(cacheKey, httpClient, _cachePolicy);

        return httpClient;
    }

    [ExcludeFromCodeCoverage]
    private static string GetHttpClientCacheKey(Options options)
    {
        // Includes everything except for options.Token, which is used on request level, not http client level
        return $"{options.Authentication}:{options.Username}:{options.Password}:{options.ConnectionTimeoutSeconds}"
               + $":{options.FollowRedirects}:{options.AllowInvalidCertificate}:{options.AllowInvalidResponseContentTypeCharSet}"
               + $":{options.ThrowExceptionOnErrorResponse}:{options.AutomaticCookieHandling}";
    }

    private static async Task<HttpResponseMessage> GetHttpRequestResponseAsync(
            HttpClient httpClient,
            string url,
            IDictionary<string, string> headers,
            CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using var request = new HttpRequestMessage(new HttpMethod("GET"), new Uri(url));

        foreach (var header in headers)
            request.Headers.TryAddWithoutValidation(header.Key, header.Value);

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
}
