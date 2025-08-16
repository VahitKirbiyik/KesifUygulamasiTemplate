using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Maui.Networking;
using SQLite;

namespace KesifUygulamasiTemplate.Services
{
    public class MapDataService : IMapDataService
    {
        private readonly SQLiteAsyncConnection _database;
        private readonly IConnectivity _connectivity;
        private readonly HttpClient _httpClient;
        private readonly string _mapTileServerUrl;
        private const int MAX_CACHE_SIZE_MB = 500; // 500 MB maksimum önbellek
        private const int TILE_EXPIRY_DAYS = 30; // 30 gün tile geçerlilik süresi
        
        public MapDataService(SQLiteAsyncConnection database, IConnectivity connectivity, HttpClient httpClient)
        {
            _database = database;
            _connectivity = connectivity;
            _httpClient = httpClient;
            _mapTileServerUrl = "https://tile.openstreetmap.org/{z}/{x}/{y}.png";
            
            // Veritabanı tablosunu oluştur
            _database.CreateTableAsync<MapTile>().Wait();
        }
        
        public async Task<bool> SyncMapDataAsync(double latitude, double longitude, int radiusKm, int maxZoom = 15)
        {
            if (_connectivity.NetworkAccess != NetworkAccess.Internet)
                return false;
                
            try
            {
                // Harita merkezini ve kapsama alanını hesapla
                var boundingBox = CalculateBoundingBox(latitude, longitude, radiusKm);
                
                // Her bir zoom seviyesi için tile'ları hesapla ve indir
                for (int zoom = 10; zoom <= maxZoom; zoom++)
                {
                    var tiles = CalculateTiles(boundingBox.North, boundingBox.South, 
                                              boundingBox.East, boundingBox.West, zoom);
                    
                    foreach (var tile in tiles)
                    {
                        // Tile veritabanında mevcut mu kontrol et
                        var existingTile = await _database.Table<MapTile>()
                            .Where(t => t.X == tile.X && t.Y == tile.Y && t.ZoomLevel == zoom)
                            .FirstOrDefaultAsync();
                        
                        // Eğer tile yoksa veya eskiyse, indir ve kaydet
                        if (existingTile == null || 
                            (DateTime.UtcNow - existingTile.LastUpdated).TotalDays > TILE_EXPIRY_DAYS)
                        {
                            await DownloadAndSaveTileAsync(tile.X, tile.Y, zoom, 
                                                        tile.North, tile.South, tile.East, tile.West);
                        }
                    }
                }
                
                // Önbellek boyutunu kontrol et ve gerekirse temizle
                await CleanupCacheIfNeededAsync();
                
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Harita verisi senkronizasyon hatası: {ex.Message}");
                return false;
            }
        }
        
        public async Task<IEnumerable<MapTile>> GetOfflineTilesAsync(double north, double south, double east, double west, int zoomLevel)
        {
            try
            {
                return await _database.Table<MapTile>()
                    .Where(t => t.North >= south && t.South <= north && 
                              t.East >= west && t.West <= east &&
                              t.ZoomLevel == zoomLevel)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Çevrimdışı harita verileri getirme hatası: {ex.Message}");
                return Enumerable.Empty<MapTile>();
            }
        }
        
        public async Task<bool> HasOfflineCoverageAsync(double latitude, double longitude, int zoomLevel)
        {
            try
            {
                var (x, y) = LatLongToTile(latitude, longitude, zoomLevel);
                
                var tile = await _database.Table<MapTile>()
                    .Where(t => t.X == x && t.Y == y && t.ZoomLevel == zoomLevel)
                    .FirstOrDefaultAsync();
                    
                return tile != null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Çevrimdışı kapsama kontrolü hatası: {ex.Message}");
                return false;
            }
        }
        
        public async Task<int> GetOfflineMapSizeMBAsync()
        {
            try
            {
                var sum = await _database.ExecuteScalarAsync<long>("SELECT SUM(length(Data)) FROM MapTile");
                return (int)(sum / (1024 * 1024)); // Byte -> MB
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Önbellek boyutu hesaplama hatası: {ex.Message}");
                return 0;
            }
        }
        
        public async Task<bool> ClearExpiredTilesAsync()
        {
            try
            {
                var expiryDate = DateTime.UtcNow.AddDays(-TILE_EXPIRY_DAYS);
                await _database.ExecuteAsync("DELETE FROM MapTile WHERE LastUpdated < ?", expiryDate);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Süresi dolmuş tile'ları temizleme hatası: {ex.Message}");
                return false;
            }
        }
        
        public async Task<bool> ClearAllTilesAsync()
        {
            try
            {
                await _database.DeleteAllAsync<MapTile>();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Tüm tile'ları temizleme hatası: {ex.Message}");
                return false;
            }
        }
        
