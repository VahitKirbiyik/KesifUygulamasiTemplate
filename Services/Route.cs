using System;
using System.Collections.Generic;
using Microsoft.Maui.Devices.Sensors;

namespace KesifUygulamasiTemplate.Models
{
    public class Route
    {
        public string RouteId { get; set; } = Guid.NewGuid().ToString();
        public Location Start { get; set; }
        public Location End { get; set; }
        public List<Location> Path { get; set; } = new List<Location>();
        public List<RouteStep> Steps { get; set; } = new List<RouteStep>();
        public double DistanceKm { get; set; }
        public TimeSpan Duration { get; set; }
        public TransportMode TransportMode { get; set; }
    }
    
    public class RouteStep
    {
        public string Instruction { get; set; }
        public Location StartLocation { get; set; }
        public Location EndLocation { get; set; }
        public double DistanceKm { get; set; }
        public TimeSpan Duration { get; set; }
        public string ManeuverType { get; set; } // turn-right, turn-left, etc.
    }
    
    public enum TransportMode
    {
        Driving,
        Walking,
        Bicycling,
        Transit
    }
}