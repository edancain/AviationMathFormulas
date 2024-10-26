namespace AviationMathFormulas.Core.Formulas
{
    public static class IntersectionCalculator
    {
        public static IntersectionPoint CalculateIntersection(
            GeoPoint point1, double course1,
            GeoPoint point2, double course2)
        {
            // Convert inputs to radians
            double lat1 = NavigationUtils.ToRadians(point1.Latitude);
            double lon1 = NavigationUtils.ToRadians(point1.Longitude);
            double lat2 = NavigationUtils.ToRadians(point2.Latitude);
            double lon2 = NavigationUtils.ToRadians(point2.Longitude);
            double crs13 = NavigationUtils.ToRadians(course1);
            double crs23 = NavigationUtils.ToRadians(course2);

            // Calculate distance between points 1 and 2
            double dst12 = 2 * Math.Asin(Math.Sqrt(
                Math.Pow(Math.Sin((lat1 - lat2) / 2), 2) +
                Math.Cos(lat1) * Math.Cos(lat2) * Math.Pow(Math.Sin((lon1 - lon2) / 2), 2)
            ));

            // Calculate courses between points 1 and 2
            double crs12, crs21;
            if (Math.Sin(lon2 - lon1) < 0)
            {
                crs12 = Math.Acos((Math.Sin(lat2) - Math.Sin(lat1) * Math.Cos(dst12)) /
                                 (Math.Sin(dst12) * Math.Cos(lat1)));
                crs21 = 2 * Math.PI - Math.Acos((Math.Sin(lat1) - Math.Sin(lat2) * Math.Cos(dst12)) /
                                               (Math.Sin(dst12) * Math.Cos(lat2)));
            }
            else
            {
                crs12 = 2 * Math.PI - Math.Acos((Math.Sin(lat2) - Math.Sin(lat1) * Math.Cos(dst12)) /
                                               (Math.Sin(dst12) * Math.Cos(lat1)));
                crs21 = Math.Acos((Math.Sin(lat1) - Math.Sin(lat2) * Math.Cos(dst12)) /
                                 (Math.Sin(dst12) * Math.Cos(lat2)));
            }

            // Calculate angles
            double ang1 = NavigationUtils.Mod(crs13 - crs12 + Math.PI, 2 * Math.PI) - Math.PI;
            double ang2 = NavigationUtils.Mod(crs21 - crs23 + Math.PI, 2 * Math.PI) - Math.PI;

            // Check special cases
            if (Math.Abs(Math.Sin(ang1)) < NavigationConstants.EPS &&
                Math.Abs(Math.Sin(ang2)) < NavigationConstants.EPS)
            {
                return new IntersectionPoint(0, 0, IntersectionResult.Infinite);
            }

            // Check for ambiguous intersection
            // This occurs when the great circles intersect at two points
            if (Math.Abs(dst12) > NavigationConstants.EPS &&
                Math.Sin(ang1) * Math.Sin(ang2) < 0)
            {
                return new IntersectionPoint(0, 0, IntersectionResult.Ambiguous);
            }

            // Calculate intersection point
            ang1 = Math.Abs(ang1);
            ang2 = Math.Abs(ang2);

            double ang3 = Math.Acos(-Math.Cos(ang1) * Math.Cos(ang2) +
                                   Math.Sin(ang1) * Math.Sin(ang2) * Math.Cos(dst12));

            double dst13 = Math.Atan2(Math.Sin(dst12) * Math.Sin(ang1) * Math.Sin(ang2),
                                     Math.Cos(ang2) + Math.Cos(ang1) * Math.Cos(ang3));

            // Calculate intersection coordinates
            double lat3 = Math.Asin(Math.Sin(lat1) * Math.Cos(dst13) +
                                   Math.Cos(lat1) * Math.Sin(dst13) * Math.Cos(crs13));

            double dlon = Math.Atan2(Math.Sin(crs13) * Math.Sin(dst13) * Math.Cos(lat1),
                                    Math.Cos(dst13) - Math.Sin(lat1) * Math.Sin(lat3));

            double lon3 = NavigationUtils.Mod(lon1 - dlon + Math.PI, 2 * Math.PI) - Math.PI;

            return new IntersectionPoint(
                NavigationUtils.ToDegrees(lat3),
                NavigationUtils.ToDegrees(lon3),
                IntersectionResult.Unique
            );
        }
    }
}