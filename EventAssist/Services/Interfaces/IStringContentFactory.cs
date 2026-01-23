namespace EventAssist.Services.Interfaces
{
    public interface IStringContentFactory
    {
        StringContent CreateMailerSendRequest(string to, string subject, string html);
    }
}
