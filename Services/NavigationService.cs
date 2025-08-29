using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using KesifUygulamasiTemplate.Helpers;

namespace KesifUygulamasiTemplate.Services
{
    public class NavigationService
    {
        private readonly IGlobalExceptionHandler _exceptionHandler;
        private INavigation Navigation => Application.Current?.MainPage?.Navigation;

        public NavigationService(IGlobalExceptionHandler exceptionHandler)
        {
            _exceptionHandler = exceptionHandler ?? throw new ArgumentNullException(nameof(exceptionHandler));
        }

        public async Task PushAsync(Page page, bool animated = true)
        {
            if (Navigation != null && page != null)
            {
                try
                {
                    await Navigation.PushAsync(page, animated);
                }
                catch (Exception ex)
                {
                    _exceptionHandler.HandleException(ex);
                    _exceptionHandler.LogUserFriendlyError("Sayfa açılırken bir hata oluştu", ex);
                }
            }
        }

        public async Task PopAsync(bool animated = true)
        {
            if (Navigation != null && Navigation.NavigationStack.Count > 1)
            {
                try
                {
                    await Navigation.PopAsync(animated);
                }
                catch (Exception ex)
                {
                    _exceptionHandler.HandleException(ex);
                    _exceptionHandler.LogUserFriendlyError("Sayfadan çıkılırken bir hata oluştu", ex);
                }
            }
        }

        public async Task PushModalAsync(Page page, bool animated = true)
        {
            if (Navigation != null && page != null)
            {
                try
                {
                    await Navigation.PushModalAsync(page, animated);
                }
                catch (Exception ex)
                {
                    _exceptionHandler.HandleException(ex);
                    _exceptionHandler.LogUserFriendlyError("Modal sayfa açılırken bir hata oluştu", ex);
                }
            }
        }

        public async Task PopModalAsync(bool animated = true)
        {
            if (Navigation != null && Navigation.ModalStack.Count > 0)
            {
                try
                {
                    await Navigation.PopModalAsync(animated);
                }
                catch (Exception ex)
                {
                    _exceptionHandler.HandleException(ex);
                    _exceptionHandler.LogUserFriendlyError("Modal sayfadan çıkılırken bir hata oluştu", ex);
                }
            }
        }

        public async Task PopToRootAsync(bool animated = true)
        {
            if (Navigation != null && Navigation.NavigationStack.Count > 1)
            {
                try
                {
                    await Navigation.PopToRootAsync(animated);
                }
                catch (Exception ex)
                {
                    _exceptionHandler.HandleException(ex);
                    _exceptionHandler.LogUserFriendlyError("Ana sayfaya dönülürken bir hata oluştu", ex);
                }
            }
        }

        public async Task GoToAsync(string route, bool animated = true)
        {
            if (!string.IsNullOrWhiteSpace(route))
            {
                try
                {
                    await Shell.Current.GoToAsync(route, animated);
                }
                catch (Exception ex)
                {
                    _exceptionHandler.HandleException(ex);
                    _exceptionHandler.LogUserFriendlyError($"'{route}' rotasına gidilirken bir hata oluştu", ex);
                }
            }
        }

        public async Task GoToAsync(string route, IDictionary<string, object> parameters, bool animated = true)
        {
            if (!string.IsNullOrWhiteSpace(route))
            {
                try
                {
                    await Shell.Current.GoToAsync(route, animated, parameters);
                }
                catch (Exception ex)
                {
                    _exceptionHandler.HandleException(ex);
                    _exceptionHandler.LogUserFriendlyError($"'{route}' rotasına parametrelerle gidilirken bir hata oluştu", ex);
                }
            }
        }

        public int NavigationStackCount => Navigation?.NavigationStack?.Count ?? 0;
        public int ModalStackCount => Navigation?.ModalStack?.Count ?? 0;
        public bool CanGoBack => NavigationStackCount > 1;
    }
}
