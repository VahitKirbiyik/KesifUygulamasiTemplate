using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;
using KesifUygulamasiTemplate.Models;

namespace KesifUygulamasiTemplate.Services
{
    /// <summary>
    /// Offline harita tile caching servisi
    /// Harita tiles'larını indirip offline kullanım için saklar
    /// </summary>
    public class OfflineMapTileService
    {
        private readonly HttpClient _httpClient;
        private readonly DatabaseService _databaseService;
        private readonly ConnectivityService _connectivityService;
        private const string TILE_CACHE_DIR = "maptiles";
        private const int MAX_ZOOM_LEVEL = 16;
        private const int MIN_ZOOM_LEVEL = 10;

        public OfflineMapTileService(HttpClient httpClient, DatabaseService databaseService, ConnectivityService connectivityService)
        {
            _httpClient = httpClient;
            _databaseService = databaseService;
            _connectivityService = connectivityService;
        }

        /// <summary>
        /// Belirtilen bölge için harita tiles'larını indirir ve önbelleğe alır
        /// </summary>
        public async Task DownloadMapTilesAsync(double northLat, double southLat, double westLng, double eastLng, int zoomLevel = 14)
        {
            if (!_connectivityService.IsConnected)
            {
                throw new InvalidOperationException("İnternet bağlantısı gerekli");
            }

            // Zoom seviyesini sınırla
            zoomLevel = Math.Clamp(zoomLevel, MIN_ZOOM_LEVEL, MAX_ZOOM_LEVEL);

            var tiles = CalculateTilesForRegion(northLat, southLat, westLng, eastLng, zoomLevel);

            foreach (var tile in tiles)
            {
                await DownloadAndCacheTileAsync(tile.X, tile.Y, zoomLevel);
                await Task.Delay(100); // API rate limiting için
            }
        }

        /// <summary>
        /// Bölge için gerekli tile koordinatlarını hesaplar
        /// </summary>
        private List<MapTile> CalculateTilesForRegion(double northLat, double southLat, double westLng, double eastLng, int zoom)
        {
            var tiles = new List<MapTile>();

            // Latitude ve longitude'u tile koordinatlarına dönüştür
            var northTile = LatToTileY(northLat, zoom);
            var southTile = LatToTileY(southLat, zoom);
            var westTile = LngToTileX(westLng, zoom);
            var eastTile = LngToTileX(eastLng, zoom);

            // Tile grid'ini oluştur
            for (int y = Math.Min(northTile, southTile); y <= Math.Max(northTile, southTile); y++)
            {
                for (int x = Math.Min(westTile, eastTile); x <= Math.Max(westTile, eastTile); x++)
                {
                    tiles.Add(new MapTile
                    {
                        X = x,
                        Y = y,
                        Zoom = zoom,
                        LastAccessed = DateTime.Now
                    });
                }
            }

            return tiles;
        }

        /// <summary>
        /// Tek bir tile'ı indirir ve önbelleğe alır
        /// </summary>
        private async Task DownloadAndCacheTileAsync(int x, int y, int zoom)
        {
            try
            {
                // OpenStreetMap tile URL'i (ücretsiz)
                var tileUrl = $"https://tile.openstreetmap.org/{zoom}/{x}/{y}.png";

                var response = await _httpClient.GetAsync(tileUrl);
                if (!response.IsSuccessStatusCode)
                    return;

                var tileData = await response.Content.ReadAsByteArrayAsync();

                // Tile'ı veritabanına kaydet
                await SaveTileToDatabaseAsync(x, y, zoom, tileData);

                // İsteğe bağlı: dosyaya da kaydet
                await SaveTileToFileAsync(x, y, zoom, tileData);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Tile indirme hatası ({x},{y},{zoom}): {ex.Message}");
            }
        }

        /// <summary>
        /// Önbellekten tile'ı alır
        /// </summary>
        public async Task<byte[]> GetCachedTileAsync(int x, int y, int zoom)
        {
            // Önce veritabanından dene
            var tile = await _databaseService.GetMapTileAsync(x, y, zoom);
            if (tile != null)
            {
                // Son erişim zamanını güncelle
                tile.LastAccessed = DateTime.Now;
                await _databaseService.UpdateMapTileAsync(tile);
                return tile.TileData;
            }

            // Dosyadan dene
            return await LoadTileFromFileAsync(x, y, zoom);
        }

