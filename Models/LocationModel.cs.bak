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

        private string name = string.Empty;
        public string Name
        {
            get => name;
            set
            {
                if (name != value)
                {
                    name = value;
                    OnPropertyChanged();
                }
            }
        }

        private string address = string.Empty;
        public string Address
        {
            get => address;
            set
            {
                if (address != value)
                {
                    address = value;
                    OnPropertyChanged();
                }
            }
        }

        private string category = string.Empty;
        public string Category
        {
            get => category;
            set
            {
                if (category != value)
                {
                    category = value;
                    OnPropertyChanged();
                }
            }
        }

        private string iconName = string.Empty;
        public string IconName
        {
            get => iconName;
            set
            {
                if (iconName != value)
                {
                    iconName = value;
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
