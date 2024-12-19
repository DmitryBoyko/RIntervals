using RIntervals.Interfaces;
using RIntervals.ViewClasses;

namespace RIntervals.BaseClasses
{
    /// <summary>
    /// Класс предназначен для работы с интервалами времени и может использоваться в разных контекстах, 
    /// где необходимо работать с временными диапазонами и связанной с ними дополнительной информацией.
    /// 
    /// Класс Interval<T> представляет временной интервал с дополнительной возможностью привязки 
    /// к произвольному источнику данных. Он реализует операции над интервалами, такие как объединение, 
    /// сдвиг, разделение и проверку пересечений.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class Interval<T> where T : IIntervalSource
    {
        #region Prts

        /// <summary>
        /// Начало интервала
        /// </summary>
        public DateTime Start { get; set; }

        /// <summary>
        /// Конец интервала
        /// </summary>
        public DateTime End { get; set; }

        /// <summary>
        /// Произвольный исходный объект
        /// </summary>
        public T Source { get; set; }

        /// <summary>
        /// Произвольный объект для хранения данных например, скорость, вес.
        /// </summary>
        public object? AdditionalData { get; set; }

        #endregion

        #region Ctrs

        /// <summary>
        /// Конструктор: инициализация через разные способы
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="source"></param>
        /// <exception cref="ArgumentException"></exception>
        public Interval(DateTime start, DateTime end, T source = default)
        {
            if (start > end)
                throw new ArgumentException("Start time cannot be after end time.");

            Start = start;
            End = end;
            Source = source;
        }

        /// <summary>
        /// Конструктор через произвольный источник
        /// </summary>
        /// <param name="source"></param>
        /// <exception cref="ArgumentException"></exception>
        public Interval(T source)
        {
            // Проверка на null для source
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (source.StartTime > source.EndTime)
                throw new ArgumentException("Start time cannot be after end time.");

            Start = source.StartTime;
            End = source.EndTime;
            Source = source;
        }

        /// <summary>
        /// Конструктор через произвольный источник и дополнительные данные
        /// </summary>
        /// <param name="source"></param>
        /// <param name="additionalData"></param>
        /// <exception cref="ArgumentException"></exception>
        public Interval(T source, object additionalData)
        {
            // Проверка на null для source
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (source.StartTime > source.EndTime)
                throw new ArgumentException("Start time cannot be after end time.");

            Start = source.StartTime;
            End = source.EndTime;
            Source = source;
            AdditionalData = additionalData;
        }

        #endregion

        #region Init methods

        /// <summary>
        /// Метод для инициализации поля AdditionalData
        /// </summary>
        /// <param name="additionalData"></param>
        public void SetAdditionalData(object additionalData)
        {
            AdditionalData = additionalData;
        }

        #endregion

        #region Get AdditionalData methods

        /// <summary>
        /// Получение дополнительного значения как Double
        /// </summary>
        /// <returns></returns>
        public double? GetAdditionalValueAsDouble()
        {
            if (AdditionalData == null)
            {
                return null; // Если данных нет, возвращаем null
            }

            // Попытка приведения AdditionalData к double
            if (AdditionalData is double d)
            {
                return d;
            }

            // Если AdditionalData является строкой, пробуем преобразовать её в double
            if (AdditionalData is string str && double.TryParse(str, out double result))
            {
                return result;
            }

            // Если это int, long, или другой числовой тип, можем попытаться привести их к double
            if (AdditionalData is IConvertible convertible)
            {
                try
                {
                    return Convert.ToDouble(convertible);
                }
                catch
                {
                    return null; // Если не удалось конвертировать, возвращаем null
                }
            }

            return null; // Для всех остальных типов, которые не можем привести к double, возвращаем null
        }

        public int? GetAdditionalValueAsInt()
        {
            if (AdditionalData == null)
            {
                return null; // Если данных нет, возвращаем null
            }

            // Попытка приведения AdditionalData к int
            if (AdditionalData is int i)
            {
                return i;
            }

            // Если AdditionalData является строкой, пробуем преобразовать её в int
            if (AdditionalData is string str && int.TryParse(str, out int result))
            {
                return result;
            }

            // Если это другой числовой тип, пробуем конвертировать его в int
            if (AdditionalData is IConvertible convertible)
            {
                try
                {
                    return Convert.ToInt32(convertible);
                }
                catch
                {
                    return null; // Если не удалось конвертировать, возвращаем null
                }
            }

            return null; // Для всех остальных типов, которые не можем привести к int, возвращаем null
        }

        public float? GetAdditionalValueAsFloat()
        {
            if (AdditionalData == null)
            {
                return null; // Если данных нет, возвращаем null
            }

            // Попытка приведения AdditionalData к float
            if (AdditionalData is float f)
            {
                return f;
            }

            // Если AdditionalData является строкой, пробуем преобразовать её в float
            if (AdditionalData is string str && float.TryParse(str, out float result))
            {
                return result;
            }

            // Если это другой числовой тип, пробуем конвертировать его в float
            if (AdditionalData is IConvertible convertible)
            {
                try
                {
                    return Convert.ToSingle(convertible);
                }
                catch
                {
                    return null; // Если не удалось конвертировать, возвращаем null
                }
            }

            return null; // Для всех остальных типов, которые не можем привести к float, возвращаем null
        }

        public string GetAdditionalValueAsString()
        {
            if (AdditionalData == null)
            {
                return "No data";
            }

            switch (AdditionalData)
            {
                case string str:
                    return str;
                case int i:
                    return i.ToString();
                case double d:
                    return d.ToString("F2"); // Формат с двумя знаками после запятой для чисел
                case DateTime dt:
                    return dt.ToString("yyyy-MM-dd HH:mm:ss"); // Пример формата для DateTime
                default:
                    return AdditionalData.ToString();
            }
        }

        #endregion

        #region Операции над интервалами

        /// <summary>
        /// Объединение интервалов
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public Interval<T> UnionWith(Interval<T> other)
        {
            if (!IntersectsWith(other))
                throw new InvalidOperationException("Intervals do not intersect.");

            var start = Start < other.Start ? Start : other.Start;
            var end = End > other.End ? End : other.End;

            return new Interval<T>(Source) { Start = start, End = end };
        }

        /// <summary>
        /// Продолжительность интервала (Вычисление длительности интервала)
        /// </summary>
        public TimeSpan Duration()
        {
            return End - Start;
        }

        /// <summary>
        /// Разделение интервала
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public List<Interval<T>> DifferenceWith(Interval<T> other)
        {
            var result = new List<Interval<T>>();

            if (!IntersectsWith(other))
            {
                result.Add(this);
                return result;
            }

            if (Start < other.Start)
            {
                result.Add(new Interval<T>(Source) { Start = Start, End = other.Start });
            }

            if (End > other.End)
            {
                result.Add(new Interval<T>(Source) { Start = other.End, End = End });
            }

            return result;
        }

        /// <summary>
        /// Сдвиг интервала
        /// </summary>
        /// <param name="shiftBy"></param>
        /// <returns></returns>
        public Interval<T> Shift(TimeSpan shiftBy)
        {
            return new Interval<T>(Source) { Start = Start + shiftBy, End = End + shiftBy };
        }

        /// <summary>
        /// Проверка включения
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Contains(Interval<T> other)
        {
            return Start <= other.Start && End >= other.End;
        }

        /// <summary>
        /// Разделение интервала на несколько
        /// </summary>
        /// <param name="splitInterval"></param>
        /// <returns></returns>
        public List<Interval<T>> Split(TimeSpan splitInterval)
        {
            var result = new List<Interval<T>>();
            var currentStart = Start;

            while (currentStart + splitInterval <= End)
            {
                var currentEnd = currentStart + splitInterval;
                result.Add(new Interval<T>(Source) { Start = currentStart, End = currentEnd });
                currentStart = currentEnd;
            }

            if (currentStart < End)
            {
                result.Add(new Interval<T>(Source) { Start = currentStart, End = End });
            }

            return result;
        }

        /// <summary>
        /// Проверка на пересечение с конкретным временем
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public bool ContainsTime(DateTime time)
        {
            return time >= Start && time <= End;
        }

        /// <summary>
        /// Анализ незанятых промежутков
        /// </summary>
        /// <param name="intervals"></param>
        /// <returns></returns>
        public static List<Interval<T>> GetFreeIntervals(List<Interval<T>> intervals)
        {
            var sortedIntervals = intervals.OrderBy(i => i.Start).ToList();
            var freeIntervals = new List<Interval<T>>();

            DateTime lastEnd = DateTime.MinValue;

            foreach (var interval in sortedIntervals)
            {
                if (interval.Start > lastEnd)
                {
                    freeIntervals.Add(new Interval<T>(interval.Source) { Start = lastEnd, End = interval.Start });
                }
                lastEnd = interval.End;
            }

            return freeIntervals;
        }

        /// <summary>
        /// Пересекаются ли два временных интервала
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool IntersectsWith(Interval<T> other)
        {
            // Интервалы пересекаются, если один из них начинается до того, как заканчивается другой,
            // и заканчивается после того, как начинается другой.
            return Start < other.End && End > other.Start;
        }

        /// <summary>
        /// Применение коэффициента
        /// </summary>
        /// <param name="factor"></param>
        /// <returns></returns>
        public Interval<T> Scale(double factor)
        {
            var duration = Duration();
            var newDuration = TimeSpan.FromTicks((long)(duration.Ticks * factor));

            var newStart = Start;
            var newEnd = newStart + newDuration;

            return new Interval<T>(Source) { Start = newStart, End = newEnd };
        }

        /// <summary>
        /// Слияние двух интервалов (если они пересекаются)
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public Interval<T> MergeWith(Interval<T> other)
        {
            if (!IntersectsWith(other))
                throw new InvalidOperationException("Intervals do not overlap.");

            // Если типы источников совпадают
            if (typeof(T) != typeof(NullIntervalSource))
            {
                // Слияние интервалов без использования "заглушки"
                var start = Start < other.Start ? Start : other.Start;
                var end = End > other.End ? End : other.End;

                // Создаем новый интервал с тем же источником, если он не NullIntervalSource
                var result = new Interval<T>(Source) { Start = start, End = end };
                return result;
            }

            // Если источники несовместимы, используем специальный источник "заглушку"
            var nullSource = new NullIntervalSource();
            var startFallback = Start < other.Start ? Start : other.Start;
            var endFallback = End > other.End ? End : other.End;

            // Создаем новый интервал с "заглушкой" для слияния
            var fallbackResult = new Interval<NullIntervalSource>(nullSource) { Start = startFallback, End = endFallback };

            // Преобразуем его в возвращаемый тип T
            return ConvertToIntervalT(fallbackResult);
        }

        /// <summary>
        /// Метод для преобразования интервала с заглушкой в нужный тип
        /// </summary>
        /// <param name="fallbackResult"></param>
        /// <returns></returns>
        private Interval<T> ConvertToIntervalT(Interval<NullIntervalSource> fallbackResult)
        {
            // Преобразуем результат с "заглушкой" в тип Interval<T>, если возможно
            // Допустим, нам нужно оставить его как есть, но с типом T
            var result = new Interval<T>(Source) { Start = fallbackResult.Start, End = fallbackResult.End };
            return result;
        }

        #endregion

        #region Операции над точками и интервалами

        /// <summary>
        /// Проверка, находится ли точка внутри интервала
        /// </summary>
        public bool IsPointInside(Point point)
        {
            // Check if the AtTime of the Point is between the Start and End of the Interval
            return point.AtTime >= Start && point.AtTime <= End;
        }

        #endregion

        public override string ToString()
        {
            return $"[{Start} - {End}], Source: {Source}";
        }
    }
}