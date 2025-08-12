#if ANDROID
using KesifUygulamasiTemplate.Services.Interfaces;

namespace KesifUygulamasiTemplate.Platforms.Android
{
    public class ARCoreService : IARPlatformService
    {
        public void StartARSession()
        {
            // ARCore baþlatma kodlarý (native Android)
        }
        public void StopARSession()
        {
            // ARCore durdurma kodlarý (native Android)
        }
    }
}
#endif