        /// <summary>
        /// Tile'ın önbellekte olup olmadığını kontrol eder
        /// </summary>
        public async Task<bool> IsTileCachedAsync(int x, int y, int zoom)
        {
            var tile = await _databaseService.GetMapTileAsync(x, y, zoom);
            if (tile != null)
                return true;

            return await File.ExistsAsync(GetTileFilePath(x, y, zoom));
        }

        /// <summary>
        /// Eski tiles'ları temizler
        /// </summary>
        public async Task CleanOldTilesAsync(int daysOld = 30)
        {
            var cutoffDate = DateTime.Now.AddDays(-daysOld);
            await _databaseService.DeleteOldMapTilesAsync(cutoffDate);

            // Dosya sistemindeki eski tile'ları da temizle
            await CleanOldTileFilesAsync(cutoffDate);
        }

        /// <summary>
        /// Önbellek istatistiklerini alır
        /// </summary>
        public async Task<MapCacheStats> GetCacheStatsAsync()
        {
            var tileCount = await _databaseService.GetMapTileCountAsync();
            var totalSize = await _databaseService.GetMapTilesTotalSizeAsync();
            var oldestTile = await _databaseService.GetOldestMapTileAsync();

            return new MapCacheStats
            {
                TotalTiles = tileCount,
                TotalSizeBytes = totalSize,
                OldestTileDate = oldestTile?.LastAccessed
            };
        }

        #region Yardımcı Metodlar

        private async Task SaveTileToDatabaseAsync(int x, int y, int zoom, byte[] tileData)
        {
            var tile = new MapTile
            {
                X = x,
                Y = y,
                Zoom = zoom,
                TileData = tileData,
                DownloadedDate = DateTime.Now,
                LastAccessed = DateTime.Now
            };

            await _databaseService.SaveMapTileAsync(tile);
        }

        private async Task SaveTileToFileAsync(int x, int y, int zoom, byte[] tileData)
        {
            var filePath = GetTileFilePath(x, y, zoom);
            var directory = Path.GetDirectoryName(filePath);

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            await File.WriteAllBytesAsync(filePath, tileData);
        }

        private async Task<byte[]> LoadTileFromFileAsync(int x, int y, int zoom)
        {
            var filePath = GetTileFilePath(x, y, zoom);
            if (File.Exists(filePath))
            {
                return await File.ReadAllBytesAsync(filePath);
            }
            return null;
        }

        private async Task CleanOldTileFilesAsync(DateTime cutoffDate)
        {
            var tileDir = Path.Combine(FileSystem.AppDataDirectory, TILE_CACHE_DIR);
            if (!Directory.Exists(tileDir))
                return;

            var files = Directory.GetFiles(tileDir, "*.png", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                try
                {
                    var fileInfo = new FileInfo(file);
                    if (fileInfo.LastAccessTime < cutoffDate)
                    {
                        File.Delete(file);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Dosya silme hatası: {ex.Message}");
                }
            }
        }

        private string GetTileFilePath(int x, int y, int zoom)
        {
            var tileDir = Path.Combine(FileSystem.AppDataDirectory, TILE_CACHE_DIR, zoom.ToString(), x.ToString());
            return Path.Combine(tileDir, $"{y}.png");
        }

        // Tile koordinat dönüşüm yardımcıları
        private int LngToTileX(double lng, int zoom)
        {
            return (int)Math.Floor((lng + 180.0) / 360.0 * Math.Pow(2.0, zoom));
        }

        private int LatToTileY(double lat, int zoom)
        {
            return (int)Math.Floor((1.0 - Math.Log(Math.Tan(lat * Math.PI / 180.0) + 1.0 / Math.Cos(lat * Math.PI / 180.0)) / Math.PI) / 2.0 * Math.Pow(2.0, zoom));
        }

        #endregion
    }

    /// <summary>
    /// Harita önbellek istatistikleri
    /// </summary>
    public class MapCacheStats
    {
        public int TotalTiles { get; set; }
        public long TotalSizeBytes { get; set; }
        public DateTime? OldestTileDate { get; set; }

        public string TotalSizeFormatted => $"{TotalSizeBytes / 1024.0 / 1024.0:F2} MB";
    }
}
