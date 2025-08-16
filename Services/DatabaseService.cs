using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;
using KesifUygulamasiTemplate.Models;
using SQLite;

namespace KesifUygulamasiTemplate.Services
{
    public class DatabaseService
    {
        readonly SQLiteAsyncConnection db;
        private bool _isInitialized = false;

        // Parametresiz yapıcı: DI container'dan doğrudan kullanılabilir
        public DatabaseService()
            : this(Path.Combine(FileSystem.AppDataDirectory, "locations.db3"))
        {
        }

        // Asıl yapıcı: verilen dosya yolunu kullanır
        public DatabaseService(string dbPath)
        {
            db = new SQLiteAsyncConnection(dbPath);
            db.CreateTableAsync<MoonData>().Wait();
        }

        // Veritabanını başlatır
        public async Task InitializeAsync()
        {
            if (_isInitialized)
                return;

            try
            {
                await db.CreateTableAsync<LocationModel>();
                _isInitialized = true;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Veritabanı başlatılamadı: {ex.Message}", ex);
            }
        }

        // Tüm kayıtları getirir
        public async Task<List<LocationModel>> GetAllAsync()
        {
            await EnsureInitializedAsync();
            return await db.Table<LocationModel>().ToListAsync();
        }

        // ID ile kayıt getirir
        public async Task<LocationModel?> GetByIdAsync(int id)
        {
            await EnsureInitializedAsync();
            return await db.Table<LocationModel>().Where(l => l.Id == id).FirstOrDefaultAsync().ConfigureAwait(false);
        }

        // Yeni kayıt ekler veya var olanı günceller
        public async Task<int> SaveLocationAsync(LocationModel location)
        {
            await EnsureInitializedAsync();
            
            if (location == null)
                throw new ArgumentNullException(nameof(location));

            location.CreatedAt = location.CreatedAt == default ? DateTime.UtcNow : location.CreatedAt;
            
            return location.Id != 0 
                ? await db.UpdateAsync(location) 
                : await db.InsertAsync(location);
        }

        // Kayıt siler
        public async Task<int> DeleteLocationAsync(LocationModel location)
        {
            await EnsureInitializedAsync();
            
            if (location == null)
                throw new ArgumentNullException(nameof(location));
                
            return await db.DeleteAsync(location);
        }

        // Offline senkronizasyon için eksik metodlar
        public async Task<List<LocationModel>> GetAllOfflineLocationsAsync()
        {
            await EnsureInitializedAsync();
            // Offline flag'i olan konumları getir (gelecekte offline flag eklenebilir)
            return await db.Table<LocationModel>().ToListAsync();
        }

        public async Task<int> DeleteOfflineLocationAsync(LocationModel location)
        {
            await EnsureInitializedAsync();
            return await DeleteLocationAsync(location);
        }

        // MoonData için CRUD işlemleri
        public Task<int> InsertMoonDataAsync(MoonData data)
            => db.InsertAsync(data);

        public Task<int> UpdateMoonDataAsync(MoonData data)
            => db.UpdateAsync(data);

        public Task<MoonData> GetMoonDataAsync(double lat, double lon, DateTime date)
            => db.Table<MoonData>()
                .Where(x => x.Latitude == lat && x.Longitude == lon && x.Date == date.Date)
                .FirstOrDefaultAsync();

        public Task<List<MoonData>> GetAllMoonDataAsync()
            => db.Table<MoonData>().ToListAsync();

        public Task<int> DeleteOldDataAsync(DateTime cutoffDate)
            => db.Table<MoonData>()
                .Where(x => x.Date < cutoffDate)
                .DeleteAsync();

        // Veritabanının başlatıldığından emin olur
        private async Task EnsureInitializedAsync()
        {
            if (!_isInitialized)
                await InitializeAsync();
        }
    }
}
