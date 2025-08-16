// API anahtarlar� ve kullan�c� bilgileri i�in g�venli depolama
public class SecureDataService : ISecureDataService
{
    public async Task<string> GetSecureValueAsync(string key)
    {
        try
        {
            return await SecureStorage.Default.GetAsync(key) 
                ?? throw new KeyNotFoundException($"G�venli de�er bulunamad�: {key}");
        }
        catch (Exception ex) when (ex is not KeyNotFoundException)
        {
            throw new SecurityException("G�venli veri eri�imi ba�ar�s�z", ex);
        }
    }
    
    public async Task SetSecureValueAsync(string key, string value)
    {
        await SecureStorage.Default.SetAsync(key, value);
    }
}
