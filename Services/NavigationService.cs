using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace KesifUygulamasiTemplate.Services
{
    public class NavigationService
    {
        private INavigation Navigation => Application.Current?.MainPage?.Navigation;

        public async Task PushAsync(Page page, bool animated = true)
        {
            if (Navigation != null && page != null)
            {
                try { await Navigation.PushAsync(page, animated); }
                catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"Navigation error: {ex.Message}"); }
            }
        }

        public async Task PopAsync(bool animated = true)
        {
            if (Navigation != null && Navigation.NavigationStack.Count > 1)
            {
                try { await Navigation.PopAsync(animated); }
                catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"Navigation pop error: {ex.Message}"); }
            }
        }

        public async Task PushModalAsync(Page page, bool animated = true)
        {
            if (Navigation != null && page != null)
            {
                try { await Navigation.PushModalAsync(page, animated); }
                catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"Modal navigation error: {ex.Message}"); }
            }
        }

        public async Task PopModalAsync(bool animated = true)
        {
            if (Navigation != null && Navigation.ModalStack.Count > 0)
            {
                try { await Navigation.PopModalAsync(animated); }
                catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"Modal pop error: {ex.Message}"); }
            }
        }

        public async Task PopToRootAsync(bool animated = true)
        {
            if (Navigation != null && Navigation.NavigationStack.Count > 1)
            {
                try { await Navigation.PopToRootAsync(animated); }
                catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"Pop to root error: {ex.Message}"); }
            }
        }

        public async Task GoToAsync(string route, bool animated = true)
        {
            if (!string.IsNullOrWhiteSpace(route))
            {
                try { await Shell.Current.GoToAsync(route, animated); }
                catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"Shell navigation error: {ex.Message}"); }
            }
        }

        public async Task GoToAsync(string route, IDictionary<string, object> parameters, bool animated = true)
        {
            if (!string.IsNullOrWhiteSpace(route))
            {
                try { await Shell.Current.GoToAsync(route, animated, parameters); }
                catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"Shell navigation with parameters error: {ex.Message}"); }
            }
        }

        public int NavigationStackCount => Navigation?.NavigationStack?.Count ?? 0;
        public int ModalStackCount => Navigation?.ModalStack?.Count ?? 0;
        public bool CanGoBack => NavigationStackCount > 1;
    }
}
