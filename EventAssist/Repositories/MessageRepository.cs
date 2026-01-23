using EventAssist.Contexts;
using EventAssist.Models.Records;
using EventAssist.Repositories.Interfaces;

namespace EventAssist.Repositories
{
    public class MessageRepository(AppDbContext context) : IMessageRepository
    {
        public List<MessageRecord> GetMessages(int chatId, int index, int limit)
        {
            return [.. context.Messages
                .Where(m => m.ChatId == chatId)
                .OrderByDescending(m => m.SentDate)
                .Skip(index * limit)
                .Take(limit)
                .OrderBy(m => m.SentDate)];
        }

        public List<MessageRecord> GetMessages(int chatId)
        {
            return [.. context.Messages
                .Where(m => m.ChatId == chatId)
                .OrderBy(m => m.SentDate)];
        }

        public List<MessageRecord> GetMessagesByIds(List<int> messageIds)
        {
            return [.. context.Messages.Where(m => messageIds.Contains(m.Id))];
        }

        public void AddMessage(MessageRecord record)
        {
            context.Messages.Add(record);
            context.SaveChanges();
        }

        public void UpdateMessages(List<MessageRecord> messages)
        {
            context.Messages.UpdateRange(messages);
            context.SaveChanges();
        }
    }
}
