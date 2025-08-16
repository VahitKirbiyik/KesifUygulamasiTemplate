using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KesifUygulamasiTemplate.Models;
using SQLite;

namespace KesifUygulamasiTemplate.Services
{
    public class FavoritePlacesService : IFavoritePlacesService
    {
        private readonly SQLiteAsyncConnection _database;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        
        public FavoritePlacesService(SQLiteAsyncConnection database)
        {
            _database = database;
            
            // Veritaban� tablosunu olu�tur
            _database.CreateTableAsync<FavoritePlace>().Wait();
        }
        
        public async Task<int> AddFavoritePlaceAsync(FavoritePlace place)
        {
            if (place == null)
                throw new ArgumentNullException(nameof(place));
            
            await _semaphore.WaitAsync();
            try
            {
                place.CreatedAt = DateTime.UtcNow;
                return await _database.InsertAsync(place);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Favori yer ekleme hatas�: {ex.Message}");
                return -1;
            }
            finally
            {
                _semaphore.Release();
            }
        }
        
        public async Task<IEnumerable<FavoritePlace>> GetAllFavoritePlacesAsync()
        {
            try
            {
                // T�m favori yerleri al ve isme g�re s�rala
                var places = await _database.Table<FavoritePlace>().ToListAsync();
                
                // �nce sabitlenmi� (pinned) olanlar�, sonra isme g�re s�rala
                return places
                    .OrderByDescending(p => p.IsPinned)
                    .ThenBy(p => p.Name);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Favori yerleri getirme hatas�: {ex.Message}");
                return Enumerable.Empty<FavoritePlace>();
            }
        }
        
        public async Task<bool> DeleteFavoritePlaceAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Ge�ersiz ID");
                
            await _semaphore.WaitAsync();
            try
            {
                int result = await _database.DeleteAsync<FavoritePlace>(id);
                return result > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Favori yer silme hatas�: {ex.Message}");
                return false;
            }
            finally
            {
                _semaphore.Release();
            }
        }
        
        public async Task<FavoritePlace> GetFavoritePlaceByIdAsync(int id)
        {
            try
            {
                return await _database.GetAsync<FavoritePlace>(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Favori yer getirme hatas� (ID={id}): {ex.Message}");
                return null;
            }
        }
        
        public async Task<bool> UpdateFavoritePlaceAsync(FavoritePlace place)
        {
            if (place == null)
                throw new ArgumentNullException(nameof(place));
                
            if (place.Id <= 0)
                throw new ArgumentException("G�ncellenecek favori yerin ge�erli bir ID'si olmal�d�r");
                
            await _semaphore.WaitAsync();
            try
            {
                place.UpdatedAt = DateTime.UtcNow;
                int result = await _database.UpdateAsync(place);
                return result > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Favori yer g�ncelleme hatas�: {ex.Message}");
                return false;
            }
            finally
            {
                _semaphore.Release();
            }
        }
        
        public async Task<IEnumerable<FavoritePlace>> GetFavoritePlacesByCategoryAsync(string category)
        {
            if (string.IsNullOrEmpty(category))
                return await GetAllFavoritePlacesAsync();
                
            try
            {
                var places = await _database.Table<FavoritePlace>()
                    .Where(p => p.Category == category)
                    .ToListAsync();
                    
                return places
                    .OrderByDescending(p => p.IsPinned)
                    .ThenBy(p => p.Name);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Kategoriye g�re favori yerleri getirme hatas�: {ex.Message}");
                return Enumerable.Empty<FavoritePlace>();
            }
        }
        
        public async Task<IEnumerable<string>> GetAllCategoriesAsync()
        {
            try
            {
                // T�m kategorileri al (benzersiz)
                return await _database.Table<FavoritePlace>()
                    .Select(p => p.Category)
                    .Distinct()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Kategorileri getirme hatas�: {ex.Message}");
                return Enumerable.Empty<string>();
            }
        }
        
        // Ek yard�mc� metodlar
        
        public async Task<IEnumerable<FavoritePlace>> SearchFavoritePlacesAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetAllFavoritePlacesAsync();
                
            try
            {
                // Arama terimini k���k harfe �evir
                searchTerm = searchTerm.ToLowerInvariant();
                
                // T�m yerleri al ve sonra filtreleme yap (SQLite LIKE sorgusu s�n�rl� oldu�u i�in)
                var allPlaces = await _database.Table<FavoritePlace>().ToListAsync();
                
                // �sim, a��klama veya adres i�inde arama terimini i�eren yerleri filtrele
                return allPlaces
                    .Where(p => 
                        p.Name.ToLowerInvariant().Contains(searchTerm) ||
                        (p.Description != null && p.Description.ToLowerInvariant().Contains(searchTerm)) ||
                        (p.Address != null && p.Address.ToLowerInvariant().Contains(searchTerm)))
                    .OrderByDescending(p => p.IsPinned)
                    .ThenBy(p => p.Name);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Favori yerlerde arama hatas�: {ex.Message}");
                return Enumerable.Empty<FavoritePlace>();
            }
        }
        
        public async Task<IEnumerable<FavoritePlace>> GetNearbyFavoritePlacesAsync(double latitude, double longitude, double radiusKm = 5.0)
        {
            try
            {
                // T�m yerleri al
                var allPlaces = await _database.Table<FavoritePlace>().ToListAsync();
                
                // Mesafeye g�re filtrele
                return allPlaces
                    .Select(p => new
                    {
                        Place = p,
                        Distance = CalculateDistance(latitude, longitude, p.Latitude, p.Longitude)
                    })
                    .Where(x => x.Distance <= radiusKm)
                    .OrderBy(x => x.Distance)
                    .Select(x => x.Place);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Yak�ndaki favori yerleri getirme hatas�: {ex.Message}");
                return Enumerable.Empty<FavoritePlace>();
            }
        }
        
        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371; // D�nya yar��ap� (km)
            var dLat = ToRadians(lat2 - lat1);
            var dLon = ToRadians(lon2 - lon1);
            
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
                    
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }
        
        private double ToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }
    }
}