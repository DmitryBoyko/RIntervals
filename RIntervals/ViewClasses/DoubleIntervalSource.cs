using RIntervals.Interfaces;

namespace RIntervals.ViewClasses
{
    public class DoubleIntervalSource(DateTime start, DateTime end, double value) : IIntervalSource
    {
        public double Value { get; set; } = value;

        public DateTime StartTime { get; set; } = start;

        public DateTime EndTime { get; set; } = end;

        public override string ToString()
        {
            return $"Source: [{StartTime} - {EndTime}] Value: {Value}";
        }
    }
}