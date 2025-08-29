using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace KesifUygulamasiTemplate.ViewModels
{
    public class LocationsPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public async Task LoadAsync()
        {
            // Buraya veri yükleme işlemleri gelecek
            await Task.Delay(500); // Simülasyon
        }
    }
}
