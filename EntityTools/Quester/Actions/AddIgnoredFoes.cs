using Infrastructure.Reflection;
using Astral.Logic.Classes.Map;
using EntityTools.Editors;
using EntityTools.Tools.Combats.IgnoredFoes;
using MyNW.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using Action = Astral.Quester.Classes.Action;

namespace EntityTools.Quester.Actions
{
    [Serializable]
    public class AddIgnoredFoes : Action, INotifyPropertyChanged
    {
        #region Опции команды
        [Description("IDs of the Enemies ignoring in the combat")]
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

        [Description("The time period (ms) during which the Enemies listed in the 'Foes' will be ignored.\n\r" +
            "Zero value interpreted like whole bot session")]
        [Category("Required")]
        public int Timeout
        {
            get => _timeout;
            set
            {
                value = Math.Max(value, 0);
                if (_timeout != value)
                {
                    _timeout = value;
                    OnPropertyChanged();
                }
            }
        }
        int _timeout = 30_000;

        [XmlIgnore]
        [Editor(typeof(IgnoredFoesTestEditor), typeof(UITypeEditor))]
        [Description("Нажми на кнопку '...', чтобы увидеть отладочную информацию")]
        public string TestInfo => "Нажми на кнопку '...' =>";
        #endregion




        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion




        #region Интерфейс Action
        public override bool NeedToRun => true;
        public override ActionResult Run()
        {
            IgnoredFoesCore.Add(_foes, _timeout);
            return ActionResult.Completed;
        }
        public override string ActionLabel => $"{GetType().Name} : {_foes.Count}";
        public override string InternalDisplayName => string.Empty;
        public override bool UseHotSpots => false;
        protected override bool IntenalConditions => _foes.Count > 0;
        protected override Vector3 InternalDestination => Vector3.Empty;
        protected override ActionValidity InternalValidity
        {
            get
            {
                if (_foes.Count > 0)
                    return Empty.ActionValidity;
                return new ActionValidity($"{nameof(Foes)} list is empty");
            }
        }
        public override void GatherInfos() { }
        public override void InternalReset() { }
        public override void OnMapDraw(GraphicsNW graph) { } 
        #endregion
    }
}
