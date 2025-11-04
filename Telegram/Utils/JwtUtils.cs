using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace TestShopApp.Telegram.Utils
{
    public static class JwtUtils
    {
        private static readonly string _secret = "L4x!fdZ7u9vQm@#Wc&4x!Ea*HgK2r!fdf$1pb$1L&4x!Ea*HZ7u9vQm@#Wc&4x!f$L4x!fdZ7u9Ea*HgK2rX7uNNf$pWc&EadZ7uNNgK2rXdZ7uNNvQm@#Wc&XdZ*HgKL4x4x!Ea*HgK2rXdZ9vQm@#2rXb1pb$1L9vQm@#pWc&Ea*HgK2rXb";
        public static string GenerateJwtToken(Dictionary<string, string> payload)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>();
            foreach (var pair in payload)
            {
                claims.Add(new Claim(pair.Key, pair.Value));
            }

            var token = new JwtSecurityToken(
                issuer: "https://localhost:7040",
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(15),
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public static Dictionary<string, string> ValidateAndReadPayload(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = "https://localhost:7040",

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret)),

                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

                var jwtToken = validatedToken as JwtSecurityToken;

                var result = new Dictionary<string, string>();
                foreach (var claim in jwtToken.Claims)
                {
                    result[claim.Type] = claim.Value;
                }

                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
