#if IOS
using KesifUygulamasiTemplate.Services.Interfaces;
using System.Diagnostics;

namespace KesifUygulamasiTemplate.Platforms.iOS
{
    public class ARServiceiOS : IARService
    {
        public void StartARSession()
        {
            Debug.WriteLine("ARKit oturumu baþlatýldý (iOS)");
            // ARKit baþlatma kodlarý buraya
        }
        public void StopARSession()
        {
            Debug.WriteLine("ARKit oturumu durduruldu (iOS)");
            // ARKit durdurma kodlarý buraya
        }
        public void LoadModel(string modelPath)
        {
            Debug.WriteLine($"ARKit modeli yüklendi: {modelPath}");
            // Model yükleme kodlarý buraya
        }
    }
}
#endif
