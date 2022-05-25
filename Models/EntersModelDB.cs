using System;

namespace FakeUsersAPI.Models
{
    public class EntersModelDB : IEquatable<EntersModelDB?>
    {
            public Guid IdUser { get; set; }
            public DateTime DateIn { get; set; }
            public string? SecondNameUser { get; set; }
            public string? FirstNameUser { get; set; }
            public string? OS { get; set; }
            public string? Browser { get; set; }
            public string? Email { get; set; }
            public string? IPAddress { get; set; }
            public string? UserAgent { get; set; }
            public string? Series { get; set; }
            public string? Number { get; set; }
            public string? UnitCode { get; set; }

            public string? UnitName { get; set; }
            public DateTime? DateIssue { get; set; }

        public override bool Equals(object? obj)
        {
            return Equals(obj as EntersModelDB);
        }

        public bool Equals(EntersModelDB? other)
        {
            return other != null &&
                   IdUser.Equals(other.IdUser) &&
                   DateIn == other.DateIn &&
                   SecondNameUser == other.SecondNameUser &&
                   FirstNameUser == other.FirstNameUser &&
                   OS == other.OS &&
                   Browser == other.Browser &&
                   Email == other.Email &&
                   IPAddress == other.IPAddress &&
                   UserAgent == other.UserAgent &&
                   Series == other.Series &&
                   Number == other.Number &&
                   UnitCode == other.UnitCode &&
                   UnitName == other.UnitName &&
                   DateIssue == other.DateIssue;
        }

        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add(IdUser);
            hash.Add(DateIn);
            hash.Add(SecondNameUser);
            hash.Add(FirstNameUser);
            hash.Add(OS);
            hash.Add(Browser);
            hash.Add(Email);
            hash.Add(IPAddress);
            hash.Add(UserAgent);
            hash.Add(Series);
            hash.Add(Number);
            hash.Add(UnitCode);
            hash.Add(UnitName);
            hash.Add(DateIssue);
            return hash.ToHashCode();
        }
    }
}
