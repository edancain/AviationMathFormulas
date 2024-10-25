
namespace GreatCircleNavigation
{
    /// <summary>
    /// Provides course calculation functionality
    /// </summary>
    public static class CourseCalculator
    {
        public static double CalculateInitialCourse(GeoPoint point1, GeoPoint point2)
        {
            double lat1 = NavigationUtils.ToRadians(point1.Latitude);
            double lon1 = NavigationUtils.ToRadians(point1.Longitude);
            double lat2 = NavigationUtils.ToRadians(point2.Latitude);
            double lon2 = NavigationUtils.ToRadians(point2.Longitude);

            if (Math.Cos(lat1) < NavigationConstants.EPS)
            {
                return (lat1 > 0) ? Math.PI : 2 * Math.PI;
            }

            double d = DistanceCalculator.CalculateDistance(point1, point2);

            if (Math.Sin(lon2 - lon1) < 0)
            {
                return Math.Acos((Math.Sin(lat2) - Math.Sin(lat1) * Math.Cos(d)) /
                               (Math.Sin(d) * Math.Cos(lat1)));
            }
            else
            {
                return 2 * Math.PI - Math.Acos((Math.Sin(lat2) - Math.Sin(lat1) * Math.Cos(d)) /
                                             (Math.Sin(d) * Math.Cos(lat1)));
            }
        }
    }
}