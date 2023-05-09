using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using AStar;
using Astral.Quester.Classes;
using Infrastructure.Annotations;
using MyNW.Internals;
using QuesterAction = Astral.Quester.Classes.Action;

namespace Infrastructure.Quester
{
    public abstract class BaseQuesterProfileProxy : INotifyPropertyChanged
    {
        protected abstract Profile RealProfile { get; }

#if DEBUG
        [Browsable(true)]
        [Category("File")]
#else
        [Browsable(false)] 
#endif
        public abstract string ProfilePath { get; protected set; }

#if DEBUG
        [Browsable(true)]
        [Category("File")]
#else
        [Browsable(false)] 
#endif        
        public bool Saved
        {
            get => RealProfile.Saved;
            set
            {
                RealProfile.Saved = value;
                OnPropertyChanged();
            }
        }

        #region Meshes
        /// <summary>
        /// Путевой граф для карты, на которой находится персонаж
        /// </summary>
        [Browsable(false)]
        public virtual Graph CurrentMesh
        {
            get
            {
                var mapsMeshes = _mapsMeshes;
                lock (mapsMeshes)
                {
                    var localPlayer = EntityManager.LocalPlayer;
                    if (localPlayer.IsValid)
                    {
                        string mapName = localPlayer.MapState.MapName;
                        if (!string.IsNullOrEmpty(mapName))
                        {
                            if (mapsMeshes.ContainsKey(mapName))
                            {
                                return mapsMeshes[mapName];
                            }
                            else if (QuesterHelper.LoadMeshFromZipFile(CurrentProfileZipMeshFile, mapName, out Graph mesh))
                            {
                                mapsMeshes.Add(mapName, mesh);
                                return mesh;
                            }
                            else
                            {
                                var newMesh = new Graph();
                                mapsMeshes.Add(mapName, newMesh);
                                return newMesh;
                            }
                        }
                    }
                }
                return new Graph();
            }
        }

        /// <summary>
        /// Набор путевых графов, ассоциированных с профилем
        /// </summary>
        [Browsable(false)]
        public Dictionary<string, Graph> MapsMeshes
        {
            get => _mapsMeshes;
            set
            {
                _mapsMeshes = value;
                OnPropertyChanged();
            }
        }
        protected Dictionary<string, Graph> _mapsMeshes = new Dictionary<string, Graph>();

        /// <summary>
        /// Имя файла, в котором хранятся файлы путевых графов
        /// </summary>
#if DEBUG
        [Browsable(true)]
        [Category("File")]
#else
        [Browsable(false)] 
#endif
        public string CurrentProfileZipMeshFile
        {
            get
            {
                if (_currentProfileZipMeshFile is null)
                {
                    if (!string.IsNullOrEmpty(ProfilePath))
                    {
                        if (RealProfile.UseExternalMeshFile && !string.IsNullOrEmpty(RealProfile.ExternalMeshFileName))
                            _currentProfileZipMeshFile = Path.Combine(Path.GetDirectoryName(ProfilePath) ?? string.Empty,
                                RealProfile.ExternalMeshFileName);
                        else _currentProfileZipMeshFile = ProfilePath;
                        if (!File.Exists(_currentProfileZipMeshFile))
                            _currentProfileZipMeshFile = string.Empty;
                    }
                    else _currentProfileZipMeshFile = string.Empty;
                }
                return _currentProfileZipMeshFile;
            }
        }
        private string _currentProfileZipMeshFile;

        /// <summary>
        /// Признак использования внешнего файла мешей
        /// </summary>
        [Category("External Meshes")]
        public bool UseExternalMeshes
        {
            get => RealProfile.UseExternalMeshFile;
            set
            {
                OnPropertyChanged();
                RealProfile.UseExternalMeshFile = value;
                _currentProfileZipMeshFile = default;
            }
        }

