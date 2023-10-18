namespace Frends.ServiceNow.DeleteRecord.Tests;

using System;
using System.Threading.Tasks;
using Frends.ServiceNow.DeleteRecord.Definitions;
using NUnit.Framework;

[TestFixture]
internal class UnitTests
{
    private static readonly string BaseUrl = Environment.GetEnvironmentVariable("Frends_ServiceNow_Url");
    private static readonly string OAuthUser = "oauth.user";
    private static readonly string OAuthPass = Environment.GetEnvironmentVariable("Frends_ServiceNow_OAuthPass");
    private static readonly string ClientId = Environment.GetEnvironmentVariable("Frends_ServiceNow_ClientId");
    private static readonly string ClientSecret = Environment.GetEnvironmentVariable("Frends_ServiceNow_ClientSecret");
    private static readonly string BasicUser = "basicauth.user";
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
            Headers = Array.Empty<Header>(),
        };

        _options = new Options
        {
            Authentication = Authentication.OAuth,
            Token = _accessToken,
            AllowInvalidCertificate = true,
            AllowInvalidResponseContentTypeCharSet = true,
            AutomaticCookieHandling = true,
            ConnectionTimeoutSeconds = 30,
            FollowRedirects = true,
            ThrowExceptionOnErrorResponse = true,
        };
    }

    [Test]
    public async Task ServiceNow_DeleteRecord()
    {
        var sys_id = await Helpers.CreateRecord(_input.Url, _accessToken);
        _input.Url = _input.Url + $"/{sys_id}";

        var result = await ServiceNow.DeleteRecord(_input, _options, default);
        Assert.AreEqual(204, result.StatusCode);
    }

    [Test]
    public async Task ServiceNow_DeleteRecordWithBasicAuth()
    {
        var sys_id = await Helpers.CreateRecord(_input.Url, _accessToken);
        _input.Url = _input.Url + $"/{sys_id}";
        _options.Authentication = Authentication.Basic;
        _options.Token = null;
        _options.Username = BasicUser;
        _options.Password = BasicPass;

        var result = await ServiceNow.DeleteRecord(_input, _options, default);
        Assert.AreEqual(204, result.StatusCode);
    }
}
