using System;
using System.Linq;

namespace FakeUsersAPI.Services
{
    public class CoordLocate
    {
        
        //from fly Spb-Chelyabinsk (1916 km / 3 ч = 640 км/ч)
        private const double SPEED = 640.0;

        public bool DistanceIsRealForFly(
            double[] latlon1,
            double[] latlon2,
            DateTime time1,
            DateTime time2)
        {
            var distance = DistanceBetweenTwoCoords(latlon1, latlon2, 'K');
            var timeInHours = time2.Subtract(time1).TotalHours;
            var realTime = distance / SPEED;
            return realTime < timeInHours;
        }

        public double DistanceBetweenTwoCoords(
           double[]? latlon1,
           double[]? latlon2,
           char? unit)
        {
            if (latlon1?.Length <= 0
                || latlon2?.Length <= 0
                || !unit.HasValue
                || unit == default)
            {
                return 0;
            }

            var lat1 = latlon1.ElementAtOrDefault(0);
            var lat2 = latlon2.ElementAtOrDefault(0);
            var lon1 = latlon1.ElementAtOrDefault(1);
            var lon2 = latlon2.ElementAtOrDefault(1);

            if ((lat1 == lat2) && (lon1 == lon2)
                || latlon1.ToList().Concat(latlon2.ToList())
                    .Any(i => i == default))
            {
                return 0;
            }

            var theta = lon1 - lon2;
            var preDist = ConvertDist(GetDist(lat1, lat2, theta));

            if (unit == 'K') //is kilometers
            {
                return preDist * 1.609344;
            }
            if (unit == 'N') // is nautical miles
            {
                return preDist * 0.8684;
            }
            return preDist;
        }

        /// <summary>
        /// converts decimal degrees to radians
        /// </summary>
        private double DegreesToRadians(double deg) =>
            deg * Math.PI / 180.0;

        /// <summary>
        /// converts radians to decimal degrees
        /// </summary>
        private double RadiansToDegrees(double rad) =>
            rad / Math.PI * 180.0;

        private double GetDist(double lat1, double lat2, double theta) =>
            Math.Sin(DegreesToRadians(lat1)) * Math.Sin(DegreesToRadians(lat2))
            + Math.Cos(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2))
                                               * Math.Cos(DegreesToRadians(theta));

        private double ConvertDist(double dist)
        {
            dist = Math.Acos(dist);
            dist = RadiansToDegrees(dist);
            return dist * 60 * 1.1515;
        }


    }
}
