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