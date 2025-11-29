using System.Security.Cryptography;
using System.Text;

namespace TestShopApp.App.Utils
{
    public static class GroupLinkGenerator
    {
        private static readonly string _secretKey = Environment.GetEnvironmentVariable("SECRET_KEY")
            ?? throw new InvalidOperationException("SECRET_KEY not found");

        public static string GenerateInviteCode(long ownerId, string groupNumber)
        {
            try
            {
                var data = $"{ownerId}_{groupNumber}";
                var encrypted = EncryptAES256(data);

                return Convert.ToBase64String(encrypted)
                    .Replace("+", "-")
                    .Replace("/", "_")
                    .Replace("=", "");
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to generate invite code", ex);
            }
        }

        public static (long ownerId, string? groupNumber) DecryptInviteCode(string inviteCode)
        {
            try
            {
                var base64 = inviteCode
                    .Replace("-", "+")
                    .Replace("_", "/");

                var padding = (4 - base64.Length % 4) % 4;
                base64 += new string('=', padding);

                var encryptedBytes = Convert.FromBase64String(base64);
                var decrypted = DecryptAES256(encryptedBytes);

                var parts = decrypted.Split('_');

                if (parts.Length != 2 || !long.TryParse(parts[0], out long ownerId))
                {
                    throw new FormatException("Invalid invite code format");
                }

                return (ownerId, parts[1]);
            }
            catch (Exception ex)
            {
                return (0, null);
            }
        }

        private static byte[] EncryptAES256(string plainText)
        {
            using var aes = Aes.Create();
            aes.KeySize = 256;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using var sha256 = SHA256.Create();
            aes.Key = sha256.ComputeHash(Encoding.UTF8.GetBytes(_secretKey));

            aes.GenerateIV();

            using var encryptor = aes.CreateEncryptor();
            var plainBytes = Encoding.UTF8.GetBytes(plainText);
            var encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

            var result = new byte[aes.IV.Length + encryptedBytes.Length];
            Buffer.BlockCopy(aes.IV, 0, result, 0, aes.IV.Length);
            Buffer.BlockCopy(encryptedBytes, 0, result, aes.IV.Length, encryptedBytes.Length);

            return result;
        }

        private static string DecryptAES256(byte[] encryptedData)
        {
            using var aes = Aes.Create();
            aes.KeySize = 256;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using var sha256 = SHA256.Create();
            aes.Key = sha256.ComputeHash(Encoding.UTF8.GetBytes(_secretKey));

            var iv = new byte[16];
            Buffer.BlockCopy(encryptedData, 0, iv, 0, 16);
            aes.IV = iv;

            var encryptedBytes = new byte[encryptedData.Length - 16];
            Buffer.BlockCopy(encryptedData, 16, encryptedBytes, 0, encryptedBytes.Length);

            using var decryptor = aes.CreateDecryptor();
            var decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);

            return Encoding.UTF8.GetString(decryptedBytes);
        }

        public static string GenerateInviteLink(long ownerId, string groupNumber, string botUsername, string appName = "testapp")
        {
            var code = GenerateInviteCode(ownerId, groupNumber);

            botUsername = botUsername.TrimStart('@');

            if (!string.IsNullOrEmpty(appName))
            {
                return $"https://t.me/{botUsername}/{appName}?startapp=invite_{code}";
            }
            else
            {
                return $"https://t.me/{botUsername}?start=invite_{code}";
            }
        }
    }
}
