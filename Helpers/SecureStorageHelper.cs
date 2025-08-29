using System.Threading.Tasks;
using Microsoft.Maui.Storage;

namespace KesifUygulamasiTemplate.Helpers
{
    public static class SecureStorageHelper
    {
        private const string ApiKeyKey = "GoogleMapsApiKey";

        // API Key'i güvenli şekilde kaydet
        public static async Task SetApiKeyAsync(string apiKey)
        {
            await SecureStorage.SetAsync(ApiKeyKey, apiKey);
        }

        // API Key'i güvenli şekilde al
        public static async Task<string> GetApiKeyAsync()
        {
            try
            {
                var key = await SecureStorage.GetAsync(ApiKeyKey);
                return key ?? string.Empty;
            }
            catch
            {
                // SecureStorage desteklenmiyorsa boş dön
                return string.Empty;
            }
        }
    }
}
