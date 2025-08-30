using System;
using Microsoft.Maui.Networking;

namespace KesifUygulamasiTemplate.Services
{
    public class ConnectivityService
    {
        public bool IsConnected => Connectivity.Current.NetworkAccess == NetworkAccess.Internet;

        public event Action<bool> ConnectivityChanged;

        public ConnectivityService()
        {
            Connectivity.ConnectivityChanged += (s, e) =>
            {
                ConnectivityChanged?.Invoke(IsConnected);
            };
        }
    }
}
