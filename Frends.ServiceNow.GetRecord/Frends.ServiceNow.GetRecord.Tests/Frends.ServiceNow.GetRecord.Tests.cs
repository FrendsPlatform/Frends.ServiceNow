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
    private static readonly string UserName = Environment.GetEnvironmentVariable("Frends_ServiceNow_UserName");
    private static readonly string Password = Environment.GetEnvironmentVariable("Frends_ServiceNow_Password");
    private static readonly string ClientId = Environment.GetEnvironmentVariable("Frends_ServiceNow_ClientId");
    private static readonly string ClientSecret = Environment.GetEnvironmentVariable("Frends_ServiceNow_ClientSecret");
    private string _accessToken;
    private Input _input;
    private Options _options;

    [OneTimeSetUp]
    public async Task OnetimeSetup()
    {
        _accessToken = await Helpers.GetAccessToken(BaseUrl, ClientId, ClientSecret, UserName, Password);
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
    public async Task ServiceNow_Test()
    {
        var result = await ServiceNow.GetRecord(_input, _options, default);
        Assert.AreEqual(200, result.StatusCode);
        Assert.IsNotNull(result.Body);
    }
}
