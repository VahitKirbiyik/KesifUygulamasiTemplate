using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace KesifUygulamasi.Services
{
    public class NotificationService
    {
        public async Task ShowNotificationAsync(string title, string message)
        {
            var mainPage = Application.Current?.Windows.FirstOrDefault()?.Page;

            if (mainPage == null)
                return;

            try
            {
                await mainPage.DisplayAlert(title, message, "Tamam");
            }
            catch (System.Exception ex)
            {
                // Fallback: Debug output if UI is not available
                System.Diagnostics.Debug.WriteLine($"Notification failed: {ex.Message}");
            }
        }

        public async Task<bool> ShowConfirmationAsync(string title, string message, string accept = "Evet", string cancel = "Hayır")
        {
            var mainPage = Application.Current?.Windows.FirstOrDefault()?.Page;

            if (mainPage == null)
                return false;

            try
            {
                return await mainPage.DisplayAlert(title, message, accept, cancel);
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Confirmation dialog failed: {ex.Message}");
                return false;
            }
        }
    }
}
