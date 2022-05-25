using System;

namespace FakeUsersAPI.Models
{
    public class PassUserModelDB : IEquatable<PassUserModelDB?>
    {
        public string? Series { get; set; }
        public string? Number { get; set; }
        public string? UnitCode { get; set; }

        public string? UnitName { get; set; }
        public DateTime DateIssue { get; set; }

        public Guid UserId { get; set; }

        public override bool Equals(object? obj)
        {
            return Equals(obj as PassUserModelDB);
        }

        public bool Equals(PassUserModelDB? other)
        {
            return other != null &&
                   Series == other.Series &&
                   Number == other.Number &&
                   UnitCode == other.UnitCode &&
                   UnitName == other.UnitName &&
                   DateIssue == other.DateIssue &&
                   UserId.Equals(other.UserId);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Series, Number, UnitCode, UnitName, DateIssue, UserId);
        }
    }
}
