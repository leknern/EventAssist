using EventAssist.Models.DTOs;
using EventAssist.Models.Enums;
using Google.GenAI.Types;

namespace EventAssist.Services.Interfaces
{
    public interface IMessageService
    {
        List<MessageResponse> GetMessages(int chatId, int index, int limit);
        List<MessageResponse> GetMessages(int chatId);
        List<Content> GetContents(int chatId);
        Task SendAiAgentMessageAsync(Part messagePart, MessageType type, int chatId);
    }
}
