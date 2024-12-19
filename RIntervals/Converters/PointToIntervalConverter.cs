using RIntervals.BaseClasses;
using RIntervals.ViewClasses;
using System.Collections.Immutable;

namespace RIntervals.Converters
{
    public static class PointToIntervalConverter
    {
        public static IntervalCollection<DoubleIntervalSource>
            ConvertToIntervalCollection(List<Point> points, bool sortPoints = true, , bool includeFinalInterval = false)
        {
            var intervals = new IntervalCollection<DoubleIntervalSource>();

            // Optionally sort points by AtTime to ensure intervals are correct
            var sortedPoints = sortPoints
                ? points.OrderBy(p => p.AtTime).ToImmutableList()
                : [.. points]; // If sortPoints is false, don't sort

            for (int i = 0; i < sortedPoints.Count - 1; i++)
            {
                var startPoint = sortedPoints[i];
                var endPoint = sortedPoints[i + 1];

                // Create a new DoubleIntervalSource for each consecutive pair of points
                var intervalSource = new DoubleIntervalSource(startPoint.AtTime, endPoint.AtTime, startPoint.Value);

                // Create an Interval and add it to the collection
                var interval = new Interval<DoubleIntervalSource>(intervalSource);
                intervals.AddInterval(interval);
            }

            // Optionally add an interval for the last point
            if (includeFinalInterval && sortedPoints.Count > 1)
            {
                var lastPoint = sortedPoints.Last();
                var secondLastPoint = sortedPoints[sortedPoints.Count - 2];

                // Calculate the average interval time for the last point (if available)
                var timeDiff = lastPoint.AtTime - secondLastPoint.AtTime;
                var avgInterval = timeDiff.TotalSeconds; // You can adjust this to other units like milliseconds or minutes if needed

                // Calculate the new end time by adding the average interval to the last point
                var newEndTime = lastPoint.AtTime.AddSeconds(avgInterval);

                // Create a new interval for the last point
                var intervalSource = new DoubleIntervalSource(lastPoint.AtTime, newEndTime, lastPoint.Value);

                // Create an Interval and add it to the collection
                var interval = new Interval<DoubleIntervalSource>(intervalSource);
                intervals.AddInterval(interval);
            }

            return intervals;
        }
    }
}
