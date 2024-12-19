namespace RIntervals.Interfaces
{
    /// <summary>
    /// Дополнительный интерфейс для поддержки инициализации через исходные классы
    /// </summary>
    public interface IIntervalSource
    {
        /// <summary>
        /// Начало интервала
        /// </summary>
        DateTime StartTime { get; }

        /// <summary>
        /// Конец интервала
        /// </summary>
        DateTime EndTime { get; }
    }
}