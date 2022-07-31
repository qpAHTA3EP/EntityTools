using Astral.Logic.UCC.Classes;
using EntityTools.Core.Interfaces;
using EntityTools.Core.Proxies;
using EntityTools.Editors;
using EntityTools.UCC.Conditions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

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
        public bool EexecuteSequentially { get; set; } = true;
#if CUSTOM_UCC_CONDITION_EDITOR
#if DEVELOPER
        [Category("Optional")]
        [Editor(typeof(UccConditionListEditor), typeof(UITypeEditor))]
        //[TypeConverter(typeof(ExpandableObjectConverter))]
#else
        [Browsable(false)]
#endif
        public UCCConditionPack CustomConditions
        {
            get => _customConditions;
            set
            {
                if (ReferenceEquals(_customConditions, value))
                    return;
                if (value is null)
                    _customConditions.Conditions.Clear();
                else _customConditions = value;

                NotifyPropertyChanged();
            }
        }
        private UCCConditionPack _customConditions = new UCCConditionPack(); 
#endif

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
        #endregion
        #endregion

        #region Интерфейс команды

#if CUSTOM_UCC_CONDITION_EDITOR
        public override bool NeedToRun => _actions.Count > 0 && _customConditions.IsOK(this); 
#else
        public override bool NeedToRun => _actions.Count > 0;
#endif

        public override bool Run()
        {
            return (_actionPlayer ?? (_actionPlayer = new ActionsPlayer(_actions))).playActionList(!EexecuteSequentially) > 0;
        }
        private ActionsPlayer _actionPlayer;

        public override UCCAction Clone()
        {
            return BaseClone(new UCCActionPack
            {
                _actions = _actions.Select(a => a.Clone()).ToList(),
#if CUSTOM_UCC_CONDITION_EDITOR
                _customConditions = _customConditions.Clone() as UCCConditionPack 
#endif
            });
        }

        public override string ToString()
        {
            var name = ActionName;
            if (string.IsNullOrEmpty(name) || name == "New Action")
                return "ActionPack";
            else return "ActionPack: " + name;
        }
        #endregion

        #region Взаимодействие с EntityToolsCore
        [NonSerialized]
        internal IUccActionEngine Engine;


        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            //Engine.OnPropertyChanged(this, propertyName);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public UCCActionPack()
        {
            Engine = new UccActionProxy(this);
        }
        private IUccActionEngine MakeProxy()
        {
            return new UccActionProxy(this);
        }
        #endregion
    }
}
