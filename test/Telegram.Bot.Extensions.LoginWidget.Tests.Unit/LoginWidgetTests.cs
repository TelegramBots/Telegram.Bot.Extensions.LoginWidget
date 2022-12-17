using System.Collections.Generic;
using Xunit;

namespace Telegram.Bot.Extensions.TelegramLogin.Tests.Unit;

public class LoginWidgetTests : IClassFixture<LoginWidgetTestsFixture>
{
    private readonly LoginWidgetTestsFixture _fixture;

    private readonly LoginWidget _loginWidget;

    public LoginWidgetTests(LoginWidgetTestsFixture fixture)
    {
        _fixture = fixture;

        _loginWidget = new LoginWidget(_fixture.Token)
        {
            AllowedTimeOffset = 60
        };
    }

    [Fact]
    public void Detect_MissingField_AuthDate()
    {
        Dictionary<string, string?> fields = new()
        {
            { "id", string.Empty },
            { "hash", string.Empty }
        };

        Authorization authorizationResult = _loginWidget.CheckAuthorization(fields);

        Assert.Equal(Authorization.MissingFields, authorizationResult);
    }

    [Fact]
    public void Detect_MissingField_Id()
    {
        Dictionary<string, string?> fields = new()
        {
            { "auth_date", string.Empty },
            { "hash", string.Empty }
        };

        Authorization authorizationResult = _loginWidget.CheckAuthorization(fields);

        Assert.Equal(Authorization.MissingFields, authorizationResult);
    }

    [Fact]
    public void Detect_MissingField_Hash()
    {
        Dictionary<string, string?> fields = new()
        {
            { "auth_date", string.Empty },
            { "id", string.Empty },
        };

        Authorization authorizationResult = _loginWidget.CheckAuthorization(fields);

        Assert.Equal(Authorization.MissingFields, authorizationResult);
    }

    [Fact]
    public void Detect_InvalidFormat_AuthDate()
    {
        Dictionary<string, string?> fields = new()
        {
            { "auth_date", "Not a number" },
            { "id", string.Empty },
            { "hash", "d5e0dfc1d85d8e0488647a8e62adc55bcf49a8ef598a446f42186b646f35728e" }
        };

        Authorization authorizationResult = _loginWidget.CheckAuthorization(fields);

        Assert.Equal(Authorization.InvalidAuthDateFormat, authorizationResult);
    }

    [Fact]
    public void Detect_TooOldAuthorization()
    {
        Dictionary<string, string?> fields = new()
        {
            // Test with January 1st 1970
            { "auth_date", "0" },
            { "id", string.Empty },
            { "hash", "d5e0dfc1d85d8e0488647a8e62adc55bcf49a8ef598a446f42186b646f35728e" }
        };

        Authorization authorizationResult = _loginWidget.CheckAuthorization(fields);

        Assert.Equal(Authorization.TooOld, authorizationResult);
    }

    [Fact]
    public void AllowedTimeOffset_Respected()
    {
        Dictionary<string, string?> fields = new()
        {
            { "auth_date", _fixture.CurrentTimestamp },
            { "id", string.Empty },
            { "hash", "d5e0dfc1d85d8e0488647a8e62adc55bcf49a8ef598a446f42186b646f35728e" }
        };

        Authorization authorizationResult = _loginWidget.CheckAuthorization(fields);

        Assert.NotEqual(Authorization.TooOld, authorizationResult);
    }

    [Fact]
    public void Recognises_Valid_Authorization()
    {
        // ValidTests contains valid test data generated using the TestBotToken
        foreach (SortedDictionary<string, string?> fields in _fixture.ValidTests)
        {
            Authorization authorizationResult = _loginWidget.CheckAuthorization(fields);

            Assert.Equal(Authorization.Valid, authorizationResult);
        }
    }

    [Fact]
    public void Recognises_Invalid_Authorization()
    {
        // InvalidTests contains invalid test data generated using the TestBotToken
        foreach (SortedDictionary<string, string?> fields in _fixture.InvalidTests)
        {
            Authorization authorizationResult = _loginWidget.CheckAuthorization(fields);

            Assert.Equal(Authorization.InvalidHash, authorizationResult);
        }
    }

    [Fact]
    public void Real_Data_Valid()
    {
        LoginWidget loginWidget = new(LoginWidgetTestsFixture.RealLifeDataTests_Token)
        {
            AllowedTimeOffset = int.MaxValue
        };

        foreach (SortedDictionary<string, string?> testData in LoginWidgetTestsFixture.RealLifeDataTests)
        {
            Authorization authorizationResult = loginWidget.CheckAuthorization(testData);

            Assert.Equal(Authorization.Valid, authorizationResult);
        }
    }
}
