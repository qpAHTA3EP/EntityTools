using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace EntityTools.Settings
{
    [Serializable]
    public class MapperSettings : PluginSettingsBase
    {
        /// <summary>
        /// Активация и деактивация патча
        /// </summary>
        [NonSerialized]
        private bool patch = true;
        public bool Patch
        {
            get => patch;
            set
            {
                if (patch != value)
                {
                    patch = value;
                    base.NotifyPropertyChanged(nameof(Patch));
                }
            }
        }

        /// <summary>
        /// Расстояние между соединяемыми точками пути, в режиме добавления
        /// </summary>
        [NonSerialized]
        private int waypointDistance = 20;
        public int WaypointDistance
        {
            get => waypointDistance;
            set
            {
                if (waypointDistance != value)
                {
                    waypointDistance = Math.Max(5, value);
                    base.NotifyPropertyChanged(nameof(WaypointDistance));
                    WaypointEquivalenceDistance = Math.Min(waypointDistance/2, waypointEquivalenceDistance);
                }
            }
        }

        /// <summary>
        /// Расстояние между точками пути, меньше которого они считаются совпадающими
        /// </summary>
        [NonSerialized]
        private int waypointEquivalenceDistance = 5;
        public int WaypointEquivalenceDistance
        {
            get => waypointEquivalenceDistance;
            set
            {
                if (waypointEquivalenceDistance != value)
                {
                    waypointEquivalenceDistance = Math.Max(2, Math.Min(value, WaypointDistance/2)); ;
                    base.NotifyPropertyChanged(nameof(WaypointEquivalenceDistance));
                }
            }
        }

        /// <summary>
        /// Максимальная допустимая разница высот между точками пути, соединяемыми в режиме добавления
        /// </summary>
        [NonSerialized]
        private int maxElevationDifference = 10;
        public int MaxElevationDifference
        {
            get => maxElevationDifference;
            set
            {
                if (maxElevationDifference != value)
                {
                    maxElevationDifference = Math.Max(2, value);
                    base.NotifyPropertyChanged(nameof(MaxElevationDifference));
                }
            }
        }

        /// <summary>
        /// Использование кэша при прокладывании пути
        /// </summary>
        [NonSerialized]
        private bool cacheActive = true;
        public bool CacheActive
        {
            get => cacheActive;
            set
            {
                if (cacheActive != value)
                {
                    cacheActive = value;
                    base.NotifyPropertyChanged(nameof(CacheActive));
                }
            }
        }
        /// <summary>
        /// Время обновления кэша
        /// </summary>
        public int CacheRegenTimeout
        {
            get => cacheRegenTimeout;
            set
            {
                if (value != cacheRegenTimeout)
                {
                    cacheRegenTimeout = Math.Max(1000, value);
                    base.NotifyPropertyChanged(nameof(cacheRegenTimeout));
                }
            }
        }
        private int cacheRegenTimeout = 5000;

        /// <summary>
        /// Принудительное связывание новой точки пути с предыдущей точкой
        /// Сохранять состояние данного флага между сессиями астрала нет смысла - установлен атрибут XmlIgnore
        /// </summary>
        [NonSerialized]
        private bool forceLinkingWaypoint = true;
        [XmlIgnore]
        public bool ForceLinkingWaypoint
        {
            get => forceLinkingWaypoint;
            set
            {
                if (forceLinkingWaypoint != value)
                {
                    forceLinkingWaypoint = value;
                    base.NotifyPropertyChanged(nameof(ForceLinkingWaypoint));
                }
            }
        }

        /// <summary>
        /// Принудительное связывание новой точки пути с предыдущей точкой
        /// </summary>
        [NonSerialized]
        private bool linearPath = true;
        public bool LinearPath
        {
            get => linearPath;
            set
            {
                if (linearPath != value)
                {
                    linearPath = value;
                    base.NotifyPropertyChanged(nameof(LinearPath));
                }
            }
        }
    }
}
