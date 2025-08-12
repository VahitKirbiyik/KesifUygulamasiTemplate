using Microsoft.UI.Xaml;

namespace KesifUygulamasiTemplate.WinUI;

/// <summary>
/// Windows platform için MauiWinUIApplication implementasyonu
/// </summary>
public partial class App : MauiWinUIApplication
{
    /// <summary>
    /// Windows App constructor
    /// </summary>
    public App()
    {
        this.InitializeComponent();
    }

    /// <summary>
    /// MAUI App'i oluşturur
    /// </summary>
    /// <returns>MauiApp instance</returns>
    protected override MauiApp CreateMauiApp()
    {
        return MauiProgram.CreateMauiApp();
    }
}
