using System;
using System.Collections.Generic;
using Microsoft.Maui.Devices.Sensors;
using SQLite;

namespace KesifUygulamasiTemplate.Models
{
    [Table("SavedRoutes")]
    public class SavedRoute
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        
        [Required, MaxLength(100)]
        public string Name { get; set; }
        
        public string Description { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? UpdatedAt { get; set; }
        
        public double StartLatitude { get; set; }
        
        public double StartLongitude { get; set; }
        
        public double EndLatitude { get; set; }
        
        public double EndLongitude { get; set; }
        
        public double DistanceKm { get; set; }
        
        public TimeSpan EstimatedDuration { get; set; }
        
        public string TransportMode { get; set; } = "Driving";
        
        public bool IsFavorite { get; set; } = false;
        
        [Ignore]
        public List<Location> Points { get; set; } = new();
    }
    
    [Table("RoutePoints")]
    public class RoutePoint
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        
        [Indexed]
        public int RouteId { get; set; }
        
        public double Latitude { get; set; }
        
        public double Longitude { get; set; }
        
        public int Sequence { get; set; }
    }
}