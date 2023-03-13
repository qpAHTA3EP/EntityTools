#define DEBUG_INSERTINSIGNIA

using Astral.Logic.Classes.Map;
using Astral.Quester.UIEditors;
using EntityTools.Editors;
using EntityTools.Forms;
using EntityTools.Quester.Mapper;
using EntityTools.Tools.Classes;
using EntityTools.Tools.CustomRegions;
using EntityTools.Tools.Navigation;
using EntityTools.Tools.Powers;
using Infrastructure;
using MyNW.Classes;
using MyNW.Internals;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Xml.Serialization;
using Action = Astral.Quester.Classes.Action;
using PositionEditor = EntityTools.Editors.PositionEditor;
// ReSharper disable InconsistentNaming

namespace EntityTools.Quester.Actions
{
    [Serializable]
    public class ExecutePowerExt : Action, INotifyPropertyChanged
    {
        #region Power
        [Editor(typeof(PowerIdEditor), typeof(UITypeEditor))]
        [Category("Power")]
        public string PowerId
        {
            get => powerId;
            set
            {
                if (powerId != value)
                {
                    powerId = value;
                    _lastPowerId = value;
                    InternalReset();
                    NotifyPropertyChanged();
                }
            }
        }
        private string powerId = string.Empty;
        private readonly PowerCache powerCache = new PowerCache(string.Empty);

        [Description("Time to cast the power. Minimum is 500 ms")]
        [Category("Power")]
        [DisplayName("CastingTime (ms)")]
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

        [Description("Time to wait after executing the power. Minimum is 500 ms")]
        [Category("Power")]
        [DisplayName("Pause (ms)")]
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
        [Description("The position that the character should stand to use the power specified '" + nameof(PowerId) + "'")]
        [Editor(typeof(PositionEditor), typeof(UITypeEditor))]
        [Category("Location")]
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

        [Description("The collection of the CustomRegions specifying the area within which action could be applied.\n" +
                     "Ignored if empty.")]
        [Editor(typeof(CustomRegionCollectionEditor), typeof(UITypeEditor))]
        [Category("Location")]
        [DisplayName("CustomRegions")]
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

        [Description("The name of the Map where action could be applied.\n" +
                     "Ignored if empty.")]
        [Editor(typeof(CurrentMapEdit), typeof(UITypeEditor))]
        [Category("Location")]
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

        [Description("The name of the ingame region of the map where action could be applied.\n" +
                     "Ignored if '" + nameof(CurrentMap) + "' is empty.")]
        [Editor(typeof(CurrentRegionEdit), typeof(UITypeEditor))]
        [Category("Location")]
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

