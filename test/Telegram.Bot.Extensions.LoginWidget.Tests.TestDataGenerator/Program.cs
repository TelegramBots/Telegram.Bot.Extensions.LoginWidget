using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using Newtonsoft.Json;

namespace Telegram.Bot.Extensions.LoginWidget.Tests.TestDataGenerator
{
    class Program
    {
        static readonly Random _random = new Random();

        static void Main(string[] args)
        {
            string testTokenPath = @"..\..\..\..\Telegram.Bot.Extensions.LoginWidget.Tests.Unit\TestData\TestToken.txt";
            string validTestsPath = @"..\..\..\..\Telegram.Bot.Extensions.LoginWidget.Tests.Unit\TestData\ValidTests.json";
            string invalidTestsPath = @"..\..\..\..\Telegram.Bot.Extensions.LoginWidget.Tests.Unit\TestData\InvalidTests.json";

            string testToken = RandomString();

            int testCount = 12;
            Dictionary<string, string>[] validTests = new Dictionary<string, string>[testCount];
            Dictionary<string, string>[] invalidTests = new Dictionary<string, string>[testCount];
            
            using (HMACSHA256 hmac = new HMACSHA256())
            {
                using (SHA256 sha256 = SHA256.Create())
                {
                    hmac.Key = sha256.ComputeHash(Encoding.ASCII.GetBytes(testToken));
                }

                for (int i = 0; i < testCount; i++)
                {
                    Dictionary<string, string> fields = new Dictionary<string, string>(6);
                    fields.Add("auth_date", RandomTime());
                    fields.Add("first_name", RandomString());
                    fields.Add("id", RandomString());
                    fields.Add("photo_url", RandomString());
                    fields.Add("username", RandomString());
                    fields.Add("hash", ComputeHash(fields, hmac));
                    validTests[i] = fields;
                }
                for (int i = 0; i < testCount; i++)
                {
                    Dictionary<string, string> fields = new Dictionary<string, string>(validTests[i]);
                    switch (i % 6)
                    {
                        case 0:
                            fields["auth_date"] = RandomTime();
                            break;

                        case 1:
                            fields["first_name"] = RandomString();
                            break;

                        case 2:
                            fields["id"] = RandomString();
                            break;

                        case 3:
                            fields["photo_url"] = RandomString();
                            break;

                        case 4:
                            fields["username"] = RandomString();
                            break;

                        case 5:
                            fields["hash"] = RandomString(_random.Next() % 2 == 0 ? 64 : _random.Next(1, 100));
                            break;
                    }
                    invalidTests[i] = fields;
                }
            }

            File.WriteAllText(testTokenPath, testToken);
            File.WriteAllText(validTestsPath, JsonConvert.SerializeObject(validTests, Formatting.Indented));
            File.WriteAllText(invalidTestsPath, JsonConvert.SerializeObject(invalidTests, Formatting.Indented));
        }

        static string RandomString(int length = 10)
        {
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                byte[] random = new byte[length];
                rng.GetBytes(random);
                return Convert.ToBase64String(random).Substring(0, length);
            }
        }
        static string RandomTime()
        {
            long time = new DateTime(2018, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks / 10000;
            time += _random.Next(-1000000, 1000000);
            return time.ToString();
        }

        static string ComputeHash(Dictionary<string, string> fields, HMACSHA256 hmac)
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
