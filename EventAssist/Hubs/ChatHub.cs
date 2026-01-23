using EventAssist.Models.DTOs;
using EventAssist.Models.Enums;
using EventAssist.Models.Records;
using EventAssist.Repositories.Interfaces;
using EventAssist.Services.Interfaces;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Serilog;

namespace EventAssist.Hubs
{
    [Authorize]
    public class ChatHub(
        IAuthService authService,
        IChatRepository chatRepository,
        IUserRepository userRepository,
        IMessageRepository messageRepository,
        IAiAgentService aiAgentService) : Hub
    {
        public override async Task OnConnectedAsync()
        {
            try
            {
                int userId = authService.GetUserId(Context.User!);
                UserRecord user = userRepository.GetUserById(userId);

                List<ChatRecord> chats = user.Roles.Any(role => role.Name == "HelpdeskAgent")
                    ? chatRepository.GetNonClosedChats()
                    : chatRepository.GetChatsByUserId(userId);

                foreach (ChatRecord chat in chats)
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, $"Chat_{chat.Id}");
                }

                user.IsOnline = true;
                userRepository.UpdateUser(user);

                await Clients.All.SendAsync("UserConnected", user.Id);
                Log.Information("User {UserId} connected to ChatHub", userId);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred in ChatHub OnConnectedAsync");
            }
            
            await base.OnConnectedAsync();
        }

        public async Task JoinChatGroup(int chatId)
        {
            try
            {
                int userId = authService.GetUserId(Context.User!);
                UserRecord user = userRepository.GetUserById(userId);

                ChatRecord chat = chatRepository.GetChatById(chatId);

                if (chat.UserId == userId || user.Roles.Any(role => role.Name == "HelpdeskAgent"))
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, $"Chat_{chatId}");
                    Log.Information("Connection {ConnectionId} joined chat group {ChatId}", Context.ConnectionId, chatId);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while joining chat group {ChatId}", chatId);
            }
        }

        public async Task StartTyping(int chatId, MessageSender sender)
        {
            try
            {
                await Clients.All.SendAsync("StartTyping", chatId, sender);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while starting typing indicator for chat {ChatId}", chatId);
            }
        }

        public async Task StopTyping(int chatId, MessageSender sender)
        {
            try
            {
                await Clients.All.SendAsync("StopTyping", chatId, sender);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while stopping typing indicator for chat {ChatId}", chatId);
            }
        }

        public async Task SendMessage(int chatId, string text)
        {
            try
            {
                int userId = authService.GetUserId(Context.User!);
                ChatRecord chat = chatRepository.GetChatById(chatId, userId);

                MessageSender sender = chat.UserId == userId ? MessageSender.User : MessageSender.CustomerSupportAgent;
                MessageRecord message = new()
                {
                    Text = text,
                    Type = MessageType.Text,
                    Sender = sender,
                    SentDate = DateTime.UtcNow,
                    ChatId = chatId
                };
                
                messageRepository.AddMessage(message);
                await Clients.Group($"Chat_{chatId}").SendAsync("ReceiveMessage", chatId, message.Adapt<MessageResponse>());

                Log.Information("Message sent in chat {ChatId} by {Sender}", chatId, sender);

                if (sender == MessageSender.User && chat.Status == ChatStatus.Opened)
                {
                    await aiAgentService.RespondUserMessage(chatId, userId);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while sending message in chat {ChatId}", chatId);
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            try
            {
                int userId = authService.GetUserId(Context.User!);
                UserRecord user = userRepository.GetUserById(userId);

                List<ChatRecord> chats = user.Roles.Any(role => role.Name == "HelpdeskAgent")
                    ? chatRepository.GetNonClosedChats()
                    : chatRepository.GetChatsByUserId(userId);

                foreach (ChatRecord chat in chats)
                {
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Chat_{chat.Id}");
                }

                user.IsOnline = false;
                userRepository.UpdateUser(user);

                await Clients.All.SendAsync("UserDisconnected", user.Id);
                Log.Information("User {UserId} disconnected from ChatHub", userId);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred in ChatHub OnDisconnectedAsync");
            }
            
            await base.OnDisconnectedAsync(exception);
        }
    }
}
