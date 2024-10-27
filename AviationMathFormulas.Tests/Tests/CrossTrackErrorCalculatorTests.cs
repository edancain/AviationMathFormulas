using Microsoft.VisualStudio.TestTools.UnitTesting;
using AviationMathFormulas.Core.Formulas;

namespace AviationMathFormulas.Tests
{
    [TestClass]
    public class CrossTrackCorrectionTests
    {
        private const double tolerance = 0.1; // Tolerance for degree comparisons
        private const double distanceTolerance = 0.5; // Tolerance for distance calculations in km
        private const double speedTolerance = 1.0; // Tolerance for speed calculations in knots

        // Common test points
        private readonly GeoPoint newYork = new GeoPoint(40.7128, -74.0060);
        private readonly GeoPoint london = new GeoPoint(51.5074, -0.1278);
        private readonly GeoPoint boston = new GeoPoint(42.3601, -71.0589); // Off track point

        // Common wind scenarios
        private readonly WindData noWind = new WindData { Speed = 0, Direction = 0 };
        private readonly WindData westWind = new WindData { Speed = 25, Direction = 270 };
        private readonly WindData headWind = new WindData { Speed = 25, Direction = 0 };
        private readonly WindData tailWind = new WindData { Speed = 25, Direction = 180 };

        [TestMethod]
        public void CalculateReturnToPathCorrection_NoWind_ReturnsCorrectCourse()
        {
            // Arrange
            var start = new GeoPoint(0, 0);
            var end = new GeoPoint(0, 10);
            var current = new GeoPoint(1, 5); // Right of track
            double trueAirspeed = 120;

            // Act
            var correction = CrossTrackCalculator.CalculateReturnToPathCorrection(
                start, end, current, trueAirspeed, noWind);

            // Assert
            Assert.IsTrue(correction.CorrectionAngle < 0); // Should turn left
            Assert.AreEqual(trueAirspeed, correction.GroundSpeed, speedTolerance); // No wind, so GS = TAS
            Assert.AreEqual(0, correction.WindCorrectionAngle, tolerance); // No wind correction needed
        }

        [TestMethod]
        public void CalculateReturnToPathCorrection_WithCrossWind_AppliesWindCorrection()
        {
            // Arrange
            var start = new GeoPoint(0, 0);
            var end = new GeoPoint(0, 10);
            var current = new GeoPoint(1, 5);
            double trueAirspeed = 120;

            // Act
            var correction = CrossTrackCalculator.CalculateReturnToPathCorrection(
                start, end, current, trueAirspeed, westWind);

            // Assert
            Assert.IsTrue(correction.WindCorrectionAngle != 0); // Should have wind correction
            Assert.IsTrue(correction.GroundSpeed != trueAirspeed); // Ground speed should differ from TAS
        }

        [TestMethod]
        public void CalculateDirectToDestination_NoWind_ReturnsDirectCourse()
        {
            // Arrange
            double trueAirspeed = 120;

            // Act
            var correction = CrossTrackCalculator.CalculateDirectToDestinationCorrection(
                newYork, london, boston, trueAirspeed, noWind);

            // Assert
            Assert.AreEqual(0, correction.WindCorrectionAngle, tolerance);
            Assert.AreEqual(trueAirspeed, correction.GroundSpeed, speedTolerance);
        }

        [TestMethod]
        public void CalculateDirectToDestination_WithHeadWind_ReducesGroundSpeed()
        {
            // Arrange
            double trueAirspeed = 120;

            // Act
            var correction = CrossTrackCalculator.CalculateDirectToDestinationCorrection(
                newYork, london, boston, trueAirspeed, headWind);

            // Assert
            Assert.IsTrue(correction.GroundSpeed < trueAirspeed);
            Assert.IsTrue(Math.Abs(correction.WindCorrectionAngle) < tolerance); // Minimal crab angle needed
        }

        [TestMethod]
        public void CalculateDirectToDestination_WithTailWind_IncreasesGroundSpeed()
        {
            // Arrange
            double trueAirspeed = 120;

            // Act
            var correction = CrossTrackCalculator.CalculateDirectToDestinationCorrection(
                newYork, london, boston, trueAirspeed, tailWind);

            // Assert
            Assert.IsTrue(correction.GroundSpeed > trueAirspeed);
            Assert.IsTrue(Math.Abs(correction.WindCorrectionAngle) < tolerance); // Minimal crab angle needed
        }

