using System;
using SQLite;

namespace KesifUygulamasiTemplate.Models
{
    public class MoonData
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime Date { get; set; }
        public DateTime RiseTime { get; set; }
        public DateTime SetTime { get; set; }
        
        /// <summary>
        /// Ay fazý (0.0 - 1.0 arasý, 0 = Yeni Ay, 1 = Dolunay)
        /// </summary>
        public double Phase { get; set; }
        
        /// <summary>
        /// Ayýn azimuth açýsý (derece)
        /// </summary>
        public double Azimuth { get; set; }
        
        /// <summary>
        /// Ayýn yükseklik açýsý (derece)
        /// </summary>
        public double Altitude { get; set; }
        
        /// <summary>
        /// Ay fazýnýn adý (Türkçe)
        /// </summary>
        public string PhaseName { get; set; } = string.Empty;
        
        /// <summary>
        /// Ayýn aydýnlanma yüzdesi (0.0 - 1.0)
        /// </summary>
        public double Illumination { get; set; }
        
        /// <summary>
        /// Ay ile Dünya arasýndaki mesafe (km)
        /// </summary>
        public double Distance { get; set; }
        
        /// <summary>
        /// Ayýn görünür büyüklüðü
        /// </summary>
        public double ApparentMagnitude { get; set; }
        
        /// <summary>
        /// Ay fazýnýn emoji temsili
        /// </summary>
        public string PhaseEmoji => Phase switch
        {
            <= 0.05 => "??", // Yeni Ay
            <= 0.25 => "??", // Hilal
            <= 0.45 => "??", // Ýlk Dördün
            <= 0.55 => "??", // Þiþkin
            <= 0.75 => "??", // Dolunay
            <= 0.95 => "??", // Azalan Þiþkin
            _ => "??"         // Son Dördün / Azalan Hilal
        };
    }
}