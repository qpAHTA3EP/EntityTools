using AStar;
using Astral.Quester.Classes;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using QuesterAction = Astral.Quester.Classes.Action;

namespace ACTP0Tools.Classes.Quester
{
    /// <summary>
    /// Синглтон прокси-объекта, опосредующего доступ к активному загруженному профилю Quester'a: <br/>
    /// <see cref="AstralAccessors.Quester.Core.Profile"/>
    /// </summary>
    public class ActiveProfileProxy : QuesterProfileProxy, INotifyPropertyChanged
    {
        // ReSharper disable once InconsistentNaming
        private static readonly ActiveProfileProxy @this = new ActiveProfileProxy();
        
        private ActiveProfileProxy(){}

        public static ActiveProfileProxy Get() => @this;

        [Browsable(false)]
        public override Profile GetProfile()
        {
            ReconstructProfile();
            return AstralAccessors.Quester.Core.Profile;
        }
        public override void SetProfile(Profile _, string __)
        {
            var profile = AstralAccessors.Quester.Core.Profile;
            if (profile != null)
            {
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
            OnPropertyChanged(nameof(Actions));
            OnPropertyChanged(nameof(FileName));
            OnPropertyChanged(nameof(MapsMeshes));
            OnPropertyChanged(nameof(CurrentProfileZipMeshFile));
            OnPropertyChanged(nameof(CurrentMesh));
            OnPropertyChanged(nameof(KillRadius));
            OnPropertyChanged(nameof(UseExternalMeshFile));
            OnPropertyChanged(nameof(ExternalMeshFileName));
            OnPropertyChanged(nameof(DisablePet));
            OnPropertyChanged(nameof(FollowerDistance));
            OnPropertyChanged(nameof(DisableFollow));
            OnPropertyChanged(nameof(AssociateMissionsDefault));

            _customRegionsChangeNum = 0;
            _vendorsChangeNum = 0;
            _blackListChangeNum = 0;
        }

#if DEBUG
        [Browsable(true)]
        [Category("File")]
#else
        [Browsable(false)] 
#endif
        public override string FileName
        {
            get => Astral.API.CurrentSettings.LastQuesterProfile;
            set
            {
                OnPropertyChanged();
                Astral.API.CurrentSettings.LastQuesterProfile = value;
            }
        }

#if DEBUG
        [Browsable(true)]
        [Category("File")]
#else
        [Browsable(false)] 
#endif
        public override bool Saved
        {
            get => AstralAccessors.Quester.Core.Profile.Saved;
            set
            {
                OnPropertyChanged();
                AstralAccessors.Quester.Core.Profile.Saved = value;
            }
        }

        /// <summary>
        /// Путевой граф для карты, на которой находится персонаж
        /// </summary>
        [Browsable(false)]
        public override Graph CurrentMesh => AstralAccessors.Quester.Core.Meshes;

        /// <summary>
        /// Набор путевых графов, ассоциированных с профилем
        /// </summary>
        [Browsable(false)]
        public override IDictionary<string, Graph> MapsMeshes => AstralAccessors.Quester.Core.MapsMeshes;

        /// <summary>
        /// Имя файла, в котором хранятся файлы путевых графов
        /// </summary>
#if DEBUG
        [Browsable(true)]
        [Category("File")]
#else
        [Browsable(false)] 
#endif
        public override string CurrentProfileZipMeshFile => AstralAccessors.Quester.Core.CurrentProfileZipMeshFile;

        /// <summary>
        /// Набор команд quester'a
        /// </summary>
        [Browsable(false)]
        [NotifyParentProperty(true)]
        public override IEnumerable<QuesterAction> Actions
        {
            get => AstralAccessors.Quester.Core.Profile.MainActionPack.Actions;
            set
            {
                if (AstralAccessors.Controllers.Roles.IsRunning)
                    AstralAccessors.Controllers.Roles.ToggleRole(true);

                var mainActionPack = AstralAccessors.Quester.Core.Profile.MainActionPack;
                mainActionPack.Reset();
                QuesterHelper.ResetActionPlayer(mainActionPack);
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
        public override BindingList<string> BlackList
        {
            get
            {
                if (_blackList is null)
                {
                    var blList = AstralAccessors.Quester.Core.Profile.BlackList;
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
        private void BlackList_Changed(object sender, ListChangedEventArgs e) => _blackListChangeNum++;

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
                    var crList = AstralAccessors.Quester.Core.Profile.CustomRegions;
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
        private void CustomRegions_Changed(object sender, ListChangedEventArgs e) => _customRegionsChangeNum++;

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
                    var vndList = AstralAccessors.Quester.Core.Profile.Vendors;
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
        private void Vendors_Changed(object sender, ListChangedEventArgs e) => _vendorsChangeNum++;

        /// <summary>
        /// Радиус вступления в бой
        /// </summary>
        public override int KillRadius
        {
            get => AstralAccessors.Quester.Core.Profile.KillRadius;
            set
            {
                OnPropertyChanged();
                AstralAccessors.Quester.Core.Profile.KillRadius = value;
            }
        }


        /// <summary>
        /// Признак использования внешнего файла мешей
        /// </summary>
        [Category("External Meshes")]
        public override bool UseExternalMeshFile
        {
            get => AstralAccessors.Quester.Core.Profile.UseExternalMeshFile;
            set
            {
                OnPropertyChanged();
                AstralAccessors.Quester.Core.Profile.UseExternalMeshFile = value;
            }
        }

        /// <summary>
        /// Путь к внешнему файлу мешей
        /// </summary>
        [Category("External Meshes")]
        public override string ExternalMeshFileName
        {
            get => AstralAccessors.Quester.Core.Profile.ExternalMeshFileName;
            set
            {
                OnPropertyChanged();
                AstralAccessors.Quester.Core.Profile.ExternalMeshFileName = value;
            }
        }

        public override bool DisablePet
        {
            get => AstralAccessors.Quester.Core.Profile.DisablePet;
            set
            {
                OnPropertyChanged();
                AstralAccessors.Quester.Core.Profile.DisablePet = value;
            }
        }

        [Category("Follower")]
        public override int FollowerDistance
        {
            get => AstralAccessors.Quester.Core.Profile.FollowerDistance;
            set
            {
                OnPropertyChanged();
                AstralAccessors.Quester.Core.Profile.FollowerDistance = value;
            }
        }

        [Category("Follower")]
        public override bool DisableFollow
        {
            get => AstralAccessors.Quester.Core.Profile.DisableFollow;
            set
            {
                OnPropertyChanged();
                AstralAccessors.Quester.Core.Profile.DisableFollow = value;
            }
        }

        [Description("False : Ignore AssociateMission attribute if character don't have mission\r\nTrue : skip action if don't have the AssociateMission")]
        public override bool AssociateMissionsDefault
        {
            get => AstralAccessors.Quester.Core.Profile.AssociateMissionsDefault;
            set
            {
                OnPropertyChanged();
                AstralAccessors.Quester.Core.Profile.AssociateMissionsDefault = value;
            }
        }

#if false
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        public override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        } 
#endif

        /// <summary>
        /// Загрузка профиля из файла <paramref name="fileName"/>
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public override bool LoadFromFile(string fileName) => AstralAccessors.Quester.Core.PrefixLoad(fileName);

        /// <summary>
        /// Сохранение профиля в заданный файл
        /// </summary>
        public override void Save()
        {
            ReconstructProfile();

            string externalMeshes = ExternalMeshFileName;
            AstralAccessors.Quester.Core.Save();
            _customRegionsChangeNum = 0;
            _vendorsChangeNum = 0;
            _blackListChangeNum = 0;
            if (UseExternalMeshFile
                && externalMeshes != ExternalMeshFileName)
            {
                OnPropertyChanged(nameof(ExternalMeshFileName));
            }
        }

        /// <summary>
        /// Сохранение профиля в новый файл
        /// </summary>
        public override void SaveAs()
        {
            ReconstructProfile();

            string externalMeshes = ExternalMeshFileName;
            AstralAccessors.Quester.Core.Save(true);
            if (UseExternalMeshFile
                && externalMeshes != ExternalMeshFileName)
            {
                OnPropertyChanged(nameof(ExternalMeshFileName));
            }
            _customRegionsChangeNum = 0;
            _vendorsChangeNum = 0;
            _blackListChangeNum = 0;
        }

        private void ReconstructProfile()
        {
            if (_customRegions != null && _customRegionsChangeNum > 0)
                AstralAccessors.Quester.Core.Profile.CustomRegions = _customRegions.ToList();
            if (_blackList != null && _blackListChangeNum > 0)
                AstralAccessors.Quester.Core.Profile.BlackList = _blackList.ToList();
            if (_vendors != null && _vendorsChangeNum > 0)
                AstralAccessors.Quester.Core.Profile.Vendors = _vendors.ToList();
        }
    }
}
