using Microsoft.Maui.Controls;

namespace KesifUygulamasiTemplate;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // ✅ Sayfa yönlendirme tanımları yapılabilir (opsiyonel)
        Routing.RegisterRoute("login", typeof(Views.LoginPage));
        Routing.RegisterRoute("detail", typeof(Views.DetailPage));
    }
}