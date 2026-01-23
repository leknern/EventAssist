using EventAssist.Models.Records;
using EventAssist.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EventAssist.Services
{
    public class TokenProviderService(IConfiguration configuration) : ITokenProviderService
    {
        public string GetTempToken(UserRecord user)
        {
            SigningCredentials signingCredentials = GetSigningCredentials();
            int expiresInMinutes = int.Parse(configuration["JsonWebToken:ShortExpiresInMinutes"]!);

            SecurityTokenDescriptor securityTokenDescriptor = new()
            {
                Subject = new ClaimsIdentity(
                [
                    new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new("isTempToken", "True"),
                ]),
                Expires = DateTime.UtcNow.AddMinutes(expiresInMinutes),
                SigningCredentials = signingCredentials,
                Issuer = configuration["JsonWebToken:Issuer"],
                Audience = configuration["JsonWebToken:Audience"]
            };

            SecurityToken token = new JwtSecurityTokenHandler()
                .CreateToken(securityTokenDescriptor);

            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }

        public string GetToken(UserRecord user)
        {
            SigningCredentials signingCredentials = GetSigningCredentials();
            int expiresInMinutes = int.Parse(configuration["JsonWebToken:ExpiresInMinutes"]!);

            List<Claim> roleClaims = [.. user.Roles.Select(role => new Claim(ClaimTypes.Role, role.Name))];

            SecurityTokenDescriptor securityTokenDescriptor = new()
            {
                Subject = new ClaimsIdentity(
                [
                    new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new(ClaimTypes.Name, user.Name),
                    new(ClaimTypes.Email, user.Email),
                    new("profilePictureUrl", user.ProfilePictureUrl),
                    ..roleClaims,
                    new("isTwoFactorAuthEnabled", user.IsTwoFactorAuthEnabled.ToString()),
                    new("isTempToken", "False"),
                ]),
                Expires = DateTime.UtcNow.AddMinutes(expiresInMinutes),
                SigningCredentials = signingCredentials,
                Issuer = configuration["JsonWebToken:Issuer"],
                Audience = configuration["JsonWebToken:Audience"]
            };

            SecurityToken token = new JwtSecurityTokenHandler()
                .CreateToken(securityTokenDescriptor);

            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }

        private SigningCredentials GetSigningCredentials()
        {
            string secretKey = configuration["JsonWebToken:SecretKey"]!;
            SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(secretKey));
            return new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        }
    }
}
