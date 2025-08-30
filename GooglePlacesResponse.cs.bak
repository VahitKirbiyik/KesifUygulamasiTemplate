using System.Collections.Generic;

namespace KesifUygulamasi.Models
{
    public class GooglePlacesResponse
    {
        public List<GooglePlaceResult> Results { get; set; } = new List<GooglePlaceResult>();
    }

    public class GooglePlaceResult
    {
        public string Name { get; set; } = string.Empty;
        public string Vicinity { get; set; } = string.Empty;
        public GooglePlaceGeometry Geometry { get; set; } = new GooglePlaceGeometry();
    }

    public class GooglePlaceGeometry
    {
        public GooglePlaceLocation Location { get; set; } = new GooglePlaceLocation();
    }

    public class GooglePlaceLocation
    {
        public double Lat { get; set; }
        public double Lng { get; set; }
    }
}
