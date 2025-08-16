#if ANDROID
using KesifUygulamasiTemplate.Services.Interfaces;
using System.Diagnostics;

namespace KesifUygulamasiTemplate.Platforms.Android
{
    public class ARServiceAndroid : IARService
    {
        public void StartARSession()
        {
            Debug.WriteLine("ARCore oturumu baþlatýldý (Android)");
            // ARCore baþlatma kodlarý buraya
        }
        public void StopARSession()
        {
            Debug.WriteLine("ARCore oturumu durduruldu (Android)");
            // ARCore durdurma kodlarý buraya
        }
        public void LoadModel(string modelPath)
        {
            Debug.WriteLine($"ARCore modeli yüklendi: {modelPath}");
            // Model yükleme kodlarý buraya
        }
    }
}
#endif
