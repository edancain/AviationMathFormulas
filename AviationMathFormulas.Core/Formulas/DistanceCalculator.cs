using System;

namespace GreatCircleNavigation
{
    /// <summary>
    /// Provides core distance calculation functionality
    /// </summary>
    public static class DistanceCalculator
    {
        public static double CalculateDistance(GeoPoint point1, GeoPoint point2)
        {
            double lat1 = NavigationUtils.ToRadians(point1.Latitude);
            double lon1 = NavigationUtils.ToRadians(point1.Longitude);
            double lat2 = NavigationUtils.ToRadians(point2.Latitude);
            double lon2 = NavigationUtils.ToRadians(point2.Longitude);

            return Math.Acos(
                Math.Sin(lat1) * Math.Sin(lat2) +
                Math.Cos(lat1) * Math.Cos(lat2) * Math.Cos(lon1 - lon2)
            );
        }
    }
}