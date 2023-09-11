namespace Frends.ServiceNow.GetRecord.Tests;

using System;
using System.Threading.Tasks;
using Frends.ServiceNow.GetRecord.Definitions;
using NUnit.Framework;

/// <summary>
/// Test class.
/// </summary>
[TestFixture]
public class TestClass
{
    private static readonly string BaseUrl = Environment.GetEnvironmentVariable("Frends_ServiceNow_Url");
    private static readonly string OAuthUser = "admin";
    private static readonly string OAuthPass = Environment.GetEnvironmentVariable("Frends_ServiceNow_OAuthPass");
    private static readonly string ClientId = Environment.GetEnvironmentVariable("Frends_ServiceNow_ClientId");
    private static readonly string ClientSecret = Environment.GetEnvironmentVariable("Frends_ServiceNow_ClientSecret");
    private static readonly string BasicUser = "integration.user";
    private static readonly string BasicPass = Environment.GetEnvironmentVariable("Frends_ServiceNow_BasicPass");

    private string _accessToken;
    private Input _input;
    private Options _options;

    [OneTimeSetUp]
    public async Task OnetimeSetup()
    {
        _accessToken = await Helpers.GetAccessToken(BaseUrl, ClientId, ClientSecret, OAuthUser, OAuthPass);
    }

    [SetUp]
    public void Setup()
    {
        _input = new Input
        {
            Url = BaseUrl + "/api/now/table/incident",
            Headers = new Header[]
            {
                new Header { Name = "Content-Type", Value = "application/json" },
            },
        };

        _options = new Options
        {
            Authentication = Authentication.OAuth,
            AllowInvalidCertificate = true,
            AllowInvalidResponseContentTypeCharSet = true,
            AutomaticCookieHandling = true,
            ConnectionTimeoutSeconds = 30,
            FollowRedirects = true,
            ThrowExceptionOnErrorResponse = true,
            Token = _accessToken,
        };
    }

    [Test]
    public async Task ServiceNow_TestOauth()
    {
        var result = await ServiceNow.GetRecord(_input, _options, default);
        Assert.AreEqual(200, result.StatusCode);
        Assert.IsNotNull(result.Body);
    }

    [Test]
    public async Task ServiceNow_TestBasic()
    {
        _options.Authentication = Authentication.Basic;
        _options.Token = null;
        _options.Username = BasicUser;
        _options.Password = BasicPass;
        var result = await ServiceNow.GetRecord(_input, _options, default);
        Assert.AreEqual(200, result.StatusCode);
        Assert.IsNotNull(result.Body);
    }
}
