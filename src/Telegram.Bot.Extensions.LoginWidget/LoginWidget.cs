using System;
using System.Text;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Linq;

namespace Telegram.Bot.Extensions.LoginWidget
{
    /// <summary>
    /// A helper class used to verify authorization data
    /// </summary>
    public class LoginWidget : IDisposable
    {
        /// <summary>
        /// How old (in seconds) can authorization attempts be to be considered valid (compared to the auth_date field)
        /// </summary>
        public long AllowedTimeOffset = 30;

        private bool _disposed = false;
        private readonly HMACSHA256 _hmac;
        private static readonly DateTime _unixStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// Construct a new <see cref="LoginWidget"/> instance
        /// </summary>
        /// <param name="token">The bot API token used as a secret parameter when checking authorization</param>
        public LoginWidget(string token)
        {
            if (token == null) throw new ArgumentNullException(nameof(token));

            using (SHA256 sha256 = SHA256.Create())
            {
                _hmac = new HMACSHA256(sha256.ComputeHash(Encoding.ASCII.GetBytes(token)));
            }
        }

        /// <summary>
        /// Checks whether the authorization data received from the user is valid
        /// </summary>
        /// <param name="fields">A collection containing query string fields as key-value pairs</param>
        /// <returns></returns>
        public Authorization CheckAuthorization(Dictionary<string, string> fields)
        {
            if (_disposed) throw new ObjectDisposedException(nameof(LoginWidget));
            if (fields == null) throw new ArgumentNullException(nameof(fields));
            if (fields.Count < 6) return Authorization.MissingFields;

            if (!fields.ContainsKey(Field.AuthDate) ||
                !fields.ContainsKey(Field.FirstName) ||
                !fields.ContainsKey(Field.Id) ||
                !fields.ContainsKey(Field.PhotoUrl) ||
                !fields.ContainsKey(Field.Username) ||
                !fields.ContainsKey(Field.Hash)
            ) return Authorization.MissingFields;

            if (fields[Field.Hash].Length != 64) return Authorization.InvalidHash;

            if (!long.TryParse(fields[Field.AuthDate], out long timestamp))
                return Authorization.InvalidAuthDateFormat;

            if (Math.Abs(DateTime.UtcNow.Subtract(_unixStart).TotalSeconds - timestamp) > AllowedTimeOffset)
                return Authorization.TooOld;

            string data_check_string =
                Field.AuthDate  + "=" + fields[Field.AuthDate]    + '\n' +
                Field.FirstName + "=" + fields[Field.FirstName]   + '\n' +
                Field.Id        + "=" + fields[Field.Id]          + '\n' +
                (fields.ContainsKey(Field.LastName) ? (Field.LastName + "=" + fields[Field.LastName] + '\n') : "") +
                Field.PhotoUrl  + "=" + fields[Field.PhotoUrl]    + '\n' +
                Field.Username  + "=" + fields[Field.Username];

            byte[] signature = _hmac.ComputeHash(Encoding.UTF8.GetBytes(data_check_string));
            string hash = fields[Field.Hash];

            // Taken from: bool MihaZupan.Algorithms.Hex.ByteArrayMatchesHexString_Lowercase(byte[] bytes, string hex)
            // Adapted from: https://stackoverflow.com/a/14333437/6845657
            for (int i = 0; i < signature.Length; i++)
            {
                if (hash[i * 2] != 87 + (signature[i] >> 4) + ((((signature[i] >> 4) - 10) >> 31) & -39)) return Authorization.InvalidHash;
                if (hash[i * 2 + 1] != 87 + (signature[i] & 0xF) + ((((signature[i] & 0xF) - 10) >> 31) & -39)) return Authorization.InvalidHash;
            }

            // Alternative method (14x slower):
            // if (BitConverter.ToString(signature).Replace("-", "").ToLower() != hash) return Authorization.InvalidHash;
            return Authorization.Valid;
        }

        /// <summary>
        /// Checks whether the authorization data received from the user is valid
        /// </summary>
        /// <param name="fields">A collection containing query string fields as key-value pairs</param>
        /// <returns></returns>
        public Authorization CheckAuthorization(IEnumerable<KeyValuePair<string, string>> fields) =>
            CheckAuthorization(fields.ToDictionary(f => f.Key, f => f.Value));

        /// <summary>
        /// Checks whether the authorization data received from the user is valid
        /// </summary>
        /// <param name="fields">A collection containing query string fields as key-value pairs</param>
        /// <returns></returns>
        public Authorization CheckAuthorization(IEnumerable<Tuple<string, string>> fields) =>
            CheckAuthorization(fields.ToDictionary(f => f.Item1, f => f.Item2));

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                _hmac?.Dispose();
            }
        }

        private static class Field
        {
            public const string AuthDate = "auth_date";
            public const string FirstName = "first_name";
            public const string LastName = "last_name";
            public const string Id = "id";
            public const string PhotoUrl = "photo_url";
            public const string Username = "username";
            public const string Hash = "hash";
        }
    }
}
