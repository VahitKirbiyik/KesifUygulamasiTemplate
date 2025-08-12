using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using SQLite;

namespace KesifUygulamasiTemplate.Models
{
    [Table("Locations")]
    public class LocationModel : INotifyPropertyChanged
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        private string title = string.Empty;
        public string Title
        {
            get => title;
            set
            {
                if (title != value)
                {
                    title = value;
                    OnPropertyChanged();
                }
            }
        }

        private string description = string.Empty;
        public string Description
        {
            get => description;
            set
            {
                if (description != value)
                {
                    description = value;
                    OnPropertyChanged();
                }
            }
        }

        private double latitude;
        public double Latitude
        {
            get => latitude;
            set
            {
                if (latitude != value)
                {
                    latitude = value;
                    OnPropertyChanged();
                }
            }
        }

        private double longitude;
        public double Longitude
        {
            get => longitude;
            set
            {
                if (longitude != value)
                {
                    longitude = value;
                    OnPropertyChanged();
                }
            }
        }

        private DateTime createdAt = DateTime.UtcNow;
        public DateTime CreatedAt
        {
            get => createdAt;
            set
            {
                if (createdAt != value)
                {
                    createdAt = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
