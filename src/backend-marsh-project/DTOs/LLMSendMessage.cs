using System.Text.Json.Serialization;

namespace backend_marsh_project.DTOs
{
    public class LLMSendMessage
    {
        public List<LLMMessage> Messages { get; set; } = new List<LLMMessage>{
            new LLMMessage
            {
                Role = "system",
                Content = "You are an IT Inventory Specialist. Your task is to convert technical specifications into a single, human-readable sentence. Do not include any introductory text like Here is the description. Only return the generated sentence."
            },
            new LLMMessage
            {
                Role = "user",
                Content = string.Empty
            },
        };
        public string Model { get; set; } = "llama-3.1-8b-instant";
        public int Temperature { get; set; } = 1;
        [JsonPropertyName("max_completion_tokens")]
        public int MaxCompletionTokens { get; set; } = 1024;
        [JsonPropertyName("top_p")]
        public int TopP { get; set; } = 1;
        public bool Stream { get; set; } = false;
        public string? Stop { get; set; } = null;

        public void SetMessage(string  message)
        {
            this.Messages[1].Content = message;
        }
    }

    public class LLMMessage
    {
        public string Role { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }
}
