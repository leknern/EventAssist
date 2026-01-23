namespace EventAssist.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendForgotPasswordAsync(string email, string token);
    }
}
