using RIntervals.Interfaces;

namespace RIntervals.ViewClasses
{
    /// <summary>
    /// Реализация IIntervalSource по умолчанию
    /// </summary>
    public class MyIntervalSource : IIntervalSource
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public MyIntervalSource(DateTime start, DateTime end)
        {
            StartTime = start;
            EndTime = end;
        }

        public override string ToString()
        {
            return $"Source: [{StartTime} - {EndTime}]";
        }
    }
}