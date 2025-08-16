#if IOS
using KesifUygulamasiTemplate.Services.Interfaces;

namespace KesifUygulamasiTemplate.Platforms.iOS
{
    public class ARKitService : IARPlatformService
    {
        public void StartARSession()
        {
            // ARKit baþlatma kodlarý (native iOS)
        }
        public void StopARSession()
        {
            // ARKit durdurma kodlarý (native iOS)
        }
    }
}
#endif
