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
                    base.NotifyPropertyChanged(nameof(Patch));
                    patch = value;
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
                    base.NotifyPropertyChanged(nameof(WaypointDistance));
                    waypointDistance = Math.Max(5, value);
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
                    base.NotifyPropertyChanged(nameof(MaxElevationDifference));
                    maxElevationDifference = Math.Max(2, value);
                }
            }
        }

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
                    base.NotifyPropertyChanged(nameof(ForceLinkingWaypoint));
                    forceLinkingWaypoint = value;
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
                    base.NotifyPropertyChanged(nameof(LinearPath));
                    linearPath = value;
                }
            }
        }
    }
}
