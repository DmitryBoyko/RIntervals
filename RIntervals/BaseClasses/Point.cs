using RIntervals.Interfaces;

namespace RIntervals.BaseClasses
{
    public class Point : IPointSource
    {
        /// <summary>
        /// The time when the point is recorded.
        /// </summary>
        public DateTime AtTime { get; private set; }

        /// <summary>
        /// The value associated with the point.
        /// </summary>
        public double Value { get; private set; }


        public Point(DateTime atDateTime, double value)
        {
            AtTime = atDateTime;
            Value = value;
        }
    }
}