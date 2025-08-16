namespace KesifUygulamasiTemplate.Services
{
    public interface ICompassCalibrationService
    {
        bool IsCalibrationRequired { get; }
        bool IsCalibrating { get; }
        double CalibrationAccuracy { get; }
        
        void StartCalibration();
        void StopCalibration();
        void ResetCalibration();
    }
}