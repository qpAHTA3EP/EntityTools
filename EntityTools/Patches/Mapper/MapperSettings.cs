﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using EntityTools.Patches.Mapper;

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
            get
            {
                var screenSize = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Size;
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
                    var screenSize = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Size;
                    if (value.X < 0 || value.X > screenSize.Width)
                        value.X = 60;
                    if (value.Y < 0 || value.Y > screenSize.Height)
                        value.Y = 60;

                    _location = value;
                    base.NotifyPropertyChanged(nameof(Location));
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
                var screenSize = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Size;
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
                    base.NotifyPropertyChanged(nameof(Size));
                }
            }
        }
        private Size _size = new Size(396, 396);

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
        /// Видимость панели редактирования графа (мешей)
        /// </summary>
        [Bindable(true)]
        public bool EditMeshesBarVisible
        {
            get => _editMeshesBarVisible;
            set
            {
                if (_editMeshesBarVisible != value)
                {
                    _editMeshesBarVisible = value;
                    base.NotifyPropertyChanged(nameof(EditMeshesBarVisible));
                }
            }
        }
        //[NonSerialized]
        private bool _editMeshesBarVisible = true;

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
                    base.NotifyPropertyChanged(nameof(StatusBarVisible));
                }
            }
        }
        //[NonSerialized]
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
                    base.NotifyPropertyChanged(nameof(RedrawMapperTimeout));
                }
            }
        }
        //[NonSerialized]
        private int redrawMapperTimeout = 100;

    }
#endif
}
