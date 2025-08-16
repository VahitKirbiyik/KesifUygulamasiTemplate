using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Maui.Devices.Sensors;

namespace KesifUygamamasiTemplate.Services
{
    public enum EmergencyType { Hospital, Police, Pharmacy, GasStation }

    public interface IEmergencyPointsService
    {
        Task<List<(string name, Location location)>> GetNearbyAsync(Location userLocation, EmergencyType type, double radiusKm = 5);
    }
}
