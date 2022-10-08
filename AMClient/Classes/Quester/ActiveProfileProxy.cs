using AStar;
using Astral.Quester.Classes;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using QuesterAction = Astral.Quester.Classes.Action;

namespace ACTP0Tools.Classes.Quester
{
    public class ActiveProfileProxy : QuesterProfileProxy, INotifyPropertyChanged
    {
        [Browsable(false)]
        public override Profile Profile => AstralAccessors.Quester.Core.Profile;

        [Browsable(false)]
        public override string FileName
        {
            get => Astral.API.CurrentSettings.LastQuesterProfile;
            set
            {
                OnPropertyChanged();
                Astral.API.CurrentSettings.LastQuesterProfile = value;
            }
        }

        [Browsable(false)]
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
        [Browsable(false)]
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
                OnPropertyChanged();
                var actions = AstralAccessors.Quester.Core.Profile.MainActionPack.Actions;
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
            if (_customRegions != null && _customRegionsChangeNum > 0)
                AstralAccessors.Quester.Core.Profile.CustomRegions = _customRegions.ToList();
            if (_blackList != null && _blackListChangeNum > 0)
                AstralAccessors.Quester.Core.Profile.BlackList = _blackList.ToList();
            if (_vendors != null && _vendorsChangeNum > 0)
                AstralAccessors.Quester.Core.Profile.Vendors = _vendors.ToList();

            AstralAccessors.Quester.Core.Save();
            _customRegionsChangeNum = 0;
            _blackListChangeNum = 0;
        }

        /// <summary>
        /// Сохранение профиля в новый файл
        /// </summary>
        public override void SaveAs()
        {
            if (_customRegions != null && _customRegionsChangeNum > 0)
                AstralAccessors.Quester.Core.Profile.CustomRegions = _customRegions.ToList();
            if (_blackList != null && _blackListChangeNum > 0)
                AstralAccessors.Quester.Core.Profile.BlackList = _blackList.ToList();
            if (_vendors != null && _vendorsChangeNum > 0)
                AstralAccessors.Quester.Core.Profile.Vendors = _vendors.ToList();

            AstralAccessors.Quester.Core.Save(true);
            _customRegionsChangeNum = 0;
            _blackListChangeNum = 0;
        }
    }
}
