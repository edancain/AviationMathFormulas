namespace AviationMathFormulas.Tests
{
    [TestClass]
    public class IntersectionCalculatorTests
    {
        private readonly GeoPoint point1 = new GeoPoint(40.7128, -74.0060);
        private readonly GeoPoint point2 = new GeoPoint(51.5074, -0.1278);

        [TestMethod]
        public void CalculateIntersection_ValidInputs_ReturnsUniquePoint()
        {
            // Arrange
            double course1 = 45.0;
            double course2 = 315.0;

            // Act
            IntersectionPoint result = IntersectionCalculator.CalculateIntersection(
                point1, course1, point2, course2);

            // Assert
            Assert.AreEqual(IntersectionResult.Unique, result.Result);
            // Add specific coordinate checks based on known values
        }

        [TestMethod]
        public void CalculateIntersection_ParallelCourses_ReturnsInfinite()
        {
            // Arrange
            var point1 = new GeoPoint(0.0, 0.0);
            var point2 = new GeoPoint(0.0, 10.0); // Points on same latitude
            // Arrange
            double course1 = 90.0;
            double course2 = 90.0;

            // Act
            IntersectionPoint result = IntersectionCalculator.CalculateIntersection(
                point1, course1, point2, course2);

            // Assert
            Assert.AreEqual(IntersectionResult.Infinite, result.Result);
        }

        [TestMethod]
        public void CalculateIntersection_AmbiguousCase_ReturnsAmbiguous()
        {
            // Arrange
            var point1 = new GeoPoint(0.0, 0.0);
            var point2 = new GeoPoint(0.0, 120.0);  // Points on equator, far apart
            double course1 = 30.0;   // Course from point1
            double course2 = 210.0;  // Course from point2 (opposite direction)

            // Act
            IntersectionPoint result = IntersectionCalculator.CalculateIntersection(
                point1, course1, point2, course2);

            // Assert
            Assert.AreEqual(IntersectionResult.Ambiguous, result.Result);
        }
    }
}