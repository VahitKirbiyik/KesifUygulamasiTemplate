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
        /// Ay faz� (0.0 - 1.0 aras�, 0 = Yeni Ay, 1 = Dolunay)
        /// </summary>
        public double Phase { get; set; }
        
        /// <summary>
        /// Ay�n azimuth a��s� (derece)
        /// </summary>
        public double Azimuth { get; set; }
        
        /// <summary>
        /// Ay�n y�kseklik a��s� (derece)
        /// </summary>
        public double Altitude { get; set; }
        
        /// <summary>
        /// Ay faz�n�n ad� (T�rk�e)
        /// </summary>
        public string PhaseName { get; set; } = string.Empty;
        
        /// <summary>
        /// Ay�n ayd�nlanma y�zdesi (0.0 - 1.0)
        /// </summary>
        public double Illumination { get; set; }
        
        /// <summary>
        /// Ay ile D�nya aras�ndaki mesafe (km)
        /// </summary>
        public double Distance { get; set; }
        
        /// <summary>
        /// Ay�n g�r�n�r b�y�kl���
        /// </summary>
        public double ApparentMagnitude { get; set; }
        
        /// <summary>
        /// Ay faz�n�n emoji temsili
        /// </summary>
        public string PhaseEmoji => Phase switch
        {
            <= 0.05 => "??", // Yeni Ay
            <= 0.25 => "??", // Hilal
            <= 0.45 => "??", // �lk D�rd�n
            <= 0.55 => "??", // �i�kin
            <= 0.75 => "??", // Dolunay
            <= 0.95 => "??", // Azalan �i�kin
            _ => "??"         // Son D�rd�n / Azalan Hilal
        };
    }
}