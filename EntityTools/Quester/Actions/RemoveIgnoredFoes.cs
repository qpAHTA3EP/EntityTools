using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

using ACTP0Tools.Reflection;

using Astral.Logic.Classes.Map;
using EntityTools.Annotations;
using EntityTools.Core.Interfaces;
using EntityTools.Editors;
using EntityTools.Tools.Combats.IgnoredFoes;

using MyNW.Classes;

using Action = Astral.Quester.Classes.Action;

namespace EntityTools.Quester.Actions
{
    [Serializable]
    public class RemoveIgnoredFoes : Action, INotifyPropertyChanged
    {
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
                OnPropertyChanged();
            }
        }
        List<string> _foes = new List<string>();
        
        [XmlIgnore]
        [Editor(typeof(IgnoredFoesTestEditor), typeof(UITypeEditor))]
        [Description("Нажми на кнопку '...', чтобы проверить работу команды")]
        public string TestInfo => "Нажми на кнопку '...' =>";
        #endregion


        

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = default)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion




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
    }
}
