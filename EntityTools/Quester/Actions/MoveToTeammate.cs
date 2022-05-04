using Astral.Logic.Classes.Map;
using EntityTools.Core.Interfaces;
using EntityTools.Core.Proxies;
using EntityTools.Editors;
using EntityTools.Tools.CustomRegions;
using EntityTools.Tools.Targeting;
using MyNW.Classes;
using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using Action = Astral.Quester.Classes.Action;
// ReSharper disable InconsistentNaming

namespace EntityTools.Quester.Actions
{
    [Serializable]
    public class MoveToTeammate : Action, INotifyPropertyChanged
    {
        #region General
#if DEVELOPER
        [Description("The selection of the supported teammate")]
        [Category("General")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
#else
        [Browsable(false)]
#endif
        public TeammateSupport SupportOptions
        {
            get => supportOptions;
            set
            {
                supportOptions = value;
                NotifyPropertyChanged();
            }
        }
        private TeammateSupport supportOptions = new TeammateSupport();

#if DEVELOPER
        [XmlIgnore]
        [Editor(typeof(TargetSelectorTestEditor), typeof(UITypeEditor))]
        [Description("Test the Teammate searching.")]
        [Category("General")]
        public string TestSearch => @"Push button '...' =>";
#endif 
        #endregion


        #region ManageCombatOptions
#if DEVELOPER
        [Description("Distance to the Teammate by which it is necessary to approach.\n" +
                     "Keep in mind that the distance below 5 is too small to display on the Mapper")]
        [Category("Manage Combat Options")]
        [DisplayName("CombatDistance")]
#else
        [Browsable(false)]
#endif

        public float Distance
        {
            get => distance; set
            {
                if (Math.Abs(distance - value) > 0.1f)
                {
                    distance = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private float distance = 30;

#if DEVELOPER
        [Description("Enable '" + nameof(IgnoreCombat) + "' profile value while playing action")]
        [Category("Manage Combat Options")]
#else
        [Browsable(false)]
#endif
        public bool IgnoreCombat
        {
            get => ignoreCombat; set
            {
                if (ignoreCombat != value)
                {
                    ignoreCombat = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool ignoreCombat = true;


#if DEVELOPER
        [Description("Sets the ucc option '" + nameof(IgnoreCombatMinHP) + "' when disabling combat mode.\n" +
                     "Options ignored if the value is -1.")]
        [Category("Manage Combat Options")]
#else
        [Browsable(false)]
#endif
        public int IgnoreCombatMinHP
        {
            get => _ignoreCombatMinHp; set
            {
                if (value < -1)
                    value = 0;
                if (value > 100)
                    value = 100;
                if (_ignoreCombatMinHp != value)
                {
                    _ignoreCombatMinHp = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private int _ignoreCombatMinHp = -1;
#if DEVELOPER
        [Description("Special check before disabling combat while playing action.\n" +
                     "The condition is checking when option '" + nameof(IgnoreCombat) + "' is active.")]
        [Category("Manage Combat Options")]
        [Editor(typeof(QuesterConditionEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(ExpandableObjectConverter))]
#else
        [Browsable(false)]
#endif
        public Astral.Quester.Classes.Condition IgnoreCombatCondition
        {
            get => _ignoreCombatCondition;
            set
            {
                if (value != _ignoreCombatCondition)
                {
                    _ignoreCombatCondition = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private Astral.Quester.Classes.Condition _ignoreCombatCondition;

#if DEVELOPER
        [Description("The battle is aborted outside '" + nameof(AbortCombatDistance) + "' radius from the Teammate.\n" +
                     "The combat is restored within the '" + nameof(Distance) + "' radius.\n" +
                     "However, this is not performed if the value less than '" + nameof(Distance) + "' or '" + nameof(IgnoreCombat) + "' is False.")]
        [Category("Manage Combat Options")]
#else
        [Browsable(false)]
#endif
        public uint AbortCombatDistance
        {
            get => abortCombatDistance; set
            {
                if (abortCombatDistance != value)
                {
                    abortCombatDistance = value;
                    NotifyPropertyChanged();
                }
            }
        }
        internal uint abortCombatDistance;
        #endregion


#if DEVELOPER
        [Description("CustomRegion names collection")]
        [Editor(typeof(CustomRegionCollectionEditor), typeof(UITypeEditor))]
        [Category("Optional")]
#else
        [Browsable(false)]
#endif
        public CustomRegionCollection CustomRegions
        {
            get => customRegions;
            set
            {
                if (customRegions != value)
                {
                    customRegions = value;
                    NotifyPropertyChanged();
                }
            }
        }
        internal CustomRegionCollection customRegions = new CustomRegionCollection();


        #region Interruptions
#if DEVELOPER
        [Description("True: Complete an action when the Teammate is closer than 'Distance'\n" +
                     "False: Follow an Teammate regardless of its distance")]
        [Category("Interruptions")]
#else
        [Browsable(false)]
#endif
        public bool StopOnApproached
        {
            get => stopOnApproached; set
            {
                if (stopOnApproached != value)
                {
                    stopOnApproached = value;
                    NotifyPropertyChanged();
                }
            }
        }
        internal bool stopOnApproached;

#if DEVELOPER
        [Description("The command is interrupted upon teammate search timer reaching zero (ms).\n" +
                     "Set zero value to infinite search.")]
        [Category("Interruptions")]
#else
        [Browsable(false)]
#endif
        public int TeammateSearchTime
        {
            get => teammateSearchTime; set
            {
                if (value < 0)
                    value = 0;
                if (teammateSearchTime != value)
                {
                    teammateSearchTime = value;
                    NotifyPropertyChanged();
                }
            }
        }
        internal int teammateSearchTime = 10_000; 
        #endregion


        [XmlIgnore]
        [Browsable(false)]
        public override string Category => "Basic";


        #region Взаимодействие с ядром EntityToolsCore
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            _engine.OnPropertyChanged(this, propertyName);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [XmlIgnore]
        [NonSerialized]
        private IQuesterActionEngine _engine;

        public MoveToTeammate()
        {
            _engine = new QuesterActionProxy(this);
        }

        public void Bind(IQuesterActionEngine engine)
        {
            _engine = engine;
        }
        public void Unbind()
        {
            _engine = new QuesterActionProxy(this);
            PropertyChanged = null;
        }
        #endregion

        // Интерфейс Quester.Action, реализованный через ActionEngine
        public override bool NeedToRun => _engine.NeedToRun;
        public override ActionResult Run() => _engine.Run();
        public override string ActionLabel => _engine.ActionLabel;
        public override string InternalDisplayName => string.Empty;
        public override bool UseHotSpots => _engine.UseHotSpots;
        protected override bool IntenalConditions => _engine.InternalConditions;
        protected override Vector3 InternalDestination => _engine.InternalDestination;
        protected override ActionValidity InternalValidity => _engine.InternalValidity;
        public override void GatherInfos() => _engine.GatherInfos();
        public override void InternalReset() => _engine.InternalReset();
        public override void OnMapDraw(GraphicsNW graph) => _engine.OnMapDraw(graph);
    }
}
