using System;
using System.Collections.Generic;

namespace KesifUygulamasiTemplate.Models
{
    public class StreetViewPanorama
    {
        public string Id { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Heading { get; set; }
        public double Pitch { get; set; }
        public string Copyright { get; set; } = string.Empty;
        public DateTime? DateCaptured { get; set; }
        public List<StreetViewLink> Links { get; set; } = new List<StreetViewLink>();
    }
}
