using EventAssist.Models.DTOs;
using EventAssist.Models.Enums;
using EventAssist.Models.Records;
using EventAssist.Repositories.Interfaces;
using EventAssist.Services.Interfaces;
using Google.Authenticator;
using Mapster;
using System.Security.Claims;

namespace EventAssist.Services
{
    public class AuthService(
        IEmailService emailService,
        IUserRepository userRepository,
        IAuthSecurityService authSecurityService,
        ITokenProviderService tokenProviderService) : IAuthService
    {
        public async Task ForgotPasswordAsync(string email)
        {
            UserRecord? user = userRepository.TryGetUser(email);
            if (user is not null)
            {
                string passwordResetToken = authSecurityService.GetRandomCode(length: 6);
                user.PasswordResetToken = passwordResetToken;
                userRepository.UpdateUser(user);
                await emailService.SendForgotPasswordAsync(email, passwordResetToken);
            }
        }

        public TwoFactorQrCodeResponse GetTwoFactorQrCode(int userId)
        {
            UserRecord user = userRepository.GetUserById(userId);

            if (user.TwoFactorAuthSecret is null)
            {
                user.TwoFactorAuthSecret = authSecurityService.CreateTwoFactorAuthSecret(length: 20);
                userRepository.UpdateUser(user);
            }

            TwoFactorAuthenticator authenticator = new();
            SetupCode setupCode = authenticator.GenerateSetupCode("EventAssist",
                user.Email,
                user.TwoFactorAuthSecret,
                secretIsBase32: true);

            return new()
            {
                QrCodeUrl = setupCode.QrCodeSetupImageUrl,
                InputKey = setupCode.ManualEntryKey
            };
        }

        public string Login(LoginRequest request)
        {
            UserRecord? user = userRepository.TryGetUser(request.Email) 
                ?? throw new Exception("Bad password or email");

            if (user.LockoutEndDate >= DateTime.UtcNow)
            {
                throw new Exception("Locked account");
            }

            if (authSecurityService.ComparePassword(request.Password, user.PasswordHash, user.PasswordSalt))
            {
                user.LoginErrorCount = 0;
                user.LastLogin = DateTime.UtcNow;
                userRepository.UpdateUser(user);

                return user.IsTwoFactorAuthEnabled 
                    ? tokenProviderService.GetTempToken(user) 
                    : tokenProviderService.GetToken(user);
            }

            if (user.LastLoginErrorDate + TimeSpan.FromMinutes(10) < DateTime.UtcNow)
            {
                user.LoginErrorCount = 1;
            }
            else
            {
                user.LoginErrorCount++;
            }

            user.LastLoginErrorDate = DateTime.UtcNow;
            
            if (user.LoginErrorCount >= 5)
            {
                user.LockoutEndDate = DateTime.UtcNow + TimeSpan.FromMinutes(15);
                user.LoginErrorCount = 0;
            }

            userRepository.UpdateUser(user);

            throw new Exception("Bad password or email");
        }

        public void Register(RegisterRequest request)
        {
            if (userRepository.TryGetUser(request.Email) is not null)
            {
                throw new Exception("User already exists");
            }

            authSecurityService.CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            UserRecord user = request.Adapt<UserRecord>();
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            user.CreatedDate = DateTime.UtcNow;

            userRepository.AddUser(user);
        }

        public void ResetPassword(ResetPasswordRequest request)
        {
            UserRecord user = userRepository.GetUserByPwdResetToken(request.PasswordResetToken);

            if (user.PasswordResetToken != request.PasswordResetToken)
            {
                throw new Exception("Invalid password reset token");
            }

            authSecurityService.CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            user.PasswordResetToken = null;
            user.UpdatedDate = DateTime.UtcNow;

            userRepository.UpdateUser(user);
        }

        public void TurnOnTwoFactorAuth(int userId, string twoFactorAuthCode)
        {
            UserRecord user = userRepository.GetUserById(userId);

            TwoFactorAuthenticator authenticator = new();
            if (!authenticator.ValidateTwoFactorPIN(user.TwoFactorAuthSecret, twoFactorAuthCode, secretIsBase32: true))
            {
                throw new Exception("Invalid two-factor code.");
            }

            user.IsTwoFactorAuthEnabled = true;
            userRepository.UpdateUser(user);
        }

        public void TurnOffTwoFactorAuth(int userId)
        {
            UserRecord user = userRepository.GetUserById(userId);
            user.IsTwoFactorAuthEnabled = false;

            userRepository.UpdateUser(user);
        }

        public string TwoFactorLogin(int userId, string twoFactorAuthCode)
        {
            UserRecord user = userRepository.GetUserById(userId);
            TwoFactorAuthenticator authenticator = new();

            if (!authenticator.ValidateTwoFactorPIN(user.TwoFactorAuthSecret, twoFactorAuthCode, secretIsBase32: true))
            {
                throw new Exception("Invalid two-factor code.");
            }

            return tokenProviderService.GetToken(user);
        }

        public void ChangePassword(int userId, ChangePasswordRequest request)
        {
            UserRecord user = userRepository.GetUserById(userId);

            if (!authSecurityService.ComparePassword(request.Password, user.PasswordHash, user.PasswordSalt))
            {
                throw new Exception("Bad password.");
            }

            authSecurityService.CreatePasswordHash(request.NewPassword, out byte[] passwordHash, out byte[] passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            userRepository.UpdateUser(user);
        }
        
        public int GetUserId(ClaimsPrincipal claims)
        {
            string? userId = claims.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            return userId is null ? throw new Exception("User ID not found in claims.") : int.Parse(userId);
        }
    }
}
