using Azure.Core;
using EventAssist.Models.DTOs;
using EventAssist.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace EventAssist.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        [HttpPost]
        public IActionResult Register(RegisterRequest request)
        {
            try
            {
                authService.Register(request);
                Log.Information("User {Email} registered successfully.", request.Email);

                return Ok();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while registering user {Email}.", request.Email);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> ForgotPasswordAsync(string email)
        {
            try
            {
                await authService.ForgotPasswordAsync(email);
                Log.Information("Password reset link sent to {Email}.", email);
                return Ok();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while sending password reset link to {Email}.", email);
                return BadRequest(ex.Message);
            }
        }


        [HttpPost]
        public IActionResult ResetPassword(ResetPasswordRequest request)
        {
            try
            {
                authService.ResetPassword(request);
                Log.Information("Password reset successfully for token.");
                return Ok();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while resetting password.");
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Authorize(Policy = "RequireFullToken")]
        public IActionResult ChangePassword(ChangePasswordRequest request)
        {
            try
            {
                int userId = authService.GetUserId(User);
                authService.ChangePassword(userId, request);
                Log.Information("User {UserId} changed password successfully.", userId);
                return Ok();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while changing password.");
                return BadRequest(ex.Message);
            }
        } 

        [HttpPost]
        public IActionResult Login(LoginRequest request)
        {
            try
            {
                string response = authService.Login(request);
                Log.Information("User {Email} logged in successfully.", request.Email);
                return Ok(response);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while logging in user {Email}.", request.Email);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Authorize]
        public IActionResult TwoFactorLogin(string twoFactorAuthCode)
        {
            try
            {
                int userId = authService.GetUserId(User);
                string response = authService.TwoFactorLogin(userId, twoFactorAuthCode);
                Log.Information("User {UserId} completed two-factor login.", userId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred during two-factor login.");
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Authorize(Policy = "RequireFullToken")]
        public IActionResult GetTwoFactorQrCode()
        {
            try
            {
                int userId = authService.GetUserId(User);
                TwoFactorQrCodeResponse twoFactorQrCode = authService.GetTwoFactorQrCode(userId);
                Log.Information("User {UserId} retrieved two-factor QR code.", userId);
                return Ok(twoFactorQrCode);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while getting two-factor QR code.");
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Authorize(Policy = "RequireFullToken")]
        public IActionResult TurnOnTwoFactorAuth(string twoFactorAuthCode)
        {
            try
            {
                int userId = authService.GetUserId(User);
                authService.TurnOnTwoFactorAuth(userId, twoFactorAuthCode);
                Log.Information("User {UserId} turned on two-factor authentication.", userId);
                return Ok();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while turning on two-factor authentication.");
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Authorize(Policy = "RequireFullToken")]
        public IActionResult TurnOffTwoFactorAuth()
        {
            try
            {
                int userId = authService.GetUserId(User);
                authService.TurnOffTwoFactorAuth(userId);
                Log.Information("User {UserId} turned off two-factor authentication.", userId);
                return Ok();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while turning off two-factor authentication.");
                return BadRequest(ex.Message);
            }
        }
    }
}
