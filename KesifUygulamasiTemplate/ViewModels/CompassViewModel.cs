using System;
using System.Windows.Input;
using KesifUygulamasiTemplate.Services;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices.Sensors;

namespace KesifUygulamasiTemplate.ViewModels
{
    public class CompassViewModel : BaseViewModel, IDisposable
    {
        private readonly ICompassService _compassService;
        private readonly ICompassCalibrationService _calibrationService;
        private double _heading;
        private bool _isCalibrating;
        private double _calibrationAccuracy;
        private bool _needsCalibration;
        private bool _disposed;
        
        public double Heading
        {
            get => _heading;
            set => SetProperty(ref _heading, value);
        }
        
        public bool IsCalibrating
        {
            get => _isCalibrating;
            set => SetProperty(ref _isCalibrating, value);
        }
        
        public double CalibrationAccuracy
        {
            get => _calibrationAccuracy;
            set => SetProperty(ref _calibrationAccuracy, value);
        }
        
        public bool NeedsCalibration
        {
            get => _needsCalibration;
            set => SetProperty(ref _needsCalibration, value);
        }
        
        public ICommand StartCompassCommand { get; }
        public ICommand StopCompassCommand { get; }
        public ICommand CalibrateCommand { get; }

        public CompassViewModel(ICompassService compassService, ICompassCalibrationService calibrationService)
        {
            _compassService = compassService ?? throw new ArgumentNullException(nameof(compassService));
            _calibrationService = calibrationService ?? throw new ArgumentNullException(nameof(calibrationService));
            
            StartCompassCommand = new Command(StartCompass);
            StopCompassCommand = new Command(StopCompass);
            CalibrateCommand = new Command(StartCalibration);
            
            _compassService.HeadingChanged += OnHeadingChanged;
        }

        private void OnHeadingChanged(double newHeading)
        {
            Microsoft.Maui.ApplicationModel.MainThread.BeginInvokeOnMainThread(() =>
            {
                Heading = newHeading;
            });
        }
        
        public void StartCompass()
        {
            if (IsBusy)
                return;
                
            try
            {
                IsBusy = true;
                ErrorMessage = string.Empty;
                
                // Check if calibration is required
                NeedsCalibration = _calibrationService.IsCalibrationRequired;
                
                if (!NeedsCalibration)
                {
                    _compassService.Start();
                }
                else
                {
                    ErrorMessage = "Pusula kalibrasyonu gerekiyor. Lütfen kalibrasyon yapýn.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Pusula baþlatýlamadý: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }
        
        public void StopCompass()
        {
            if (IsBusy)
                return;
                
            try
            {
                IsBusy = true;
                _compassService.Stop();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Pusula durdurulamadý: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }
        
        private void StartCalibration()
        {
            if (IsBusy || IsCalibrating)
                return;
                
            try
            {
                IsCalibrating = true;
                _calibrationService.StartCalibration();
                
                // Start a timer to update calibration status
                Device.StartTimer(TimeSpan.FromMilliseconds(500), () =>
                {
                    CalibrationAccuracy = _calibrationService.CalibrationAccuracy;
                    IsCalibrating = _calibrationService.IsCalibrating;
                    
                    if (!IsCalibrating)
                    {
                        NeedsCalibration = _calibrationService.IsCalibrationRequired;
                        
                        // If calibration is successful, start the compass
                        if (!NeedsCalibration)
                        {
                            StartCompass();
                        }
                    }
                    
                    // Continue timer while calibrating
                    return IsCalibrating;
                });
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Kalibrasyon baþlatýlamadý: {ex.Message}";
                IsCalibrating = false;
            }
        }
        
        public void Dispose()
        {
            if (!_disposed)
            {
                _compassService.HeadingChanged -= OnHeadingChanged;
                _compassService.Stop();
                _disposed = true;
            }
        }
    }
}
