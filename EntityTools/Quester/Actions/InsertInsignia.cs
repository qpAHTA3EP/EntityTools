#define DEBUG_INSERTINSIGNIA

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using Astral.Logic.Classes.Map;
using EntityTools.Core.Interfaces;
using EntityTools.Core.Proxies;
using MyNW.Classes;
using Action = Astral.Quester.Classes.Action;

namespace EntityTools.Quester.Actions
{
    [Serializable]
    public class InsertInsignia : Action, INotifyPropertyChanged
    {
        #region Взаимодействие с EntityToolsCore
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [NonSerialized]
        private IQuesterActionEngine Engine;

        public InsertInsignia()
        {
            Engine = new QuesterActionProxy(this);
        }
        public void Bind(IQuesterActionEngine engine)
        {
            Engine = engine;
        }
        public void Unbind()
        {
            Engine = new QuesterActionProxy(this);
            PropertyChanged = null;
        }
        private IQuesterActionEngine MakeProxy()
        {
            return new QuesterActionProxy(this);
        }
        #endregion

        public override bool NeedToRun => LazyInitializer.EnsureInitialized(ref Engine, MakeProxy).NeedToRun;
        public override ActionResult Run() => LazyInitializer.EnsureInitialized(ref Engine, MakeProxy).Run();
        public override string ActionLabel => LazyInitializer.EnsureInitialized(ref Engine, MakeProxy).ActionLabel;
        public override string InternalDisplayName => string.Empty;
        public override bool UseHotSpots => LazyInitializer.EnsureInitialized(ref Engine, MakeProxy).UseHotSpots;
        protected override bool IntenalConditions => LazyInitializer.EnsureInitialized(ref Engine, MakeProxy).InternalConditions;
        protected override Vector3 InternalDestination => LazyInitializer.EnsureInitialized(ref Engine, MakeProxy).InternalDestination;
        protected override ActionValidity InternalValidity => LazyInitializer.EnsureInitialized(ref Engine, MakeProxy).InternalValidity;
        public override void GatherInfos() => LazyInitializer.EnsureInitialized(ref Engine, MakeProxy).GatherInfos();
        public override void InternalReset() => LazyInitializer.EnsureInitialized(ref Engine, MakeProxy).InternalReset();
        public override void OnMapDraw(GraphicsNW graph) => LazyInitializer.EnsureInitialized(ref Engine, MakeProxy).OnMapDraw(graph);
    }
}
