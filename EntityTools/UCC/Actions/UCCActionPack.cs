using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

using Astral.Logic.UCC.Classes;
// ReSharper disable InconsistentNaming

namespace EntityTools.UCC.Actions
{
    [Serializable]
    public class UCCActionPack : UCCAction, INotifyPropertyChanged
    {
        #region Опции команды
        [Category("Required")]
        [Description("The list of the ucc-action")]
        [Browsable(false)]
        public List<UCCAction> Actions
        {
            get => _actions;
            set
            {
                if (!ReferenceEquals(_actions, value))
                {
                    if (value is null)
                        _actions.Clear();
                    else _actions = value.ToList();
                
                    _actionPlayer = null;
                    NotifyPropertyChanged();
                }
            }
        }
        private List<UCCAction> _actions = new List<UCCAction>();


        [Category("Optional")]
        [Description("Execute all ucc-action one after another without interruption after first succeeded one.")]
        public bool ExecuteSequentially { get; set; } = true;

        #region Hide Inherited Properties
        [XmlIgnore]
        [Browsable(false)]
        public new Astral.Logic.UCC.Ressources.Enums.Unit Target { get; set; }

        [XmlIgnore]
        [Browsable(false)]
        public new int Range { get; set; }

        [XmlIgnore]
        [Browsable(false)]
        public new int Random { get; set; }

        [XmlIgnore]
        [Browsable(false)]
        public new string Label { get; set; }
        #endregion
        #endregion




        #region Интерфейс команды
        public override bool NeedToRun => _actions.Count > 0;

        public override bool Run()
        {
            return ActionsPlayer.playActionList(!ExecuteSequentially) > 0;
        }

        protected ActionsPlayer ActionsPlayer => _actionPlayer ?? (_actionPlayer = new ActionsPlayer(_actions));
        private ActionsPlayer _actionPlayer;

        public override UCCAction Clone()
        {
            return BaseClone(new UCCActionPack
            {
                _actions = _actions.Select(a => a.Clone()).ToList(),
            });
        }

        public override string ToString()
        {
            var name = ActionName;
            return string.IsNullOrEmpty(name) || name == "New Action" 
                 ? "ActionPack" 
                 : "ActionPack: " + name;
        }
        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
