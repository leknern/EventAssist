using Google.GenAI.Types;

namespace EventAssist.Services.Interfaces
{
    public interface IAiModelConfigurationBuilder
    {
        IAiModelConfigurationBuilder WithSystemInstruction();
        IAiModelConfigurationBuilder WithTools();
        IAiModelConfigurationBuilder WithThinkingConfig();
        GenerateContentConfig Build();
    }
}
