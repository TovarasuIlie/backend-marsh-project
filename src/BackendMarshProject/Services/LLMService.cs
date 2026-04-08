using Azure;
using backend_marsh_project.DTOs;
using backend_marsh_project.Entities;
using System.Net.Http.Headers;
using System.Text.Json;

namespace backend_marsh_project.Services
{
    public class LLMService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;

        public LLMService (IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            _httpClientFactory = httpClientFactory;
            _config = config;
        }

        public async Task<string> GeneratedResponse(string inputMessage)
        {
            var httpClient = _httpClientFactory.CreateClient("AI-HttpClient");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _config["LLMAPIKey"]);

            LLMSendMessage message = new LLMSendMessage();
            message.SetMessage(inputMessage);

            var apiResponse = await httpClient.PostAsJsonAsync<LLMSendMessage>("openai/v1/chat/completions", message);

            if(!apiResponse.IsSuccessStatusCode)
            {
                throw new Exception("Model didn't responde!");
            }

            var jsonResponse = await apiResponse.Content.ReadFromJsonAsync<JsonElement>();
            return jsonResponse.GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString() ?? "No description provided.";
        }
    }
}
