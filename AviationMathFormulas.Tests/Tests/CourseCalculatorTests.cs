namespace AviationMathFormulas.Tests
{
    [TestClass]
    public class CourseCalculatorTests
    {
        private readonly GeoPoint newYork = new GeoPoint(40.7128, -74.0060);
        private readonly GeoPoint london = new GeoPoint(51.5074, -0.1278);
        private readonly double expectedCourse = 308.79; // degrees (approximate)
        private const double tolerance = 0.1; // degrees

        [TestMethod]
        public void CalculateInitialCourse_NewYorkToLondon_ReturnsCorrectCourse()
        {
            // Act
            double course = CourseCalculator.CalculateInitialCourse(newYork, london);
            double courseDegrees = NavigationUtils.ToDegrees(course);

            // Assert
            Assert.AreEqual(expectedCourse, courseDegrees, tolerance);
        }

        [TestMethod]
        public void CalculateInitialCourse_FromNorthPole_ReturnsCorrectCourse()
        {
            // Arrange
            var northPole = new GeoPoint(90.0, 0.0);
            var point = new GeoPoint(80.0, 0.0);

            // Act
            double course = CourseCalculator.CalculateInitialCourse(northPole, point);
            double courseDegrees = NavigationUtils.ToDegrees(course);

            // Assert
            Assert.AreEqual(180.0, courseDegrees, tolerance);
        }
    }
}