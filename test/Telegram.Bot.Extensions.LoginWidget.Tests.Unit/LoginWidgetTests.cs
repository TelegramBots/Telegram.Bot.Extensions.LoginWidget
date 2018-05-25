using System;
using System.Collections.Generic;
using Xunit;

namespace Telegram.Bot.Extensions.LoginWidget.Tests.Unit
{
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
            Dictionary<string, string> fields = new Dictionary<string, string>()
            {
                { "first_name", string.Empty },
                { "id",         string.Empty },
                { "photo_url",  string.Empty },
                { "username",   string.Empty },
                { "hash",       string.Empty }
            };

            Authorization authorizationResult = _loginWidget.CheckAuthorization(fields);

            Assert.Equal(Authorization.MissingFields, authorizationResult);
        }

        [Fact]
        public void Detect_MissingField_FirstName()
        {
            Dictionary<string, string> fields = new Dictionary<string, string>()
            {
                { "auth_date",  string.Empty },
                { "id",         string.Empty },
                { "photo_url",  string.Empty },
                { "username",   string.Empty },
                { "hash",       string.Empty }
            };

            Authorization authorizationResult = _loginWidget.CheckAuthorization(fields);

            Assert.Equal(Authorization.MissingFields, authorizationResult);
        }

        [Fact]
        public void Detect_MissingField_Id()
        {
            Dictionary<string, string> fields = new Dictionary<string, string>()
            {
                { "auth_date",  string.Empty },
                { "first_name", string.Empty },
                { "photo_url",  string.Empty },
                { "username",   string.Empty },
                { "hash",       string.Empty }
            };

            Authorization authorizationResult = _loginWidget.CheckAuthorization(fields);

            Assert.Equal(Authorization.MissingFields, authorizationResult);
        }

        [Fact]
        public void Detect_MissingField_PhotoUrl()
        {
            Dictionary<string, string> fields = new Dictionary<string, string>()
            {
                { "auth_date",  string.Empty },
                { "first_name", string.Empty },
                { "id",         string.Empty },
                { "username",   string.Empty },
                { "hash",       string.Empty }
            };

            Authorization authorizationResult = _loginWidget.CheckAuthorization(fields);

            Assert.Equal(Authorization.MissingFields, authorizationResult);
        }

        [Fact]
        public void Detect_MissingField_Username()
        {
            Dictionary<string, string> fields = new Dictionary<string, string>()
            {
                { "auth_date",  string.Empty },
                { "first_name", string.Empty },
                { "id",         string.Empty },
                { "photo_url",  string.Empty },
                { "hash",       string.Empty }
            };

            Authorization authorizationResult = _loginWidget.CheckAuthorization(fields);

            Assert.Equal(Authorization.MissingFields, authorizationResult);
        }

        [Fact]
        public void Detect_MissingField_Hash()
        {
            Dictionary<string, string> fields = new Dictionary<string, string>()
            {
                { "auth_date",  string.Empty },
                { "first_name", string.Empty },
                { "id",         string.Empty },
                { "photo_url",  string.Empty },
                { "username",   string.Empty }
            };

            Authorization authorizationResult = _loginWidget.CheckAuthorization(fields);

            Assert.Equal(Authorization.MissingFields, authorizationResult);
        }

        [Fact]
        public void Detect_InvalidFormat_AuthDate()
        {
            Dictionary<string, string> fields = new Dictionary<string, string>()
            {
                { "auth_date",  "Not a number" },
                { "first_name", string.Empty },
                { "id",         string.Empty },
                { "photo_url",  string.Empty },
                { "username",   string.Empty },
                { "hash",       "d5e0dfc1d85d8e0488647a8e62adc55bcf49a8ef598a446f42186b646f35728e" }
            };

            Authorization authorizationResult = _loginWidget.CheckAuthorization(fields);

            Assert.Equal(Authorization.InvalidAuthDateFormat, authorizationResult);
        }

        [Fact]
        public void Detect_TooOldAuthorization()
        {
            Dictionary<string, string> fields = new Dictionary<string, string>()
            {
                // Test with January 1st 1970
                { "auth_date",  "0" },
                { "first_name", string.Empty },
                { "id",         string.Empty },
                { "photo_url",  string.Empty },
                { "username",   string.Empty },
                { "hash",       "d5e0dfc1d85d8e0488647a8e62adc55bcf49a8ef598a446f42186b646f35728e" }
            };

            Authorization authorizationResult = _loginWidget.CheckAuthorization(fields);

            Assert.Equal(Authorization.TooOld, authorizationResult);
        }

        [Fact]
        public void AllowedTimeOffset_Respected()
        {
            Dictionary<string, string> fields = new Dictionary<string, string>()
            {
                { "auth_date",  _fixture.CurrentTimestamp },
                { "first_name", string.Empty },
                { "id",         string.Empty },
                { "photo_url",  string.Empty },
                { "username",   string.Empty },
                { "hash",       "d5e0dfc1d85d8e0488647a8e62adc55bcf49a8ef598a446f42186b646f35728e" }
            };

            Authorization authorizationResult = _loginWidget.CheckAuthorization(fields);

            Assert.NotEqual(Authorization.TooOld, authorizationResult);
        }

        [Fact]
        public void Recognises_Valid_Authorization()
        {
            // ValidTests contains valid test data generated using the TestBotToken
            foreach (Dictionary<string, string> fields in _fixture.ValidTests)
            {
                Authorization authorizationResult = _loginWidget.CheckAuthorization(fields);

                Assert.Equal(Authorization.Valid, authorizationResult);
            }
        }

        [Fact]
        public void Recognises_Invalid_Authorization()
        {
            // InvalidTests contains invalid test data generated using the TestBotToken
            foreach (Dictionary<string, string> fields in _fixture.InvalidTests)
            {
                Authorization authorizationResult = _loginWidget.CheckAuthorization(fields);

                Assert.Equal(Authorization.InvalidHash, authorizationResult);
            }
        }
    }
}
