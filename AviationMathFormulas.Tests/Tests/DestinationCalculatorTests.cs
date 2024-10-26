namespace AviationMathFormulas.Tests
{
    [TestClass]
    public class DestinationCalculatorTests
    {
        private readonly GeoPoint start = new GeoPoint(40.7128, -74.0060);
        private const double course = 45.0;
        private const double distance = 1000.0;

        [TestMethod]
        public void CalculateDestination_ValidInputs_ReturnsCorrectPoint()
        {
            // Act
            GeoPoint result = DestinationCalculator.CalculateDestination(start, course, distance);

            // Assert
            Assert.IsNotNull(result);
            // Add specific coordinate checks based on known values
        }

        [TestMethod]
        public void CalculateDestination_ZeroDistance_ReturnsSamePoint()
        {
            // Act
            GeoPoint result = DestinationCalculator.CalculateDestination(start, course, 0);

            // Assert
            Assert.AreEqual(start.Latitude, result.Latitude, NavigationConstants.EPS);
            Assert.AreEqual(start.Longitude, result.Longitude, NavigationConstants.EPS);
        }
    }
}