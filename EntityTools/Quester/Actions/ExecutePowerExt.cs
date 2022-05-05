#define DEBUG_INSERTINSIGNIA

using Astral.Logic.Classes.Map;
using EntityTools.Core.Interfaces;
using EntityTools.Core.Proxies;
using EntityTools.Editors;
using MyNW.Classes;
using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Xml.Serialization;
using Astral.Quester.UIEditors;
using EntityTools.Tools.Classes;
using EntityTools.Tools.CustomRegions;
using Action = Astral.Quester.Classes.Action;
using PositionEditor = EntityTools.Editors.PositionEditor;

namespace EntityTools.Quester.Actions
{
    [Serializable]
    public class ExecutePowerExt : Action, INotifyPropertyChanged
    {
        #region Взаимодействие с EntityToolsCore
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [NonSerialized]
        internal IQuesterActionEngine Engine;

        public ExecutePowerExt()
        {
            Engine = new QuesterActionProxy(this);
            if (string.IsNullOrEmpty(powerId))
                powerId = _lastPowerId;
            if (_targetRad > 0)
                TargetRadius = _targetRad;
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


        #region Power
#if DEVELOPER
        [Editor(typeof(PowerIdEditor), typeof(UITypeEditor))]
        [Category("Power")]
#else
        [Browsable(false)]
#endif
        public string PowerId
        {
            get => powerId;
            set
            {
                if (powerId != value)
                {
                    powerId = value;
                    _lastPowerId = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private string powerId = string.Empty;
        [NonSerialized]
        private static string _lastPowerId;

#if DEVELOPER
        [Description("Time to cast the power. Minimum is 500 ms")]
        [Category("Power")]
        [DisplayName("CastingTime (ms)")]
#else
        [Browsable(false)]
#endif
        public int CastingTime
        {
            get => castingTime;
            set
            {
                if (castingTime != value)
                {
                    if (value < 500)
                        value = 500;
                    castingTime = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private int castingTime = 500;

#if DEVELOPER
        [Description("Time to wait after executing the power. Minimum is 500 ms")]
        [Category("Power")]
        [DisplayName("Pause (ms)")]
#else
        [Browsable(false)]
#endif
        public int Pause
        {
            get => pause;
            set
            {
                if (value < 500)
                    value = 500;
                if (pause != value)
                {
                    pause = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private int pause = 500;
        #endregion


        #region Location
        /// <summary>
        /// Точка, в которой необходимо использовать заданное умение
        /// </summary>
#if DEVELOPER
        [Description("The position that the character should stand to use the power specified '" + nameof(PowerId) + "'")]
        [Editor(typeof(PositionEditor), typeof(UITypeEditor))]
        [Category("Location")]
#else
        [Browsable(false)]
#endif   
        public Vector3 InitialPosition
        {
            get => initialPosition;
            set
            {
                if (value != initialPosition)
                {
                    initialPosition = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private Vector3 initialPosition = Vector3.Empty;

#if DEVELOPER
        [Description("The collection of the CustomRegions specifying the area within which action could be applied.\n" +
                     "Ignored if empty.")]
        [Editor(typeof(CustomRegionCollectionEditor), typeof(UITypeEditor))]
        [Category("Location")]
        [DisplayName("CustomRegions")]
        //[TypeConverter(typeof(ExpandableObjectConverter))]
#else
        [Browsable(false)]
#endif
        [NotifyParentProperty(true)]
        public CustomRegionCollection CustomRegionNames
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
        private CustomRegionCollection customRegions = new CustomRegionCollection();

#if DEVELOPER
        [Description("The name of the Map where action could be applied.\n" +
                     "Ignored if empty.")]
        [Editor(typeof(CurrentMapEdit), typeof(UITypeEditor))]
        [Category("Location")]
#else
        [Browsable(false)]
#endif        
        public string CurrentMap
        {
            get => currentMap;
            set
            {
                if (value != currentMap)
                {
                    currentMap = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private string currentMap;

#if DEVELOPER
        [Description("The name of the ingame region of the map where action could be applied.\n" +
                     "Ignored if '" + nameof(CurrentMap) + "' is empty.")]
        [Editor(typeof(CurrentRegionEdit), typeof(UITypeEditor))]
        [Category("Location")]
#else
        [Browsable(false)]
#endif        
        public string CurrentRegion
        {
            get => currentRegion;
            set
            {
                if (value != currentRegion)
                {
                    currentRegion = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private string currentRegion;

#if DEVELOPER
        [Description("The interval of Z-coordinate within which the Character can reach the '" + nameof(InitialPosition) + "' and the current action can be applied.\n" +
                     "This condition is ignored when the '" + nameof(Range<float>.Min) + "' equals to '" + nameof(Range<float>.Max) + "'")]
        [Category("Location")]
        [Editor(typeof(ZRangeEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [DisplayName("ZRange")]
#else
        [Browsable(false)]
#endif 
        public Range<float> ZRange
        {
            get => zRange;
            set
            {
                if (!zRange.Equals(value))
                {
                    zRange = value.Clone();
                }
            }
        }
        private Range<float> zRange = new Range<float>();
        #endregion


        #region Target
        /// <summary>
        /// Точка, используемая в качестве цели для применения умения <see cref="PowerId"/>
        /// </summary>
#if DEVELOPER
        [Description("The position used as the target point for the power specified by '" + nameof(PowerId) + "'")]
        [Editor(typeof(PositionEditor), typeof(UITypeEditor))]
        [Category("Target")]
#else
        [Browsable(false)]
#endif   
        public Vector3 TargetPosition
        {
            get => targetPosition;
            set
            {
                if (value != targetPosition)
                {
                    targetPosition = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private Vector3 targetPosition = Vector3.Empty;

#if DEVELOPER
        [Description("The radius of target area centered on '" + nameof(TargetPosition) + "'.\n" +
                     "After executing the '" + nameof(PowerId) + "' the character must be closer to the '" + nameof(TargetPosition) + "' than the '" + nameof(TargetRadius) + "' to complete action.\n" +
                     "The action will be continued if the character is farther from the '" + nameof(TargetPosition) + "' than the specified '" + nameof(TargetRadius) + "'.\n" +
                     "If value is zero the action completed after power executed.")]
        [Category("Target")]
#else
        [Browsable(false)]
#endif 
        public uint TargetRadius
        {
            get => targetRad;
            set
            {
                targetRad = value;
                _targetRad = value;
                NotifyPropertyChanged();
            }
        }
        private uint targetRad;
        #endregion


        #region ManageCombatOptions
#if DEVELOPER
        [Description("Enable '" + nameof(IgnoreCombat) + "' profile value while playing action")]
        [Category("Manage Combat Option")]
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
        #endregion


        #region DefaultOption
#if DEVELOPER
        [Description("The default value of the '" + nameof(TargetRadius) + "' for each new command '" + nameof(ExecutePowerExt) + "'.")]
        [Category("Default option")]
#else
        [Browsable(false)]
#endif 
        [XmlIgnore]
        public uint DefaultTargetRadius
        {
            get => _targetRad;
            set => _targetRad = value;
        }
        [NonSerialized]
        private static uint _targetRad;

#if DEVELOPER
        [Description("The offset of Z-coordinate from the '" + nameof(InitialPosition) + "'.\n" +
                     "When the new '" + nameof(ExecutePowerExt) + "' command  will be added to the profile the '" + nameof(zRange) + "' will be calculated as follows:\n" +
                     "Min = " + nameof(InitialPosition) + "." + nameof(Vector3.Z) + " - " + nameof(ZDeviation) + "\n" +
                     "Max = " + nameof(InitialPosition) + "." + nameof(Vector3.Z) + " + " + nameof(ZDeviation))]
        [Category("Default Option")]
        [DisplayName("ZDeviation")]
#else
        [Browsable(false)]
#endif 
        [XmlIgnore]
        public uint ZDeviation
        {
            get => _zDev;
            set
            {
                _zDev = value;
            }
        }
        [NonSerialized]
        private static uint _zDev = 0; 
        #endregion


        private Astral.Quester.Classes.Condition _ignoreCombatCondition;
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
