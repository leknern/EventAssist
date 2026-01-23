namespace EventAssist.Services.Interfaces
{
    public interface IAuthSecurityService
    {
        void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt);
        bool ComparePassword(string password, byte[] passwordHash, byte[] passwordSalt);
        string GetRandomCode(int length);
        string CreateTwoFactorAuthSecret(int length);
    }
}
