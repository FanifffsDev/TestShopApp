using System.Security.Cryptography;
using System.Text;

namespace TestShopApp.Telegram.Utils
{
    public static class TgAuthUtils
    {
        private readonly static string _token = "8344409652:AAHcBXyAMfqsZxZ3M5tSqlIKjKw6IEfLJ8g";
        public static bool VerifyInitData(string initDataRaw)
        {
            if (string.IsNullOrEmpty(initDataRaw) || string.IsNullOrEmpty(_token))
                return false;

            var parsedData = ParseQuery(initDataRaw);
            if (!parsedData.TryGetValue("hash", out string? receivedHash))
                return false;

            parsedData.Remove("hash");

            var dataCheckString = string.Join("\n",
                parsedData
                    .OrderBy(kvp => kvp.Key)
                    .Select(kvp => $"{kvp.Key}={kvp.Value}")
            );

            using var hmacKey = new HMACSHA256(Encoding.UTF8.GetBytes("WebAppData"));
            var secretKey = hmacKey.ComputeHash(Encoding.UTF8.GetBytes(_token));

            using var hmac = new HMACSHA256(secretKey);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dataCheckString));
            var computedHashHex = BitConverter.ToString(computedHash).Replace("-", "").ToLowerInvariant();

            return computedHashHex == receivedHash;
        }

        private static Dictionary<string, string> ParseQuery(string query)
        {
            return query
                .Split('&', StringSplitOptions.RemoveEmptyEntries)
                .Select(part => part.Split('=', 2))
                .Where(p => p.Length == 2)
                .ToDictionary(p => p[0], p => Uri.UnescapeDataString(p[1]));
        }
    }
}
