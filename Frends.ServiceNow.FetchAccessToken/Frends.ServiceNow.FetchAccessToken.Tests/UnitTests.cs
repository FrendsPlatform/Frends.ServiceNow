namespace Frends.ServiceNow.FetchAccessToken.Tests;

using System;
using System.Threading.Tasks;
using Frends.ServiceNow.FetchAccessToken.Definitions;
using NUnit.Framework;

[TestFixture]
internal class UnitTests
{
    private static readonly string Url = Environment.GetEnvironmentVariable("Frends_ServiceNow_Url");
    private static readonly string ClientId = Environment.GetEnvironmentVariable("Frends_ServiceNow_ClientId");
    private static readonly string ClientSecret = Environment.GetEnvironmentVariable("Frends_ServiceNow_ClientSecret");
    private static readonly string Username = "oauth.user";
    private static readonly string Password = Environment.GetEnvironmentVariable("Frends_ServiceNow_OAuthPass");
    private static Input _input = new Input();
    private static Options _options = new Options();

    [SetUp]
    public void Setup()
    {
        _input = new Input()
        {
            Url = Url,
            GrantType = GrantType.Password,
            ClientId = ClientId,
            ClientSecret = ClientSecret,
            Username = Username,
            Password = Password,
        };

        _options = new Options()
        {
            ConnectionTimeoutSeconds = 30,
            AllowInvalidCertificate = true,
            AllowInvalidResponseContentTypeCharSet = true,
            AutomaticCookieHandling = true,
            FollowRedirects = true,
            ThrowExceptionOnErrorResponse = true,
        };
    }

    [Test]
    public async Task ServiceNow_TestPasswordGrantType()
    {
        var result = await ServiceNow.FetchAccessToken(_input, _options, default);
        Assert.AreEqual(200, result.StatusCode);
        Assert.IsNotNull(result.Body.access_token);
    }

    [Test]
    public async Task ServiceNow_TestRefreshTokenGrantType()
    {
        var result = await ServiceNow.FetchAccessToken(_input, _options, default);
        var refreshToken = result.Body.refresh_token;
        _input.GrantType = GrantType.Refresh_token;
        _input.Username = null;
        _input.Password = null;
        _input.RefreshToken = refreshToken;
        result = await ServiceNow.FetchAccessToken(_input, _options, default);
        Assert.AreEqual(200, result.StatusCode);
        Assert.IsNotNull(result.Body.access_token);
    }

    [Test]
    public async Task ServiceNow_TestCustomGrantType()
    {
        var input = new Input()
        {
            Url = Url,
            GrantType = GrantType.Custom,
            Properties = new CustomProperty[]
            {
                new CustomProperty() { Name = "grant_type", Value = "password" },
                new CustomProperty() { Name = "client_id", Value = ClientId },
                new CustomProperty() { Name = "client_secret", Value = ClientSecret },
                new CustomProperty() { Name = "username", Value = Username },
                new CustomProperty() { Name = "password", Value = Password },
            },
        };

        var result = await ServiceNow.FetchAccessToken(input, _options, default);
        Assert.AreEqual(200, result.StatusCode);
        Assert.IsNotNull(result.Body.access_token);

        input.Properties = new CustomProperty[]
        {
            new CustomProperty() { Name = "grant_type", Value = "refresh_token" },
            new CustomProperty() { Name = "client_id", Value = ClientId },
            new CustomProperty() { Name = "client_secret", Value = ClientSecret },
            new CustomProperty() { Name = "refresh_token", Value = result.Body.refresh_token },
        };

        result = await ServiceNow.FetchAccessToken(input, _options, default);
        Assert.AreEqual(200, result.StatusCode);
        Assert.IsNotNull(result.Body.access_token);
    }
}
