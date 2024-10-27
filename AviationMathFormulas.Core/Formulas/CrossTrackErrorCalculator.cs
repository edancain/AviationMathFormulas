namespace AviationMathFormulas.Core.Formulas
{
    /// <summary>
    /// Result of a cross track error calculation including both cross track and along track distances
    /// </summary>
    public class CrossTrackResult
    {
        /// <summary>
        /// Cross Track Distance (XTD) - distance off course.
        /// Positive means right of course, negative means left of course.
        /// </summary>
        public double CrossTrackDistance { get; set; }

        /// <summary>
        /// Along Track Distance (ATD) - distance from start along the course to point abeam current position
        /// </summary>
        public double AlongTrackDistance { get; set; }

        public CrossTrackResult(double xtd, double atd)
        {
            CrossTrackDistance = xtd;
            AlongTrackDistance = atd;
        }
    }

    /// <summary>
    /// Points on great circle at specified distance from reference point
    /// </summary>
    public class EquidistantPoints
    {
        public bool PointsExist { get; set; }
        public GeoPoint? Point1 { get; set; }
        public GeoPoint? Point2 { get; set; }

        public EquidistantPoints(bool exist, GeoPoint? p1 = null, GeoPoint? p2 = null)
        {
            PointsExist = exist;
            Point1 = p1;
            Point2 = p2;
        }
    }

    public class CourseCorrection
    {
        public double CorrectionAngle { get; set; }       // Basic correction to path
        public double WindCorrectionAngle { get; set; }   // Additional correction for wind
        public double TotalCorrection { get; set; }       // Combined correction
        public double NewCourse { get; set; }             // New course to fly
        public double Distance { get; set; }              // Distance to intercept
        public double GroundSpeed { get; set; }           // Expected ground speed
        
        public CourseCorrection(double correction, double windCorrection, 
            double newCourse, double distance, double groundSpeed)
        {
            CorrectionAngle = correction;
            WindCorrectionAngle = windCorrection;
            TotalCorrection = correction + windCorrection;
            NewCourse = newCourse;
            Distance = distance;
            GroundSpeed = groundSpeed;
        }
    }

    public static class CrossTrackCalculator
    {
        /// <summary>
        /// Calculates Cross Track Error and Along Track Distance
        /// </summary>
        /// <param name="start">Starting point (A)</param>
        /// <param name="end">Destination point (B)</param>
        /// <param name="current">Current position (D)</param>
        /// <returns>CrossTrackResult containing XTD and ATD</returns>
        public static CrossTrackResult CalculateCrossTrackError(
            GeoPoint start, GeoPoint end, GeoPoint current)
        {
            // Handle pole cases
            if (Math.Abs(Math.Abs(start.Latitude) - 90) < NavigationConstants.EPS)
            {
                return CalculatePolarCrossTrackError(start, end, current);
            }

            // Calculate courses and distance
            double courseAB = CourseCalculator.CalculateInitialCourse(start, end);
            double courseAD = CourseCalculator.CalculateInitialCourse(start, current);
            double distanceAD = DistanceCalculator.CalculateDistance(start, current);

            // Calculate Cross Track Distance
            double xtd = Math.Asin(
                Math.Sin(distanceAD) * Math.Sin(courseAD - courseAB)
            );

            // Calculate Along Track Distance using appropriate formula based on distance
            double atd;
            if (distanceAD < 0.1) // For very short distances (< ~630km)
            {
                atd = Math.Asin(
                    Math.Sqrt(
                        Math.Pow(Math.Sin(distanceAD), 2) - Math.Pow(Math.Sin(xtd), 2)
                    ) / Math.Cos(xtd)
                );
            }
            else
            {
                atd = Math.Acos(Math.Cos(distanceAD) / Math.Cos(xtd));
            }

            return new CrossTrackResult(xtd, atd);
        }

        /// <summary>
        /// Calculates Cross Track Error when starting point is a pole
        /// </summary>
        private static CrossTrackResult CalculatePolarCrossTrackError(
            GeoPoint start, GeoPoint end, GeoPoint current)
        {
            double xtd;
            if (start.Latitude > 0) // North Pole
            {
                xtd = NavigationUtils.ToRadians(current.Longitude - end.Longitude);
            }
            else // South Pole
            {
                xtd = NavigationUtils.ToRadians(end.Longitude - current.Longitude);
            }

            // For polar cases, ATD is simply the co-latitude of the current point
            double atd = NavigationUtils.ToRadians(90 - Math.Abs(current.Latitude));

            return new CrossTrackResult(xtd, atd);
        }

        /// <summary>
        /// Finds points on the great circle through A and B that lie a specified distance from point D
        /// </summary>
        public static EquidistantPoints FindEquidistantPoints(
            GeoPoint start, GeoPoint end, GeoPoint reference, double distance)
        {
            double courseAB = CourseCalculator.CalculateInitialCourse(start, end);
            double courseAD = CourseCalculator.CalculateInitialCourse(start, reference);
            double distanceAD = DistanceCalculator.CalculateDistance(start, reference);

            // Calculate intermediate values
            double A = courseAD - courseAB;
            double b = distanceAD;
            double r = Math.Sqrt(
                Math.Pow(Math.Cos(b), 2) + Math.Pow(Math.Sin(b), 2) * Math.Pow(Math.Cos(A), 2)
            );
            double p = Math.Atan2(Math.Sin(b) * Math.Cos(A), Math.Cos(b));

            // Check if points exist
            if (Math.Pow(Math.Cos(distance), 2) > Math.Pow(r, 2))
            {
                return new EquidistantPoints(false);
            }

            // Calculate points if they exist
            double dp1 = p + Math.Acos(Math.Cos(distance) / r);
            double dp2 = p - Math.Acos(Math.Cos(distance) / r);

            // Convert distances and course to points
            var point1 = DestinationCalculator.CalculateDestination(start, 
                NavigationUtils.ToDegrees(courseAB), 
                NavigationUtils.ToDegrees(dp1) * NavigationConstants.EARTH_RADIUS_KM);
            
            var point2 = DestinationCalculator.CalculateDestination(start,
                NavigationUtils.ToDegrees(courseAB),
                NavigationUtils.ToDegrees(dp2) * NavigationConstants.EARTH_RADIUS_KM);

            return new EquidistantPoints(true, point1, point2);
        }

        /// <summary>
        /// Calculates course correction to return to the original great circle path
        /// </summary>
        public static CourseCorrection CalculateReturnToPathCorrection(
            GeoPoint start, GeoPoint end, GeoPoint current, 
            double trueAirspeed, WindData wind)
        {
            // Calculate original course and current position details
            double originalCourse = NavigationUtils.ToDegrees(
                CourseCalculator.CalculateInitialCourse(start, end));
            
            var crossTrackResult = CalculateCrossTrackError(start, end, current);
            
            // Calculate basic correction angle
            double currentToEndDistance = DistanceCalculator.CalculateDistance(current, end);
            double basicCorrection = Math.Asin(
                Math.Sin(crossTrackResult.CrossTrackDistance) / Math.Sin(currentToEndDistance)
            );
            basicCorrection = NavigationUtils.ToDegrees(basicCorrection);
            
            // Determine turn direction based on XTD
            basicCorrection = -Math.Sign(crossTrackResult.CrossTrackDistance) * Math.Abs(basicCorrection);
            
            // Calculate wind correction
            double windCorrection = CalculateWindCorrectionAngle(
                originalCourse + basicCorrection, trueAirspeed, wind);
            
            // Calculate total correction and new course
            double totalCorrection = basicCorrection + windCorrection;
            double newCourse = NavigationUtils.Mod(originalCourse + totalCorrection, 360);
            
            // Calculate distance and ground speed
            double distance = Math.Abs(
                crossTrackResult.CrossTrackDistance * NavigationConstants.EARTH_RADIUS_KM / 
                Math.Sin(NavigationUtils.ToRadians(basicCorrection))
            );
            
            double groundSpeed = CalculateGroundSpeed(newCourse, trueAirspeed, wind);

            return new CourseCorrection(
                basicCorrection,
                windCorrection,
                newCourse,
                distance,
                groundSpeed
            );
        }

        private static double CalculateWindCorrectionAngle(
            double course, double trueAirspeed, WindData wind)
        {
            double courseRad = NavigationUtils.ToRadians(course);
            double windDirRad = NavigationUtils.ToRadians(wind.Direction);
            
            double wca = Math.Asin(
                (wind.Speed * Math.Sin(windDirRad - courseRad)) / trueAirspeed
            );
            
            return NavigationUtils.ToDegrees(wca);
        }

        private static double CalculateGroundSpeed(
            double course, double trueAirspeed, WindData wind)
        {
            double courseRad = NavigationUtils.ToRadians(course);
            double windDirRad = NavigationUtils.ToRadians(wind.Direction);
            
            return trueAirspeed * Math.Cos(NavigationUtils.ToRadians(
                CalculateWindCorrectionAngle(course, trueAirspeed, wind))) +
                wind.Speed * Math.Cos(windDirRad - courseRad);
        }

        /// <summary>
        /// Calculates direct course correction to reach the destination
        /// </summary>
        public static CourseCorrection CalculateDirectToDestinationCorrection(
            GeoPoint start, GeoPoint end, GeoPoint current,
            double trueAirspeed, WindData wind)
        {
            // Calculate original and direct courses
            double originalCourse = NavigationUtils.ToDegrees(
                CourseCalculator.CalculateInitialCourse(start, end));
            double directCourse = NavigationUtils.ToDegrees(
                CourseCalculator.CalculateInitialCourse(current, end));

            // Calculate basic correction angle
            double correctionAngle = NavigationUtils.Mod((directCourse - originalCourse + 180), 360) - 180;

            // Calculate wind correction
            double windCorrection = CalculateWindCorrectionAngle(directCourse, trueAirspeed, wind);

            // Calculate new course with wind correction
            double newCourse = NavigationUtils.Mod(directCourse + windCorrection, 360);

            // Calculate distance and ground speed
            double distance = DistanceCalculator.CalculateDistance(current, end) * 
                NavigationConstants.EARTH_RADIUS_KM;
            double groundSpeed = CalculateGroundSpeed(newCourse, trueAirspeed, wind);

            return new CourseCorrection(
                correctionAngle,
                windCorrection,
                newCourse,
                distance,
                groundSpeed
            );
        }

        /// <summary>
        /// Determines if a point is left or right of track
        /// </summary>
        public static bool IsRightOfTrack(GeoPoint start, GeoPoint end, GeoPoint current)
        {
            var crossTrackResult = CalculateCrossTrackError(start, end, current);
            return crossTrackResult.CrossTrackDistance > 0;
        }
    }
}