// Services/SearchHistoryService.cs
public class SearchHistoryService : ISearchHistoryService
{
    private readonly SQLiteConnection _database;
    private const int MAX_HISTORY_ITEMS = 50;
    
    public SearchHistoryService(SQLiteConnection database)
    {
        _database = database;
        _database.CreateTable<SearchHistoryItem>();
    }
    
    public void AddSearchQuery(string query, double? latitude = null, double? longitude = null)
    {
        var existing = _database.Table<SearchHistoryItem>()
            .FirstOrDefault(h => h.Query.ToLower() == query.ToLower());
            
        if (existing != null)
        {
            existing.SearchCount++;
            existing.LastSearchedAt = DateTime.UtcNow;
            if (latitude.HasValue && longitude.HasValue)
            {
                existing.Latitude = latitude;
                existing.Longitude = longitude;
            }
            _database.Update(existing);
        }
        else
        {
            _database.Insert(new SearchHistoryItem
            {
                Query = query,
                SearchCount = 1,
                LastSearchedAt = DateTime.UtcNow,
                Latitude = latitude,
                Longitude = longitude
            });
            
            // Limit the history size
            var count = _database.Table<SearchHistoryItem>().Count();
            if (count > MAX_HISTORY_ITEMS)
            {
                var oldest = _database.Table<SearchHistoryItem>()
                    .OrderBy(h => h.LastSearchedAt)
                    .First();
                _database.Delete<SearchHistoryItem>(oldest.Id);
            }
        }
    }
    
    public List<string> GetSuggestions(string partialQuery, int limit = 5)
    {
        return _database.Table<SearchHistoryItem>()
            .Where(h => h.Query.ToLower().Contains(partialQuery.ToLower()))
            .OrderByDescending(h => h.SearchCount)
            .ThenByDescending(h => h.LastSearchedAt)
            .Take(limit)
            .Select(h => h.Query)
            .ToList();
    }
    
    public List<SearchHistoryItem> GetRecentSearches(int limit = 10)
    {
        return _database.Table<SearchHistoryItem>()
            .OrderByDescending(h => h.LastSearchedAt)
            .Take(limit)
            .ToList();
    }
    
    public void ClearSearchHistory()
    {
        _database.DeleteAll<SearchHistoryItem>();
    }
}

// Models/SearchHistoryItem.cs
public class SearchHistoryItem
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    
    [Required, MaxLength(200)]
    public string Query { get; set; }
    
    public int SearchCount { get; set; } = 1;
    
    public DateTime LastSearchedAt { get; set; } = DateTime.UtcNow;
    
    public double? Latitude { get; set; }
    
    public double? Longitude { get; set; }
}