        [Description("The interval of Z-coordinate within which the Character can reach the '" + nameof(InitialPosition) + "' and the current action can be applied.\n" +
                     "This condition is ignored when the '" + nameof(Range<float>.Min) + "' equals to '" + nameof(Range<float>.Max) + "'")]
        [Category("Location")]
        [Editor(typeof(ZRangeEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [DisplayName("ZRange")]
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
        [Description("The position used as the target point for the power specified by '" + nameof(PowerId) + "'")]
        [Editor(typeof(PositionEditor), typeof(UITypeEditor))]
        [Category("Target")]
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

        [Description("The radius of target area centered on '" + nameof(TargetPosition) + "'.\n" +
                     "After executing the '" + nameof(PowerId) + "' the character must be closer to the '" + nameof(TargetPosition) + "' than the '" + nameof(TargetRadius) + "' to complete action.\n" +
                     "The action will be continued if the character is farther from the '" + nameof(TargetPosition) + "' than the specified '" + nameof(TargetRadius) + "'.\n" +
                     "If value is zero the action completed after power executed.")]
        [Category("Target")]
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
        [Description("Enable '" + nameof(IgnoreCombat) + "' profile value while playing action")]
        [Category("Manage Combat Options")]
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

        [Description("Sets the ucc option '" + nameof(IgnoreCombatMinHP) + "' when disabling combat mode.\n" +
                     "Options ignored if the value is -1.")]
        [Category("Manage Combat Options")]
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

        [Description("Special check before disabling combat while playing action.\n" +
                     "The condition is checking when option '" + nameof(IgnoreCombat) + "' is active.")]
        [Category("Manage Combat Options")]
        [Editor(typeof(QuesterConditionEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(ExpandableObjectConverter))]
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
        #endregion


        #region DefaultOption
        [Description("The default value of the '" + nameof(PowerId) + "' for each new command '" + nameof(ExecutePowerExt) + "'.")]
        [Editor(typeof(PowerIdEditor), typeof(UITypeEditor))]
        [Category("Default Options")]
        [XmlIgnore]
        public static string PowerIdCache
        {
            get => _lastPowerId;
            set => _lastPowerId = value;
        }

        [NonSerialized]
        private static string _lastPowerId;

        [Description("The default value of the '" + nameof(TargetRadius) + "' for each new command '" + nameof(ExecutePowerExt) + "'.")]
        [Category("Default Options")]
        [XmlIgnore]
        public uint DefaultTargetRadius
        {
            get => _targetRad;
            set => _targetRad = value;
        }
        [NonSerialized]
        private static uint _targetRad;

        [Description("The offset of Z-coordinate from the '" + nameof(InitialPosition) + "'.\n" +
                     "When the new '" + nameof(ExecutePowerExt) + "' command  will be added to the profile the '" + nameof(zRange) + "' will be calculated as follows:\n" +
                     "Min = " + nameof(InitialPosition) + "." + nameof(Vector3.Z) + " - " + nameof(ZDeviation) + "\n" +
                     "Max = " + nameof(InitialPosition) + "." + nameof(Vector3.Z) + " + " + nameof(ZDeviation))]
        [Category("Default Option")]
        [DisplayName("ZDeviation")]
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
        private static uint _zDev;
        #endregion




        public ExecutePowerExt()
        {
            if (string.IsNullOrEmpty(powerId))
                powerId = _lastPowerId;
            if (_targetRad > 0)
                TargetRadius = _targetRad;
            InternalReset();
        }



        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion




        #region Интерфейс Action
        public override bool NeedToRun
        {
            get
            {
                bool debugInfo = EntityTools.Config.Logger.QuesterActions.DebugExecutePowerExt;
                string currentMethodName = debugInfo ? string.Concat(actionIDstr, '.', MethodBase.GetCurrentMethod()?.Name ?? nameof(Run)) : string.Empty;

                if (!IntenalConditions)
                {
                    // Чтобы прервать цикл обработки команды нужно, чтобы Engine перешел к обработке Run()
                    if (debugInfo)
                        ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: {nameof(IntenalConditions)} failed.");
                    return true;
                }

                var d = MapperHelper.SquareDistance2D(EntityManager.LocalPlayer.Location, InitialPosition);

                if (d > 25
                    && IgnoreCombat
                    && CheckingIgnoreCombatCondition())
                {
                    AstralAccessors.Quester.FSM.States.Combat.SetIgnoreCombat(true, IgnoreCombatMinHP, 5_000);
                    if (debugInfo)
                        ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: Set ignore combat.");
                    return false;
                }
                AstralAccessors.Quester.FSM.States.Combat.SetIgnoreCombat(false);

                return d <= 25 && powerCache.GetPower() != null;
            }
        }
        
        public override ActionResult Run()
        {
            bool debugInfo = EntityTools.Config.Logger.QuesterActions.DebugExecutePowerExt;
            string currentMethodName = debugInfo ? string.Concat(actionIDstr, '.', MethodBase.GetCurrentMethod()?.Name ?? nameof(Run)) : string.Empty;


            if (!IntenalConditions)
            {
                if (debugInfo)
                    ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: {nameof(IntenalConditions)} failed. ActionResult={ActionResult.Fail}.");
                return ActionResult.Fail;
            }


            if (debugInfo)
                ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: starts");
            NavigationHelper.StopNavigationCompletely();

            var currentPower = powerCache.GetPower();

            if (currentPower is null)
            {
                if (debugInfo)
                    ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: Fail to get Power '{PowerId}' by '" + nameof(PowerId) + "'");
                return ActionResult.Fail;
            }

            if (debugInfo)
                ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: Activating Power '{PowerId}'");

            var powResult = currentPower.ExecutePower(TargetPosition, CastingTime, Pause, 0, false, debugInfo);

            switch (powResult)
            {
                case PowerResult.Fail:
                    if (debugInfo)
                        ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: ActionResult => {ActionResult.Fail}");
                    return ActionResult.Fail;
                case PowerResult.Skip:
                    if (debugInfo)
                        ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: ActionResult => {ActionResult.Skip}");
                    return ActionResult.Skip;
            }

            // Проверяем необходимость и возможность повторения команды
            var rad = TargetRadius;
            if (rad > 0)
            {
                rad = Math.Max(rad, 5);
                rad *= rad;
                var pos = EntityManager.LocalPlayer.Location.Clone();
                var dist2 = MapperHelper.SquareDistance3D(pos, TargetPosition);
                if (dist2 > rad)
                {
                    bool intCheck = internalCheck();
                    var actRes = intCheck
                        ? ActionResult.Running
                        : ActionResult.Fail;
                    if (debugInfo)
                        ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: Current player position: <{pos.X:N2}, {pos.Y:N2}, {pos.Z:N2}>. Distance to {nameof(TargetPosition)} is {Math.Sqrt(dist2)}.\n" +
                                                          $"InternalConditions '{intCheck}' => {actRes}");
                    // Расстояние от персонажа до TargetPosition больше TargetRadius,
                    // поэтому выполнение команды нужно повторить
                    return actRes;
                }
                if (debugInfo)
                    ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: Current player position: <{pos.X:N2}, {pos.Y:N2}, {pos.Z:N2}>. Distance to {nameof(TargetPosition)} is {Math.Sqrt(dist2)} => {ActionResult.Completed}");
            }
            else if (debugInfo)
                ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: ActionResult => {ActionResult.Completed}");

            return ActionResult.Completed;
        }

        public override string ActionLabel
        {
            get
            {
                if (string.IsNullOrEmpty(label))
                {
                    var id = PowerId;
                    if (string.IsNullOrEmpty(id))
                        label = GetType().Name;
                    else label = $@"{GetType().Name}: [{id}]";
                }
                return label;
            }
        }
        private string label;
        private string actionIDstr;

        public override string InternalDisplayName => string.Empty;

        public override bool UseHotSpots => false;
        protected override bool IntenalConditions => internalCheck();
        protected override Vector3 InternalDestination
        {
            get
            {
                if (!internalCheck())
                    return Vector3.Empty;

                var d = MapperHelper.SquareDistance2D(EntityManager.LocalPlayer.Location, InitialPosition);
                if (d > 25)
                    return InitialPosition;

                return Vector3.Empty;
            }
        }

        protected override ActionValidity InternalValidity
        {
            get
            {
                var iniPos = InitialPosition;
                if (!iniPos.IsValid)
                    return new ActionValidity($"{nameof(InitialPosition)} is invalid");
                if (!TargetPosition.IsValid)
                    return new ActionValidity($"{nameof(TargetPosition)} is invalid");
                if (string.IsNullOrEmpty(PowerId))
                    return new ActionValidity($"{nameof(PowerId)} is invalid");
                if (zRange.IsValid && zRange.Outside(iniPos.Z))
                    return new ActionValidity($"{nameof(InitialPosition)} is out of {nameof(ZRange)}");
                return new ActionValidity();
            }
        }
        
        public override void GatherInfos()
        {
            var player = EntityManager.LocalPlayer;
            if (player.IsValid && !player.IsLoading
                && TargetSelectForm.GUIRequest("Set InitialPosition",
                                               "Move the Character to the position where the Power could be activated and press F12")
                   == DialogResult.OK)
            {
                var iniPos = player.Location;
                if (iniPos.IsValid)
                {
                    var node = AstralAccessors.Quester.Core.CurrentProfile.CurrentMesh.ClosestNode(iniPos.X, iniPos.Y, iniPos.Z, out double dist, false);

                    if (node is null
                        || dist > 10)
                    {
                        InitialPosition = iniPos;
                    }
                    else iniPos = InitialPosition = node.Position;

                    if (TargetSelectForm.GUIRequest("Set TargetPosition",
                            "Move the Character to the position on which the Power should be target before activated and press F12") ==
                        DialogResult.OK)
                    {
                        var tarPos = EntityManager.LocalPlayer.Location;
                        if (tarPos.IsValid)
                        {
                            node = AstralAccessors.Quester.Core.CurrentProfile.CurrentMesh.ClosestNode(tarPos.X, tarPos.Y, tarPos.Z,
                               out dist, false);

                            if (node is null
                                || dist > 10)
                            {
                                TargetPosition = tarPos;
                            }
                            else TargetPosition = node.Position;

                            CurrentMap = player.MapState.MapName;
                            CurrentRegion = player.RegionInternalName;

                            var zi = iniPos.Z;
                            var zDev = ZDeviation;
                            if (zDev > 0)
                            {
                                zRange.Min = (float)Math.Floor(zi - zDev);
                                zRange.Max = (float)Math.Round(zi + zDev);
                            }
                            var pwId = PowerIdEditor.GetPowerId(PowerId);
                            if (string.IsNullOrEmpty(pwId))
                            {
                                PowerId = PowerIdEditor.GetPowerId(PowerId);
                            }
                        }
                    }
                }
            }
        }

        public override void InternalReset()
        {
            powerCache.Reset(PowerId);
            label = string.Empty;

            internalCheck = initialize_internalCheck;
            actionIDstr = string.Concat(GetType().Name, '[', ActionID, ']');
        }

        public override void OnMapDraw(GraphicsNW graphics)
        {
            var iniPos = InitialPosition;
            var tarPos = TargetPosition;
            if (graphics is MapperGraphics mapGraphics)
            {
                if (iniPos.IsValid)
                {
                    if (tarPos.IsValid)
                    {
                        mapGraphics.FillRhombCentered(Brushes.Orange, tarPos, 16, 16);
                        mapGraphics.DrawLine(Pens.Orange, iniPos.X, iniPos.Y, tarPos.X, tarPos.Y);
                    }
                    mapGraphics.FillCircleCentered(Brushes.Yellow, iniPos, 12);
                }
                else if (tarPos.IsValid)
                {
                    mapGraphics.FillRhombCentered(Brushes.Orange, tarPos, 16, 16);
                }

            }
            else
            {
                if (iniPos.IsValid)
                {
                    if (tarPos.IsValid)
                    {
                        float x = tarPos.X,
                            y = tarPos.Y;
                        List<Vector3> coords = new List<Vector3>()
                        {
                            new Vector3(x, y - 5, 0),
                            new Vector3(x - 4.33f, y + 2.5f, 0),
                            new Vector3(x + 4.33f, y + 2.5f, 0)
                        };
                        graphics.drawLine(iniPos, tarPos, Pens.Orange);
                        graphics.drawFillPolygon(coords, Brushes.Orange);
                    }

                    graphics.drawEllipse(iniPos, MapperHelper.Size_12x12, Pens.Yellow);
                }
                else if (tarPos.IsValid)
                {
                    float x = tarPos.X,
                        y = tarPos.Y;
                    List<Vector3> coords = new List<Vector3>()
                    {
                        new Vector3(x, y - 5, 0),
                        new Vector3(x - 4.33f, y + 2.5f, 0),
                        new Vector3(x + 4.33f, y + 2.5f, 0)
                    };
                    graphics.drawFillPolygon(coords, Brushes.Orange);
                }
            }

        }
        #endregion




        /// <summary>
        /// Проверка условия отключения боя <see cref="ExecutePowerExt.IgnoreCombatCondition"/>
        /// </summary>
        /// <returns>Результат проверки <see cref="ExecutePowerExt.IgnoreCombatCondition"/> либо True, если оно не задано.</returns>
        private bool CheckingIgnoreCombatCondition()
        {
            var check = IgnoreCombatCondition;
            if (check != null)
                return check.IsValid;
            return true;
        }




        #region internalCheck
        /// <summary>
        /// Функтор проверки <see cref="IntenalConditions"/>
        /// </summary>
        private Func<bool> internalCheck;

        /// <summary>
        /// Функтор, инициализирующий предикат проверки <see cref="IntenalConditions"/>
        /// </summary>
        /// <returns></returns>
        private bool initialize_internalCheck()
        {
            Func<bool> check;

            var iniPos = InitialPosition;
            var tarPos = TargetPosition;
            var powId = PowerId;
            if (!iniPos.IsValid || !tarPos.IsValid || string.IsNullOrEmpty(powId)
                || zRange.IsValid && zRange.Outside(EntityManager.LocalPlayer.Location.Z))
            {
                internalCheck = () => false;
                return false;
            }

            var crSet = CustomRegionNames;
            if (crSet.Count > 0)
            {
                if (zRange.IsValid)
                {
                    var map = CurrentMap;
                    if (string.IsNullOrEmpty(map))
                    {
                        check = internalCheck_RegionSet_zRange;
                    }
                    else
                    {
                        var reg = CurrentRegion;
                        if (string.IsNullOrEmpty(reg))
                        {
                            check = internalCheck_Map_RegionSet_zRange;
                        }
                        else
                        {
                            check = internalCheck_MapRegion_RegionSet_zRange;
                        }
                    }
                }
                else
                {
                    var map = CurrentMap;
                    if (string.IsNullOrEmpty(map))
                    {
                        check = internalCheck_RegionSet;
                    }
                    else
                    {
                        var reg = CurrentRegion;
                        if (string.IsNullOrEmpty(reg))
                        {
                            check = internalCheck_Map_RegionSet;
                        }
                        else
                        {
                            check = internalCheck_MapRegion_RegionSet;
                        }
                    }
                }
            }
            else
            {
                if (zRange.IsValid)
                {
                    var map = CurrentMap;
                    if (string.IsNullOrEmpty(map))
                    {
                        check = internalCheck_zRange;
                    }
                    else
                    {
                        var reg = CurrentRegion;
                        if (string.IsNullOrEmpty(reg))
                        {
                            check = internalCheck_Map_zRange;
                        }
                        else
                        {
                            check = internalCheck_MapRegion_zRange;
                        }
                    }
                }
                else
                {
                    var map = CurrentMap;
                    if (string.IsNullOrEmpty(map))
                    {
                        check = internalCheck_true;
                    }
                    else
                    {
                        var reg = CurrentRegion;
                        if (string.IsNullOrEmpty(reg))
                        {
                            check = internalCheck_Map;
                        }
                        else
                        {
                            check = internalCheck_MapRegion;
                        }
                    }
                }
            }

            internalCheck = check;
            return internalCheck();
        }

        private bool internalCheck_true()
        {
            return true;
        }
        private bool internalCheck_RegionSet()
        {
            var crSet = CustomRegionNames;
            var player = EntityManager.LocalPlayer;
            var location = player.Location;
            return crSet.Within(location);
        }
        private bool internalCheck_RegionSet_zRange()
        {
            var crSet = CustomRegionNames;
            var player = EntityManager.LocalPlayer;
            var location = player.Location;
            return crSet.Within(location)
                   && zRange.Within(location.Z);
        }
        private bool internalCheck_MapRegion_RegionSet_zRange()
        {
            var map = CurrentMap;
            var reg = CurrentRegion;
            var crSet = CustomRegionNames;
            var player = EntityManager.LocalPlayer;
            var location = player.Location;
            return player.CurrentZoneMapInfo.MapName.Equals(map, StringComparison.Ordinal)
                   && player.RegionInternalName.Equals(reg, StringComparison.Ordinal)
                   && crSet.Within(location)
                   && zRange.Within(location.Z);
        }
        private bool internalCheck_Map_RegionSet_zRange()
        {
            var map = CurrentMap;
            var crSet = CustomRegionNames;
            var player = EntityManager.LocalPlayer;
            var location = player.Location;
            return player.CurrentZoneMapInfo.MapName.Equals(map, StringComparison.Ordinal)
                   && string.IsNullOrEmpty(player.RegionInternalName)
                   && crSet.Within(location)
                   && zRange.Within(location.Z);
        }
        private bool internalCheck_MapRegion_RegionSet()
        {
            var map = CurrentMap;
            var reg = CurrentRegion;
            var crSet = CustomRegionNames;
            var player = EntityManager.LocalPlayer;
            var location = player.Location;
            return player.CurrentZoneMapInfo.MapName.Equals(map, StringComparison.Ordinal)
                   && player.RegionInternalName.Equals(reg, StringComparison.Ordinal)
                   && crSet.Within(location);
        }
        private bool internalCheck_Map_RegionSet()
        {
            var map = CurrentMap;
            var crSet = CustomRegionNames;
            var player = EntityManager.LocalPlayer;
            var location = player.Location;
            return player.CurrentZoneMapInfo.MapName.Equals(map, StringComparison.Ordinal)
                   && string.IsNullOrEmpty(player.RegionInternalName)
                   && crSet.Within(location);
        }
        private bool internalCheck_Map_zRange()
        {
            var map = CurrentMap;
            var player = EntityManager.LocalPlayer;
            var location = player.Location;
            return player.CurrentZoneMapInfo.MapName.Equals(map, StringComparison.Ordinal)
                   && string.IsNullOrEmpty(player.RegionInternalName)
                   && zRange.Within(location.Z);
        }
        private bool internalCheck_MapRegion_zRange()
        {
            var map = CurrentMap;
            var reg = CurrentRegion;
            var player = EntityManager.LocalPlayer;
            var location = player.Location;
            return player.CurrentZoneMapInfo.MapName.Equals(map, StringComparison.Ordinal)
                   && player.RegionInternalName.Equals(reg, StringComparison.Ordinal)
                   && zRange.Within(location.Z);
        }
        private bool internalCheck_MapRegion()
        {
            var map = CurrentMap;
            var reg = CurrentRegion;
            var player = EntityManager.LocalPlayer;
            return player.CurrentZoneMapInfo.MapName.Equals(map, StringComparison.Ordinal)
                   && player.RegionInternalName.Equals(reg, StringComparison.Ordinal);
        }
        private bool internalCheck_Map()
        {
            var map = CurrentMap;
            var player = EntityManager.LocalPlayer;
            return player.CurrentZoneMapInfo.MapName.Equals(map, StringComparison.Ordinal)
                   && string.IsNullOrEmpty(player.RegionInternalName);
        }
        private bool internalCheck_zRange()
        {
            var player = EntityManager.LocalPlayer;
            var location = player.Location;
            return zRange.Within(location.Z);
        }
        #endregion
    }
}
