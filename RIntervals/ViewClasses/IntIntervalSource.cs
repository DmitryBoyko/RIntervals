using RIntervals.Interfaces;

namespace RIntervals.ViewClasses
{
    /// <summary>
    /// Реализация IIntervalSource для примитивных типов данных
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="value"></param>
    public class IntIntervalSource(DateTime start, DateTime end, int value) : IIntervalSource
    {
        public int Value { get; set; } = value;

        public DateTime StartTime { get; set; } = start;

        public DateTime EndTime { get; set; } = end;

        public override string ToString()
        {
            return $"Source: [{StartTime} - {EndTime}] Value: {Value}";
        }
    }
}