using EventAssist.Models.DTOs;
using System.Security.Claims;

namespace EventAssist.Services.Interfaces
{
    public interface IAuthService
    {
        void Register(RegisterRequest request);
        string Login(LoginRequest request);
        string TwoFactorLogin(int userId, string twoFactorAuthCode);
        TwoFactorQrCodeResponse GetTwoFactorQrCode(int userId);
        void TurnOnTwoFactorAuth(int userId, string twoFactorAuthCode);
        void TurnOffTwoFactorAuth(int userId);
        Task ForgotPasswordAsync(string email);
        void ResetPassword(ResetPasswordRequest request);
        void ChangePassword(int userId, ChangePasswordRequest request);
        int GetUserId(ClaimsPrincipal claims);
    }
}
