using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using ACTP0Tools;
using ACTP0Tools.Classes.Quester;
using ACTP0Tools.Reflection;
using AStar;
using Astral.Quester.Classes;
using EntityCore.Editors;
using MyNW.Internals;
using QuesterAction = Astral.Quester.Classes.Action;

namespace EntityCore.Quester.Classes
{
    public class ProfileProxy : QuesterProfileProxy
    {
        private Profile _profile;

        public ProfileProxy()
        {
            _profile = new Profile { Saved = true };
        }

        public ProfileProxy(Profile profile, string fileName)
        {
            _profile = profile is null 
                     ? new Profile { Saved = true }
                     : CopyHelper.CreateDeepCopy(profile);

            if (!string.IsNullOrEmpty(fileName))
                _fileName = Path.GetFullPath(fileName);
        }

        public override Profile GetProfile()
        {
            ReconstructProfile();
            return _profile;
        }

        /// <summary>
        /// Инициализация новым объектом <paramref name="profile"/> без удаления привязок
        /// </summary>
        public override void SetProfile(Profile profile, string fileName)
        {
            if (profile != null)
            {
                _profile = profile;
                _fileName = fileName;
                if (_customRegions != null)
                {
                    _customRegions.ListChanged -= CustomRegions_Changed;
                    _customRegions.Clear();
                    foreach (var cr in profile.CustomRegions)
                        _customRegions.Add(cr);
                    _customRegions.ListChanged += CustomRegions_Changed;
                    OnPropertyChanged(nameof(CustomRegions));
                }

                if (_blackList != null)
                {
                    _blackList.ListChanged -= BlackList_Changed;
                    _blackList.Clear();
                    foreach (var bl in profile.BlackList)
                        _blackList.Add(bl);
                    _blackList.ListChanged += BlackList_Changed;
                    OnPropertyChanged(nameof(BlackList));

                }

                if (_vendors != null)
                {
                    _vendors.ListChanged -= Vendors_Changed;
                    _vendors.Clear();
                    foreach (var vendor in profile.Vendors)
                        _vendors.AddUnique(vendor);
                    OnPropertyChanged(nameof(Vendors));
                    _vendors.ListChanged += Vendors_Changed;
                }
            }
            else
            {
                _profile = new Profile();
                _fileName = string.Empty;
                if (_customRegions != null)
                {
                    _customRegions.ListChanged -= CustomRegions_Changed;
                    _customRegions.Clear();
                    _customRegions.ListChanged += CustomRegions_Changed;
                    OnPropertyChanged(nameof(CustomRegions));
                }

                if (_blackList != null)
                {
                    _blackList.ListChanged -= BlackList_Changed;
                    _blackList.Clear();
                    _blackList.ListChanged += BlackList_Changed;
                    OnPropertyChanged(nameof(BlackList));

                }

                if (_vendors != null)
                {
                    _vendors.ListChanged -= Vendors_Changed;
                    _vendors.Clear();
                    OnPropertyChanged(nameof(Vendors));
                    _vendors.ListChanged += Vendors_Changed;
                }
            }
            _customRegionsChangeNum = 0;
            _vendorsChangeNum = 0;
            _blackListChangeNum = 0;

            lock (_mapsMeshes)
                _mapsMeshes.Clear();
            OnPropertyChanged(nameof(MapsMeshes));

            OnPropertyChanged(nameof(Actions));
            OnPropertyChanged(nameof(FileName));
            OnPropertyChanged(nameof(CurrentProfileZipMeshFile));
            OnPropertyChanged(nameof(CurrentMesh));
            OnPropertyChanged(nameof(KillRadius));
            _currentProfileZipMeshFile = null;
            OnPropertyChanged(nameof(UseExternalMeshFile));
            OnPropertyChanged(nameof(ExternalMeshFileName));
            OnPropertyChanged(nameof(DisablePet));
            OnPropertyChanged(nameof(FollowerDistance));
            OnPropertyChanged(nameof(DisableFollow));
            OnPropertyChanged(nameof(AssociateMissionsDefault));

        }


#if DEBUG
        [Browsable(true)] 
        [Category("File")]
#else
        [Browsable(false)] 
#endif
        public override string FileName
        {
            get => _fileName;
            set
            {
                OnPropertyChanged();
                _fileName = value;
                _currentProfileZipMeshFile = null;
            }
        }
        private string _fileName;

#if DEBUG
        [Browsable(true)]
        [Category("File")]
#else
        [Browsable(false)] 
#endif
        public override bool Saved
        {
            get => _profile.Saved;
            set => _profile.Saved = value;
        }

