using System;

namespace KesifUygulamasiTemplate.Models
{
    /// <summary>
    /// Harita tile modeli - offline caching için
    /// </summary>
    public class MapTile
    {
        public int Id { get; set; }
        public int X { get; set; } // Tile X koordinatı
        public int Y { get; set; } // Tile Y koordinatı
        public int Zoom { get; set; } // Zoom seviyesi
        public byte[] TileData { get; set; } // PNG tile verisi
        public byte[] Data => TileData; // Testler için alias
        public DateTime DownloadedDate { get; set; } // İndirilme tarihi
        public DateTime LastAccessed { get; set; } // Son erişim tarihi

        /// <summary>
        /// Tile'ın benzersiz anahtarı
        /// </summary>
        public string TileKey => $"{Zoom}_{X}_{Y}";

        /// <summary>
        /// Tile boyutunu byte cinsinden döndürür
        /// </summary>
        public long SizeInBytes => TileData?.Length ?? 0;

        /// <summary>
        /// Tile'ın ne kadar süredir önbellekte olduğunu hesaplar
        /// </summary>
        public TimeSpan Age => DateTime.Now - DownloadedDate;

        /// <summary>
        /// Tile'ın son erişimden ne kadar süre geçtiğini hesaplar
        /// </summary>
        public TimeSpan TimeSinceLastAccess => DateTime.Now - LastAccessed;
    }
}
