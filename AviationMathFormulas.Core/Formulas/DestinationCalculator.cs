namespace AviationMathFormulas.Core.Formulas
{
    /// <summary>
    /// Provides destination point calculation functionality
    /// </summary>
    public static class DestinationCalculator
    {
        public static GeoPoint CalculateDestination(
            GeoPoint start, double course, double distanceKm, bool useGeneralAlgorithm = false)
        {
            double d = distanceKm / NavigationConstants.EARTH_RADIUS_KM;
            double tc = NavigationUtils.ToRadians(course);

            if (useGeneralAlgorithm || distanceKm > 5000)
            {
                return CalculateDestinationGeneral(start, tc, d);
            }
            else
            {
                return CalculateDestinationSimple(start, tc, d);
            }
        }

        private static GeoPoint CalculateDestinationSimple(GeoPoint start, double tc, double d)
        {
            double lat1 = NavigationUtils.ToRadians(start.Latitude);
            double lon1 = NavigationUtils.ToRadians(start.Longitude);

            double lat = Math.Asin(
                Math.Sin(lat1) * Math.Cos(d) +
                Math.Cos(lat1) * Math.Sin(d) * Math.Cos(tc)
            );

            double lon;
            if (Math.Abs(Math.Cos(lat)) < NavigationConstants.EPS)
            {
                lon = lon1;
            }
            else
            {
                lon = lon1 - Math.Asin(Math.Sin(tc) * Math.Sin(d) / Math.Cos(lat));
                lon = NavigationUtils.Mod(lon + Math.PI, 2 * Math.PI) - Math.PI;
            }

            return new GeoPoint(
                NavigationUtils.ToDegrees(lat),
                NavigationUtils.ToDegrees(lon)
            );
        }

        private static GeoPoint CalculateDestinationGeneral(GeoPoint start, double tc, double d)
        {
            double lat1 = NavigationUtils.ToRadians(start.Latitude);
            double lon1 = NavigationUtils.ToRadians(start.Longitude);

            // Calculate destination latitude
            double lat = Math.Asin(
                Math.Sin(lat1) * Math.Cos(d) + 
                Math.Cos(lat1) * Math.Sin(d) * Math.Cos(tc)
            );

            // Calculate change in longitude
            double dlon = Math.Atan2(
                Math.Sin(tc) * Math.Sin(d) * Math.Cos(lat1),
                Math.Cos(d) - Math.Sin(lat1) * Math.Sin(lat)
            );

            // Calculate destination longitude
            double lon = NavigationUtils.Mod(lon1 - dlon + Math.PI, 2 * Math.PI) - Math.PI;

            return new GeoPoint(
                NavigationUtils.ToDegrees(lat),
                NavigationUtils.ToDegrees(lon)
            );
        }
    }
}