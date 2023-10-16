namespace Frends.ServiceNow.CreateRecord.Tests;

using System;
using System.Threading.Tasks;
using Frends.ServiceNow.CreateRecord.Definitions;
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
            Format = MessageFormat.JSON,
            Json = @"{""short_description"":""Hello World"",""caller_id"":""oauth.user""}",
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
    public async Task ServiceNow_TestOauth()
    {
        var result = await ServiceNow.CreateRecord(_input, _options, default);
        Assert.AreEqual(201, result.StatusCode);
        Assert.IsNotNull(result.Body);

        await Helpers.DeleteCreatedRecord($"{_input.Url}/{result.Body.result.sys_id}", _accessToken);
    }

    [Test]
    public async Task ServiceNow_TestBasic()
    {
        _options.Authentication = Authentication.Basic;
        _options.Token = null;
        _options.Username = BasicUser;
        _options.Password = BasicPass;
        var result = await ServiceNow.CreateRecord(_input, _options, default);
        Assert.AreEqual(201, result.StatusCode);
        Assert.IsNotNull(result.Body);

        await Helpers.DeleteCreatedRecord($"{_input.Url}/{result.Body.result.sys_id}", _accessToken);
    }

    [Test]
    public async Task ServiceNow_TestCreateWithJson()
    {
        var result = await ServiceNow.CreateRecord(_input, _options, default);
        Assert.AreEqual(201, result.StatusCode);
        Assert.IsNotNull(result.Body);

        await Helpers.DeleteCreatedRecord($"{_input.Url}/{result.Body.result.sys_id}", _accessToken);
    }

    [Test]
    public async Task ServiceNow_TestCreateWithXml()
    {
        _input.Format = MessageFormat.XML;
        _input.Xml = @"
<request>
    <entry>
        <short_description>Hello World</short_description>
        <caller_id>oauth.user</caller_id>
    </entry>
</request>";
        _input.Headers = new Header[]
            {
                new Header { Name = "Content-Type", Value = "application/xml" },
            };
        var result = await ServiceNow.CreateRecord(_input, _options, default);
        Assert.AreEqual(201, result.StatusCode);
        Assert.IsNotNull(result.Body);

        await Helpers.DeleteCreatedRecord($"{_input.Url}/{result.Body.result.sys_id}", _accessToken);
    }
}
