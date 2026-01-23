using EventAssist.Models.DTOs;

namespace EventAssist.Services.Interfaces
{
    public interface IChatService
    {
        ChatResponse? TryGetChat(int userId);
        List<ChatResponse> GetChats();
        Task<ChatResponse> AddChatAsync(int userId);
        Task TakeOverChatAsync(int chatId, int userId);
        Task RequestHumanSupportAsync(int chatId, int userId);
        Task UpdateCustomerSupportCommentAsync(CustomerSupportCommentRequest request, int userId);
        Task CloseChatAsync(int chatId, int userId);
    }
}
