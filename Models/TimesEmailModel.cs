using System;

namespace FakeUsersAPI.Models
{
    public class TimesEmailModel : IEquatable<TimesEmailModel?>
    {
        public DateTime DateIn { get; set; }
        public string? Email { get; set; }

        public override bool Equals(object? obj)
        {
            return Equals(obj as TimesEmailModel);
        }

        public bool Equals(TimesEmailModel? other)
        {
            return other != null &&
                   DateIn == other.DateIn &&
                   Email == other.Email;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(DateIn, Email);
        }
    }
}
