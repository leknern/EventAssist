using EventAssist.Models.Records;

namespace EventAssist.Repositories.Interfaces
{
    public interface IChatRepository
    {
        ChatRecord? TryGetChatByUserId(int userId);
        ChatRecord GetChatById(int chatId, int userId);
        ChatRecord GetChatById(int chatId);
        List<ChatRecord> GetChats();
        List<ChatRecord> GetNonClosedChats();
        List<ChatRecord> GetChatsByUserId(int userId);
        void AddChat(ChatRecord record);
        void UpdateChat(ChatRecord record);
    }
}
