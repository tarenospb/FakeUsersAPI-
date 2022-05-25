using System;
using System.Threading.Tasks;
using FakeUsersAPI.Models;
using System.Collections.Generic;

namespace FakeUsersAPI.Mappers
{
    public class IpLocateMapper
    {
        
        public double[] GetLongitudeLatitude(Ip2LocationModelDB s)
        {
            
            return new double[2] { Convert.ToDouble(s.latitude), Convert.ToDouble(s.longitude) };
        }

        public string GetReason(Ip2LocationModelDB s)
        {
            return "Country - " + s.country_name + ", region - " + s.region_name +", city - "+ s.city_name;
        }
    }
}
