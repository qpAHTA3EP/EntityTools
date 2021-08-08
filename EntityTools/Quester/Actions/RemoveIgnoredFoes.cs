using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Threading;
using System.Xml.Serialization;
using Astral.Classes.ItemFilter;
using Astral.Logic.Classes.Map;
using EntityTools.Core.Interfaces;
using EntityTools.Core.Proxies;
using EntityTools.Editors;
using EntityTools.Enums;
using AcTp0Tools.Reflection;
using EntityTools.Tools.Combats.IgnoredFoes;
using MyNW.Classes;
using Action = Astral.Quester.Classes.Action;
using AcTp0Tools;

namespace EntityTools.Quester.Actions
{
    [Serializable]
    public class RemoveIgnoredFoes : Action, INotifyPropertyChanged
    {
        #region Взаимодействие с ядром EntityToolsCore
        public event PropertyChangedEventHandler PropertyChanged;
        private RemoveIgnoredFoes @this => this;

#if false
        [XmlIgnore]
        [NonSerialized]
        internal IQuesterActionEngine Engine;
#endif
        public RemoveIgnoredFoes() { }

        private IQuesterActionEngine MakeProxie()
        {
            return new QuesterActionProxy(this);
        }
        #endregion

        #region Опции команды
        [Description("IDs of the Enemies removing from the ignored list")]
        [Editor(typeof(FoeListEditor), typeof(UITypeEditor))]
        [Category("Required")]
        public List<string> Foes
        {
            get => _foes;
            set
            {
                _foes = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Foes)));
            }
        }
        List<string> _foes = new List<string>();
        
        [XmlIgnore]
        [Editor(typeof(IgnoredFoesTestEditor), typeof(UITypeEditor))]
        [Description("Нажми на кнопку '...', чтобы проверить работу команды")]
        public string TestInfo { get; } = "Нажми на кнопку '...' =>";
        #endregion

        // Интерфес Quester.Action, реализованный через ActionEngine
#if false
        public override bool NeedToRun => LazyInitializer.EnsureInitialized(ref Engine, MakeProxie).NeedToRun;
        public override ActionResult Run() => LazyInitializer.EnsureInitialized(ref Engine, MakeProxie).Run();
        public override string ActionLabel => LazyInitializer.EnsureInitialized(ref Engine, MakeProxie).ActionLabel;
        public override string InternalDisplayName => string.Empty;
        public override bool UseHotSpots => LazyInitializer.EnsureInitialized(ref Engine, MakeProxie).UseHotSpots;
        protected override bool IntenalConditions => LazyInitializer.EnsureInitialized(ref Engine, MakeProxie).InternalConditions;
        protected override Vector3 InternalDestination => LazyInitializer.EnsureInitialized(ref Engine, MakeProxie).InternalDestination;
        protected override ActionValidity InternalValidity => LazyInitializer.EnsureInitialized(ref Engine, MakeProxie).InternalValidity;
        public override void GatherInfos() => LazyInitializer.EnsureInitialized(ref Engine, MakeProxie).GatherInfos();
        public override void InternalReset() => LazyInitializer.EnsureInitialized(ref Engine, MakeProxie).InternalReset();
        public override void OnMapDraw(GraphicsNW graph) => LazyInitializer.EnsureInitialized(ref Engine, MakeProxie).OnMapDraw(graph); 
#else
        public override bool NeedToRun => true;
        public override ActionResult Run()
        {
            IgnoredFoesCore.Remove(_foes.Count > 0 ? _foes : null);
            return ActionResult.Completed;
        }
        public override string ActionLabel => string.Concat(GetType().Name, " : ", _foes.Count > 0 ? _foes.Count.ToString() : "All");
        public override string InternalDisplayName => string.Empty;
        public override bool UseHotSpots => false;
        protected override bool IntenalConditions => true;
        protected override Vector3 InternalDestination => Vector3.Empty;
        protected override ActionValidity InternalValidity => Empty.ActionValidity;
        public override void GatherInfos() { }
        public override void InternalReset() { }
        public override void OnMapDraw(GraphicsNW graph) { }
#endif
    }
}
