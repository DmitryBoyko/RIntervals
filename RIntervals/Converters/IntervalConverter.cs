using RIntervals.BaseClasses;
using RIntervals.Interfaces;

namespace RIntervals.Converters
{
    public static class IntervalConverter
    {
        /// <summary>
        /// Конвертирование произвольного класса в Interval<T>.
        /// Предполагается что исходный класс содержит похожими свойствами начала и конца.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="includeSource"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static Interval<T> ConvertToInterval<T, TSource>(TSource source, bool includeSource = false) where T : IIntervalSource
        {
            // Рефлексия для поиска свойств Begin и End
            var startProperty = typeof(TSource).GetProperty("Begin");
            var endProperty = typeof(TSource).GetProperty("End");

            if (startProperty == null || endProperty == null)
            {
                throw new ArgumentException("Source class must have Begin and End properties.");
            }

            var start = (DateTime)startProperty.GetValue(source);
            var end = (DateTime)endProperty.GetValue(source);

            // Создание объекта типа T (это может быть кастомный тип, реализующий IIntervalSource)
            var intervalSource = (T)Activator.CreateInstance(typeof(T), new object[] { start, end });

            var interval = new Interval<T>(intervalSource);

            // Optionally save the source object
            if (includeSource)
            {
                interval.Source = intervalSource;
            }

            return interval;
        }

        /// <summary>
        /// Конвертирование произвольного класса в Interval<T>.
        /// Позволяет передавать списки для имен свойств StartTime и EndTime. Вместо жестко заданных строк "Begin" и "End" 
        /// можно передавать списки имен свойств для поиска соответствующих значений в объекте.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="startPropertyNames"></param>
        /// <param name="endPropertyNames"></param>
        /// <param name="includeSource"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static Interval<T> ConvertToInterval<T, TSource>(TSource source, List<string> startPropertyNames,
            List<string> endPropertyNames, bool includeSource = false) where T : IIntervalSource
        {
            DateTime start = DateTime.MinValue;
            DateTime end = DateTime.MinValue;

            // Поиск свойства для начала интервала
            foreach (var startName in startPropertyNames)
            {
                var startProperty = typeof(TSource).GetProperty(startName);
                if (startProperty != null)
                {
                    start = (DateTime)startProperty.GetValue(source);
                    break; // Прерываем после первого найденного свойства
                }
            }

            // Если не нашли подходящее свойство для начала, выбрасываем исключение
            if (start == DateTime.MinValue)
            {
                throw new ArgumentException("Не найдено свойство для начала интервала.");
            }

            // Поиск свойства для конца интервала
            foreach (var endName in endPropertyNames)
            {
                var endProperty = typeof(TSource).GetProperty(endName);
                if (endProperty != null)
                {
                    end = (DateTime)endProperty.GetValue(source);
                    break; // Прерываем после первого найденного свойства
                }
            }

            // Если не нашли подходящее свойство для конца, выбрасываем исключение
            if (end == DateTime.MinValue)
            {
                throw new ArgumentException("Не найдено свойство для конца интервала.");
            }

            // Создаем объект типа T с полученными значениями для StartTime и EndTime
            var intervalSource = (T)Activator.CreateInstance(typeof(T), new object[] { start, end });

            var interval = new Interval<T>(intervalSource);

            // Optionally save the source object
            if (includeSource)
            {
                interval.Source = intervalSource;
            }

            return interval;
        }
    }
}

/*
 public class MyIntervalSource : IIntervalSource
{
    public DateTime StartTime { get; }
    public DateTime EndTime { get; }

    public MyIntervalSource(DateTime startTime, DateTime endTime)
    {
        StartTime = startTime;
        EndTime = endTime;
    }
}

// Использование
public class Program
{
    public static void Main(string[] args)
    {
        var customObj = new MyCustomClass
        {
            Begin = DateTime.Now,
            End = DateTime.Now.AddHours(1)
        };

        // Преобразование в Interval<MyIntervalSource>
        var interval = IntervalConverter.ConvertToInterval<MyIntervalSource, MyCustomClass>(customObj);

        Console.WriteLine($"Start: {interval.Start}, End: {interval.End}");
    }
}

// Использование
public class Program
{
    public static void Main(string[] args)
    {
        var customObj = new MyCustomClass
        {
            StartTime = DateTime.Now,
            EndTime = DateTime.Now.AddHours(1)
        };

        var startProperties = new List<string> { "StartTime", "Begin" };
        var endProperties = new List<string> { "EndTime", "Finish" };

        // Преобразование в Interval<MyIntervalSource>
        var interval = ConvertToInterval<MyIntervalSource, MyCustomClass>(customObj, startProperties, endProperties);

        Console.WriteLine($"Start: {interval.Start}, End: {interval.End}");
    }
}

 */
