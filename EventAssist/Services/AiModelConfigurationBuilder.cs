using EventAssist.Services.Interfaces;
using Google.GenAI.Types;
using Type = Google.GenAI.Types.Type;

namespace EventAssist.Services
{
    public class AiModelConfigurationBuilder(IConfiguration configuration) : IAiModelConfigurationBuilder
    {
        private readonly GenerateContentConfig config = new();

        public IAiModelConfigurationBuilder WithSystemInstruction()
        {
            config.SystemInstruction = new Content()
            {
                Parts = [
                    new Part()
                    {
                        Text = System.IO.File.ReadAllText(configuration["PromptFilePath"]!)
                    }
                ]
            };
            return this;
        }

        public IAiModelConfigurationBuilder WithTools()
        {
            Tool tool = new()
            {
                FunctionDeclarations =
                [
                    new FunctionDeclaration()
                    {
                        Name = "AddEvent",
                        Description = "Create an event based on user request",
                        Parameters = new()
                        {
                            Type = Type.OBJECT,
                            Properties = new()
                            {
                                { "Name", new() { Type = Type.STRING, Description = "Title of the event" } },
                                { "Description", new() { Type = Type.STRING, Description = "Description of the event" } },
                                { "Occurrence", new() { Type = Type.STRING, Description = "Date of the event in YYYY-MM-DD format" } }
                            },
                            Required = ["Name", "Occurrence"]
                        }
                    },
                    new FunctionDeclaration() {
                        Name = "GetEventsByUserId",
                        Description =
                            "Returns the user's existing calendar events. " +
                            "Use this function when the user asks about their events, schedule, agenda, upcoming plans, " +
                            "or needs information derived from their stored events (e.g. next event, event summary)."
                    },
                    new FunctionDeclaration()
                    {
                        Name = "CloseChat",
                        Description =
                            "Call this function ONLY when the user's problem has been fully resolved and no further assistance is needed. " +
                            "Use it to explicitly end the conversation when the user confirms completion, expresses satisfaction, or says goodbye. " +
                            "Do NOT call this function if there are pending questions, unresolved tasks, or if the user might need further help.",
                    },
                ]
            };

            config.Tools = [tool];
            return this;
        }

        public IAiModelConfigurationBuilder WithThinkingConfig()
        {
            config.ThinkingConfig = new ThinkingConfig()
            {
                ThinkingBudget = 0
            };
            return this;
        }

        public GenerateContentConfig Build()
        {
            return config;
        }
    }
}
