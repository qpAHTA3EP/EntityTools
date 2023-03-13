using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using Infrastructure.Quester;
using Astral.Quester.Classes;
using EntityTools.Editors;

namespace EntityTools.Quester.Editor.Classes
{
    public sealed class ProfileProxy : BaseQuesterProfileProxy
    {
        protected override Profile RealProfile => _realProfile;
        private Profile _realProfile;

#if DEBUG
        [Browsable(true)]
        [Category("File")]
#else
        [Browsable(false)] 
#endif
        public override string ProfilePath
        {
            get => _profilePath;
            protected set
            {
                OnPropertyChanged();
                _profilePath = value;
                ResetCachedMeshes();
            }
        }
        private string _profilePath;


        /// <summary>
        /// Путь к внешнему файлу мешей
        /// </summary>
        [Category("External Meshes")]
        [Editor(typeof(RelativeMeshesFilePathEditor), typeof(UITypeEditor))]
        public override string ExternalMeshFileName
        {
            get => base.ExternalMeshFileName;
            set => base.ExternalMeshFileName = value;
        }

        public ProfileProxy()
        {
            _realProfile = new Profile { Saved = true };
        }

        public ProfileProxy(Profile profile, string fileName)
        {
            _realProfile = profile ?? new Profile();
            RealProfile.Saved = true;

            if (!string.IsNullOrEmpty(fileName))
                ProfilePath = Path.GetFullPath(fileName);
        }

        /// <summary>
        /// Инициализация новым объектом <paramref name="profile"/> без удаления привязок
        /// </summary>
        public override void SetProfile(Profile profile, string fileName)
        {
            if (ReferenceEquals(RealProfile, profile))
                return;

            if (profile is null)
                profile = new Profile();
            AssignInternals(profile);
            _realProfile = profile;
            ProfilePath = fileName;

        }

        #region SavingEvent
        public delegate void BeforeSavingEvent();

        public event BeforeSavingEvent OnSavingEvent; 
        #endregion


        /// <summary>
        /// Сохранение профиля в заданный файл
        /// </summary>
        public override void Save()
        {
            if(RealProfile is null)
                return;

            OnSavingEvent?.Invoke();

            base.Save();
        }
        /// <summary>
        /// Сохранение профиля в новый файл
        /// </summary>
        /// <returns></returns>
        public override void SaveAs()
        {
            if (RealProfile is null)
                return;

            OnSavingEvent?.Invoke();

            base.SaveAs();
        }
    }
}
