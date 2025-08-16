namespace KesifUygulamasiTemplate.Services.Interfaces
{
    public interface IARService
    {
        void StartARSession();
        void StopARSession();
        void LoadModel(string modelPath);
    }
}