        [TestMethod]
        public void CalculateDirectToDestination_WithCrossWind_RequiresCrabAngle()
        {
            // Arrange
            double trueAirspeed = 120;

            // Act
            var correction = CrossTrackCalculator.CalculateDirectToDestinationCorrection(
                newYork, london, boston, trueAirspeed, westWind);

            // Assert
            Assert.IsTrue(Math.Abs(correction.WindCorrectionAngle) > tolerance); // Should have significant crab angle
        }

        [TestMethod]
        public void IsRightOfTrack_RightSide_ReturnsTrue()
        {
            // Arrange
            var start = new GeoPoint(0, 0);
            var end = new GeoPoint(0, 10);
            var current = new GeoPoint(1, 5); // Right of track

            // Act
            bool isRight = CrossTrackCalculator.IsRightOfTrack(start, end, current);

            // Assert
            Assert.IsTrue(isRight);
        }

        [TestMethod]
        public void IsRightOfTrack_LeftSide_ReturnsFalse()
        {
            // Arrange
            var start = new GeoPoint(0, 0);
            var end = new GeoPoint(0, 10);
            var current = new GeoPoint(-1, 5); // Left of track

            // Act
            bool isRight = CrossTrackCalculator.IsRightOfTrack(start, end, current);

            // Assert
            Assert.IsFalse(isRight);
        }

        [TestMethod]
        public void CalculateCrossTrackError_OnTrack_ReturnsZeroXTD()
        {
            // Arrange
            var start = new GeoPoint(0, 0);
            var end = new GeoPoint(0, 10);
            var current = new GeoPoint(0, 5); // On track

            // Act
            var result = CrossTrackCalculator.CalculateCrossTrackError(start, end, current);

            // Assert
            Assert.AreEqual(0, result.CrossTrackDistance, tolerance);
        }

        [TestMethod]
        public void CalculateReturnToPathCorrection_StrongCrossWind_LargeWindCorrection()
        {
            // Arrange
            var strongCrossWind = new WindData { Speed = 50, Direction = 90 }; // Strong wind from east
            double trueAirspeed = 120;

            // Act
            var correction = CrossTrackCalculator.CalculateReturnToPathCorrection(
                newYork, london, boston, trueAirspeed, strongCrossWind);

            // Assert
            Assert.IsTrue(Math.Abs(correction.WindCorrectionAngle) > 10); // Should have significant wind correction
            Assert.IsTrue(correction.GroundSpeed != trueAirspeed); // Ground speed should be affected
        }

        [TestMethod]
        public void CalculateReturnToPathCorrection_WeakWind_SmallWindCorrection()
        {
            // Arrange
            var weakWind = new WindData { Speed = 5, Direction = 90 }; // Weak wind from east
            double trueAirspeed = 120;

            // Act
            var correction = CrossTrackCalculator.CalculateReturnToPathCorrection(
                newYork, london, boston, trueAirspeed, weakWind);

            // Assert
            Assert.IsTrue(Math.Abs(correction.WindCorrectionAngle) < 5); // Should have minimal wind correction
            Assert.IsTrue(Math.Abs(correction.GroundSpeed - trueAirspeed) < 5); // Ground speed should be minimally affected
        }

        [TestMethod]
        public void CalculateReturnToPathCorrection_ExtremeWind_HandlesCorrectly()
        {
            // Arrange
            var extremeWind = new WindData { Speed = 100, Direction = 270 }; // Very strong wind
            double trueAirspeed = 120;

            // Act
            var correction = CrossTrackCalculator.CalculateReturnToPathCorrection(
                newYork, london, boston, trueAirspeed, extremeWind);

            // Assert
            Assert.IsTrue(correction.WindCorrectionAngle != 0);
            Assert.IsTrue(correction.TotalCorrection != correction.CorrectionAngle);
            Assert.IsTrue(correction.GroundSpeed > 0);
        }
    }
}