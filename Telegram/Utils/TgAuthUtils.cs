using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using TestShopApp.Common.Data;

namespace TestShopApp.Telegram.Utils
{
    public static class TgAuthUtils
    {
        private readonly static string _token = Environment.GetEnvironmentVariable("TOKEN");
        public static (bool, TgUser?) VerifyInitData(string initDataRaw)
        {
            if (string.IsNullOrEmpty(initDataRaw) || string.IsNullOrEmpty(_token))
                return (false, null);

            var parsedData = ParseQuery(initDataRaw);
            if (!parsedData.TryGetValue("hash", out string? receivedHash) || parsedData.Count != 5)
                return (false, null);

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


            if (computedHashHex == receivedHash)
            {
                try
                {
                    var data = JsonSerializer.Deserialize<TgUser>(parsedData["user"],
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (data == null)
                        return (false, null);

                    return (true, data);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[VerifiedTgData] Deserialization error: {ex.Message}");
                    return (false, null);
                }
            }
            return (false, null);
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
