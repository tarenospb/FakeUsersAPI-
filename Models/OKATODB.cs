using System;

namespace FakeUsersAPI.Models
{
    public class OKATODB : IEquatable<OKATODB?>
    {
        public string? CodeOkato { get; set; }
        public string? CodeRegion { get; set; }
        public string? NameDistrict { get; set; }

        public override bool Equals(object? obj)
        {
            return Equals(obj as OKATODB);
        }

        public bool Equals(OKATODB? other)
        {
            return other != null &&
                   CodeOkato == other.CodeOkato &&
                   CodeRegion == other.CodeRegion &&
                   NameDistrict == other.NameDistrict;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(CodeOkato, CodeRegion, NameDistrict);
        }
    }
}
