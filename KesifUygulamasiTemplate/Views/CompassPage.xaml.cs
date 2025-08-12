using System;
using Microsoft.Maui.Controls;
using KesifUygulamasiTemplate.ViewModels;

namespace KesifUygulamasiTemplate.Views
{
    public partial class CompassPage : ContentPage
    {
        private readonly CompassViewModel _viewModel;
        
        public CompassPage(CompassViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
            BindingContext = _viewModel;
        }
        
        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.StartCompass();
        }
        
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _viewModel.StopCompass();
        }
    }
}