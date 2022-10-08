using ACTP0Tools.Annotations;
using AStar;
using Astral.Quester.Classes;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using QuesterAction = Astral.Quester.Classes.Action;

namespace ACTP0Tools.Classes.Quester
{
    public abstract class QuesterProfileProxy
    {
        [Browsable(false)]
        public abstract Profile Profile { get; }

        [Browsable(false)] 
        public abstract string FileName { get; set; }

        [Browsable(false)]
        public abstract bool Saved { get; set; }

        /// <summary>
        /// Путевой граф для карты, на которой находится персонаж
        /// </summary>
        [Browsable(false)]
        public abstract Graph CurrentMesh { get; }

        /// <summary>
        /// Набор путевых графов, ассоциированных с профилем
        /// </summary>
        [Browsable(false)]
        public abstract IDictionary<string, Graph> MapsMeshes { get; }

        /// <summary>
        /// Имя файла, в котором хранятся файлы путевых графов
        /// </summary>
        [Browsable(false)]
        public abstract string CurrentProfileZipMeshFile { get; }

        /// <summary>
        /// Набор команд quester'a
        /// </summary>
        [Browsable(false)]
        [NotifyParentProperty(true)]
        public abstract IEnumerable<QuesterAction> Actions { get; set; }

        /// <summary>
        /// Набор идентификаторов противников, которые на должны быть атакованы (игнорируются в бою).
        /// </summary>
        [Browsable(false)]
        public abstract BindingList<string> BlackList { get; }

        /// <summary>
        /// Набор <see cref="CustomRegion"/>
        /// </summary>
        [Browsable(false)]
        [NotifyParentProperty(true)]
        public abstract BindingList<CustomRegion> CustomRegions { get; }

        /// <summary>
        /// Набор описаний продавцов.
        /// </summary>
        [Browsable(false)]
        [NotifyParentProperty(true)]
        public abstract BindingList<NPCInfos> Vendors { get; }

        /// <summary>
        /// Радиус вступления в бой
        /// </summary>
        public virtual int KillRadius
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
        public abstract bool UseExternalMeshFile { get; set; }

        /// <summary>
        /// Путь к внешнему файлу мешей
        /// </summary>
        [Category("External Meshes")]
        public abstract string ExternalMeshFileName { get; set; }

        public abstract bool DisablePet { get; set; }

        [Category("Follower")]
        public abstract int FollowerDistance { get; set; }

        [Category("Follower")]
        public abstract bool DisableFollow { get; set; }

        [Description(
            "False : Ignore AssociateMission attribute if character don't have mission\r\nTrue : skip action if don't have the AssociateMission")]
        public abstract bool AssociateMissionsDefault { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Загрузка профиля из файла <paramref name="fileName"/>
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public abstract bool LoadFromFile(string fileName);

        /// <summary>
        /// Сохранение профиля в заданный файл
        /// </summary>
        public abstract void Save();

        /// <summary>
        /// Сохранение профиля в новый файл
        /// </summary>
        public abstract void SaveAs();
    }
}
