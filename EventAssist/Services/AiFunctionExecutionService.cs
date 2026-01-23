using EventAssist.Models.DTOs;
using EventAssist.Models.Enums;
using EventAssist.Services.Interfaces;
using Google.GenAI.Types;
using Mapster;

namespace EventAssist.Services
{
    public class AiFunctionExecutionService(
        IMessageService messageService,
        IEventService eventService,
        IChatService chatService) : IAiFunctionExecutionService
    {
        public async Task<Content> ProcessFunctionCall(Part messagePart, int chatId, int userId)
        {
            await messageService.SendAiAgentMessageAsync(messagePart, MessageType.FunctionCall, chatId);

            Content content = new()
            {
                Parts = [
                    new Part()
                    {
                        FunctionResponse = new FunctionResponse()
                        {
                            Name = messagePart.FunctionCall!.Name,
                        }
                    }
                ]
            };

            switch (messagePart.FunctionCall!.Name)
            {
                case "AddEvent":
                    EventRequest request = messagePart.Adapt<EventRequest>();
                    EventResponse response = await eventService.AddEventAiAgentAsync(request, userId);

                    content.Parts.First().FunctionResponse!.Response = new Dictionary<string, object>
                    {
                        { "AddedEvent", response }
                    };

                    break;
                case "GetEventsByUserId":
                    List<EventResponse> events = eventService.GetEventsByUserId(userId);
                    
                    content.Parts.First().FunctionResponse!.Response = new Dictionary<string, object>
                    {
                        { "Events", events }
                    };

                    break;
                case "CloseChat":
                    await chatService.CloseChatAsync(chatId, userId);
                    
                    break;
            }

            await messageService.SendAiAgentMessageAsync(content.Parts.First(), MessageType.FunctionResponse, chatId);
            return content;
        }
    }
}
