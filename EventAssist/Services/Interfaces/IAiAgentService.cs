namespace EventAssist.Services.Interfaces
{
    public interface IAiAgentService
    {
        Task RespondUserMessage(int chatId, int userId);
    }
}
