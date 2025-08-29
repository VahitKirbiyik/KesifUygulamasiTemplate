using Microsoft.Maui.Devices.Sensors;
using System;

namespace KesifUygulamasiTemplate.Services;

public class CompassService
{
    public event EventHandler<CompassChangedEventArgs> CompassChanged;

    public void Start()
    {
        Compass.ReadingChanged += (s, e) =>
        {
            CompassChanged?.Invoke(this, e);
        };
        Compass.Start(SensorSpeed.UI);
    }

    public void Stop()
    {
        Compass.Stop();
    }
}
