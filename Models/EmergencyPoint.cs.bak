using System;
using System.Collections.Generic;
using SQLite;

namespace KesifUygulamasiTemplate.Models
{
    [Table("EmergencyPoints")]
    public class EmergencyPoint
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public EmergencyPointType Type { get; set; } = EmergencyPointType.All;

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public string Address { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public bool IsVerified { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? LastUpdated { get; set; }

        // Optional external id from remote APIs
        public string ExternalId { get; set; } = string.Empty;
    }

    public enum EmergencyPointType
    {
        All = 0,
        Hospital = 1,
        PoliceStation = 2,
        FireStation = 3,
        Pharmacy = 4,
        GasStation = 5,
        EmergencyShelter = 6,
        Other = 99
    }
}
