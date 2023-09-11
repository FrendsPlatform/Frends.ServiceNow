namespace Frends.ServiceNow.GetRecord.Tests;

using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json.Linq;

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
}
