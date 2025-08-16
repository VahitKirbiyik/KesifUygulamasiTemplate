using System;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace KesifUygulamasiTemplate.Services
{
    /// <summary>
    /// Geliþmiþ navigasyon servisi
    /// MVVM katmanýndan animasyonlu sayfa geçiþleri ve geliþmiþ navigasyon iþlemleri
    /// </summary>
    public class NavigationService
    {
        private INavigation Navigation => Application.Current?.MainPage?.Navigation;

        /// <summary>
        /// Yeni bir sayfaya animasyonlu geçiþ yapar
        /// </summary>
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
                    System.Diagnostics.Debug.WriteLine($"Navigation error: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Sayfadan geri döner
        /// </summary>
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
                    System.Diagnostics.Debug.WriteLine($"Navigation pop error: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Modal olarak yeni bir sayfa açar
        /// </summary>
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
                    System.Diagnostics.Debug.WriteLine($"Modal navigation error: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Modal sayfadan geri döner
        /// </summary>
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
                    System.Diagnostics.Debug.WriteLine($"Modal pop error: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Ana sayfaya döner (tüm stack'i temizler)
        /// </summary>
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
                    System.Diagnostics.Debug.WriteLine($"Pop to root error: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Shell tabanlý navigasyon (URI ile)
        /// </summary>
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
                    System.Diagnostics.Debug.WriteLine($"Shell navigation error: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Shell tabanlý navigasyon (parametrelerle)
        /// </summary>
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
                    System.Diagnostics.Debug.WriteLine($"Shell navigation with parameters error: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Mevcut navigation stack sayýsýný döner
        /// </summary>
        public int NavigationStackCount => Navigation?.NavigationStack?.Count ?? 0;

        /// <summary>
        /// Modal stack sayýsýný döner
        /// </summary>
        public int ModalStackCount => Navigation?.ModalStack?.Count ?? 0;

        /// <summary>
        /// Geri dönülebilir mi kontrol eder
        /// </summary>
        public bool CanGoBack => NavigationStackCount > 1;
    }
}
