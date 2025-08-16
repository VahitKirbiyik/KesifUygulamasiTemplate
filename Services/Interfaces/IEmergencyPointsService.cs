using System.Collections.Generic;
using System.Threading.Tasks;

namespace KesifUygulamasiTemplate.Services
{
    public interface IEmergencyPointsService
    {
        Task<IEnumerable<EmergencyPoint>> GetAllEmergencyPointsAsync(int limit = 100);
        Task<EmergencyPoint> GetEmergencyPointByIdAsync(int id);
        Task<IEnumerable<EmergencyPoint>> GetNearbyEmergencyPointsAsync(double latitude, double longitude, double radiusKm = 5, EmergencyPointType type = EmergencyPointType.All);
        Task<int> AddEmergencyPointAsync(EmergencyPoint point);
        Task<bool> UpdateEmergencyPointAsync(EmergencyPoint point);
        Task<bool> RemoveEmergencyPointAsync(int id);
        Task<bool> ClearAllEmergencyPointsAsync();
    }
}