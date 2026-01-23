using EventAssist.Services.Interfaces;
using Google.Authenticator;
using System.Security.Cryptography;
using System.Text;

namespace EventAssist.Services
{
    public class AuthSecurityService : IAuthSecurityService
    {
        public bool ComparePassword(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using HMACSHA512 hmac = new(passwordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(passwordHash);
        }

        public void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using HMACSHA512 hmac = new();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }

        public string CreateTwoFactorAuthSecret(int length)
        {
            byte[] bytes = new byte[length];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }

            return Base32Encoding.ToString(bytes);
        }

        public string GetRandomCode(int length)
        {
            string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            byte[] randomNumbers = RandomNumberGenerator.GetBytes(length);

            string code = string.Empty;
            for (int index = 0; index < length; index++)
            {
                code += characters[randomNumbers[index] % characters.Length];
            }
            return code;
        }
    }
}
