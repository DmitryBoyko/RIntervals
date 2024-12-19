namespace RIntervals.Interfaces
{
    public interface IPointSource
    {
        DateTime AtTime { get; }

        double Value { get; }
    }
}