        // Yardımcı metotlar
        private async Task<bool> DownloadAndSaveTileAsync(int x, int y, int zoom, 
                                                       double north, double south, 
                                                       double east, double west)
        {
            try
            {
                var url = _mapTileServerUrl
                    .Replace("{z}", zoom.ToString())
                    .Replace("{x}", x.ToString())
                    .Replace("{y}", y.ToString());
                
                var tileData = await _httpClient.GetByteArrayAsync(url);
                
                var existingTile = await _database.Table<MapTile>()
                    .Where(t => t.X == x && t.Y == y && t.ZoomLevel == zoom)
                    .FirstOrDefaultAsync();
                
                if (existingTile != null)
                {
                    existingTile.Data = tileData;
                    existingTile.LastUpdated = DateTime.UtcNow;
                    await _database.UpdateAsync(existingTile);
                }
                else
                {
                    await _database.InsertAsync(new MapTile
                    {
                        X = x,
                        Y = y,
                        ZoomLevel = zoom,
                        North = north,
                        South = south,
                        East = east,
                        West = west,
                        Data = tileData,
                        LastUpdated = DateTime.UtcNow
                    });
                }
                
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Tile indirme hatası: {ex.Message}");
                return false;
            }
        }
        
        private async Task CleanupCacheIfNeededAsync()
        {
            var currentSize = await GetOfflineMapSizeMBAsync();
            if (currentSize > MAX_CACHE_SIZE_MB)
            {
                // Önce süresi dolmuş tile'ları temizle
                await ClearExpiredTilesAsync();
                
                // Boyut hala büyükse, en eski tile'ları sil
                currentSize = await GetOfflineMapSizeMBAsync();
                if (currentSize > MAX_CACHE_SIZE_MB)
                {
                    var tilesToDelete = await _database.Table<MapTile>()
                        .OrderBy(t => t.LastUpdated)
                        .Take((currentSize - MAX_CACHE_SIZE_MB / 2) * 10) // Yarı boyuta inmek için kabaca hesap
                        .ToListAsync();
                    
                    foreach (var tile in tilesToDelete)
                    {
                        await _database.DeleteAsync(tile);
                    }
                }
            }
        }
        
        private (double North, double South, double East, double West) CalculateBoundingBox(
            double lat, double lng, int radiusKm)
        {
            // Dünya yarıçapı (km)
            const double R = 6371.0;
            
            // Enlem için 1 derece ~= 111 km
            double latDelta = radiusKm / 111.0;
            
            // Boylam için 1 derece, enleme bağlı olarak değişir
            double lonDelta = radiusKm / (111.0 * Math.Cos(lat * Math.PI / 180.0));
            
            return (
                North: lat + latDelta,
                South: lat - latDelta,
                East: lng + lonDelta,
                West: lng - lonDelta
            );
        }
        
        private List<(int X, int Y, double North, double South, double East, double West)> CalculateTiles(
            double north, double south, double east, double west, int zoom)
        {
            var result = new List<(int X, int Y, double North, double South, double East, double West)>();
            
            var nwTile = LatLongToTile(north, west, zoom);
            var seTile = LatLongToTile(south, east, zoom);
            
            for (int x = nwTile.X; x <= seTile.X; x++)
            {
                for (int y = nwTile.Y; y <= seTile.Y; y++)
                {
                    var tileNorth = TileToLat(y, zoom);
                    var tileSouth = TileToLat(y + 1, zoom);
                    var tileWest = TileToLon(x, zoom);
                    var tileEast = TileToLon(x + 1, zoom);
                    
                    result.Add((x, y, tileNorth, tileSouth, tileEast, tileWest));
                }
            }
            
            return result;
        }
        
        private (int X, int Y) LatLongToTile(double lat, double lon, int zoom)
        {
            int x = (int)Math.Floor((lon + 180.0) / 360.0 * (1 << zoom));
            int y = (int)Math.Floor((1.0 - Math.Log(Math.Tan(lat * Math.PI / 180.0) + 
                    1.0 / Math.Cos(lat * Math.PI / 180.0)) / Math.PI) / 2.0 * (1 << zoom));
            return (x, y);
        }
        
        private double TileToLat(int y, int zoom)
        {
            double n = Math.PI - (2.0 * Math.PI * y) / Math.Pow(2.0, zoom);
            return 180.0 / Math.PI * Math.Atan(Math.Sinh(n));
        }
        
        private double TileToLon(int x, int zoom)
        {
            return x / Math.Pow(2.0, zoom) * 360.0 - 180.0;
        }
    }
}