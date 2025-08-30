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
        /// Ay fazı (0.0 - 1.0 arası, 0 = Yeni Ay, 1 = Dolunay)
        /// </summary>
        public double Phase { get; set; }

        /// <summary>
        /// Ayın azimuth açısı (derece)
        /// </summary>
        public double Azimuth { get; set; }

        /// <summary>
        /// Ayın yükseklik açısı (derece)
        /// </summary>
        public double Altitude { get; set; }

        /// <summary>
        /// Ay fazının adı (Türkçe)
        /// </summary>
        public string PhaseName { get; set; } = string.Empty;

        /// <summary>
        /// Ayın aydınlanma yüzdesi (0.0 - 1.0)
        /// </summary>
        public double Illumination { get; set; }

        /// <summary>
        /// Ay ile Dünya arasındaki mesafe (km)
        /// </summary>
        public double Distance { get; set; }

        /// <summary>
        /// Ayın görünür büyüklüğü
        /// </summary>
        public double ApparentMagnitude { get; set; }

        /// <summary>
        /// Ay fazının emoji temsili
        /// </summary>
        public string PhaseEmoji => Phase switch
        {
            <= 0.05 => "??", // Yeni Ay
            <= 0.25 => "??", // Hilal
            <= 0.45 => "??", // İlk Dördün
            <= 0.55 => "??", // Şişkin
            <= 0.75 => "??", // Dolunay
            <= 0.95 => "??", // Azalan Şişkin
            _ => "??"         // Son Dördün / Azalan Hilal
        };
    }
}
