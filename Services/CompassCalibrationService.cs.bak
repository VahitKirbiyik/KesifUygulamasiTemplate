using System;

namespace KesifUygulamasiTemplate.Services
{
    public enum CalibrationStatus
    {
        Unknown,
        NotCalibrated,
        LowAccuracy,
        MediumAccuracy,
        HighAccuracy
    }

    public class CompassCalibrationService : ICompassCalibrationService
    {
        private CalibrationStatus _status = CalibrationStatus.Unknown;
        private bool _isCalibrating;
        private double _calibrationAccuracy;
        private readonly Random _random = new Random();

        public bool IsCalibrationRequired => _status == CalibrationStatus.NotCalibrated || _status == CalibrationStatus.LowAccuracy;
        public bool IsCalibrating => _isCalibrating;
        public double CalibrationAccuracy => _calibrationAccuracy;

        public void StartCalibration()
        {
            if (_isCalibrating)
                return;

            _isCalibrating = true;
            _status = CalibrationStatus.NotCalibrated;
            _calibrationAccuracy = 0;

            // Gerçek bir uygulamada burada sensör verileri kullanılır
            // Bu örnek için rastgele iyileşen bir doğruluk simüle ediyoruz
            System.Threading.Tasks.Task.Run(async () =>
            {
                while (_isCalibrating && _calibrationAccuracy < 0.95)
                {
                    await System.Threading.Tasks.Task.Delay(500);
                    _calibrationAccuracy += _random.NextDouble() * 0.2;

                    if (_calibrationAccuracy > 0.95)
                    {
                        _status = CalibrationStatus.HighAccuracy;
                        _isCalibrating = false;
                    }
                    else if (_calibrationAccuracy > 0.6)
                    {
                        _status = CalibrationStatus.MediumAccuracy;
                    }
                    else if (_calibrationAccuracy > 0.3)
                    {
                        _status = CalibrationStatus.LowAccuracy;
                    }
                }
            });
        }

        public void StopCalibration()
        {
            _isCalibrating = false;
        }

        public void ResetCalibration()
        {
            _status = CalibrationStatus.NotCalibrated;
            _calibrationAccuracy = 0;
            _isCalibrating = false;
        }
    }
}