        /// <summary>
        /// Путь к внешнему файлу мешей
        /// </summary>
        [Category("External Meshes")]
        public virtual string ExternalMeshFileName
        {
            get => RealProfile.ExternalMeshFileName;
            set
            {
                if (RealProfile.ExternalMeshFileName != value)
                {
                    QuesterHelper.LoadAllMeshes(_mapsMeshes, RealProfile.ExternalMeshFileName);
                    OnPropertyChanged();
                    RealProfile.Saved = false;
                    RealProfile.ExternalMeshFileName = value;
                    _currentProfileZipMeshFile = default;
                }
            }
        }
        protected void ResetCachedMeshes()
        {
            _currentProfileZipMeshFile = default;
            _mapsMeshes.Clear();
        }
        #endregion

        /// <summary>
        /// Основной (коневой) <see cref="ActionPack"/> профиля
        /// </summary>
        [Browsable(false)]
        [NotifyParentProperty(true)]
        public ActionPack MainActionPack => RealProfile.MainActionPack;

        /// <summary>
        /// Набор команд quester'a
        /// </summary>
        [Browsable(false)]
        [NotifyParentProperty(true)]
        public IEnumerable<QuesterAction> Actions
        {
            get => RealProfile.MainActionPack.Actions;
            set
            {
                if (ReferenceEquals(RealProfile, Astral.Quester.API.CurrentProfile)
                    && AstralAccessors.Controllers.Roles.IsRunning)
                    AstralAccessors.Controllers.Roles.ToggleRole(true);

                var mainActionPack = RealProfile.MainActionPack;
                mainActionPack.Reset();
                mainActionPack.ResetActionPlayer();
                var actions = mainActionPack.Actions;
                actions.Clear();
                if (value?.Any() == true)
                    actions.AddRange(value);
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Набор идентификаторов противников, которые на должны быть атакованы (игнорируются в бою).
        /// </summary>
        [Browsable(false)]
        public BindingList<string> BlackList
        {
            get
            {
                if (_blackList is null)
                {
                    var blList = RealProfile.BlackList;
                    _blackList = blList?.Count > 0
                        ? new BindingList<string>(blList)
                        : new BindingList<string>();
                    _blackListChangeNum = 0;
                    _blackList.ListChanged += BlackList_Changed;
                }
                return _blackList;
            }
        }
        private BindingList<string> _blackList;
        private int _blackListChangeNum;
        private void BlackList_Changed(object sender, ListChangedEventArgs e)
        {
            RealProfile.Saved = false;
            _blackListChangeNum++;
        }

        /// <summary>
        /// Набор <see cref="CustomRegion"/>
        /// </summary>
        [Browsable(false)]
        [NotifyParentProperty(true)]
        public BindingList<CustomRegion> CustomRegions
        {
            get
            {
                if (_customRegions is null)
                {
                    var crList = RealProfile.CustomRegions;
                    _customRegions = crList?.Count > 0
                        ? new BindingList<CustomRegion>(crList)
                        : new BindingList<CustomRegion>();
                    _customRegionsChangeNum = 0;
                    _customRegions.ListChanged += CustomRegions_Changed;
                }
                return _customRegions;
            }
        }
        private BindingList<CustomRegion> _customRegions;
        private int _customRegionsChangeNum;
        private void CustomRegions_Changed(object sender, ListChangedEventArgs e)
        {
            RealProfile.Saved = false;
            _customRegionsChangeNum++;
#if false
            if (e.ListChangedType == ListChangedType.ItemDeleted)
            {
                var stack = new System.Diagnostics.StackTrace(true);
                var sb = new StringBuilder($"{e.ListChangedType} CustomRegion[{e.NewIndex}] from profile '{ProfilePath}'.\n");
                foreach (var frame in stack.GetFrames())
                {
                    sb.AppendLine(frame.ToString());
                }
                ETLogger.WriteLine(LogType.Debug, sb.ToString());
            } 
#endif
        }



        /// <summary>
        /// Набор описаний продавцов.
        /// </summary>
        [Browsable(false)]
        [NotifyParentProperty(true)]
        public BindingList<NPCInfos> Vendors
        {
            get
            {
                if (_vendors is null)
                {
                    var vndList = RealProfile.Vendors;
                    _vendors = vndList?.Count > 0
                        ? new BindingList<NPCInfos>(vndList)
                        : new BindingList<NPCInfos>();
                    _vendorsChangeNum = 0;
                    _vendors.ListChanged += Vendors_Changed;
                }
                return _vendors;
            }
        }
        private BindingList<NPCInfos> _vendors;
        private int _vendorsChangeNum;
        private void Vendors_Changed(object sender, ListChangedEventArgs e)
        {
            RealProfile.Saved = false;
            _vendorsChangeNum++;
        }

        /// <summary>
        /// Радиус вступления в бой
        /// </summary>
        public int KillRadius
        {
            get => RealProfile.KillRadius;
            set
            {
                OnPropertyChanged();
                RealProfile.KillRadius = value;
            }
        }

        public bool DisablePet
        {
            get => RealProfile.DisablePet;
            set
            {
                RealProfile.DisablePet = value;
                OnPropertyChanged();
            }
        }

        [Category("Follower")]
        public int FollowerDistance
        {
            get => RealProfile.FollowerDistance;
            set
            {
                RealProfile.FollowerDistance = value;
                OnPropertyChanged();
            }
        }

        [Category("Follower")]
        public bool DisableFollow
        {
            get => RealProfile.DisableFollow;
            set
            {
                RealProfile.DisableFollow = value;
                OnPropertyChanged();
            }
        }

        [Description("False : Ignore AssociateMission attribute if character don't have mission\r\nTrue : skip action if don't have the AssociateMission")]
        public bool AssociateMissionsDefault
        {
            get => RealProfile.AssociateMissionsDefault;
            set
            {
                RealProfile.AssociateMissionsDefault = value;
                OnPropertyChanged();
            }
        }




        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion




        #region Сохранение/загрузка профиля

        /// <summary>
        /// Получение ссылки на проксируемый экземпляр профиля с принудительным отображением в нем всех изменений.
        /// </summary>
        /// <returns></returns>
        public virtual Profile GetProfile()
        {
            ReconstructProfile();
            return RealProfile;
        }

        protected void ReconstructProfile()
        {
            if (_customRegions != null && _customRegionsChangeNum > 0)
            {
                var newCustomRegions = _customRegions.ToList();
                //var currentCustomRetions = RealProfile.CustomRegions;
                RealProfile.CustomRegions = newCustomRegions;
                _customRegionsChangeNum = 0;
            }

            if (_blackList != null && _blackListChangeNum > 0)
            {
                var blackList = _blackList.ToList();
                RealProfile.BlackList = blackList;
                _blackListChangeNum = 0;
            }

            if (_vendors != null && _vendorsChangeNum > 0)
            {
                var vendors = _vendors.ToList();
                RealProfile.Vendors = vendors;
                _vendorsChangeNum = 0;
            }
        }

        /// <summary>
        /// Инициализация новым экземпляром профиля <paramref name="profile"/> без удаления привязок
        /// </summary>
        public abstract void SetProfile(Profile profile, string fileName);

        protected void AssignInternals(Profile profile)
        {
            if (profile != null)
            {
#if false
                if (ReferenceEquals(RealProfile, profile))
                    return; 
#endif
                if (_customRegions != null)
                {
                    _customRegions.ListChanged -= CustomRegions_Changed;
                    var newCustomRegions = profile.CustomRegions.ToList();
                    _customRegions.Clear();
                    foreach (var cr in newCustomRegions)
                        _customRegions.Add(cr);
                    _customRegions.ListChanged += CustomRegions_Changed;
                    OnPropertyChanged(nameof(CustomRegions));
                }

                if (_blackList != null)
                {
                    _blackList.ListChanged -= BlackList_Changed;
                    var newBlackList = profile.BlackList.ToList();
                    _blackList.Clear();
                    foreach (var bl in newBlackList)
                        _blackList.Add(bl);
                    _blackList.ListChanged += BlackList_Changed;
                    OnPropertyChanged(nameof(BlackList));

                }

                if (_vendors != null)
                {
                    _vendors.ListChanged -= Vendors_Changed;
                    var newVendors = profile.Vendors.ToList();
                    _vendors.Clear();
                    foreach (var vendor in newVendors)
                        _vendors.AddUnique(vendor);
                    OnPropertyChanged(nameof(Vendors));
                    _vendors.ListChanged += Vendors_Changed;
                }
            }
            else
            {
                if (_customRegions != null)
                {
                    _customRegions.Clear();
                    OnPropertyChanged(nameof(CustomRegions));
                }

                if (_blackList != null)
                {
                    _blackList.Clear();
                    OnPropertyChanged(nameof(BlackList));

                }

                if (_vendors != null)
                {
                    _vendors.Clear();
                    OnPropertyChanged(nameof(Vendors));
                }
            }

            OnPropertyChanged(nameof(Actions));
            OnPropertyChanged(nameof(ProfilePath));
            OnPropertyChanged(nameof(MapsMeshes));
            OnPropertyChanged(nameof(CurrentProfileZipMeshFile));
            OnPropertyChanged(nameof(CurrentMesh));
            OnPropertyChanged(nameof(KillRadius));
            OnPropertyChanged(nameof(UseExternalMeshes));
            OnPropertyChanged(nameof(ExternalMeshFileName));
            OnPropertyChanged(nameof(DisablePet));
            OnPropertyChanged(nameof(FollowerDistance));
            OnPropertyChanged(nameof(DisableFollow));
            OnPropertyChanged(nameof(AssociateMissionsDefault));

            _customRegionsChangeNum = 0;
            _vendorsChangeNum = 0;
            _blackListChangeNum = 0;
        }

        /// <summary>
        /// Загрузка профиля из файла <paramref name="profilePath"/>
        /// </summary>
        /// <param name="profilePath"></param>
        /// <returns></returns>
        public virtual bool LoadFromFile(string profilePath)
        {
            //if (string.IsNullOrEmpty(profilePath)
            //    || !File.Exists(profilePath))
            //    return false;

            var prof = QuesterHelper.Load(ref profilePath);
            if (prof is null)
                return false;

            SetProfile(prof, profilePath);

            return true;
        }

        /// <summary>
        /// Сохранение профиля в заданный файл
        /// </summary>
        public virtual void Save()
        {
            ReconstructProfile();

            string newProfileName = ProfilePath;
            string externalMeshes = ExternalMeshFileName;
            if (QuesterHelper.Save(RealProfile, _mapsMeshes, ProfilePath, ref newProfileName))
            {
                ProfilePath = newProfileName;
                if (UseExternalMeshes
                    && externalMeshes != ExternalMeshFileName)
                {
                    OnPropertyChanged(nameof(ExternalMeshFileName));
                }
            }
        }

        /// <summary>
        /// Сохранение профиля в новый файл
        /// </summary>
        public virtual void SaveAs()
        {
            ReconstructProfile();

            string newProfileName = string.Empty;
            string externalMeshes = ExternalMeshFileName;
            if (QuesterHelper.Save(RealProfile, _mapsMeshes, ProfilePath, ref newProfileName))
            {
                //TODO Переопределение относительных путей для PushProfileToStackAndLoad и LoadProfile
                ProfilePath = newProfileName;
                if (RealProfile.UseExternalMeshFile
                    && externalMeshes != ExternalMeshFileName)
                {
                    OnPropertyChanged(nameof(ExternalMeshFileName));
                }
            }
        }

        protected void ResetMeshesCache()
        {
            _currentProfileZipMeshFile = default;
            _mapsMeshes.Clear();
        }
        #endregion
    }
}
