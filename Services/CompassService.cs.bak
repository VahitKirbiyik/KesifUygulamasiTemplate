using Microsoft.Maui.Devices.Sensors;
using System;
using KesifUygulamasiTemplate.Services.Interfaces;

namespace KesifUygulamasiTemplate.Services;

public class CompassService : ICompassService
{
    public event Action<double>? HeadingChanged;

    public bool IsMonitoring => Compass.IsMonitoring;
    public bool IsSupported => Compass.IsSupported;

    public void Start()
    {
        Compass.ReadingChanged += (s, e) =>
        {
            HeadingChanged?.Invoke(e.Reading.HeadingMagneticNorth);
        };
        Compass.Start(SensorSpeed.UI);
    }

    public void Stop()
    {
        Compass.Stop();
    }
}
