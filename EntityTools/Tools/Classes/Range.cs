using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EntityTools.Tools.Classes
{
    /// <summary>
    /// Класс, определяющий закрытый интервал [<see cref="Min"/>, <see cref="Max"/>], включающий его границы
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public partial class Range<T> where T : IComparable { }
    public partial class Range<T> : IEquatable<Range<T>>, INotifyPropertyChanged
    {
        public Range<T> Clone()
        {
            return new Range<T>
            {
                min = min, 
                max = max
            };
        }

        private T min;
        private T max;

        /// <summary>
        /// Нижняя граница диапазона
        /// </summary>
        [NotifyParentProperty(true)]
        public T Min
        {
            get => min;
            set
            {
                // Проверка упорядоченности границ диапазона, чтобы выполнялось условие [min <= max]
                if(max.CompareTo(value) < 0)
                    return;

                if (!min.Equals(value))
                {
                    min = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Верхняя граница диапазона
        /// </summary>
        [NotifyParentProperty(true)]
        public T Max
        {
            get => max;
            set
            {
                // Проверка упорядоченности границ диапазона, чтобы выполнялось условие [min <= max]
                if (min.CompareTo(value) > 0)
                    return;

                if (!max.Equals(value))
                {
                    max = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool Within(T value)
        {
            return value.CompareTo(min) >= 0
                   && value.CompareTo(max) <= 0;
        }
        public bool Outside(T value)
        {
            return value.CompareTo(min) < 0
                   || value.CompareTo(max) > 0;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool Equals(Range<T> other)
        {
            if (other is null)
                return false;

            return other.min.Equals(min)
                   && other.max.Equals(max);
        }

        public bool IsValid => min.CompareTo(max) < 0;

        public override string ToString()
        {
            return $"[{min:N1}...{max:N1}]";
        }
    }
}
