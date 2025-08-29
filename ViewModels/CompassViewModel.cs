using System;
using System.Threading.Tasks;
using System.Windows.Input;
using KesifUygulamasiTemplate.Services;
using KesifUygulamasiTemplate.Services.Interfaces;
using Microsoft.Maui.Controls;
using KesifUygulamasiTemplate.ViewModels.Base;

namespace KesifUygulamasiTemplate.ViewModels
{
    public class CompassViewModel : BaseViewModel
    {
        private readonly ICompassService _compassService;

        private double _heading;
        public double Heading
        {
            get => _heading;
            set => SetProperty(ref _heading, value);
        }

        private bool _isActive;
        public bool IsActive
        {
            get => _isActive;
            set => SetProperty(ref _isActive, value);
        }

        public ICommand StartCompassCommand { get; }
        public ICommand StopCompassCommand { get; }

        public CompassViewModel(ICompassService compassService)
        {
            _compassService = compassService ?? throw new ArgumentNullException(nameof(compassService));

            StartCompassCommand = new Command(StartCompass);
            StopCompassCommand = new Command(StopCompass);
        }

        public void StartCompass()
        {
            try
            {
                _compassService.Start();
                IsActive = true;
            }
            catch (Exception ex)
            {
                // Handle error
                Application.Current?.MainPage?.DisplayAlert("Error", $"Failed to start compass: {ex.Message}", "OK");
            }
        }

        public void StopCompass()
        {
            _compassService.Stop();
            IsActive = false;
        }
    }
}
