namespace AviationMathFormulas.Core.Formulas
{
    /// <summary>
    /// Constants and utility methods used across the navigation calculations
    /// </summary>
    public static class NavigationConstants
    {
        /// <summary>
        /// Machine epsilon for floating-point comparisons
        /// </summary>
        public const double EPS = 1e-10;

        /// <summary>
        /// Earth's radius in kilometers
        /// </summary>
        public const double EARTH_RADIUS_KM = 6371.0;
    }
}