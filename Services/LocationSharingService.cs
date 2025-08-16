using System.Threading.Tasks;
using Microsoft.Maui.ApplicationModel.DataTransfer;
using QRCoder;
using System.IO;

namespace KesifUygamamasiTemplate.Services
{
    public class LocationSharingService : ILocationSharingService
    {
        public async Task ShareLocationAsync(double lat, double lon, string label = null)
        {
            var url = await GenerateGeoUrlAsync(lat, lon);
            var request = new ShareTextRequest
            {
                Text = label != null ? $"{label}\n{url}" : url,
                Title = "Konum paylaş"
            };
            await Share.Default.RequestAsync(request);
        }

        public Task<string> GenerateGeoUrlAsync(double lat, double lon)
        {
            var url = $"https://maps.google.com/?q={lat},{lon}";
            return Task.FromResult(url);
        }

        public Task<string> GenerateQrCodeBase64Async(double lat, double lon)
        {
            var url = $"https://maps.google.com/?q={lat},{lon}";
            using var qrGen = new QRCodeGenerator();
            var data = qrGen.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new Base64QRCode(data);
            var base64 = qrCode.GetGraphic(20);
            return Task.FromResult(base64);
        }
    }
}
