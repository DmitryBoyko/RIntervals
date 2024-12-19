using RIntervals.Interfaces;

namespace RIntervals.ViewClasses
{
    /// <summary>
    /// Реализация "пустого" источника.
    /// Заглушка для источника, реализующая IIntervalSource.
    /// </summary>
    public class NullIntervalSource : IIntervalSource
    {
        public DateTime StartTime => DateTime.MinValue; // Псевдовыходное значение

        public DateTime EndTime => DateTime.MinValue; // Псевдовыходное значение

        public override string ToString()
        {
            return "NullIntervalSource";
        }
    }
}