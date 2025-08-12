using System;
using Microsoft.Maui.Devices.Sensors;

namespace KesifUygulamasiTemplate.Services
{
    public class CompassService : ICompassService
    {
        public event Action<double> HeadingChanged;
        public bool IsMonitoring { get; private set; }
        public bool IsSupported => Compass.Default.IsSupported;

        public CompassService()
        {
            Compass.Default.ReadingChanged += OnReadingChanged;
        }

        public void Start()
        {
            if (!IsSupported)
                throw new NotSupportedException("Cihaz pusula sensörünü desteklemiyor.");

            if (!IsMonitoring)
            {
                Compass.Default.Start(SensorSpeed.UI);
                IsMonitoring = true;
            }
        }

        public void Stop()
        {
            if (IsMonitoring)
            {
                Compass.Default.Stop();
                IsMonitoring = false;
            }
        }

        private void OnReadingChanged(object sender, CompassChangedEventArgs e)
        {
            HeadingChanged?.Invoke(e.Reading.HeadingMagneticNorth);
        }
    }
}
