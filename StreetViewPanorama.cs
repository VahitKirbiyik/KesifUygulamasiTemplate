using System;
using System.Collections.Generic;

namespace KesifUygulamasiTemplate.Models
{
    public class StreetViewPanorama
    {
        public string PanoramaId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Heading { get; set; }
        public double Pitch { get; set; }
        public string Copyright { get; set; }
        public DateTime DateCaptured { get; set; }
        public List<StreetViewLink> Links { get; set; } = new List<StreetViewLink>();
    }

    public class StreetViewLink
    {
        public string PanoramaId { get; set; }
        public string Description { get; set; }
        public double Heading { get; set; }
    }
}