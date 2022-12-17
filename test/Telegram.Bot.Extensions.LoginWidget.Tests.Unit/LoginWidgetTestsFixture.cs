using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace Telegram.Bot.Extensions.TelegramLogin.Tests.Unit;

public class LoginWidgetTestsFixture
{
    private const int _testCount = 10;

    public readonly string Token = RandomString();

    public readonly string CurrentTimestamp;

    public const string RealLifeDataTests_Token = "324335643:AAHdDjFRqowmRegO7AHW4PzayNFzkIoMOww";
    public static readonly SortedDictionary<string, string?>[] RealLifeDataTests =
    {
        new()
        {
            { "id", "168175103" },
            { "first_name", "Miha" },
            { "last_name", "Zupan" },
            { "username", "MihaZupan" },
            { "photo_url", "https://t.me/i/userpic/320/MihaZupan.jpg" },
            { "auth_date", "1540852587" },
            { "hash", "5b108abf4749846e96c4aa449eb65246c500d29cd6711463166bd2ffcf87285f" }
        },
        new()
        {
            { "id", "168175103" },
            { "first_name", "Miha" },
            { "last_name", "Zupan" },
            { "username", "MihaZupan" },
            { "photo_url", "https://t.me/i/userpic/320/MihaZupan.jpg" },
            { "auth_date", "1540852662" },
            { "hash", "2f917a0cbd0779cc1f06bc089ebc9079dc946818117d5e2e1ebfdcaa9c60d797" }
        },
        new()
        {
            { "id", "168175103" },
            { "first_name", "Miha" },
            { "last_name", "Zupan" },
            { "username", "MihaZupan" },
            { "photo_url", "https://t.me/i/userpic/320/MihaZupan.jpg" },
            { "auth_date", "1540852698" },
            { "hash", "7855000860fb319cf98c9f26456fd5b9d078d0cfef88997392334be0c1c6b10c" }
        },
        new()
        {
            { "id", "168175103" },
            { "first_name", "Miha" },
            { "last_name", null },
            { "username", "MihaZupan" },
            { "photo_url", "https://t.me/i/userpic/320/MihaZupan.jpg" },
            { "auth_date", "1540852698" },
            { "hash", "2093184fdda5535316db9b4d77422f8afd21023b3212d58f12c5da5b84b85fa2" }
        },
    };

    public readonly SortedDictionary<string, string?>[] ValidTests = new SortedDictionary<string, string?>[_testCount];
    public readonly SortedDictionary<string, string?>[] InvalidTests = new SortedDictionary<string, string?>[_testCount];

    public LoginWidgetTestsFixture()
    {
        CurrentTimestamp = ((long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds).ToString(CultureInfo.InvariantCulture);

        using HMACSHA256 hmac = new();
        hmac.Key = SHA256.HashData(Encoding.ASCII.GetBytes(Token));

        FillValidData(hmac);
        FillInvalidData();
    }

    private void FillValidData(HMACSHA256 hmac)
    {
        for (int i = 0; i < _testCount; i++)
        {
            SortedDictionary<string, string?> fields = new()
            {
                { "auth_date", CurrentTimestamp },
                { "id", RandomString() },
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
            SortedDictionary<string, string?> fields = new()
            {
                { "auth_date", CurrentTimestamp },
                { "id", (i % 2) == 0 ? RandomString() : ValidTests[i]["id"] },
                { "hash", (i % 2) == 1 ? RandomString(64) : ValidTests[i]["hash"] },
                { RandomString(), RandomString() }
            };

            InvalidTests[i] = fields;
        }
    }

    private static string RandomString(int length = 10)
    {
        using RandomNumberGenerator rng = RandomNumberGenerator.Create();
        byte[] random = new byte[length];
        rng.GetBytes(random);
        return Convert.ToBase64String(random)[..length];
    }

    private static string ComputeHash(SortedDictionary<string, string?> fields, HMACSHA256 hmac)
    {
        fields.Remove("hash");
        StringBuilder dataStringBuilder = new(256);
        foreach (KeyValuePair<string, string?> field in fields)
        {
            if (!string.IsNullOrEmpty(field.Value))
            {
                dataStringBuilder.Append(field.Key);
                dataStringBuilder.Append('=');
                dataStringBuilder.Append(field.Value);
                dataStringBuilder.Append('\n');
            }
        }
        --dataStringBuilder.Length; // Remove the last \n

        byte[] signature = hmac.ComputeHash(Encoding.UTF8.GetBytes(dataStringBuilder.ToString()));

        return BitConverter.ToString(signature).Replace("-", "").ToLower();
    }
}
