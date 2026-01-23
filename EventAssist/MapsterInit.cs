using EventAssist.Models.DTOs;
using EventAssist.Models.Records;
using Google.GenAI.Types;
using Mapster;

namespace EventAssist
{
    public class MapsterInit
    {
        public static void RegisterMappings()
        {
            TypeAdapterConfig<ChatRecord, ChatResponse>
                .NewConfig()
                .Map(
                    dest => dest.User,
                    src => src.User.Adapt<ChatMember>()
                )
                .Map(
                    dest => dest.CustomerSupportAgent,
                    src => src.CustomerSupportAgent != null
                        ? src.CustomerSupportAgent.Adapt<ChatMember>()
                        : null
                )
                .Map(
                    dest => dest.LastMessage,
                    src => src.Messages == null || src.Messages.Count == 0
                        ? null
                        : src.Messages.First().Adapt<MessageResponse>()
                );

            TypeAdapterConfig<Part, EventRequest>
                .NewConfig()
                .Map(
                    dest => dest.Name,
                    src => src.FunctionCall != null && src.FunctionCall.Args != null && src.FunctionCall.Args.ContainsKey("Name")
                        ? src.FunctionCall.Args["Name"].ToString()!
                        : string.Empty
                )
                .Map(
                    dest => dest.Description,
                    src => src.FunctionCall != null && src.FunctionCall.Args != null && src.FunctionCall.Args.ContainsKey("Description")
                        ? (src.FunctionCall.Args["Description"] != null ? src.FunctionCall.Args["Description"].ToString() : null)
                        : null
                )
                .Map(
                    dest => dest.Occurrence,
                    src => src.FunctionCall != null && src.FunctionCall.Args != null && src.FunctionCall.Args.ContainsKey("Occurrence")
                        ? DateTime.Parse(src.FunctionCall.Args["Occurrence"].ToString()!)
                        : default
                );
        }
    }
}
