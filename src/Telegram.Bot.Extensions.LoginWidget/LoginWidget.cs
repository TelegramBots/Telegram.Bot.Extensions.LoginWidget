using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Telegram.Bot.Extensions.TelegramLogin;

/// <summary>
/// A helper class used to verify authorization data
/// </summary>
public class LoginWidget : IDisposable
{
    /// <summary>
    /// How old (in seconds) can authorization attempts be to be considered valid (compared to the auth_date field)
    /// </summary>
    public long AllowedTimeOffset { get; set; } = 30;

    private bool _disposed = false;

    private readonly HMACSHA256 _hmac;
#if NET6_0_OR_GREATER
    private static readonly DateTime _unixStart = DateTime.UnixEpoch;
#else
    private static readonly DateTime _unixStart = new(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
#endif

    /// <summary>
    /// Construct a new <see cref="LoginWidget"/> instance
    /// </summary>
    /// <param name="token">The bot API token used as a secret parameter when checking authorization</param>
    public LoginWidget(string token)
    {
        if (token == null) throw new ArgumentNullException(nameof(token));

#if NET6_0_OR_GREATER
        _hmac = new HMACSHA256(SHA256.HashData(Encoding.ASCII.GetBytes(token)));
#else
        using SHA256 sha256 = SHA256.Create();
        _hmac = new HMACSHA256(sha256.ComputeHash(Encoding.ASCII.GetBytes(token)));
#endif
    }

    /// <summary>
    /// Checks whether the authorization data received from the user is valid
    /// </summary>
    /// <param name="fields">A collection containing query string fields as sorted key-value pairs</param>
    /// <returns></returns>
    public Authorization CheckAuthorization(SortedDictionary<string, string?> fields)
    {
        if (_disposed) throw new ObjectDisposedException(nameof(LoginWidget));
        if (fields == null) throw new ArgumentNullException(nameof(fields));
        if (fields.Count < 3) return Authorization.MissingFields;

        if (!fields.ContainsKey(Field.Id) ||
            !fields.TryGetValue(Field.AuthDate, out string? authDate) ||
            !fields.TryGetValue(Field.Hash, out string? hash)
        )
        {
            return Authorization.MissingFields;
        }

        if (hash?.Length != 64) return Authorization.InvalidHash;

        if (!long.TryParse(
            s: authDate,
            style: NumberStyles.Integer,
            provider: CultureInfo.InvariantCulture,
            result: out long timestamp))
        {
            return Authorization.InvalidAuthDateFormat;
        }

        if (Math.Abs(DateTime.UtcNow.Subtract(_unixStart).TotalSeconds - timestamp) > AllowedTimeOffset)
            return Authorization.TooOld;

        fields.Remove(Field.Hash);
        StringBuilder dataStringBuilder = new(256);
        foreach (var field in fields)
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

        byte[] signature = _hmac.ComputeHash(Encoding.UTF8.GetBytes(dataStringBuilder.ToString()));

        // Adapted from: https://stackoverflow.com/a/14333437/6845657
        for (int i = 0; i < signature.Length; i++)
        {
            if (hash[i * 2] != 87 + (signature[i] >> 4) + ((((signature[i] >> 4) - 10) >> 31) & -39)) return Authorization.InvalidHash;
            if (hash[(i * 2) + 1] != 87 + (signature[i] & 0xF) + ((((signature[i] & 0xF) - 10) >> 31) & -39)) return Authorization.InvalidHash;
        }

        return Authorization.Valid;
    }

    /// <summary>
    /// Checks whether the authorization data received from the user is valid
    /// </summary>
    /// <param name="fields">A collection containing query string fields as key-value pairs</param>
    /// <returns></returns>
    public Authorization CheckAuthorization(IDictionary<string, string?> fields)
    {
        if (fields == null) throw new ArgumentNullException(nameof(fields));
        return CheckAuthorization(new SortedDictionary<string, string?>(fields, StringComparer.Ordinal));
    }

    /// <summary>
    /// Checks whether the authorization data received from the user is valid
    /// </summary>
    /// <param name="fields">A collection containing query string fields as key-value pairs</param>
    /// <returns></returns>
    public Authorization CheckAuthorization(IEnumerable<KeyValuePair<string, string?>> fields) =>
        CheckAuthorization(fields.ToDictionary(f => f.Key, f => f.Value, StringComparer.Ordinal));

    /// <summary>
    /// Checks whether the authorization data received from the user is valid
    /// </summary>
    /// <param name="fields">A collection containing query string fields as key-value pairs</param>
    /// <returns></returns>
    public Authorization CheckAuthorization(IEnumerable<Tuple<string, string?>> fields) =>
        CheckAuthorization(fields.ToDictionary(f => f.Item1, f => f.Item2, StringComparer.Ordinal));

    /// <inheritdoc/>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _hmac?.Dispose();
            }

            _disposed = true;
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    private static class Field
    {
        public const string AuthDate = "auth_date";
        public const string Id = "id";
        public const string Hash = "hash";
    }
}
