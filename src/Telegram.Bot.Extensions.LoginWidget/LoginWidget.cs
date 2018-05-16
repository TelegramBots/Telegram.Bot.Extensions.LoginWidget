using System;
using System.Text;
using System.Collections.Generic;
using System.Security.Cryptography;

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
            if (token == null) throw new ArgumentNullException("token");

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
            if (_disposed) throw new ObjectDisposedException("LoginWidget");
            if (fields == null) throw new ArgumentNullException("fields");
            if (fields.Count < 6) return Authorization.MissingFields;

            if (!fields.ContainsKey("auth_date")) return Authorization.MissingFields;
            if (!fields.ContainsKey("first_name")) return Authorization.MissingFields;
            if (!fields.ContainsKey("id")) return Authorization.MissingFields;
            if (!fields.ContainsKey("photo_url")) return Authorization.MissingFields;
            if (!fields.ContainsKey("username")) return Authorization.MissingFields;
            if (!fields.ContainsKey("hash")) return Authorization.MissingFields;

            if (long.TryParse(fields["auth_date"], out long timestamp))
            {
                if (Math.Abs(DateTime.UtcNow.Subtract(_unixStart).TotalSeconds - timestamp) > AllowedTimeOffset)
                    return Authorization.TooOld;
            }
            else return Authorization.InvalidAuthDateFormat;

            string hash = fields["hash"];
            if (hash.Length != 64) return Authorization.InvalidHash;

            string data_check_string =
                "auth_date="    + fields["auth_date"]   + '\n' +
                "first_name="   + fields["first_name"]  + '\n' +
                "id="           + fields["id"]          + '\n' +
                "photo_url="    + fields["photo_url"]   + '\n' +
                "username="     + fields["username"];

            byte[] signature = _hmac.ComputeHash(Encoding.UTF8.GetBytes(data_check_string));

            // Taken from: bool MihaZupan.Algorithms.Hex.ByteArrayMatchesHexString_Lowercase(byte[] bytes, string hex)
            // Addapted from: https://stackoverflow.com/a/14333437/6845657
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
        public Authorization CheckAuthorization(IEnumerable<KeyValuePair<string, string>> fields)
        {
            Dictionary<string, string> fieldsDictionary = new Dictionary<string, string>(6);
            foreach (KeyValuePair<string, string> field in fields)
            {
                fieldsDictionary.Add(field.Key, field.Value);
            }
            return CheckAuthorization(fieldsDictionary);
        }

        /// <summary>
        /// Checks whether the authorization data received from the user is valid
        /// </summary>
        /// <param name="fields">A collection containing query string fields as key-value pairs</param>
        /// <returns></returns>
        public Authorization CheckAuthorization(IEnumerable<Tuple<string, string>> fields)
        {
            Dictionary<string, string> fieldsDictionary = new Dictionary<string, string>(6);
            foreach (Tuple<string, string> field in fields)
            {
                fieldsDictionary.Add(field.Item1, field.Item2);
            }
            return CheckAuthorization(fieldsDictionary);
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                _hmac?.Dispose();
            }
        }
    }
}
