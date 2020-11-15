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
        [Description("Активация патча окна Mapper'a")]
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
        [Category("Mapping")]
        [Description("Расстояние между добавляемыми путевыми точками")]
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
        [Category("Mapping")]
        [Description("Радиус, в пределах которого точки пути считаются эквивалентными (новые не добавляются)")]
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
        [Category("Mapping")]
        [Description("Максимальная допустимая разница высот между соединяемыми точками пути")]
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
        [Category("Mapping Cache")]
        [Description("Активация кэша путевых точек в режиме прокладывания пути")]
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
        [Category("Mapping Cache")]
        [Description("Радиус в пределах которого кэшируются путевые точки")]
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
        [Category("Mapping Cache")]
        [Description("Период времени между обновлениями кэша")]
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
        [Category("Mapping")]
        [Description("Флаг принудительного связывания с предыдущей точкой пути")]
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
        [Category("Mapping")]
        [Description("Флаг прокладывания линейного пути (без связей с пересекающимися путями)")]
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
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public MapperFormSettings MapperForm { get; set; } = new MapperFormSettings();

        public override string ToString()
        {
            return patch ? "Advanced Mapper" : "Standard Mapper";
        }
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
        [Category("Window")]
        [Description("Координаты окна на экране")]
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
        [Category("Window")]
        [Description("Размер окна на экране в пикселях")]
        public Size Size
        {
            get
            {
                var screenSize = Screen.PrimaryScreen.Bounds.Size;
                if (_size.Width < 200 || _size.Width > screenSize.Width)
                    _size.Width = 406;
                if (_size.Height < 200 || _size.Height > screenSize.Height)
                    _size.Height = 406;
                return _size;
            }

            set
            {
                if (_size != value)
                {
                    if (value.Width < 200)
                    {
                        if (value.Height < 200)
                            _size = new Size(200, 200);
                        else _size = new Size(200, value.Height);
                    }
                    else if(value.Height < 200)
                        _size = new Size(value.Width, 200);
                    else _size = value;
                    NotifyPropertyChanged(nameof(Size));
                }
            }
        }
        private Size _size = new Size(396, 396);

        /// <summary>
        /// Видимость панели прокладывания путей
        /// </summary>
        [Bindable(true)]
        [Category("Toolbar")]
        [Description("Видимость панели инструментов 'Mapping Tools'")]
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
        [Category("Toolbar")]
        [Description("Видимость панели инструментов 'Mashes Tools'")]
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
        [Category("Toolbar")]
        [Description("Видимость панели инструментов 'Node Tools'")]
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
        [Category("Toolbar")]
        [Description("Видимость панели инструментов 'CustomRegion Tools'")]
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
        [Category("Toolbar")]
        [Description("Видимость строки состояния")]
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
        [Category("Window")]
        [Description("Интервал времени между обновлением (перерисовкой) окна Mapper'a")]
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

        /// <summary>
        /// Толщина слоя, отображаемого в Mapper'e
        /// Задает максимальную разность высот целевой точки (игрока) и вершин
        /// </summary>
        [Bindable(true)]
        [Category("Advanced")]
        [Description("Толщина слоя, отображаемого в Mapper'e\n\r" +
            "Задает максимальную разность высот целевой точки (игрока) и отображаемых путевых точек")]
        public uint LayerDepth
        {
            get => layerDepth;
            set
            {
                if (value != layerDepth)
                {
                    layerDepth = value;
                    NotifyPropertyChanged(nameof(LayerDepth));
                }
            }
        }
        private uint layerDepth = 100;

        [Bindable(true)]
        [Category("Customization")]
        [Description("Цвет двунаправленного пути (BidirectionalPath)")]
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

        [Bindable(true)]
        [Category("Customization")]
        [Description("Цвет однонаправленного пути (UnidirectionalPath)")]
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

        [Bindable(true)]
        [Category("Customization")]
        [Description("Цвет фона")]
        [XmlElement(Type = typeof(XmlColor))]
        public Color BackgroundColor
        {
            get => backgroundColor;
            set
            {
                if (value != backgroundColor)
                {
                    backgroundColor = value;
                    NotifyPropertyChanged(nameof(BackgroundColor));
                }
            }
        }
        private Color backgroundColor = Color.Black;

        [Bindable(true)]
        [Category("Customization")]
        [Description("Отображение врагов")]
        public bool DrawEnemies
        {
            get => _drawEnemies;
            set
            {
                if (_drawEnemies != value)
                {
                    _drawEnemies = value;
                    NotifyPropertyChanged(nameof(DrawEnemies));
                }
            }
        }
        private bool _drawEnemies = true;

        [Bindable(true)]
        [XmlElement(Type = typeof(XmlColor))]
        [Category("Customization")]
        [Description("Цвет врагов")]
        public Color EnemyColor
        {
            get => enemiesColor;
            set
            {
                if (value != enemiesColor)
                {
                    enemiesColor = value;
                    NotifyPropertyChanged(nameof(EnemyColor));
                }
            }
        }
        private Color enemiesColor = Color.OrangeRed;


        [Bindable(true)]
        [Category("Customization")]
        [Description("Отображение дружественных NPC")]
        public bool DrawFriends
        {
            get => _drawFriends;
            set
            {
                if (_drawFriends != value)
                {
                    _drawFriends = value;
                    NotifyPropertyChanged(nameof(DrawFriends));
                }
            }
        }
        private bool _drawFriends = true;

        [Bindable(true)]
        [Category("Customization")]
        [Description("Цвет дружественных NPC")]
        [XmlElement(Type = typeof(XmlColor))]
        public Color FriendColor
        {
            get => friendsColor;
            set
            {
                if (value != friendsColor)
                {
                    friendsColor = value;
                    NotifyPropertyChanged(nameof(FriendColor));
                }
            }
        }
        private Color friendsColor = Color.Green;

        [Bindable(true)]
        [Category("Customization")]
        [Description("Отображение игроков")]
        public bool DrawPlayers
        {
            get => _drawPlayers;
            set
            {
                if (_drawPlayers != value)
                {
                    _drawPlayers = value;
                    NotifyPropertyChanged(nameof(DrawPlayers));
                }
            }
        }
        private bool _drawPlayers = true;

        [Bindable(true)]
        [Category("Customization")]
        [Description("Цвет игроков")]
        [XmlElement(Type = typeof(XmlColor))]
        public Color PlayerColor
        {
            get => playersColor;
            set
            {
                if (value != playersColor)
                {
                    playersColor = value;
                    NotifyPropertyChanged(nameof(PlayerColor));
                }
            }
        }
        private Color playersColor = Color.LawnGreen;

        [Bindable(true)]
        [Category("Customization")]
        [Description("Отображение прочих NPC")]
        public bool DrawOtherNPC
        {
            get => _drawOtherNpc;
            set
            {
                if (_drawOtherNpc != value)
                {
                    _drawOtherNpc = value;
                    NotifyPropertyChanged(nameof(DrawOtherNPC));
                }
            }
        }
        private bool _drawOtherNpc = true;

        [Bindable(true)]
        [XmlElement(Type = typeof(XmlColor))]
        [Category("Customization")]
        [Description("Цвет прочих NPC")]
        public Color OtherNPCColor
        {
            get => npcColor;
            set
            {
                if (value != npcColor)
                {
                    npcColor = value;
                    NotifyPropertyChanged(nameof(OtherNPCColor));
                }
            }
        }
        private Color npcColor = Color.LightGray;

        [Bindable(true)]
        [Category("Customization")]
        [Description("Отображение точек взаимодействия (Node)")]
        public bool DrawNodes
        {
            get => _drawNodes;
            set
            {
                if (_drawNodes != value)
                {
                    _drawNodes = value;
                    NotifyPropertyChanged(nameof(DrawNodes));
                }
            }
        }
        private bool _drawNodes = true;

        [Bindable(true)]
        [Category("Customization")]
        [Description("Цвет точек взаимодействия (Node)")]
        [XmlElement(Type = typeof(XmlColor))]
        public Color NodeColor
        {
            get => nodesColor;
            set
            {
                if (value != nodesColor)
                {
                    nodesColor = value;
                    NotifyPropertyChanged(nameof(NodeColor));
                }
            }
        }
        private Color nodesColor = Color.YellowGreen;

        [Bindable(true)]
        [Category("Customization")]
        [Description("Отображение точек проверки талантов (SkillNode)")]
        public bool DrawSkillNodes
        {
            get => _drawSkillnodes;
            set
            {
                if (_drawSkillnodes != value)
                {
                    _drawSkillnodes = value;
                    NotifyPropertyChanged(nameof(DrawSkillNodes));
                }
            }
        }
        private bool _drawSkillnodes = true;

        [Bindable(true)]
        [Category("Customization")]
        [Description("Цвет точек проверки талантов (SkillNode)")]
        [XmlElement(Type = typeof(XmlColor))]
        public Color SkillNodeColor
        {
            get => skillnodesColor;
            set
            {
                if (value != skillnodesColor)
                {
                    skillnodesColor = value;
                    NotifyPropertyChanged(nameof(SkillNodeColor));
                }
            }
        }
        private Color skillnodesColor = Color.Gold;

        public override string ToString()
        {
            return "Advanced Mapper settings";
        }
    }
#endif
}
