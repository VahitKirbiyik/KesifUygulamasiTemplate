using System;
using SQLite;

namespace KesifUygulamasiTemplate.Models
{
    [Table("EmergencyPoints")]
    public class EmergencyPoint
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        
        [Required, MaxLength(100)]
        public string Name { get; set; }
        
        public string Description { get; set; }
        
        [Required]
        public double Latitude { get; set; }
        
        [Required]
        public double Longitude { get; set; }
        
        public string Address { get; set; }
        
        public string PhoneNumber { get; set; }
        
        [Required]
        public EmergencyPointType Type { get; set; }
        
        public bool IsVerified { get; set; } = false;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? UpdatedAt { get; set; }
        
        // API kaynak kimliði (opsiyonel)
        public string ExternalId { get; set; }
    }
    
    public enum EmergencyPointType
    {
        All = 0,
        Hospital = 1,
        PoliceStation = 2,
        FireStation = 3,
        Pharmacy = 4,
        GasStation = 5,
        EmergencyShelter = 6
    }
}