using Android.App;
using Android.Runtime;
using Microsoft.Maui;
using Microsoft.Maui.Hosting;

namespace KesifUygulamasiTemplate
{
    /// <summary>
    /// Android platformu için ana uygulama sýnýfý
    /// </summary>
    [Application]
    public class MainApplication : MauiApplication
    {
        /// <summary>
        /// MainApplication constructor'ý
        /// </summary>
        /// <param name="handle">JNI handle</param>
        /// <param name="ownership">JNI ownership</param>
        public MainApplication(nint handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
        }

        /// <summary>
        /// MAUI uygulamasýný oluþturur
        /// </summary>
        /// <returns>MauiApp instance'ý</returns>
        protected override MauiApp CreateMauiApp()
        {
            return MauiProgram.CreateMauiApp();
        }
    }
}
