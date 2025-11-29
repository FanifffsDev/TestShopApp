using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using TestShopApp.Domain.Base;

namespace TestShopApp.Telegram.Utils
{
    public static class TgAuthUtils
    {
        private readonly static string _token = Environment.GetEnvironmentVariable("TOKEN");

        public static (bool, AuthUser?) VerifyInitData(string initDataRaw)
        {
            if (string.IsNullOrEmpty(initDataRaw) || string.IsNullOrEmpty(_token))
                return (false, null);

            var parsedData = ParseQuery(initDataRaw);

            if (!parsedData.TryGetValue("hash", out string? receivedHash) || parsedData.Count < 5)
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
                    var data = JsonSerializer.Deserialize<AuthUser>(parsedData["user"],
                        new JsonSerializerOptions { 
                            PropertyNameCaseInsensitive = true,
                            UnmappedMemberHandling = JsonUnmappedMemberHandling.Skip
                        });

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

        public static string GenerateExtendedInitData(AuthUser user, bool isRegistered, long authDate = 0)
        {
            if (string.IsNullOrEmpty(_token))
                throw new InvalidOperationException("TOKEN environment variable is not set");

            if (authDate == 0)
                authDate = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            var userDataExtended = new Dictionary<string, object>
            {
                { "id", user.Id },
                { "first_name", user.FirstName ?? "" },
                { "last_name", user.LastName ?? "" },
                { "username", user.Username ?? "" },
                { "language_code", user.LanguageCode ?? "en" }
            };

            if (user.IsPremium)
                userDataExtended.Add("is_premium", true);

            var userJson = JsonSerializer.Serialize(userDataExtended, new JsonSerializerOptions
            {
                WriteIndented = false
            });

            var dataDict = new Dictionary<string, string>
            {
                { "auth_date", authDate.ToString() },
                { "is_registered", isRegistered.ToString().ToLower() },
                { "query_id", Guid.NewGuid().ToString() },
                { "user", userJson }
            };

            var dataCheckString = string.Join("\n",
                dataDict
                    .OrderBy(kvp => kvp.Key)
                    .Select(kvp => $"{kvp.Key}={kvp.Value}")
            );

            using var hmacKey = new HMACSHA256(Encoding.UTF8.GetBytes("WebAppData"));
            var secretKey = hmacKey.ComputeHash(Encoding.UTF8.GetBytes(_token));

            using var hmac = new HMACSHA256(secretKey);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dataCheckString));
            var hashHex = BitConverter.ToString(computedHash).Replace("-", "").ToLowerInvariant();

            dataDict.Add("hash", hashHex);

            var queryString = string.Join("&",
                dataDict
                    .OrderBy(kvp => kvp.Key) 
                    .Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}")
            );

            return queryString;
        }

        public static (bool isValid, AuthUser? user, bool isRegistered) VerifyExtendedInitData(string initDataRaw)
        {
            if (string.IsNullOrEmpty(initDataRaw) || string.IsNullOrEmpty(_token))
                return (false, null, false);

            var parsedData = ParseQuery(initDataRaw);

            if (!parsedData.TryGetValue("hash", out string? receivedHash))
                return (false, null, false);

            bool isRegistered = false;
            if (parsedData.TryGetValue("is_registered", out string? isRegisteredStr))
            {
                isRegistered = bool.TryParse(isRegisteredStr, out bool parsedIsRegistered) && parsedIsRegistered;
            }

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

            if (computedHashHex != receivedHash)
            {
                Console.WriteLine($"[VerifyExtendedInitData] Hash mismatch. Expected: {receivedHash}, Got: {computedHashHex}");
                Console.WriteLine($"[VerifyExtendedInitData] DataCheckString: {dataCheckString}");
                return (false, null, false);
            }

            try
            {
                if (!parsedData.TryGetValue("user", out string? userJson))
                    return (false, null, false);

                var user = JsonSerializer.Deserialize<AuthUser>(userJson,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (user == null)
                    return (false, null, false);

                return (true, user, isRegistered);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[VerifyExtendedInitData] Deserialization error: {ex.Message}");
                Console.WriteLine($"[VerifyExtendedInitData] User JSON: {parsedData.GetValueOrDefault("user", "N/A")}");
                return (false, null, false);
            }
        }

        public static string UpdateRegistrationStatus(string initDataRaw, bool isRegistered)
        {
            if (string.IsNullOrEmpty(initDataRaw) || string.IsNullOrEmpty(_token))
                throw new InvalidOperationException("Invalid initData or TOKEN");

            var (isValid, user, _) = VerifyExtendedInitData(initDataRaw);

            if (!isValid || user == null)
            {
                var (isValidStandard, userStandard) = VerifyInitData(initDataRaw);

                if (!isValidStandard || userStandard == null)
                    throw new InvalidOperationException("Invalid initData");

                user = userStandard;
            }

            return GenerateExtendedInitData(user, isRegistered);
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