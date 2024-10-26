namespace AviationMathFormulas.Core.Formulas
{
    /// <summary>
    /// Represents a geographic coordinate point
    /// </summary>
    public class GeoPoint
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public GeoPoint(double latitude, double longitude)
        {
            NavigationUtils.ValidateCoordinates(latitude, longitude, "point");
            Latitude = latitude;
            Longitude = longitude;
        }

        public override string ToString()
        {
            return $"{Latitude:F4}°, {Longitude:F4}°";
        }
    }
}