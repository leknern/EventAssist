using EventAssist.Models.Records;

namespace EventAssist.Repositories.Interfaces
{
    public interface IMessageRepository
    {
        List<MessageRecord> GetMessages(int chatId, int index, int limit);
        List<MessageRecord> GetMessages(int chatId);
        List<MessageRecord> GetMessagesByIds(List<int> messageIds);
        void UpdateMessages(List<MessageRecord> messages);
        void AddMessage(MessageRecord record);
    }
}
