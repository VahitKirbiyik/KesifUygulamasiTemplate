using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace KesifUygulamasiTemplate.Services
{
    public class OllamaService
    {
        private readonly HttpClient _http;

        public OllamaService()
        {
            _http = new HttpClient();
            _http.BaseAddress = new Uri("http://localhost:11434"); // Ollama server
        }

        public async Task<string> GenerateTextAsync(string model, string prompt)
        {
            try
            {
                var content = new StringContent(JsonSerializer.Serialize(new { model, prompt }), Encoding.UTF8, "application/json");
                var response = await _http.PostAsync("/v1/generate", content);
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                return json; // Sonuç olarak yanıt döner
            }
            catch (Exception ex)
            {
                return $"Hata: {ex.Message}";
            }
        }
    }
}
