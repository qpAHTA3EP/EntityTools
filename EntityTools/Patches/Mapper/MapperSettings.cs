using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        [Bindable(true)]
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
        //[NonSerialized]
        private bool patch = true;

        /// <summary>
        /// Расстояние между соединяемыми точками пути, в режиме добавления
        /// </summary>
        [Bindable(true)]
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
        //[NonSerialized]
        private int waypointDistance = 20;

        /// <summary>
        /// Расстояние между точками пути, меньше которого они считаются совпадающими
        /// </summary>
        [Bindable(true)]
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
        //[NonSerialized]
        private int waypointEquivalenceDistance = 5;

        /// <summary>
        /// Максимальная допустимая разница высот между точками пути, соединяемыми в режиме добавления
        /// </summary>
        [Bindable(true)]
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
        //[NonSerialized]
        private int maxElevationDifference = 10;

        /// <summary>
        /// Использование кэша при прокладывании пути
        /// </summary>
        [Bindable(true)]
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
        //[NonSerialized]
        private bool cacheActive = true;

        /// <summary>
        /// Время обновления кэша
        /// </summary>
        [Bindable(true)]
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
        //[NonSerialized]
        private int cacheRegenTimeout = 5000;

        /// <summary>
        /// Принудительное связывание новой точки пути с предыдущей точкой
        /// Сохранять состояние данного флага между сессиями астрала нет смысла - установлен атрибут XmlIgnore
        /// </summary>
        [XmlIgnore]
        [Bindable(true)]
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
        //[NonSerialized]
        private bool forceLinkingWaypoint = true;

        /// <summary>
        /// Принудительное связывание новой точки пути с предыдущей точкой
        /// </summary>
        [XmlIgnore]
        [Bindable(true)]
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
        //[NonSerialized]
        private bool linearPath = true;
    }
}
