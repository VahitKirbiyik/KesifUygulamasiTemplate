namespace KesifUygulamasiTemplate.Models
{
    public class TrafficInfo
    {
        public int CongestionLevel { get; set; } // 0-100 aras� (0: ak�c�, 100: �ok yo�un)
        public TimeSpan DelayTime { get; set; }
        public TimeSpan TypicalTravelTime { get; set; }
        public TimeSpan CurrentTravelTime { get; set; }
        public DateTime LastUpdated { get; set; }
        
        public string CongestionDescription => CongestionLevel switch
        {
            < 10 => "Ak�c�",
            < 30 => "Az yo�un",
            < 60 => "Orta yo�un",
            < 85 => "Yo�un",
            _ => "�ok yo�un"
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
            4 => "�ok ciddi",
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