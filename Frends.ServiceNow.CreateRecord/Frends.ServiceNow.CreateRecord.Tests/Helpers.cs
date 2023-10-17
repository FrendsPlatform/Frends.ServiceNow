namespace Frends.ServiceNow.CreateRecord.Tests;

using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Net.Http;

internal static class Helpers
{
    public static async Task<string> GetAccessToken(string baseUrl, string clientId, string clientSecret, string username, string password)
    {
        var tokenUrl = baseUrl + "/oauth_token.do";

        using var client = new HttpClient();

        var content = new FormUrlEncodedContent(new[]
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

    public static async Task DeleteCreatedRecord(string url, string accessToken)
    {
        using var client = new HttpClient();

        using var content = new StringContent(string.Empty);
        var headers = new Dictionary<string, string> { { "Authorization", $"Bearer {accessToken}" } };
        _ = await ServiceNow.GetHttpRequestResponseAsync(client, url, content, headers, default, "DELETE");
    }
}
