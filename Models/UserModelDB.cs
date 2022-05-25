using System;
using System.Collections.Generic;

namespace FakeUsersAPI.Models
{
    public class UserModelDB : IEquatable<UserModelDB?>
    {
            public Guid IdUser { get; set; }
            public string? SecondNameUser { get; set; }
            public string? FirstNameUser { get; set; }

            public string? OS { get; set; }
            public string? Browser { get; set; }
            public DateTime DateBirth { get; set; }
            public string? Login { get; set; }
            public string? Email { get; set; }
            public string? IPAddress { get; set; }
            public string? UserAgent { get; set; }
            public PassUserModelDB? Passport { get; set; }
            public bool Lock { get; set; }
            public DateTime DateIn { get; set; }

        public override bool Equals(object? obj)
        {
            return Equals(obj as UserModelDB);
        }

        public bool Equals(UserModelDB? other)
        {
            return other != null &&
                   IdUser.Equals(other.IdUser) &&
                   SecondNameUser == other.SecondNameUser &&
                   FirstNameUser == other.FirstNameUser &&
                   OS == other.OS &&
                   Browser == other.Browser &&
                   DateBirth == other.DateBirth &&
                   Login == other.Login &&
                   Email == other.Email &&
                   IPAddress == other.IPAddress &&
                   UserAgent == other.UserAgent &&
                   EqualityComparer<PassUserModelDB?>.Default.Equals(Passport, other.Passport) &&
                   Lock == other.Lock &&
                   DateIn == other.DateIn;
        }

        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add(IdUser);
            hash.Add(SecondNameUser);
            hash.Add(FirstNameUser);
            hash.Add(OS);
            hash.Add(Browser);
            hash.Add(DateBirth);
            hash.Add(Login);
            hash.Add(Email);
            hash.Add(IPAddress);
            hash.Add(UserAgent);
            hash.Add(Passport);
            hash.Add(Lock);
            hash.Add(DateIn);
            return hash.ToHashCode();
        }
    }
    

}
