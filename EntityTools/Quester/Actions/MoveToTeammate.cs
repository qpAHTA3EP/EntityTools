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
        #region Опции команды
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
                OnPropertyChanged();
            }
        }
        internal TeammateSupport supportOptions = new TeammateSupport();

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
                    OnPropertyChanged();
                }
            }
        }
        internal CustomRegionCollection customRegions = new CustomRegionCollection();


#if DEVELOPER
        [Description("Distance to the Teammate by which it is necessary to approach.\n" +
                     "Keep in mind that the distance below 5 is too small to display on the Mapper")]
        [Category("Interruptions")]
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
                    OnPropertyChanged();
                }
            }
        }
        internal float distance = 30;

#if DEVELOPER
        [Description("Enable '"+nameof(IgnoreCombat)+"' profile value while playing action")]
        [Category("Interruptions")]
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
                    OnPropertyChanged();
                }
            }
        }
        internal bool ignoreCombat = true;

#if DEVELOPER
        [Description("The minimum percentage of the health  (in range 0-99) above which combat can be interrupted")]
        [Category("Interruptions")]
#else
        [Browsable(false)]
#endif
        public int IgnoreCombatMinHP
        {
            get => ignoreCombatMinHP; set
            {
                if (value < 0)
                    value = 0;
                else if(value > 99)
                {
                    value = 99;
                }
                if (ignoreCombatMinHP != value)
                {
                    ignoreCombatMinHP = value;
                    OnPropertyChanged();
                }
            }
        }
        internal int ignoreCombatMinHP = 50;

#if DEVELOPER
        [Description("The battle is aborted outside '"+nameof(AbortCombatDistance) + "' radius from the Teammate.\n" +
                     "The combat is restored within the '"+nameof(Distance)+"' radius.\n" +
                     "However, this is not performed if the value less than '"+ nameof(Distance) +"' or '"+nameof(IgnoreCombat)+"' is False.")]
        [Category("Interruptions")]
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
                    OnPropertyChanged();
                }
            }
        }
        internal uint abortCombatDistance;

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
                    OnPropertyChanged();
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
                    OnPropertyChanged();
                }
            }
        }
        internal int teammateSearchTime = 10_000;

#if DEVELOPER
        [XmlIgnore]
        [Editor(typeof(TargetSelectorTestEditor), typeof(UITypeEditor))]
        [Description("Нажми на кнопку '...' чтобы увидеть тестовую информацию")]
        public string TestInfo { get; } = @"Нажми на кнопку '...' чтобы увидеть больше =>";
#endif

        [XmlIgnore]
        [Browsable(false)]
        public override string Category => "Basic";
        #endregion

        #region Взаимодействие с ядром EntityToolsCore
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            Engine.OnPropertyChanged(this, propertyName);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [XmlIgnore]
        [NonSerialized]
        internal IQuesterActionEngine Engine;

        public MoveToTeammate()
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
        #endregion

        // Интерфейс Quester.Action, реализованный через ActionEngine
        public override bool NeedToRun => Engine.NeedToRun;
        public override ActionResult Run() => Engine.Run();
        public override string ActionLabel => Engine.ActionLabel;
        public override string InternalDisplayName => string.Empty;
        public override bool UseHotSpots => Engine.UseHotSpots;
        protected override bool IntenalConditions => Engine.InternalConditions;
        protected override Vector3 InternalDestination => Engine.InternalDestination;
        protected override ActionValidity InternalValidity => Engine.InternalValidity;
        public override void GatherInfos() => Engine.GatherInfos();
        public override void InternalReset() => Engine.InternalReset();
        public override void OnMapDraw(GraphicsNW graph) => Engine.OnMapDraw(graph);
    }
}
