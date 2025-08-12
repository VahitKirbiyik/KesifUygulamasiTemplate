using System.Threading.Tasks;

namespace KesifUygulamasiTemplate.Services.Interfaces
{
    public interface ISecureDataService
    {
        Task<string> GetSecureValueAsync(string key);
        Task SetSecureValueAsync(string key, string value);
    }
}