namespace Frends.ServiceNow.FetchAccessToken;

using System.Collections.Generic;
using System;
using System.ComponentModel;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net.Http;
using System.Runtime.Caching;
using Frends.ServiceNow.FetchAccessToken.Definitions;

/// <summary>
/// Main class of the Task.
/// </summary>
public static class ServiceNow
{
    private static readonly IHttpClientFactory ClientFactory = new HttpClientFactory();
    private static readonly ObjectCache ClientCache = MemoryCache.Default;
    private static readonly CacheItemPolicy CachePolicy = new CacheItemPolicy() { SlidingExpiration = TimeSpan.FromHours(1) };

    /// <summary>
    /// Frends Task for fetching access token for ServiceNow instance.
    /// [Documentation](https://tasks.frends.com/tasks/frends-tasks/Frends.ServiceNow.FetchAccessToken).
    /// </summary>
    /// <param name="input">Input parameters.</param>
    /// <param name="options">Options parameters.</param>
    /// <param name="cancellationToken">Cancellation token given by Frends.</param>
    /// <returns>Object { dynamic Body, int StatusCode }</returns>
    public static async Task<Result> FetchAccessToken([PropertyTab] Input input, [PropertyTab] Options options, CancellationToken cancellationToken)
    {
        var httpClient = GetHttpClientForOptions(options);

        FormUrlEncodedContent content = GetHttpContent(input);

        using var responseMessage = await GetHttpRequestResponseAsync(
                httpClient,
                $"{input.Url}/oauth_token.do",
                content,
                cancellationToken)
            .ConfigureAwait(false);

        var rBody = TryParseRequestStringResultAsJToken(await responseMessage.Content
            .ReadAsStringAsync(cancellationToken)
            .ConfigureAwait(false));

        if (!responseMessage.IsSuccessStatusCode && options.ThrowExceptionOnErrorResponse)
        {
            throw new WebException(
                $"Request to '{input.Url}' failed with status code {(int)responseMessage.StatusCode}. Response body: {rBody}");
        }

        return new Result(rBody, (int)responseMessage.StatusCode);
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

    private static FormUrlEncodedContent GetHttpContent(Input input)
    {
        FormUrlEncodedContent content;

        switch (input.GrantType)
        {
            case GrantType.Password:
                content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", input.GrantType.ToString().ToLower()),
                    new KeyValuePair<string, string>("client_id", input.ClientId),
                    new KeyValuePair<string, string>("client_secret", input.ClientSecret),
                    new KeyValuePair<string, string>("username", input.Username),
                    new KeyValuePair<string, string>("password", input.Password),
                });
                break;
            case GrantType.Refresh_token:
                content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", input.GrantType.ToString().ToLower()),
                    new KeyValuePair<string, string>("client_id", input.ClientId),
                    new KeyValuePair<string, string>("refresh_token", input.RefreshToken),
                    new KeyValuePair<string, string>("client_secret", input.ClientSecret),
                });
                break;
            default:
                var properties = new List<KeyValuePair<string, string>>();
                foreach (var property in input.Properties)
                    properties.Add(new KeyValuePair<string, string>(property.Name, property.Value));
                content = new FormUrlEncodedContent(properties);
                break;
        }

        content.Headers.Clear();
        content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

        return content;
    }

    private static async Task<HttpResponseMessage> GetHttpRequestResponseAsync(
            HttpClient httpClient,
            string url,
            FormUrlEncodedContent content,
            CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using var request = new HttpRequestMessage(new HttpMethod("POST"), new Uri(url))
        {
            Content = content,
        };

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
        return $"{options.ConnectionTimeoutSeconds}"
               + $":{options.FollowRedirects}:{options.AllowInvalidCertificate}:{options.AllowInvalidResponseContentTypeCharSet}"
               + $":{options.ThrowExceptionOnErrorResponse}:{options.AutomaticCookieHandling}";
    }
}
