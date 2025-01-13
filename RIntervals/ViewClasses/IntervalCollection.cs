using RIntervals.BaseClasses;
using RIntervals.Interfaces;

namespace RIntervals.ViewClasses
{
    /// <summary>
    /// Класс IntervalCollection<T> представляет собой коллекцию интервалов времени, 
    /// где каждый интервал имеет начало и конец, а также может быть ассоциирован с некоторым источником данных (IIntervalSource). 
    /// 
    /// Этот класс предоставляет возможности для добавления интервалов, а также выполнения различных операций над интервалами, 
    /// таких как объединение, пересечение и выделение отдельных частей интервалов. Класс поддерживает операции над интервалами, 
    /// которые могут быть полезны в широком диапазоне приложений, включая планирование, обработку данных о времени и другие задачи.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class IntervalCollection<T> where T : IIntervalSource
    {
        private readonly List<Interval<T>> intervals = new List<Interval<T>>();

        #region Get data

        /// <summary>
        /// Получить все интервалы
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Interval<T>> GetIntervals()
        {
            return intervals;
        }

        /// <summary>
        /// Получить все интервалы
        /// </summary>
        /// <returns></returns>
        public List<Interval<T>> Intervals => Interval<T>.GetFreeIntervals(intervals);

        #endregion

        #region Init the collection

        /// <summary>
        /// Добавить интервал в коллекцию
        /// </summary>
        /// <param name="interval"></param>
        public void AddInterval(Interval<T> interval)
        {
            intervals.Add(interval);
        }

        /// <summary>
        /// Добавить интервал в коллекцию
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="intervalSource"></param>
        public void AddInterval(DateTime startTime, DateTime endTime, T intervalSource)
        {
            var interval = new Interval<T>(startTime, endTime, intervalSource);

            intervals.Add(interval);
        }

        /// <summary>
        /// Add an item of type T to the collection, ensuring the type matches
        /// </summary>
        /// <param name="item"></param>
        public void AddInterval<TSource>(TSource item) where TSource : IIntervalSource
        {
            if (item is T sourceAsT)
            {
                var interval = new Interval<T>(sourceAsT.StartTime, sourceAsT.EndTime, sourceAsT);
                intervals.Add(interval);
            }
            else
            {
                throw new ArgumentException($"Invalid type: {typeof(TSource)}. Expected: {typeof(T)}");
            }
        }

        #endregion

        #region Операции над интервалами коллекции

        /// <summary>
        /// Find intervals with unique values in sequences starting from the first value in the collection.
        /// Only the first interval in each sequence of consecutive intervals with the same value is included.
        /// </summary>
        /// <returns>A list of intervals containing the first interval of each unique sequence of values.</returns>
        public List<Interval<T>> FindUniqueIntervalsByFirstValueSequence()
        {
#if NET5_0_OR_GREATER
            // Ensure T is assignable to DoubleIntervalSource
            if (!typeof(T).IsAssignableTo(typeof(DoubleIntervalSource)))
#else
    if (!typeof(DoubleIntervalSource).IsAssignableFrom(typeof(T)))
#endif
            {
                throw new InvalidOperationException("This method is only applicable to collections of DoubleIntervalSource.");
            }

            if (intervals.Count == 0)
            {
                return new List<Interval<T>>(); // Return an empty list if there are no intervals
            }

            var uniqueIntervals = new List<Interval<T>>();
            Interval<T>? lastAddedInterval = null;
            double? lastValue = null;

            foreach (var interval in intervals)
            {
                var source = interval.Source as DoubleIntervalSource;

                if (source == null)
                {
                    throw new InvalidOperationException("Interval does not contain a valid DoubleIntervalSource.");
                }

                if (lastValue == null || source.Value != lastValue)
                {
                    // Add the interval if it's the first or its value differs from the last added interval
                    uniqueIntervals.Add(interval);
                    lastValue = source.Value;
                    lastAddedInterval = interval;
                }
            }

            return uniqueIntervals;
        }


        /// <summary>
        /// Find intervals with unique values in sequences of intervals sharing the same value.
        /// For a given value, only the first interval in the sequence of consecutive intervals is included.
        /// </summary>
        /// <param name="targetValue">The target value to filter the sequences.</param>
        /// <returns>A list of intervals containing the first interval of each sequence with the specified value.</returns>
        public List<Interval<T>> FindUniqueIntervalsByValueSequence(double targetValue)
        {
#if NET5_0_OR_GREATER
            // Ensure T is assignable to DoubleIntervalSource
            if (!typeof(T).IsAssignableTo(typeof(DoubleIntervalSource)))
#else
    if (!typeof(DoubleIntervalSource).IsAssignableFrom(typeof(T)))
#endif
            {
                throw new InvalidOperationException("This method is only applicable to collections of DoubleIntervalSource.");
            }

            var uniqueIntervals = new List<Interval<T>>();

            Interval<T>? lastAddedInterval = null;

            foreach (var interval in intervals)
            {
                var source = interval.Source as DoubleIntervalSource;

                if (source != null && source.Value == targetValue)
                {
                    // Add the first interval in a new sequence of matching intervals
                    if (lastAddedInterval == null || !ReferenceEquals(lastAddedInterval, interval))
                    {
                        uniqueIntervals.Add(interval);
                        lastAddedInterval = interval;
                    }
                }
                else
                {
                    // Reset last added interval when the value changes
                    lastAddedInterval = null;
                }
            }

            return uniqueIntervals;
        }


        /// <summary>
        /// Find all intervals where the Value property of DoubleIntervalSource matches the specified value.
        /// </summary>
        /// <param name="targetValue">The value to search for.</param>
        /// <returns>List of intervals where Value matches the target value.</returns>
        public List<Interval<T>> FindIntervalsByValue(double targetValue)
        {
#if NET5_0_OR_GREATER
            // Use IsAssignableTo in .NET 5 or later
            if (!typeof(T).IsAssignableTo(typeof(DoubleIntervalSource)))
#else
    // Use IsAssignableFrom for earlier versions
    if (!typeof(DoubleIntervalSource).IsAssignableFrom(typeof(T)))
#endif
            {
                throw new InvalidOperationException("This method is only applicable to collections of DoubleIntervalSource.");
            }

            return intervals
                .Where(interval => (interval.Source as DoubleIntervalSource)?.Value == targetValue)
                .ToList();
        }

        /// <summary>
        /// Найти все интервалы, пересекающиеся с заданным
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public List<Interval<T>> FindIntersects(Interval<T> target)
        {
            return intervals.Where(i => i.IntersectsWith(target)).ToList();
        }

        /// <summary>
        /// Слить все пересекающиеся интервалы
        /// </summary>
        /// <returns></returns>
        public List<Interval<T>> MergeIntersectingIntervals()
        {
            if (intervals.Count == 0) return [];

            // Сортируем интервалы по началу
            var sortedIntervals = intervals.OrderBy(i => i.Start).ToList();
            var merged = new List<Interval<T>>();
            var current = sortedIntervals[0];

            foreach (var next in sortedIntervals.Skip(1))
            {
                if (current.IntersectsWith(next))
                {
                    // Сливаем интервалы, если они пересекаются
                    current = current.MergeWith(next);
                }
                else
                {
                    merged.Add(current);
                    current = next;
                }
            }

            merged.Add(current);
            return merged;
        }

        /// <summary>
        /// Слить все не пересекающиеся интервалы
        /// </summary>
        /// <returns></returns>
        public List<Interval<T>> MergeNonIntersectingIntervals()
        {
            if (intervals.Count == 0) return [];

            // Сортируем интервалы по началу
            var sortedIntervals = intervals.OrderBy(i => i.Start).ToList();
            var merged = new List<Interval<T>>();
            var current = sortedIntervals[0];

            foreach (var next in sortedIntervals.Skip(1))
            {
                if (!current.IntersectsWith(next))
                {
                    // Если интервалы не пересекаются, сливаем их
                    merged.Add(current);
                    current = next;
                }
                else
                {
                    // Если интервалы пересекаются, оставляем текущий, так как они не будут сливаться
                    // если они пересекаются, но не будут объединяться
                    current = new Interval<T>(
                        start: current.Start < next.Start ? current.Start : next.Start,
                        end: current.End > next.End ? current.End : next.End,
                        source: current.Source
                    );
                }
            }

            // Добавляем последний интервал
            merged.Add(current);
            return merged;
        }

        /// <summary>
        /// Выделить все пересекающиеся интервалы в отдельный список
        /// </summary>
        /// <returns></returns>
        public List<Interval<T>> GetIntersectingIntervals()
        {
            if (intervals.Count == 0) return [];

            // Сортируем интервалы по началу
            var sortedIntervals = intervals.OrderBy(i => i.Start).ToList();
            var intersecting = new List<Interval<T>>();
            var current = sortedIntervals[0];

            foreach (var next in sortedIntervals.Skip(1))
            {
                if (current.IntersectsWith(next))
                {
                    // Если интервалы пересекаются, добавляем их в список пересекающихся
                    intersecting.Add(current);
                    intersecting.Add(next);
                    current = current.MergeWith(next); // Сливаем интервалы для дальнейшей обработки
                }
                else
                {
                    current = next; // Переходим к следующему интервалу, если пересечений нет
                }
            }

            // Добавляем последний интервал, если он был частью пересечения
            if (!intersecting.Contains(current))
            {
                intersecting.Add(current);
            }

            return intersecting.Distinct().ToList(); // Возвращаем уникальные пересекающиеся интервалы
        }

        /// <summary>
        /// Выделить только пересекающиеся части интервалов в отдельный список.
        /// </summary>
        /// <returns></returns>
        public List<Interval<T>> GetIntersectingParts()
        {
            if (intervals.Count == 0) return [];

            // Сортируем интервалы по началу
            var sortedIntervals = intervals.OrderBy(i => i.Start).ToList();
            var intersectingParts = new List<Interval<T>>();

            var current = sortedIntervals[0];

            foreach (var next in sortedIntervals.Skip(1))
            {
                if (current.IntersectsWith(next))
                {
                    // Находим пересечение интервалов
                    var intersectStart = current.Start < next.Start ? next.Start : current.Start;
                    var intersectEnd = current.End > next.End ? next.End : current.End;

                    // Добавляем пересечение в список
                    intersectingParts.Add(new Interval<T>(intersectStart, intersectEnd, current.Source));
                }

                // Перемещаемся к следующему интервалу
                current = next;
            }

            return intersectingParts;
        }

        #endregion

        #region Операции над точками и интервалами

        /// <summary>
        /// Найти все интервалы, в которых находится точка.
        /// </summary>
        /// <param name="point">Точка, которую нужно проверить</param>
        /// <returns>Коллекция интервалов, в которых содержится точка</returns>
        public List<Interval<T>> FindIntervalsContainingPoint(Point point)
        {
            var result = new List<Interval<T>>();

            // Проверка каждого интервала, содержится ли точка в интервале
            foreach (var interval in intervals)
            {
                if (interval.IsPointInside(point))
                {
                    result.Add(interval); // Добавляем интервал в результат
                }
            }

            return result;
        }

        #endregion
    }
}