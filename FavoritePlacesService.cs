// Services/FavoritePlacesService.cs
public class FavoritePlacesService : IFavoritePlacesService
{
    private readonly SQLiteConnection _database;
    
    public FavoritePlacesService(SQLiteConnection database)
    {
        _database = database;
        _database.CreateTable<FavoritePlace>();
    }
    
    public List<FavoritePlace> GetAllFavoritePlaces()
    {
        return _database.Table<FavoritePlace>().OrderBy(p => p.Name).ToList();
    }
    
    public List<FavoritePlace> GetFavoritePlacesByCategory(string category)
    {
        return _database.Table<FavoritePlace>()
            .Where(p => p.Category == category)
            .OrderBy(p => p.Name)
            .ToList();
    }
    
    public FavoritePlace GetFavoritePlaceById(int id)
    {
        return _database.Get<FavoritePlace>(id);
    }
    
    public int AddFavoritePlace(FavoritePlace place)
    {
        place.CreatedAt = DateTime.UtcNow;
        return _database.Insert(place);
    }
    
    public bool UpdateFavoritePlace(FavoritePlace place)
    {
        place.UpdatedAt = DateTime.UtcNow;
        return _database.Update(place) > 0;
    }
    
    public bool DeleteFavoritePlace(int id)
    {
        return _database.Delete<FavoritePlace>(id) > 0;
    }
    
    public List<string> GetAllCategories()
    {
        return _database.Table<FavoritePlace>()
            .Select(p => p.Category)
            .Distinct()
            .ToList();
    }
}

// Models/FavoritePlace.cs
public class FavoritePlace
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
    
    public string Category { get; set; } = "Genel";
    
    public string IconName { get; set; } = "map_marker";
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? UpdatedAt { get; set; }
    
    public bool IsPinned { get; set; } = false;
}