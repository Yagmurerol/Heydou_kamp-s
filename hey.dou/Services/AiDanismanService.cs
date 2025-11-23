using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace hey.dou.Services
{
    public class AiDanismanService : IAiDanismanService
    {
        private readonly HttpClient _client;
        private readonly string _apiKey;

        public AiDanismanService(IConfiguration config)
        {
            _client = new HttpClient();
            _client.BaseAddress = new Uri("https://api.openai.com/");

            _apiKey = config["OpenAI:ApiKey"]
                      ?? throw new InvalidOperationException("OpenAI:ApiKey configuration is missing.");

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _apiKey);
        }

        public async Task<string> GetAnswerAsync(string question)
        {
            var payload = new
            {
                model = "gpt-4.1-mini",
                messages = new[]
                {
                    new { role = "system", content = "Sen HeyDOU üniversite portalı için Türkçe cevap veren yardımcı bir asistansın." },
                    new { role = "user",   content = question }
                },
                max_tokens = 300
            };

            var json = JsonSerializer.Serialize(payload);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");

            using var response = await _client.PostAsync("v1/chat/completions", content);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseJson);

            var answer = doc.RootElement
                            .GetProperty("choices")[0]
                            .GetProperty("message")
                            .GetProperty("content")
                            .GetString();

            return answer ?? "Şu anda cevap veremiyorum, lütfen tekrar dener misin?";
        }
    }
}
