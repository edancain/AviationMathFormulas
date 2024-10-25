namespace GreatCircleNavigation
{
    /// <summary>
    /// Represents the intersection point and calculation result
    /// </summary>
    public class IntersectionPoint : GeoPoint
    {
        public IntersectionResult Result { get; set; }

        public IntersectionPoint(double lat, double lon, IntersectionResult result)
            : base(lat, lon)
        {
            Result = result;
        }
    }
}