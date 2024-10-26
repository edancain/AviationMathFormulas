namespace AviationMathFormulas.Tests
{
    [TestClass]
    public class DistanceCalculatorTests
    {
        private readonly GeoPoint newYork = new GeoPoint(40.7128, -74.0060);
        private readonly GeoPoint london = new GeoPoint(51.5074, -0.1278);
        private readonly double expectedDistance = 5570.0; // km (approximate)
        private const double tolerance = 1.0; // km

        [TestMethod]
        public void CalculateDistance_NewYorkToLondon_ReturnsCorrectDistance()
        {
            // Act
            double distance = DistanceCalculator.CalculateDistance(newYork, london);
            double distanceKm = distance * NavigationConstants.EARTH_RADIUS_KM;

            // Assert
            Assert.AreEqual(expectedDistance, distanceKm, tolerance);
        }

        [TestMethod]
        public void CalculateDistance_SamePoint_ReturnsZero()
        {
            // Act
            double distance = DistanceCalculator.CalculateDistance(newYork, newYork);

            // Assert
            Assert.AreEqual(0, distance, NavigationConstants.EPS);
        }
    }
}