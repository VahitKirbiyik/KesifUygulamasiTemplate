using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using KesifUygulamasiTemplate.ViewModels.Base;

namespace KesifUygulamasiTemplate.Services
{
    public interface INavigationService
    {
        Task PushAsync(Page page, bool animated = true);
        Task PopAsync(bool animated = true);
        Task GoToAsync(string route, bool animated = true);
        Task GoToAsync(string route, IDictionary<string, object> parameters, bool animated = true);
        Task NavigateToAsync<TViewModel>(IDictionary<string, object> parameters = null) where TViewModel : BaseViewModel;
    }

    // NavigationService'in mesajlaþma sistemiyle entegrasyonu
    public class NavigationService : INavigationService
    {
        private readonly IServiceProvider _serviceProvider;
        
        public NavigationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        
        public async Task PushAsync(Page page, bool animated = true)
        {
            if (page != null && Shell.Current?.Navigation != null)
                await Shell.Current.Navigation.PushAsync(page, animated);
        }

        public async Task PopAsync(bool animated = true)
        {
            if (Shell.Current?.Navigation != null)
                await Shell.Current.Navigation.PopAsync(animated);
        }

        public async Task GoToAsync(string route, bool animated = true)
        {
            if (!string.IsNullOrWhiteSpace(route))
                await Shell.Current.GoToAsync(route, animated);
        }

        public async Task GoToAsync(string route, IDictionary<string, object> parameters, bool animated = true)
        {
            if (!string.IsNullOrWhiteSpace(route))
                await Shell.Current.GoToAsync(route, animated, parameters);
        }
        
        public async Task NavigateToAsync<TViewModel>(IDictionary<string, object> parameters = null) 
            where TViewModel : BaseViewModel
        {
            var viewModelType = typeof(TViewModel);
            var route = GetRouteForViewModel(viewModelType);
            
            if (parameters != null)
                await GoToAsync(route, parameters);
            else
                await GoToAsync(route);
        }

        private string GetRouteForViewModel(Type viewModelType)
        {
            // ViewModel ismine göre route oluþtur
            var viewModelName = viewModelType.Name;
            if (viewModelName.EndsWith("ViewModel"))
                viewModelName = viewModelName.Substring(0, viewModelName.Length - 9);
            
            return $"//{viewModelName}Page";
        }
    }
}
