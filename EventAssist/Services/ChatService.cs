using EventAssist.Hubs;
using EventAssist.Models.DTOs;
using EventAssist.Models.Enums;
using EventAssist.Models.Records;
using EventAssist.Repositories.Interfaces;
using EventAssist.Services.Interfaces;
using Mapster;
using Microsoft.AspNetCore.SignalR;

namespace EventAssist.Services
{
    public class ChatService(
        IChatRepository chatRepository,
        IUserRepository userRepository,
        IHubContext<ChatHub> chatHubContext) : IChatService
    {
        public ChatResponse? TryGetChat(int userId)
        {
            ChatRecord? chat = chatRepository.TryGetChatByUserId(userId);
            
            if (chat is not null)
            {
                return chat.Adapt<ChatResponse>();
            }

            return null;
        }

        public List<ChatResponse> GetChats()
        {
            return chatRepository.GetChats().Adapt<List<ChatResponse>>();
        }

        public async Task<ChatResponse> AddChatAsync(int userId)
        {
            UserRecord user = userRepository.GetUserById(userId);

            MessageRecord message = new()
            {
                Text = "Hello! How can I assist you today?",
                Sender = MessageSender.Model,
                SentDate = DateTime.UtcNow,
            };

            ChatRecord record = new()
            {
                Status = ChatStatus.Opened,
                UserId = userId,
                User = user,
                Created = DateTime.UtcNow,
                Messages = [message]
            };

            chatRepository.AddChat(record);
            ChatResponse response = record.Adapt<ChatResponse>();

            await chatHubContext.Clients.All.SendAsync("ChatAdded", response);
            return response;
        }

        public async Task TakeOverChatAsync(int chatId, int userId)
        {
            ChatRecord chat = chatRepository.GetChatById(chatId);
            UserRecord customerSupportOperator = userRepository.GetUserById(userId);

            chat.CustomerSupportAgentId = userId;
            chat.CustomerSupportAgent = customerSupportOperator;
            chat.Status = ChatStatus.OperatorAssigned;

            chatRepository.UpdateChat(chat);

            await chatHubContext.Clients.Group($"Chat_{chat.Id}").SendAsync("ChatUpdated", chat.Adapt<ChatResponse>());
        }

        public async Task RequestHumanSupportAsync(int chatId, int userId)
        {
            ChatRecord chat = chatRepository.GetChatById(chatId, userId);
            chat.HumanSupportRequired = true;
            chatRepository.UpdateChat(chat);

            await chatHubContext.Clients.Group($"Chat_{chat.Id}").SendAsync("ChatUpdated", chat.Adapt<ChatResponse>());
        }

        public async Task UpdateCustomerSupportCommentAsync(CustomerSupportCommentRequest request, int userId)
        {
            ChatRecord chat = chatRepository.GetChatById(request.ChatId, userId);
            chat.InternalNote = request.Comment;
            chatRepository.UpdateChat(chat);

            await chatHubContext.Clients.Group($"Chat_{chat.Id}").SendAsync("ChatUpdated", chat.Adapt<ChatResponse>());
        }

        public async Task CloseChatAsync(int chatId, int userId)
        {
            ChatRecord chat = chatRepository.GetChatById(chatId, userId);
            chat.Status = ChatStatus.Closed;
            chatRepository.UpdateChat(chat);

            await chatHubContext.Clients.Group($"Chat_{chat.Id}").SendAsync("ChatUpdated", chat.Adapt<ChatResponse>());
        }
    }
}
