using System;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;
using KesifUygulamasiTemplate.Services.Interfaces;
using System.Collections.Generic;
using System.Security;

namespace KesifUygulamasiTemplate.Services
{
    // API anahtarlar� ve kullan�c� bilgileri i�in g�venli depolama
    public class SecureDataService : ISecureDataService
    {
        public async Task<string> GetSecureValueAsync(string key)
        {
            try
            {
                return await SecureStorage.Default.GetAsync(key)
                    ?? throw new KeyNotFoundException($"Güvenli değer bulunamadı: {key}");
            }
            catch (Exception ex) when (ex is not KeyNotFoundException)
            {
                throw new SecurityException("Güvenli veri erişimi başarısız", ex);
            }
        }

        public async Task SetSecureValueAsync(string key, string value)
        {
            await SecureStorage.Default.SetAsync(key, value);
        }

        // ISecureDataService sync contract
        void ISecureDataService.Set(string key, string value)
        {
            // Fire-and-forget wrapper for the async Set
            _ = SetSecureValueAsync(key, value);
        }

        string? ISecureDataService.Get(string key)
        {
            try
            {
                return GetSecureValueAsync(key).GetAwaiter().GetResult();
            }
            catch
            {
                return null;
            }
        }
    }
}
