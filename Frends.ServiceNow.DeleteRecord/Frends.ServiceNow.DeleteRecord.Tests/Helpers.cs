﻿namespace Frends.ServiceNow.DeleteRecord.Tests;

using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

internal class Helpers
{
    internal static async Task<string> CreateRecord(string url, string accessToken)
    {
        using var client = new HttpClient();

        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

        var msg = @"{""short_description"":""Hello World"",""caller_id"":""oauth.user""}";

        using var content = new StringContent(msg);

        content.Headers.Clear();
        content.Headers.Add("Content-Type", "application/json");

        HttpResponseMessage response = await client.PostAsync(url, content);

        JToken rBody = ServiceNow.TryParseRequestStringResultAsJToken(await response.Content
            .ReadAsStringAsync(default)
            .ConfigureAwait(false)) as JToken;

        return rBody["result"]["sys_id"].ToString();
    }

    internal static async Task<string> GetAccessToken(string baseUrl, string clientId, string clientSecret, string username, string password)
    {
        var tokenUrl = baseUrl + "/oauth_token.do";

        using var client = new HttpClient();

        using var content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("grant_type", "password"),
            new KeyValuePair<string, string>("client_id", clientId),
            new KeyValuePair<string, string>("resource", baseUrl + "/api/now/"),
            new KeyValuePair<string, string>("client_secret", clientSecret),
            new KeyValuePair<string, string>("username", username),
            new KeyValuePair<string, string>("password", password),
        });

        content.Headers.Clear();
        content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

        HttpResponseMessage response = await client.PostAsync(tokenUrl, content);

        JToken rBody = ServiceNow.TryParseRequestStringResultAsJToken(await response.Content
            .ReadAsStringAsync(default)
            .ConfigureAwait(false)) as JToken;

        return rBody["access_token"].ToString();
    }
}