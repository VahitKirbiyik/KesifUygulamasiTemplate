using System;
using System.Threading.Tasks;
using CoordinateSharp;
using KesifUygulamasiTemplate.Models;

namespace KesifUygulamasiTemplate.Services
{
    public class MoonCompassService : IMoonCompassService
    {
        private readonly DatabaseService _databaseService;
        private readonly ConnectivityService _connectivityService;

        public MoonCompassService(DatabaseService databaseService, ConnectivityService connectivityService)
        {
            _databaseService = databaseService;
            _connectivityService = connectivityService;
        }

        public async Task<MoonData> GetMoonDataAsync(double latitude, double longitude)
        {
            var today = DateTime.Now.Date;
            
            // Offline �ncelik: �nbellekten kontrol et
            if (!_connectivityService.IsConnected)
            {
                var cached = await _databaseService.GetMoonDataAsync(latitude, longitude, today);
                if (cached != null)
                    return cached;
            }

            // CoordinateSharp ile hesaplama yap
            var coordinate = new Coordinate(latitude, longitude, DateTime.Now);
            
            var moonData = new MoonData
            {
                Id = 0,
                Latitude = latitude,
                Longitude = longitude,
                Date = today,
                RiseTime = coordinate.CelestialInfo.MoonRise ?? DateTime.MinValue,
                SetTime = coordinate.CelestialInfo.MoonSet ?? DateTime.MinValue,
                Phase = coordinate.CelestialInfo.MoonIllum.Fraction,
                Azimuth = coordinate.CelestialInfo.MoonAzimuth,
                Altitude = coordinate.CelestialInfo.MoonAltitude,
                PhaseName = GetMoonPhaseName(coordinate.CelestialInfo.MoonIllum.PhaseName),
                Illumination = coordinate.CelestialInfo.MoonIllum.Fraction,
                Distance = coordinate.CelestialInfo.MoonDistance.Kilometers
            };

            // �nbelle�e kaydet
            try
            {
                await _databaseService.InsertMoonDataAsync(moonData);
            }
            catch
            {
                // �nbellek hatas� kritik de�il
            }

            return moonData;
        }

        private string GetMoonPhaseName(MoonName phaseName)
        {
            return phaseName switch
            {
                MoonName.New_Moon => "Yeni Ay",
                MoonName.Waxing_Crescent => "Hilal",
                MoonName.First_Quarter => "�lk D�rd�n",
                MoonName.Waxing_Gibbous => "�i�kin Ay",
                MoonName.Full_Moon => "Dolunay",
                MoonName.Waning_Gibbous => "Azalan �i�kin",
                MoonName.Third_Quarter => "Son D�rd�n",
                MoonName.Waning_Crescent => "Azalan Hilal",
                _ => "Bilinmeyen"
            };
        }
    }
}