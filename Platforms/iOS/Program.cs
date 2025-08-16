using ObjCRuntime;
using UIKit;

namespace KesifUygulamasiTemplate;

/// <summary>
/// iOS platform için program entry point
/// </summary>
public class Program
{
    /// <summary>
    /// iOS uygulaması için ana entry point
    /// </summary>
    /// <param name="args">Komut satırı argümanları</param>
    static void Main(string[] args)
    {
        // iOS uygulamasını başlat
        UIApplication.Main(args, null, typeof(MauiUIApplicationDelegate));
    }
}
