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
        /// Расстояние между соединяемыми точками пути, в режиме добавления
        /// </summary>
        [NonSerialized]
        private int waipointDistance = 20;
        public int WaipointDistance
        {
            get => waipointDistance;
            set
            {
                if (waipointDistance != value && value > 0)
                {
                    base.NotifyPropertyChanged(nameof(WaipointDistance));
                    waipointDistance = value;
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
                if (maxElevationDifference != value && value > 0)
                {
                    base.NotifyPropertyChanged(nameof(MaxElevationDifference));
                    maxElevationDifference = value;
                }
            }
        }

        /// <summary>
        /// Принудительное связывание новой точки пути с предыдущей точкой
        /// </summary>
        [NonSerialized]
        private bool forceLinkingWaypoint = true;
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
