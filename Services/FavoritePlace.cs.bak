using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SQLite;

namespace KesifUygulamasiTemplate.Models
{
    [Table("FavoritePlaces")]
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
}
