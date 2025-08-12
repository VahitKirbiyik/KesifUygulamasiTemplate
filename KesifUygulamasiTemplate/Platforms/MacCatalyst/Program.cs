using ObjCRuntime;
using UIKit;

namespace KesifUygulamasiTemplate;

/// <summary>
/// MacCatalyst platform için program entry point
/// </summary>
public class Program
{
    /// <summary>
    /// MacCatalyst uygulaması için ana entry point
    /// </summary>
    /// <param name="args">Komut satırı argümanları</param>
    static void Main(string[] args)
    {
        // MacCatalyst uygulamasını başlat
        UIApplication.Main(args, null, typeof(MauiUIApplicationDelegate));
    }
}
