using SQLite;
using KesifUygulamasiTemplate.Services.Interfaces;
using KesifUygulamasiTemplate.Models;

// Services/FavoritePlacesService.cs
public class FavoritePlacesService : IFavoritePlacesService
{
    private readonly SQLiteConnection _database;

    public FavoritePlacesService(SQLiteConnection database)
    {
        _database = database;
        _database.CreateTable<FavoritePlace>();
    }

    // IFavoritePlacesService implementation
    public async Task AddFavoriteAsync(LocationModel place)
    {
        var favoritePlace = new FavoritePlace
        {
            Name = place.Name ?? "",
            Description = "",
            Latitude = place.Latitude,
            Longitude = place.Longitude,
            Address = place.Address ?? "",
            Category = place.Category ?? "Genel",
            IconName = place.IconName ?? "map_marker"
        };
        await Task.Run(() => _database.Insert(favoritePlace));
    }

    public async Task RemoveFavoriteAsync(string id)
    {
        if (int.TryParse(id, out int placeId))
        {
            await Task.Run(() => _database.Delete<FavoritePlace>(placeId));
        }
    }

    public async Task<IEnumerable<LocationModel>> GetFavoritesAsync()
    {
        var favorites = await Task.Run(() =>
            _database.Table<FavoritePlace>().ToList());

        return favorites.Select(f => new LocationModel
        {
            Name = f.Name,
            Latitude = f.Latitude,
            Longitude = f.Longitude,
            Address = f.Address,
            Category = f.Category,
            IconName = f.IconName
        });
    }

    public async Task<IEnumerable<LocationModel>> GetAllFavoritePlacesAsync()
    {
        return await GetFavoritesAsync();
    }

    // Existing methods
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

    public string Name { get; set; } = "";

    public string Description { get; set; } = "";

    public double Latitude { get; set; }

    public double Longitude { get; set; }

    public string Address { get; set; } = "";

    public string Category { get; set; } = "Genel";

    public string IconName { get; set; } = "map_marker";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public bool IsPinned { get; set; } = false;
}
