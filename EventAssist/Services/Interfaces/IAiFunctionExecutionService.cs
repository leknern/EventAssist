using Google.GenAI.Types;

namespace EventAssist.Services.Interfaces
{
    public interface IAiFunctionExecutionService
    {
        Task<Content> ProcessFunctionCall(Part messagePart, int chatId, int userId);
    }
}
