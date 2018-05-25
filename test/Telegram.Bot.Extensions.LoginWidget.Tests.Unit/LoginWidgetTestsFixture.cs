using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Telegram.Bot.Extensions.LoginWidget.Tests.Unit
{
    public class LoginWidgetTestsFixture
    {
        private const int _testCount = 12;

        private static readonly Random _random = new Random();

        public readonly string Token = RandomString();

        public readonly string CurrentTimestamp;

        public readonly Dictionary<string, string>[] ValidTests = new Dictionary<string, string>[_testCount];

        public readonly Dictionary<string, string>[] InvalidTests = new Dictionary<string, string>[_testCount];

        public LoginWidgetTestsFixture()
        {
            CurrentTimestamp = ((long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds).ToString();

            using (HMACSHA256 hmac = new HMACSHA256())
            {
                using (SHA256 sha256 = SHA256.Create())
                {
                    hmac.Key = sha256.ComputeHash(Encoding.ASCII.GetBytes(Token));
                }

                FillValidData(hmac);
                FillInvalidData();
            }
        }

        private void FillValidData(HMACSHA256 hmac)
        {
            for (int i = 0; i < _testCount; i++)
            {
                Dictionary<string, string> fields = new Dictionary<string, string>
                {
                    { "auth_date", CurrentTimestamp },
                    { "first_name", RandomString() },
                    { "id", RandomString() },
                    { "photo_url", RandomString() },
                    { "username", RandomString() }
                };
                fields.Add("hash", ComputeHash(fields, hmac));

                ValidTests[i] = fields;
            }
        }

        private void FillInvalidData()
        {
            for (int i = 0; i < _testCount; i++)
            {
                // replace field with random data
                Dictionary<string, string> fields = new Dictionary<string, string>
                {
                    { "auth_date",  CurrentTimestamp },
                    { "first_name", (i % 5) == 0 ? RandomString() : ValidTests[i]["first_name"] },
                    { "id",         (i % 5) == 1 ? RandomString() : ValidTests[i]["id"] },
                    { "photo_url",  (i % 5) == 2 ? RandomString() : ValidTests[i]["photo_url"] },
                    { "username",   (i % 5) == 3 ? RandomString() : ValidTests[i]["username"] },
                    { "hash",       (i % 5) == 4 ? RandomString(_random.Next() % 2 == 0 ? 64 : _random.Next(1, 100)) : ValidTests[i]["hash"] }
                };

                InvalidTests[i] = fields;
            }
        }

        private static string RandomString(int length = 10)
        {
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                byte[] random = new byte[length];
                rng.GetBytes(random);
                return Convert.ToBase64String(random).Substring(0, length);
            }
        }
        
        private static string ComputeHash(Dictionary<string, string> fields, HMACSHA256 hmac)
        {
            string data_check_string =
                "auth_date=" + fields["auth_date"] + '\n' +
                "first_name=" + fields["first_name"] + '\n' +
                "id=" + fields["id"] + '\n' +
                "photo_url=" + fields["photo_url"] + '\n' +
                "username=" + fields["username"];

            byte[] signature = hmac.ComputeHash(Encoding.UTF8.GetBytes(data_check_string));

            return BitConverter.ToString(signature).Replace("-", "").ToLower();
        }
    }
}
