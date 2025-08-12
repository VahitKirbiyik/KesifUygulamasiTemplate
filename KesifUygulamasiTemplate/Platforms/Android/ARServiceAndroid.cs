#if ANDROID
using KesifUygulamasiTemplate.Services.Interfaces;
using System.Diagnostics;

namespace KesifUygulamasiTemplate.Platforms.Android
{
    public class ARServiceAndroid : IARService
    {
        public void StartARSession()
        {
            Debug.WriteLine("ARCore oturumu ba�lat�ld� (Android)");
            // ARCore ba�latma kodlar� buraya
        }
        public void StopARSession()
        {
            Debug.WriteLine("ARCore oturumu durduruldu (Android)");
            // ARCore durdurma kodlar� buraya
        }
        public void LoadModel(string modelPath)
        {
            Debug.WriteLine($"ARCore modeli y�klendi: {modelPath}");
            // Model y�kleme kodlar� buraya
        }
    }
}
#endif
