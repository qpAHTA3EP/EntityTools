using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Xml.Serialization;
using EntityTools.Patches.Mapper;
using EntityTools.Tools;

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
                    NotifyPropertyChanged(nameof(Patch));
                }
            }
        }
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
                    NotifyPropertyChanged(nameof(WaypointDistance));
                    WaypointEquivalenceDistance = Math.Min(waypointDistance / 2, waypointEquivalenceDistance);
                }
            }
        }
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
                    NotifyPropertyChanged(nameof(WaypointEquivalenceDistance));
                }
            }
        }
        private int waypointEquivalenceDistance = 10;

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
                    NotifyPropertyChanged(nameof(MaxElevationDifference));
                }
            }
        }
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
                    NotifyPropertyChanged(nameof(CacheActive));
                }
            }
        }
        private bool cacheActive = true;
        
        /// <summary>
        /// Радиус кэширования
        /// </summary>
        [Bindable(true)]
        public int CacheRadius
        {
            get => cacheRadius;
            set
            {
                if (value != cacheRadius)
                {
                    cacheRadius = Math.Max(value, Math.Max(50, waypointDistance * 3));
                    NotifyPropertyChanged(nameof(CacheRadius));
                }
            }
        }
        private int cacheRadius = 50;


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
                    cacheRegenTimeout = Math.Max(500, value);
                    NotifyPropertyChanged(nameof(cacheRegenTimeout));
                }
            }
        }
        private int cacheRegenTimeout = 500;

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
                    NotifyPropertyChanged(nameof(ForceLinkingWaypoint));
                }
            }
        }
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
                    NotifyPropertyChanged(nameof(LinearPath));
                }
            }
        }
        private bool linearPath = false;

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
            get
            {
                var screenSize = Screen.PrimaryScreen.Bounds.Size;
                if (_location.X < 0 || _location.X > screenSize.Width)
                    _location.X = 60;
                if (_location.Y < 0 || _location.Y > screenSize.Height)
                    _location.Y = 60;
                return _location;
            }

            set
            {
                if (_location != value)
                {
                    var screenSize = Screen.PrimaryScreen.Bounds.Size;
                    if (value.X < 0 || value.X > screenSize.Width)
                        value.X = 60;
                    if (value.Y < 0 || value.Y > screenSize.Height)
                        value.Y = 60;

                    _location = value;
                    NotifyPropertyChanged(nameof(Location));
                }
            }
        }
        private Point _location = new Point(0, 0);

        /// <summary>
        /// Координаты Mapper
        /// </summary>
        [Bindable(true)]
        public Size Size
        {
            get
            {
                var screenSize = Screen.PrimaryScreen.Bounds.Size;
                if (_size.Width < 10 || _size.Width > screenSize.Width)
                    _size.Width = 406;
                if (_size.Height < 10 || _size.Height > screenSize.Height)
                    _size.Height = 406;
                return _size;
            }

            set
            {
                if (_size != value)
                {
                    _size = value;
                    NotifyPropertyChanged(nameof(Size));
                }
            }
        }
        private Size _size = new Size(396, 396);

        /// <summary>
        /// Видимость панели прокладывания путей
        /// </summary>
        [Bindable(true)]
        public bool MappingBarVisible
        {
            get => _mappingBarVisible;
            set
            {
                if (_mappingBarVisible != value)
                {
                    _mappingBarVisible = value;
                    NotifyPropertyChanged(nameof(MappingBarVisible));
                }
            }
        }
        private bool _mappingBarVisible = true;

        /// <summary>
        /// Видимость панели редактирования графа (мешей)
        /// </summary>
        [Bindable(true)]
        public bool MeshesBarVisible
        {
            get => _meshesBarVisible;
            set
            {
                if (_meshesBarVisible != value)
                {
                    _meshesBarVisible = value;
                    NotifyPropertyChanged(nameof(MeshesBarVisible));
                }
            }
        }
        private bool _meshesBarVisible = true;

        /// <summary>
        /// Видимость панели редактирования вершин и ребер
        /// </summary>
        [Bindable(true)]
        public bool NodeToolsBarVisible
        {
            get => _nodeToolsBarVisible;
            set
            {
                if (_nodeToolsBarVisible != value)
                {
                    _nodeToolsBarVisible = value;
                    NotifyPropertyChanged(nameof(NodeToolsBarVisible));
                }
            }
        }
        private bool _nodeToolsBarVisible = true;

        /// <summary>
        /// Видимость панели редактирования CustomRegion
        /// </summary>
        [Bindable(true)]
        public bool CustomRegionBarVisible
        {
            get => _customRegionBarVisible;
            set
            {
                if (_customRegionBarVisible != value)
                {
                    _customRegionBarVisible = value;
                    NotifyPropertyChanged(nameof(CustomRegionBarVisible));
                }
            }
        }
        private bool _customRegionBarVisible = true;

        /// <summary>
        /// Видимость строки состояния
        /// </summary>
        [Bindable(true)]
        public bool StatusBarVisible
        {
            get => _statusBarVisible;
            set
            {
                if (_statusBarVisible != value)
                {
                    _statusBarVisible = value;
                    NotifyPropertyChanged(nameof(StatusBarVisible));
                }
            }
        }
        private bool _statusBarVisible = true;

        /// <summary>
        /// Время обновления кэша
        /// </summary>
        [Bindable(true)]
        public int RedrawMapperTimeout
        {
            get => redrawMapperTimeout;
            set
            {
                if (value != redrawMapperTimeout)
                {
                    redrawMapperTimeout = Math.Max(100, value);
                    NotifyPropertyChanged(nameof(RedrawMapperTimeout));
                }
            }
        }
        private int redrawMapperTimeout = 100;

        [Bindable(true)]
        [XmlElement(Type = typeof(XmlColor))]
        public Color BidirectionalPathColor
        {
            get => biPathColor;
            set
            {
                if (value != biPathColor)
                {
                    biPathColor = value;
                    NotifyPropertyChanged(nameof(BidirectionalPathColor));
                }
            }
        }
        private Color biPathColor = Color.Red;

#if false
        public int BidirectionalPathRGBColor
        {
            get => biPathColor.ToArgb();
            set => BidirectionalPathColor = Color.FromArgb(value);
        } 
#endif

        [Bindable(true)]
        [XmlElement(Type = typeof(XmlColor))]
        public Color UnidirectionalPathColor
        {
            get => uniPathColor;
            set
            {
                if (value != uniPathColor)
                {
                    uniPathColor = value;
                    NotifyPropertyChanged(nameof(UnidirectionalPathColor));
                }
            }
        }
        private Color uniPathColor = Color.SkyBlue;

#if false
        public int UnidirectionalPathRGB
        {
            get => uniPathColor.ToArgb();
            set => UnidirectionalPathColor = Color.FromArgb(value);
        } 
#endif
    }
#endif
}
