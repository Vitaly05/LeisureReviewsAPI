using LeisureReviewsAPI.Models;
using LeisureReviewsAPI.Models.Database;
using LeisureReviewsAPI.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace LeisureReviewsAPI.Services
{
    public class JwtHelper : IJwtHelper
    {
        private readonly string key;

        private readonly string issuer;

        private readonly string audience;

        private readonly UserManager<User> userManager;

        public JwtHelper(IConfiguration configuration, UserManager<User> userManager)
        {
            key = configuration["Jwt:Key"];
            issuer = configuration["Jwt:Issuer"];
            audience = configuration["Jwt:Audience"];
            this.userManager = userManager;
        }

        public async Task<string> GenerateTokenAsync(User user)
        {
            var claims = await getClaimsAsync(user);
            return new JwtSecurityTokenHandler().WriteToken(getToken(claims));
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        public ClaimsPrincipal GetPrincipal(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = issuer,
                ValidateAudience = true,
                ValidAudience = audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken is null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512Signature, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");
            return principal;
        }

        private JwtSecurityToken getToken(IEnumerable<Claim> claims) =>
            new JwtSecurityToken(issuer: issuer, audience: audience, claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)), SecurityAlgorithms.HmacSha512Signature));

        private async Task<IEnumerable<Claim>> getClaimsAsync(User user) =>
            new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Role, string.Join(',', await userManager.GetRolesAsync(user)))
            };
    }
}
