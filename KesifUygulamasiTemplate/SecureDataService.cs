// API anahtarlarý ve kullanýcý bilgileri için güvenli depolama
public class SecureDataService : ISecureDataService
{
    public async Task<string> GetSecureValueAsync(string key)
    {
        try
        {
            return await SecureStorage.Default.GetAsync(key) 
                ?? throw new KeyNotFoundException($"Güvenli deðer bulunamadý: {key}");
        }
        catch (Exception ex) when (ex is not KeyNotFoundException)
        {
            throw new SecurityException("Güvenli veri eriþimi baþarýsýz", ex);
        }
    }
    
    public async Task SetSecureValueAsync(string key, string value)
    {
        await SecureStorage.Default.SetAsync(key, value);
    }
}
