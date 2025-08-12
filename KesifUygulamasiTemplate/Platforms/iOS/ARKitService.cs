#if IOS
using KesifUygulamasiTemplate.Services.Interfaces;

namespace KesifUygulamasiTemplate.Platforms.iOS
{
    public class ARKitService : IARPlatformService
    {
        public void StartARSession()
        {
            // ARKit ba�latma kodlar� (native iOS)
        }
        public void StopARSession()
        {
            // ARKit durdurma kodlar� (native iOS)
        }
    }
}
#endif
