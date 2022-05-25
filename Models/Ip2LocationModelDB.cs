using System;

namespace FakeUsersAPI.Models
{
    public class Ip2LocationModelDB : IEquatable<Ip2LocationModelDB?>
    {
            public string? country_name { get; set; }

        public string? region_name { get; set; }
        public string? city_name { get; set; }
            public string? latitude { get; set; }
            public string? longitude { get; set; }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Ip2LocationModelDB);
        }

        public bool Equals(Ip2LocationModelDB? other)
        {
            return other != null &&
                   country_name == other.country_name &&
                   region_name == other.region_name &&
                   city_name == other.city_name &&
                   latitude == other.latitude &&
                   longitude == other.longitude;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(country_name, region_name, city_name, latitude, longitude);
        }
    }
}
