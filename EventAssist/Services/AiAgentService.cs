using EventAssist.Hubs;
using EventAssist.Models.Enums;
using EventAssist.Services.Interfaces;
using Google.GenAI;
using Google.GenAI.Types;
using Microsoft.AspNetCore.SignalR;
namespace EventAssist.Services
{
    public class AiAgentService(
        IAiModelConfigurationBuilder modelConfigurationBuilder,
        IMessageService messageService,
        IHubContext<ChatHub> chatHubContext,
        IAiFunctionExecutionService aiFunctionExecutionService,
        IConfiguration configuration) : IAiAgentService
    {

        public async Task RespondUserMessage(int chatId, int userId)
        {
            await chatHubContext.Clients.All.SendAsync("StartTyping", chatId, MessageSender.Model);

            Client client = new(apiKey: configuration["Google:ApiKey"]);
            GenerateContentConfig config = modelConfigurationBuilder
                .WithSystemInstruction()
                .WithTools()
                .WithThinkingConfig()
                .Build();

            List<Content> contents = messageService.GetContents(chatId);

            GenerateContentResponse response = await client.Models.GenerateContentAsync(
                model: "gemini-2.5-flash",
                contents,
                config
            );

            Part responsePart = response.Candidates!.First().Content!.Parts!
                .First();

            if (responsePart?.FunctionCall is not null)
            {
                Content content = await aiFunctionExecutionService.ProcessFunctionCall(responsePart, chatId, userId);
                
                if (content.Parts!.First().FunctionResponse!.Name != "CloseChat")
                {
                    contents.Add(content);

                    GenerateContentResponse nextResponse = await client.Models.GenerateContentAsync(
                        model: "gemini-2.5-flash",
                        contents,
                        config
                    );

                    Part nextResponsePart = nextResponse.Candidates!.First().Content!.Parts!
                        .First();

                    if (nextResponsePart?.Text is not null)
                    {
                        await messageService.SendAiAgentMessageAsync(nextResponsePart, MessageType.Text, chatId);
                    }
                }
            }
            else if (responsePart?.Text is not null)
            {
                await messageService.SendAiAgentMessageAsync(responsePart, MessageType.Text, chatId);
            }

            await chatHubContext.Clients.All.SendAsync("StopTyping", chatId, MessageSender.Model);
        }
    }
}
