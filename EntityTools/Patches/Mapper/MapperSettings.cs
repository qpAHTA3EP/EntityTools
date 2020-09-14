using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace EntityTools.Settings
{
#if PATCH_ASTRAL
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
                    WaypointEquivalenceDistance = Math.Min(waypointDistance / 2, waypointEquivalenceDistance);
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
                    waypointEquivalenceDistance = Math.Max(2, Math.Min(value, WaypointDistance / 2)); ;
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

        /// <summary>
        /// Настройки формы <seealso cref="MapperFormExt"/>
        /// </summary>
        public MapperFormSettings MapperForm { get; set; } = new MapperFormSettings();
    }

    /// <summary>
    /// Настройки формы <seealso cref="MapperFormExt"/>
    /// </summary>
    [Serializable]
    public class MapperFormSettings : PluginSettingsBase
    {
        /// <summary>
        /// Координаты Mapper
        /// </summary>
        [Bindable(true)]
        public Point Location
        {
            get => location;
            set
            {
                if (location != value)
                {
                    location = value;
                    base.NotifyPropertyChanged(nameof(Location));
                }
            }
        }
        public Point location = new Point();

        /// <summary>
        /// Видимость главной панели инструментов
        /// </summary>
        [Bindable(true)]
        public bool MainToolsBarVisible
        {
            get => _mainToolsBarVisible;
            set
            {
                if (_mainToolsBarVisible != value)
                {
                    _mainToolsBarVisible = value;
                    base.NotifyPropertyChanged(nameof(MainToolsBarVisible));
                }
            }
        }
        //[NonSerialized]
        private bool _mainToolsBarVisible = true;

        /// <summary>
        /// Видимость панелии редактирования графа (мешей)
        /// </summary>
        [Bindable(true)]
        public bool EditMeshesBarVisible
        {
            get => editMeshesBarVisible;
            set
            {
                if (editMeshesBarVisible != value)
                {
                    statusBarVisible = value;
                    base.NotifyPropertyChanged(nameof(EditMeshesBarVisible));
                }
            }
        }
        //[NonSerialized]
        private bool editMeshesBarVisible = true;

        /// <summary>
        /// Видимость строки состояния
        /// </summary>
        [Bindable(true)]
        public bool StatusBarVisible
        {
            get => statusBarVisible;
            set
            {
                if (statusBarVisible != value)
                {
                    statusBarVisible = value;
                    base.NotifyPropertyChanged(nameof(StatusBarVisible));
                }
            }
        }
        //[NonSerialized]
        private bool statusBarVisible = true;
    }
#endif
}