        /// <summary>
        /// Путевой граф для карты, на которой находится персонаж
        /// </summary>
        [Browsable(false)]
        public override Graph CurrentMesh
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
                                return mapsMeshes[mapName];
                            
                            if (AstralAccessors.Quester.Core.LoadMeshFromZipFile(CurrentProfileZipMeshFile, mapName, out Graph mesh))
                            {
                                mapsMeshes.Add(mapName, mesh);
                                return mesh;
                            }
                            mesh = new Graph();
                            mapsMeshes.Add(mapName, mesh);
                            return mesh;
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
        public override IDictionary<string, Graph> MapsMeshes => _mapsMeshes;
        private Dictionary<string, Graph> _mapsMeshes = new Dictionary<string, Graph>();

        /// <summary>
        /// Имя файла, в котором хранятся файлы путевых графов
        /// </summary>
#if DEBUG
        [Browsable(true)]
        [Category("File")]
#else
        [Browsable(false)] 
#endif
        public override string CurrentProfileZipMeshFile
        {
            get
            {
                if (_currentProfileZipMeshFile is null)
                {
                    if (!string.IsNullOrEmpty(FileName))
                    {
                        if (_profile.UseExternalMeshFile && !string.IsNullOrEmpty(_profile.ExternalMeshFileName))
                            _currentProfileZipMeshFile = Path.Combine(Path.GetDirectoryName(FileName) ?? string.Empty,
                                _profile.ExternalMeshFileName);
                        else _currentProfileZipMeshFile = FileName;
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
        /// Набор команд quester'a
        /// </summary>
        [Browsable(false)]
        [NotifyParentProperty(true)]
        public override IEnumerable<QuesterAction> Actions
        {
            get => _profile.MainActionPack.Actions;
            set
            {
                OnPropertyChanged();
                var actions = _profile.MainActionPack.Actions;
                actions.Clear();
                if (value?.Any() == true)
                    actions.AddRange(value);
            }
        }

        /// <summary>
        /// Набор идентификаторов противников, которые на должны быть атакованы (игнорируются в бою).
        /// </summary>
        [Browsable(false)]
        public override BindingList<string> BlackList
        {
            get
            {
                if (_blackList is null)
                {
                    var blList = _profile.BlackList;
                    _blackList = blList?.Count > 0 
                        ? new BindingList<string>(blList)
                        : new BindingList<string>();
                    _blackList.ListChanged += BlackList_Changed;
                }
                return _blackList;
            }
        }
        private BindingList<string> _blackList;
        private int _blackListChangeNum;
        private void BlackList_Changed(object sender, ListChangedEventArgs e)
        {
            _profile.Saved = false;
            _blackListChangeNum++;
        }

        /// <summary>
        /// Набор <see cref="CustomRegion"/>
        /// </summary>
        [Browsable(false)]
        [NotifyParentProperty(true)]
        public override BindingList<CustomRegion> CustomRegions
        {
            get
            {
                if (_customRegions is null)
                {
                    var crList = _profile.CustomRegions;
                    _customRegions = crList?.Count > 0 
                        ? new BindingList<CustomRegion>(crList) 
                        : new BindingList<CustomRegion>();
                    _customRegions.ListChanged += CustomRegions_Changed;
                }
                return _customRegions;
            }
        }
        private BindingList<CustomRegion> _customRegions;
        private int _customRegionsChangeNum;
        private void CustomRegions_Changed(object sender, ListChangedEventArgs e)
        {
            _profile.Saved = false;
            _customRegionsChangeNum++;
        }

        /// <summary>
        /// Набор описаний продавцов.
        /// </summary>
        [Browsable(false)]
        [NotifyParentProperty(true)]
        public override BindingList<NPCInfos> Vendors
        {
            get
            {
                if (_vendors is null)
                {
                    var vndList = _profile.Vendors;
                    _vendors = vndList?.Count > 0 
                        ? new BindingList<NPCInfos>(vndList) 
                        : new BindingList<NPCInfos>();
                    _vendors.ListChanged += Vendors_Changed;
                }
                return _vendors;
            }
        }
        private BindingList<NPCInfos> _vendors;
        private int _vendorsChangeNum;
        private void Vendors_Changed(object sender, ListChangedEventArgs e)
        {
            _profile.Saved = false;
            _vendorsChangeNum++;
        }

        /// <summary>
        /// Радиус вступления в бой
        /// </summary>
        public override int KillRadius
        {
            get => _profile.KillRadius;
            set
            {
                OnPropertyChanged();
                _profile.KillRadius = value;
            }
        }


        /// <summary>
        /// Признак использования внешнего файла мешей
        /// </summary>
        [Category("External Meshes")]
        public override bool UseExternalMeshFile
        {
            get => _profile.UseExternalMeshFile;
            set
            {
                OnPropertyChanged();
                _profile.UseExternalMeshFile = value;
                _currentProfileZipMeshFile = null;
            }
        }

        /// <summary>
        /// Путь к внешнему файлу мешей
        /// </summary>
        [Category("External Meshes")]
        [Editor(typeof(RelativeMeshesFilePathEditor), typeof(UITypeEditor))]
        public override string ExternalMeshFileName
        {
            get => _profile.ExternalMeshFileName;
            set
            {
                if (_profile.ExternalMeshFileName != value)
                {
                    AstralAccessors.Quester.Core.LoadAllMeshes(ref _mapsMeshes, _profile.ExternalMeshFileName);
                    OnPropertyChanged();
                    _profile.Saved = false;
                    _profile.ExternalMeshFileName = value;
                    _currentProfileZipMeshFile = null; 
                }
            }
        }

        public override bool DisablePet
        {
            get => _profile.DisablePet;
            set
            {
                OnPropertyChanged();
                _profile.DisablePet = value;
            }
        }

        [Category("Follower")]
        public override int FollowerDistance
        {
            get => _profile.FollowerDistance;
            set
            {
                OnPropertyChanged();
                _profile.FollowerDistance = value;
            }
        }

        [Category("Follower")]
        public override bool DisableFollow
        {
            get => _profile.DisableFollow;
            set
            {
                OnPropertyChanged();
                _profile.DisableFollow = value;
            }
        }

        [Description("False : Ignore AssociateMission attribute if character don't have mission\r\nTrue : skip action if don't have the AssociateMission")]
        public override bool AssociateMissionsDefault
        {
            get => _profile.AssociateMissionsDefault;
            set
            {
                OnPropertyChanged();
                _profile.AssociateMissionsDefault = value; }
        }

        public delegate void BeforeSavingEvent();

        public event BeforeSavingEvent OnSavingEvent;

        /// <summary>
        /// Загрузка профиля из файла <paramref name="fileName"/>
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public override bool LoadFromFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)
                || !File.Exists(fileName))
                return false;

            var prof = AstralAccessors.Quester.Core.Load(ref fileName);
            if (prof is null)
                return false;

            SetProfile(prof, fileName);

            return true;
        }

        /// <summary>
        /// Сохранение профиля в заданный файл
        /// </summary>
        public override void Save()
        {
            if(_profile is null)
                return;

            OnSavingEvent?.Invoke();
            ReconstructProfile();

            string newProfileName = _fileName;
            string externalMeshes = _profile.ExternalMeshFileName;
            if (AstralAccessors.Quester.Core.Save(_profile, _mapsMeshes, _fileName, ref newProfileName))
            {
                _fileName = newProfileName;
                if (_profile.UseExternalMeshFile
                    && externalMeshes != _profile.ExternalMeshFileName)
                {
                    OnPropertyChanged(nameof(ExternalMeshFileName));
                }
            }
        }
        /// <summary>
        /// Сохранение профиля в новый файл
        /// </summary>
        /// <returns></returns>
        public override void SaveAs()
        {
            if (_profile is null)
                return;

            OnSavingEvent?.Invoke();
            ReconstructProfile();

            string newProfileName = string.Empty;
            string externalMeshes = _profile.ExternalMeshFileName;
            if (AstralAccessors.Quester.Core.Save(_profile, _mapsMeshes, _fileName, ref newProfileName))
            {
                //TODO Переопределение относительных путей для PushProfileToStackAndLoad 
                _fileName = newProfileName;
                if (_profile.UseExternalMeshFile
                    && externalMeshes != _profile.ExternalMeshFileName)
                {
                    OnPropertyChanged(nameof(ExternalMeshFileName));
                }
            }
        }

        private void ReconstructProfile()
        {
            if (_customRegions != null && _customRegionsChangeNum > 0)
            {
                _profile.CustomRegions = _customRegions.ToList();
                _customRegionsChangeNum = 0;
            }

            if (_blackList != null && _blackListChangeNum > 0)
            {
                _profile.BlackList = _blackList.ToList();
                _blackListChangeNum = 0;
            }

            if (_vendors != null && _vendorsChangeNum > 0)
            {
                _profile.Vendors = _vendors.ToList();
                _vendorsChangeNum = 0;
            }
        }
    }
}
