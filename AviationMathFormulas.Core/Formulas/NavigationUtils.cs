namespace GreatCircleNavigation
{
    /// <summary>
    /// Utility methods for coordinate validation and angle conversions
    /// </summary>
    public static class NavigationUtils
    {
        /// <summary>
        /// Validates that the given coordinates are within valid ranges.
        /// </summary>
        public static void ValidateCoordinates(double latitude, double longitude, string pointIdentifier)
        {
            if (latitude < -90 || latitude > 90)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(latitude),
                    $"Latitude for {pointIdentifier} point must be between -90 and 90 degrees."
                );
            }

            if (longitude < -180 || longitude > 180)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(longitude),
                    $"Longitude for {pointIdentifier} point must be between -180 and 180 degrees."
                );
            }
        }

        /// <summary>
        /// Converts degrees to radians.
        /// </summary>
        public static double ToRadians(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }

        /// <summary>
        /// Converts radians to degrees.
        /// </summary>
        public static double ToDegrees(double radians)
        {
            return radians * 180.0 / Math.PI;
        }

        /// <summary>
        /// Implements the mathematical modulo operation (different from C#'s % operator for negative numbers).
        /// </summary>
        public static double Mod(double x, double m)
        {
            double r = x % m;
            return r < 0 ? r + m : r;
        }
    }

    /// <summary>
    /// Represents the result of an intersection calculation between two radials
    /// </summary>
    public enum IntersectionResult
    {
        Unique,
        Infinite,
        Ambiguous
    }
}