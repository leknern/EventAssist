using EventAssist.Hubs;
using EventAssist.Models.DTOs;
using EventAssist.Models.Enums;
using EventAssist.Models.Records;
using EventAssist.Repositories.Interfaces;
using EventAssist.Services.Interfaces;
using Google.GenAI.Types;
using Mapster;
using Microsoft.AspNetCore.SignalR;
using System.Text.Json;

namespace EventAssist.Services
{
    public class MessageService(IMessageRepository messageRepository, IHubContext<ChatHub> chatHubContext) : IMessageService
    {
        public List<Content> GetContents(int chatId)
        {
            return [.. messageRepository
                .GetMessages(chatId)
                .Select(message =>
                {
                    Part part = new();
                    Content content = new();

                    switch(message.Type) {
                        case MessageType.Text:
                            part.Text = message.Text;
                            break;
                        case MessageType.FunctionCall:
                            part.FunctionCall = JsonSerializer.Deserialize<FunctionCall>(message.FunctionCall!);
                            break;
                        case MessageType.FunctionResponse:
                            part.FunctionResponse = JsonSerializer.Deserialize<FunctionResponse>(message.FunctionResponse!);
                            break;
                    }

                    content.Parts = [part];
                    return content;
                })];
        }

        public List<MessageResponse> GetMessages(int chatId, int index, int limit)
        {
            List<MessageRecord> messages = messageRepository.GetMessages(chatId, index, limit);
            return messages.Adapt<List<MessageResponse>>();
        }

        public List<MessageResponse> GetMessages(int chatId)
        {
            List<MessageRecord> messages = messageRepository.GetMessages(chatId);
            return messages.Adapt<List<MessageResponse>>();
        }

        public async Task SendAiAgentMessageAsync(Part messagePart, MessageType type, int chatId)
        {
            MessageRecord message = new()
            {
                Text = string.Empty,
                Type = type,
                Sender = MessageSender.Model,
                SentDate = DateTime.UtcNow,
                ChatId = chatId
            };

            switch (type)
            {
                case MessageType.Text:
                    message.Text = messagePart.Text!;
                    break;
                case MessageType.FunctionCall:
                    message.FunctionCall = JsonSerializer.Serialize(messagePart.FunctionCall);
                    break;
                case MessageType.FunctionResponse:
                    message.FunctionResponse = JsonSerializer.Serialize(messagePart.FunctionResponse);
                    break;
            }

            messageRepository.AddMessage(message);
            await chatHubContext.Clients.All.SendAsync("ReceiveMessage", chatId, message.Adapt<MessageResponse>());
        }
    }
}
