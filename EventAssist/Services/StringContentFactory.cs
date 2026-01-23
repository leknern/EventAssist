using EventAssist.Services.Interfaces;
using System.Text;
using System.Text.Json;

namespace EventAssist.Services
{
    public class StringContentFactory(IConfiguration configuration) : IStringContentFactory
    {
        private static readonly JsonSerializerOptions jsonSerializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        public StringContent CreateMailerSendRequest(string to, string subject, string html)
        {
            string domain = configuration["MessageSender:Domain"]!;
            string fromName = "EventAssist";

            string contentJson = JsonSerializer
                .Serialize(new
                {
                from = new
                {
                    email = domain,
                    name = fromName
                },
                to = new[]
                {
                    new 
                    {
                        email = to
                    }
                },
                subject,
                html
                }, 
                jsonSerializerOptions);

            return new StringContent(contentJson, Encoding.UTF8, "application/json");
        }
    }
}
