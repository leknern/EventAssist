using EventAssist.Contexts;
using EventAssist.Models.Enums;
using EventAssist.Models.Records;
using EventAssist.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EventAssist.Repositories
{
    public class ChatRepository(AppDbContext context) : IChatRepository
    {
        public ChatRecord? TryGetChatByUserId(int userId)
        {
            return context.Chats
                .Include(chat => chat.User)
                .Include(chat => chat.CustomerSupportAgent)
                .Include(c => c.Messages!
                    .OrderByDescending(m => m.SentDate)
                    .Take(1))
                .OrderByDescending(chat => chat.Created)
                .FirstOrDefault(chat => chat.UserId == userId);
        }

        public ChatRecord GetChatById(int chatId, int userId)
        {
            return context.Chats
                .Include(chat => chat.User)
                .Include(chat => chat.CustomerSupportAgent)
                .Include(c => c.Messages!
                    .OrderByDescending(m => m.SentDate)
                    .Take(1))
                .First(chat => chat.Id == chatId && (chat.UserId == userId || chat.CustomerSupportAgentId == userId));
        }

        public ChatRecord GetChatById(int chatId)
        {
            return context.Chats
                .Include(chat => chat.User)
                .Include(chat => chat.CustomerSupportAgent)
                .Include(c => c.Messages!
                    .OrderByDescending(m => m.SentDate)
                    .Take(1))
                .First(chat => chat.Id == chatId);
        }

        public List<ChatRecord> GetChats()
        {
            return [.. context.Chats
                .Include(chat => chat.User)
                .Include(chat => chat.CustomerSupportAgent)
                .Include(c => c.Messages!
                    .OrderByDescending(m => m.SentDate)
                    .Take(1))];
        }

        public void AddChat(ChatRecord chatRecord)
        {
            context.Chats.Add(chatRecord);
            context.SaveChanges();
        }

        public void UpdateChat(ChatRecord record)
        {
            context.Chats.Update(record);
            context.SaveChanges();
        }

        public List<ChatRecord> GetNonClosedChats()
        {
            return [.. context.Chats.Where(chat => chat.Status != ChatStatus.Closed)];
        }

        public List<ChatRecord> GetChatsByUserId(int userId)
        {
            return [.. context.Chats.Where(chat => chat.UserId == userId)];
        }
    }
}
