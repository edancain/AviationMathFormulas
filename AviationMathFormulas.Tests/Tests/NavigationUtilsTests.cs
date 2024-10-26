namespace AviationMathFormulas.Tests
{
    [TestClass]
    public class NavigationUtilsTests
    {
        private const double tolerance = 1e-10;

        [TestMethod]
        public void ToRadians_90Degrees_ReturnsHalfPi()
        {
            // Act
            double result = NavigationUtils.ToRadians(90.0);

            // Assert
            Assert.AreEqual(Math.PI / 2, result, tolerance);
        }

        [TestMethod]
        public void ToDegrees_HalfPi_Returns90()
        {
            // Act
            double result = NavigationUtils.ToDegrees(Math.PI / 2);

            // Assert
            Assert.AreEqual(90.0, result, tolerance);
        }

        [TestMethod]
        public void Mod_PositiveNumber_ReturnsCorrectResult()
        {
            // Act
            double result = NavigationUtils.Mod(5.0, 3.0);

            // Assert
            Assert.AreEqual(2.0, result, tolerance);
        }

        [TestMethod]
        public void Mod_NegativeNumber_ReturnsPositiveResult()
        {
            // Act
            double result = NavigationUtils.Mod(-1.0, 2 * Math.PI);

            // Assert
            Assert.IsTrue(result >= 0 && result < 2 * Math.PI);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ValidateCoordinates_InvalidLatitude_ThrowsException()
        {
            // Act
            NavigationUtils.ValidateCoordinates(91.0, 0.0, "test");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ValidateCoordinates_InvalidLongitude_ThrowsException()
        {
            // Act
            NavigationUtils.ValidateCoordinates(0.0, 181.0, "test");
        }
    }
}