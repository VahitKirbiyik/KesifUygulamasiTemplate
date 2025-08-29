using System.Threading.Tasks;

namespace KesifUygulamasi.AIHelper.OllamaPanel
{
    public class OllamaService
    {
        public async Task<string> GenerateTextAsync(string model, string prompt)
        {
            // Burada Ollama CLI veya API çağrısı yapılacak
            await Task.Delay(100); // simülasyon
            return $"Model: {model}\nYanıt: {prompt} → [simüle edilmiş cevap]";
        }
    }
}
