namespace AviationMathFormulas.Tests
{
    [TestClass]
    public class GeoPointTests
    {
        [TestMethod]
        public void Constructor_ValidCoordinates_CreatesPoint()
        {
            // Arrange & Act
            var point = new GeoPoint(40.7128, -74.0060);

            // Assert
            Assert.AreEqual(40.7128, point.Latitude);
            Assert.AreEqual(-74.0060, point.Longitude);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Constructor_InvalidLatitude_ThrowsException()
        {
            // Arrange & Act
            var point = new GeoPoint(91.0, 0.0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Constructor_InvalidLongitude_ThrowsException()
        {
            // Arrange & Act
            var point = new GeoPoint(0.0, 181.0);
        }

        [TestMethod]
        public void ToString_ValidPoint_ReturnsFormattedString()
        {
            // Arrange
            var point = new GeoPoint(40.7128, -74.0060);

            // Act
            string result = point.ToString();

            // Assert
            Assert.AreEqual("40.7128°, -74.0060°", result);
        }
    }
}