namespace KesifUygulamasiTemplate.Models
{
    public class TrafficInfo
    {
        public int CongestionLevel { get; set; } // 0-100 arasý (0: akýcý, 100: çok yoðun)
        public TimeSpan DelayTime { get; set; }
        public TimeSpan TypicalTravelTime { get; set; }
        public TimeSpan CurrentTravelTime { get; set; }
        public DateTime LastUpdated { get; set; }
        
        public string CongestionDescription => CongestionLevel switch
        {
            < 10 => "Akýcý",
            < 30 => "Az yoðun",
            < 60 => "Orta yoðun",
            < 85 => "Yoðun",
            _ => "Çok yoðun"
        };
    }

    public class TrafficIncident
    {
        public string Id { get; set; }
        public string Type { get; set; } // ACCIDENT, CONSTRUCTION, ROAD_CLOSED, etc.
        public string Description { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int SeverityCode { get; set; } // 1-4 (1: minor, 4: major)
        public string SeverityDescription => SeverityCode switch
        {
            1 => "Az etkili",
            2 => "Orta etkili",
            3 => "Ciddi",
            4 => "Çok ciddi",
            _ => "Bilinmiyor"
        };
    }

    public enum RouteOptimizationPreference
    {
        BestGuess,
        Optimistic,
        Pessimistic
    }
}