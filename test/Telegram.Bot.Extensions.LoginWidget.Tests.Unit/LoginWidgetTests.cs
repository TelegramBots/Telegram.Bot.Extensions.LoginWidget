using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Xunit;

namespace Telegram.Bot.Extensions.LoginWidget.Tests.Unit
{
    public class LoginWidgetTests
    {
        // Fake token used to generate test data
        private static readonly string TestBotToken =
            File.ReadAllText(@"..\..\..\TestData\TestToken.txt");
        private readonly LoginWidget _loginWidget;

        public LoginWidgetTests()
        {
            _loginWidget = new LoginWidget(TestBotToken)
            {
                AllowedTimeOffset = long.MaxValue // We are using old test data
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

            Assert.Equal(Authorization.MissingFields, _loginWidget.CheckAuthorization(fields));
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

            Assert.Equal(Authorization.MissingFields, _loginWidget.CheckAuthorization(fields));
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

            Assert.Equal(Authorization.MissingFields, _loginWidget.CheckAuthorization(fields));
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

            Assert.Equal(Authorization.MissingFields, _loginWidget.CheckAuthorization(fields));
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

            Assert.Equal(Authorization.MissingFields, _loginWidget.CheckAuthorization(fields));
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

            Assert.Equal(Authorization.MissingFields, _loginWidget.CheckAuthorization(fields));
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
                { "hash",       string.Empty }
            };

            Assert.Equal(Authorization.InvalidAuthDateFormat, _loginWidget.CheckAuthorization(fields));
        }

        [Fact]
        public void Detect_TooOldAuthorization()
        {
            _loginWidget.AllowedTimeOffset = 30;

            Dictionary<string, string> fields = new Dictionary<string, string>()
            {
                // Test with January 1st 2018
                { "auth_date",  (new DateTime(2018, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks / 10000).ToString() },
                { "first_name", string.Empty },
                { "id",         string.Empty },
                { "photo_url",  string.Empty },
                { "username",   string.Empty },
                { "hash",       string.Empty }
            };

            Assert.Equal(Authorization.TooOld, _loginWidget.CheckAuthorization(fields));
        }

        [Fact]
        public void AllowedTimeOffset_Respected()
        {
            Dictionary<string, string> fields = new Dictionary<string, string>()
            {
                // Test with January 1st 2018
                { "auth_date",  (new DateTime(2018, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks / 10000).ToString() },
                { "first_name", string.Empty },
                { "id",         string.Empty },
                { "photo_url",  string.Empty },
                { "username",   string.Empty },
                { "hash",       string.Empty }
            };

            // Will actually be Authorization.InvalidHash
            Assert.NotEqual(Authorization.TooOld, _loginWidget.CheckAuthorization(fields));
        }

        [Fact]
        public void Recognises_Valid_Authorization()
        {
            // ValidTests.json contains valid test data generated using the TestBotToken
            Dictionary<string, string>[] validTests =
                JsonConvert.DeserializeObject<Dictionary<string, string>[]>(File.ReadAllText(@"..\..\..\TestData\ValidTests.json"));

            foreach (Dictionary<string, string> fields in validTests)
            {
                Assert.Equal(Authorization.Valid, _loginWidget.CheckAuthorization(fields));
            }
        }

        [Fact]
        public void Recognises_Invalid_Authorization()
        {
            // InvalidTests.json contains invalid test data generated using the TestBotToken
            Dictionary<string, string>[] invalidTests =
                JsonConvert.DeserializeObject<Dictionary<string, string>[]>(File.ReadAllText(@"..\..\..\TestData\InvalidTests.json"));

            foreach (Dictionary<string, string> fields in invalidTests)
            {
                Assert.Equal(Authorization.InvalidHash, _loginWidget.CheckAuthorization(fields));
            }
        }
    }
}
