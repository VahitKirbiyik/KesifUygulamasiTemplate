using System;

namespace KesifUygulamasiTemplate.Services
{
    public interface ICompassService
    {
        event Action<double> HeadingChanged;
        bool IsMonitoring { get; }
        bool IsSupported { get; }
        void Start();
        void Stop();
    }
}