using Astral.Logic.Classes.Map;
using Astral.Logic.NW;
using Astral.Quester.UIEditors;
using EntityTools.Editors;
using EntityTools.Forms;
using EntityTools.Quester.Mapper;
using EntityTools.Tools.Classes;
using EntityTools.Tools.CustomRegions;
using EntityTools.Tools.Navigation;
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
using System.Threading;
using System.Windows.Forms;
using System.Xml.Serialization;
using Action = Astral.Quester.Classes.Action;
using PositionEditor = EntityTools.Editors.PositionEditor;
// ReSharper disable InconsistentNaming

namespace EntityTools.Quester.Actions
{
    [Serializable]
    public class Jumping : Action, INotifyPropertyChanged
    {
        #region Jumping
        [Description("Time between activation and deactivation JUMP-command. Minimum is 500 ms.")]
        [Category("Jumping")]
        [DisplayName("JumpTime (ms)")]
        public int JumpTime
        {
            get => _jumpTime;
            set
            {
                if (_jumpTime != value)
                {
                    if (value < 500)
                        value = 500;
                    _jumpTime = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private int _jumpTime = 500;

        [Description("Time to wait between activation JUMP-command and starting to move to the '" + nameof(DestinationPosition) + "'. Minimum is 0 ms.")]
        [Category("Jumping")]
        [DisplayName("DelayBeforMove (ms)")]
        public int DelayBeforMove
        {
            get => _delayBeforeMoving;
            set
            {
                if (value < 0)
                    value = 0;
                if (_delayBeforeMoving != value)
                {
                    _delayBeforeMoving = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private int _delayBeforeMoving = 500;

        [Description("Time to moving forward.\n" +
                     "When the walue is zero then forward moving stops only '" + nameof(DestinationPosition) + "' reached.")]
        [Category("Jumping")]
        [DisplayName("ForwardMovingTime (ms)")]
        public int ForwardMovingTime
        {
            get => _forwardMovingTime;
            set
            {
                if (value < 0)
                    value = 0;
                if (_forwardMovingTime != value)
                {
                    _forwardMovingTime = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private int _forwardMovingTime = 1_000;


        [Description("Toggle which activate the repeating jumps until Character reaches the '" + nameof(DestinationPosition) + "'.")]
        [Category("Jumping")]
        public bool RepeatingJumping
        {
            get => _repeatingJumping; set
            {
                _repeatingJumping = value;
                NotifyPropertyChanged();
            }
        }
        private bool _repeatingJumping = true;
        #endregion


        #region Location
        /// <summary>
        /// Точка, из которой необходимо начать прыжок
        /// </summary>
        [Description("The position that the character should stand before activate JUMP-command.")]
        [Editor(typeof(PositionEditor), typeof(UITypeEditor))]
        [Category("Location")]
        public Vector3 InitialPosition
        {
            get => _initialPosition;
            set
            {
                if (value != _initialPosition)
                {
                    _initialPosition = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private Vector3 _initialPosition = Vector3.Empty;

        [Description("The minimum distance from '" + nameof(InitialPosition) + "' at which Character can start jumping.\n" +
                     "Ignored if zero.")]
        [Editor(typeof(CurrentMapEdit), typeof(UITypeEditor))]
        [Category("Location")]
        public uint InitialTolerance
        {
            get => _iniTolerance;
            set
            {
                if (value != _iniTolerance)
                {
                    _iniTolerance = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private uint _iniTolerance = 5;

        [Description("The collection of the CustomRegions specifying the area within which action could be applied.\n" +
                     "Ignored if empty.")]
        [Editor(typeof(CustomRegionCollectionEditor), typeof(UITypeEditor))]
        [Category("Location")]
        [DisplayName("CustomRegions")]
        [NotifyParentProperty(true)]
        public CustomRegionCollection CustomRegionNames
        {
            get => _customRegions;
            set
            {
                if (_customRegions != value)
                {
                    _customRegions = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private CustomRegionCollection _customRegions = new CustomRegionCollection();

        [Description("The name of the Map where action could be applied.\n" +
                     "Ignored if empty.")]
        [Editor(typeof(CurrentMapEdit), typeof(UITypeEditor))]
        [Category("Location")]
        public string CurrentMap
        {
            get => _currentMap;
            set
            {
                if (value != _currentMap)
                {
                    _currentMap = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private string _currentMap;

        [Description("The name of the ingame region of the map where action could be applied.\n" +
                     "Ignored if '" + nameof(CurrentMap) + "' is empty.")]
        [Editor(typeof(CurrentRegionEdit), typeof(UITypeEditor))]
        [Category("Location")]
        public string CurrentRegion
        {
            get => _currentRegion;
            set
            {
                if (value != _currentRegion)
                {
                    _currentRegion = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private string _currentRegion;

        [Description("The interval of Z-coordinate within which the Character can reach the '" + nameof(InitialPosition) + "' and the current action can be applied.\n" +
                     "This condition is ignored when the '" + nameof(Range<float>.Min) + "' equals to '" + nameof(Range<float>.Max) + "'")]
        [Category("Location")]
        [Editor(typeof(ZRangeEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [DisplayName("ZRange")]
        public Range<float> ZRange
        {
            get => _zRange;
            set
            {
                if (!_zRange.Equals(value))
                {
                    _zRange = value.Clone();
                    NotifyPropertyChanged();
                }
            }
        }
        private Range<float> _zRange = new Range<float>();
        #endregion


        #region Target
        /// <summary>
        /// Точка, до которой необходимо допрыгнуть
        /// </summary>
        [Description("The position to jump to.")]
        [Editor(typeof(PositionEditor), typeof(UITypeEditor))]
        [Category("Destination")]
        public Vector3 DestinationPosition
        {
            get => _destinationPos;
            set
            {
                if (value != _destinationPos)
                {
                    _destinationPos = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private Vector3 _destinationPos = Vector3.Empty;

        [Description("The radius of upper hemisphere centered on '" + nameof(DestinationPosition) + "'.\n" +
                     "The character must be closer to the '" + nameof(DestinationPosition) + "' than the '" + nameof(DestinationRadius) + "' to complete action.\n" +
                     "The action will be continued if the character is farther from the '" + nameof(DestinationPosition) + "' than the specified '" + nameof(DestinationRadius) + "'.\n" +
                     "If value is zero then the action would be completed after executed once.")]
        [Category("Destination")]
        public uint DestinationRadius
        {
            get => _destRad;
            set
            {
                _destRad = value;
                _defaultDestR = value;
                NotifyPropertyChanged();
            }
        }
        private uint _destRad = 5;
        #endregion


        #region ManageCombatOptions
        [Description("Enable '" + nameof(IgnoreCombat) + "' profile value while playing action")]
        [Category("Manage Combat Options")]
        public bool IgnoreCombat
        {
            get => _ignoreCombat; set
            {
                if (_ignoreCombat != value)
                {
                    _ignoreCombat = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool _ignoreCombat = true;

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
        [Description("The default value of the '" + nameof(DestinationRadius) + "' for each new command '" + nameof(ExecutePowerExt) + "'.")]
        [Category("Default Options")]
        [XmlIgnore]
        public uint DefaultDestinationRadius
        {
            get => _defaultDestR;
            set
            {
                _defaultDestR = value;
                NotifyPropertyChanged();
            }
        }
        [NonSerialized]
        private static uint _defaultDestR;

        [Description("The offset of Z-coordinate from the '" + nameof(InitialPosition) + "'.\n" +
                     "When the new '" + nameof(Jumping) + "' command  will be added to the profile the '" + nameof(_zRange) + "' will be calculated as follows:\n" +
                     "Min = " + nameof(InitialPosition) + "." + nameof(Vector3.Z) + " - " + nameof(ZDeviation) + "\n" +
                     "Max = " + nameof(InitialPosition) + "." + nameof(Vector3.Z) + " + " + nameof(ZDeviation))]
        [Category("Default Options")]
        [DisplayName(nameof(ZRange) + " Radius")]
        [XmlIgnore]
        public uint ZDeviation
        {
            get => _zDev;
            set
            {
                _zDev = value;
                NotifyPropertyChanged();
            }
        }
        [NonSerialized]
        private static uint _zDev;

        [Category("Default Options")]
        [DisplayName("Waipoint detect radius. Minimum is 5")]
        [XmlIgnore]
        public uint DetectR
        {
            get => _detectR;
            set
            {
                _detectR = Math.Max(value, 5);
                NotifyPropertyChanged();
            }
        }
        [NonSerialized]
        private static uint _detectR = 10;
        #endregion




        public Jumping() {
            _actionIDstr = $"{GetType().Name}[{ActionID}]";
            _internalCheck = _internalCheckInitializer;
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
                bool debugInfo = EntityTools.Config.Logger.QuesterActions.DebugJumping;
                string currentMethodName = debugInfo ? string.Concat(_actionIDstr, '.', MethodBase.GetCurrentMethod()?.Name ?? nameof(Run)) : string.Empty;

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

                return d <= 25;
            }
        }

        public override ActionResult Run()
        {
            bool debugInfo = EntityTools.Config.Logger.QuesterActions.DebugJumping;
            string currentMethodName = debugInfo ? string.Concat(_actionIDstr, '.', MethodBase.GetCurrentMethod()?.Name ?? nameof(Run)) : string.Empty;

            if (!IntenalConditions)
            {
                if (debugInfo)
                    ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: {nameof(IntenalConditions)} failed. ActionResult={ActionResult.Fail}.");
                return ActionResult.Fail;
            }

            if (debugInfo)
                ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: starts");

            try
            {
                ApploachInitialPosition();

                ExecuteJumping();

                return CheckResult();
            }
            catch (Exception e)
            {
                ETLogger.WriteLine(LogType.Debug, 
                    string.Concat(_actionIDstr, 
                                       '.', 
                                       MethodBase.GetCurrentMethod()?.Name ?? nameof(Run), 
                                       "Catch an exception:\n", 
                                       e), 
                    true);
                NavigationHelper.StopNavigationCompletely();
                return ActionResult.Fail;
            }
        }

        private void ApploachInitialPosition()
        {
            bool debugInfo = EntityTools.Config.Logger.QuesterActions.DebugJumping;
            string currentMethodName = debugInfo ? string.Concat(_actionIDstr, '.', MethodBase.GetCurrentMethod()?.Name ?? nameof(ApploachInitialPosition)) : string.Empty;
            var pos = EntityManager.LocalPlayer.Location.Clone();
            var tolerance = Math.Max(InitialTolerance, 3);
            
            if (debugInfo)
                ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: Approach initial position at <{pos.X:N2}, {pos.Y:N2}, {pos.Z:N2}> (tolerance {tolerance})");

            Approach.PostionByDistance(InitialPosition, tolerance);

            NavigationHelper.StopNavigationCompletely();

            if (debugInfo)
                ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: Stand at {pos.Distance2DFromPlayer:N2} from initial position.");
        }

        private void ExecuteJumping()
        {
            bool debugInfo = EntityTools.Config.Logger.QuesterActions.DebugJumping;
            string currentMethodName = debugInfo ? string.Concat(_actionIDstr, '.', MethodBase.GetCurrentMethod()?.Name ?? nameof(ExecuteJumping)) : string.Empty;


            var iniPos = InitialPosition;
            var tarPos = DestinationPosition;
            var repeating = RepeatingJumping;
            var jumpingTime = Math.Max(JumpTime, 500);
            var movingTime = ForwardMovingTime;
            if (movingTime <= 0)
                movingTime = 600_000;

            if (tarPos.IsInYawFace)
            {
                tarPos.Face();
                var timeout = new Astral.Classes.Timeout(750);
                while (!_destinationPos.IsInYawFace && !timeout.IsTimedOut)
                {
                    Thread.Sleep(20);
                }
                Thread.Sleep(100);
            }
            if (debugInfo)
                ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: Turn to the '{nameof(DestinationPosition)}'.");

            var isJumpingSeries = repeating && (movingTime % jumpingTime) > 1;
            MyNW.Internals.Movements.SetMovementType(MyNW.Internals.Movements.MovementsType.Jump);
            if (debugInfo)
                ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: Start Jumping{(isJumpingSeries?"Series":"")}.");

            var delayBeforeMove = DelayBeforMove;
            if (delayBeforeMove > 0)
                Thread.Sleep(delayBeforeMove);

            MyNW.Internals.Movements.SetMovementType(MyNW.Internals.Movements.MovementsType.MoveForward);

            if (jumpingTime < movingTime)
            {
                var leftMovingTime = movingTime - jumpingTime;
                var movingTimeout = repeating 
                                  ? new Astral.Classes.Timeout(movingTime)
                                  : null;
                var touchdownStr = $"{currentMethodName}: Touchdown. {nameof(DestinationPosition)} at {DestinationPosition.Distance3DFromPlayer:N2} from Character.";
                do
                {
                    tarPos.Face();
                    jumpingTime = Math.Min(Math.Min(jumpingTime, leftMovingTime), 100);
                    MyNW.Internals.Movements.SetMovementType(MyNW.Internals.Movements.MovementsType.Jump);
                    Thread.Sleep(jumpingTime);
                    MyNW.Internals.Movements.UnsetMovementType(MyNW.Internals.Movements.MovementsType.Jump);
                    if (debugInfo)
                        ETLogger.WriteLine(LogType.Debug, touchdownStr);
                    leftMovingTime = leftMovingTime - jumpingTime;
                }
                while (repeating 
                       && !movingTimeout.IsTimedOut
                       && !IsInUpperHemisphere());
                if (!repeating)
                    Thread.Sleep(leftMovingTime);

                MyNW.Internals.Movements.UnsetMovementType(MyNW.Internals.Movements.MovementsType.MoveForward);
                
                if (debugInfo)
                    ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: Stop Jumping{(isJumpingSeries ? "Series" : "")} and Moving. {nameof(DestinationPosition)} at {DestinationPosition.Distance3DFromPlayer:N2} from Character.");
            }
            else
            {
                var timeout = new Astral.Classes.Timeout(movingTime);
                while (!timeout.IsTimedOut
                        && !IsInUpperHemisphere())
                {
                    tarPos.Face();
                    Thread.Sleep(50);
                }

                Thread.Sleep(movingTime);
                MyNW.Internals.Movements.UnsetMovementType(MyNW.Internals.Movements.MovementsType.MoveForward);
                if (debugInfo)
                    ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: Stop Moving. {nameof(DestinationPosition)} at {DestinationPosition.Distance3DFromPlayer:N2} from Character.");
                var leftMovingTime = jumpingTime - movingTime;
                if (leftMovingTime > 0)
                {
                    timeout.ChangeTime(leftMovingTime);
                    while (!timeout.IsTimedOut
                           && !IsInUpperHemisphere())
                    {
                        tarPos.Face();
                        Thread.Sleep(50);
                    }
                }
                MyNW.Internals.Movements.UnsetMovementType(MyNW.Internals.Movements.MovementsType.Jump);
                if (debugInfo)
                    ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: Stop Jumping. {nameof(DestinationPosition)} at {DestinationPosition.Distance3DFromPlayer:N2} from Character.");
            }
        }

#if false
        private void JumpingSeries()
        {
            bool debugInfo = EntityTools.Config.Logger.QuesterActions.DebugJumping;
            string currentMethodName = debugInfo ? string.Concat(_actionIDstr, '.', MethodBase.GetCurrentMethod()?.Name ?? nameof(ExecuteJumping)) : string.Empty;


            var iniPos = InitialPosition;
            var tarPos = DestinationPosition;
            var repeating = RepeatingJumping;
            var jumpingTime = Math.Max(JumpTime, 500);
            var movingTime = ForwardMovingTime;
            if (movingTime <= 0)
                movingTime = int.MaxValue;

            if (tarPos.IsInYawFace)
            {
                tarPos.Face();
                var timeout = new Astral.Classes.Timeout(750);
                while (!_destinationPos.IsInYawFace && !timeout.IsTimedOut)
                {
                    Thread.Sleep(20);
                }
                Thread.Sleep(100);
            }
            if (debugInfo)
                ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: Turn to the '{nameof(DestinationPosition)}'.");

            var isJumpingSeries = repeating && (movingTime % jumpingTime) > 1;
            MyNW.Internals.Movements.SetMovementType(MyNW.Internals.Movements.MovementsType.Jump);
            if (debugInfo)
                ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: Start Jumping{(isJumpingSeries ? "Series" : "")}.");

            var delayBeforeMove = DelayBeforMove;
            if (delayBeforeMove > 0)
                Thread.Sleep(delayBeforeMove);

            MyNW.Internals.Movements.SetMovementType(MyNW.Internals.Movements.MovementsType.MoveForward);

            var leftMovingTime = movingTime - jumpingTime;
            var movingTimeout = repeating
                              ? new Astral.Classes.Timeout(movingTime)
                              : null;
            var touchdownStr = $"{currentMethodName}: Touchdown. {nameof(DestinationPosition)} at {DestinationPosition.Distance3DFromPlayer:N2} from Character.";
            do
            {
                tarPos.Face();
                jumpingTime = Math.Min(Math.Min(jumpingTime, leftMovingTime), 100);
                MyNW.Internals.Movements.SetMovementType(MyNW.Internals.Movements.MovementsType.Jump);
                Thread.Sleep(jumpingTime);
                MyNW.Internals.Movements.UnsetMovementType(MyNW.Internals.Movements.MovementsType.Jump);
                if (debugInfo)
                    ETLogger.WriteLine(LogType.Debug, touchdownStr);
                leftMovingTime = leftMovingTime - jumpingTime;
            }
            while (repeating
                   && !movingTimeout.IsTimedOut
                   && !IsInUpperHemisphere());
            if (!repeating)
                Thread.Sleep(leftMovingTime);

            MyNW.Internals.Movements.UnsetMovementType(MyNW.Internals.Movements.MovementsType.MoveForward);

            if (debugInfo)
                ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: Stop Jumping{(isJumpingSeries ? "Series" : "")} and Moving. {nameof(DestinationPosition)} at {DestinationPosition.Distance3DFromPlayer:N2} from Character.");
        } 
#endif
        //TODO Добавить условия прерывания движения и деактивации прыжка

        private ActionResult CheckResult()
        {
            bool debugInfo = EntityTools.Config.Logger.QuesterActions.DebugJumping;
            string currentMethodName = debugInfo ? string.Concat(_actionIDstr, '.', MethodBase.GetCurrentMethod()?.Name ?? nameof(CheckResult)) : string.Empty;

            var rad = DestinationRadius;
            if (rad > 0)
            {
                rad = Math.Max(rad, 5);
                rad *= rad;
                var pos = EntityManager.LocalPlayer.Location.Clone();
                var dist2 = MapperHelper.SquareDistance3D(pos, DestinationPosition);
                if (dist2 > rad)
                {
                    bool intCheck = IntenalConditions;
                    var actRes = intCheck && RepeatingJumping
                        ? ActionResult.Running
                        : ActionResult.Fail;
                    if (debugInfo)
                        ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: Current player position: <{pos.X:N2}, {pos.Y:N2}, {pos.Z:N2}>. Distance to {nameof(DestinationPosition)} is {Math.Sqrt(dist2):N2}.\n" +
                                                          $"InternalConditions '{intCheck}' => {actRes}");
                    // Расстояние от персонажа до TargetPosition больше TargetRadius,
                    // поэтому выполнение команды нужно повторить
                    return actRes;
                }
                if (debugInfo)
                    ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: Current player position: <{pos.X:N2}, {pos.Y:N2}, {pos.Z:N2}>. Distance to {nameof(DestinationPosition)} is {Math.Sqrt(dist2):N2} => {ActionResult.Completed}");
            }
            else if (debugInfo)
                ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: ActionResult => {ActionResult.Completed}");

            return ActionResult.Completed;
        }

        private bool IsInUpperHemisphere()
        {
            bool debugInfo = EntityTools.Config.Logger.QuesterActions.DebugJumping;
            string currentMethodName = debugInfo ? string.Concat(_actionIDstr, '.', MethodBase.GetCurrentMethod()?.Name ?? nameof(ExecuteJumping)) : string.Empty;

            var pos = DestinationPosition;
            var playerPos = EntityManager.LocalPlayer.Location.Clone();

            bool isIn = playerPos.Z > pos.Z
                   && playerPos.Distance3D(pos) < DestinationRadius;

            if (debugInfo)
                ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: {isIn}");
            return isIn;
        }

        public override string ActionLabel => nameof(Jumping);
        private string _actionIDstr;

        public override string InternalDisplayName => string.Empty;

        public override bool UseHotSpots => false;
        protected override bool IntenalConditions => _internalCheck();
        protected override Vector3 InternalDestination
        {
            get
            {
                if (!_internalCheck())
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
                if (!DestinationPosition.IsValid)
                    return new ActionValidity($"{nameof(DestinationPosition)} is invalid");
                if (_zRange.IsValid && _zRange.Outside(iniPos.Z))
                    return new ActionValidity($"{nameof(InitialPosition)} is out of {nameof(ZRange)}");
                return new ActionValidity();
            }
        }

        public override void GatherInfos()
        {
            var detectR = DetectR;
            var player = EntityManager.LocalPlayer;
            if (player.IsValid && !player.IsLoading
                && TargetSelectForm.GUIRequest("Set InitialPosition",
                                               "Move the Character to the position from where the Jumping should start and press F12")
                   == DialogResult.OK)
            {
                var iniPos = player.Location;
                if (iniPos.IsValid)
                {
                    var node = AstralAccessors.Quester.Core.CurrentProfile.CurrentMesh.ClosestNode(iniPos.X, iniPos.Y, iniPos.Z, out double dist, false);

                    if (node is null
                        || dist > detectR)
                    {
                        InitialPosition = iniPos;
                    }
                    else iniPos = InitialPosition = node.Position;

                    if (TargetSelectForm.GUIRequest("Set TargetPosition",
                            "Move the Character to the position where it need to jump in and press F12") ==
                        DialogResult.OK)
                    {
                        var tarPos = EntityManager.LocalPlayer.Location;
                        if (tarPos.IsValid)
                        {
                            node = AstralAccessors.Quester.Core.CurrentProfile.CurrentMesh.ClosestNode(tarPos.X, tarPos.Y, tarPos.Z,
                               out dist, false);

                            if (node is null
                                || dist > detectR)
                            {
                                DestinationPosition = tarPos;
                            }
                            else DestinationPosition = node.Position;

                            CurrentMap = player.MapState.MapName;
                            CurrentRegion = player.RegionInternalName;

                            var zi = iniPos.Z;
                            var zDev = ZDeviation;
                            if (zDev > 0)
                            {
                                _zRange.Min = (float)Math.Floor(zi - zDev);
                                _zRange.Max = (float)Math.Round(zi + zDev);
                            }
                        }
                    }
                }
            }
        }

        public override void InternalReset()
        {
            _internalCheck = _internalCheckInitializer;
        }

        public override void OnMapDraw(GraphicsNW graphics)
        {
            var iniPos = InitialPosition;
            var tarPos = DestinationPosition;
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
        private Func<bool> _internalCheck;

        /// <summary>
        /// Функтор, инициализирующий предикат проверки <see cref="IntenalConditions"/>
        /// </summary>
        /// <returns></returns>
        private bool _internalCheckInitializer()
        {
            Func<bool> check;

            var iniPos = InitialPosition;
            var tarPos = DestinationPosition;
            if (!tarPos.IsValid
                || _zRange.IsValid && _zRange.Outside(EntityManager.LocalPlayer.Location.Z))
            {
                _internalCheck = () => false;
                return false;
            }

            var crSet = CustomRegionNames;
            if (crSet.Count > 0)
            {
                if (_zRange.IsValid)
                {
                    var map = CurrentMap;
                    if (string.IsNullOrEmpty(map))
                    {
                        check = _internalCheck_RegionSet_zRange;
                    }
                    else
                    {
                        var reg = CurrentRegion;
                        if (string.IsNullOrEmpty(reg))
                        {
                            check = _internalCheck_Map_RegionSet_zRange;
                        }
                        else
                        {
                            check = _internalCheck_MapRegion_RegionSet_zRange;
                        }
                    }
                }
                else
                {
                    var map = CurrentMap;
                    if (string.IsNullOrEmpty(map))
                    {
                        check = _internalCheck_RegionSet;
                    }
                    else
                    {
                        var reg = CurrentRegion;
                        if (string.IsNullOrEmpty(reg))
                        {
                            check = _internalCheck_Map_RegionSet;
                        }
                        else
                        {
                            check = _internalCheck_MapRegion_RegionSet;
                        }
                    }
                }
            }
            else
            {
                if (_zRange.IsValid)
                {
                    var map = CurrentMap;
                    if (string.IsNullOrEmpty(map))
                    {
                        check = _internalCheck_zRange;
                    }
                    else
                    {
                        var reg = CurrentRegion;
                        if (string.IsNullOrEmpty(reg))
                        {
                            check = _internalCheck_Map_zRange;
                        }
                        else
                        {
                            check = _internalCheck_MapRegion_zRange;
                        }
                    }
                }
                else
                {
                    var map = CurrentMap;
                    if (string.IsNullOrEmpty(map))
                    {
                        check = _internalCheck_true;
                    }
                    else
                    {
                        var reg = CurrentRegion;
                        if (string.IsNullOrEmpty(reg))
                        {
                            check = _internalCheck_Map;
                        }
                        else
                        {
                            check = _internalCheck_MapRegion;
                        }
                    }
                }
            }

            _internalCheck = check;
            return _internalCheck();
        }

        private bool _internalCheck_true()
        {
            return true;
        }
        private bool _internalCheck_RegionSet()
        {
            var crSet = CustomRegionNames;
            var player = EntityManager.LocalPlayer;
            var location = player.Location;
            return crSet.Within(location);
        }
        private bool _internalCheck_RegionSet_zRange()
        {
            var crSet = CustomRegionNames;
            var player = EntityManager.LocalPlayer;
            var location = player.Location;
            return crSet.Within(location)
                   && _zRange.Within(location.Z);
        }
        private bool _internalCheck_MapRegion_RegionSet_zRange()
        {
            var map = CurrentMap;
            var reg = CurrentRegion;
            var crSet = CustomRegionNames;
            var player = EntityManager.LocalPlayer;
            var location = player.Location;
            return player.CurrentZoneMapInfo.MapName.Equals(map, StringComparison.Ordinal)
                   && player.RegionInternalName.Equals(reg, StringComparison.Ordinal)
                   && crSet.Within(location)
                   && _zRange.Within(location.Z);
        }
        private bool _internalCheck_Map_RegionSet_zRange()
        {
            var map = CurrentMap;
            var crSet = CustomRegionNames;
            var player = EntityManager.LocalPlayer;
            var location = player.Location;
            return player.CurrentZoneMapInfo.MapName.Equals(map, StringComparison.Ordinal)
                   && string.IsNullOrEmpty(player.RegionInternalName)
                   && crSet.Within(location)
                   && _zRange.Within(location.Z);
        }
        private bool _internalCheck_MapRegion_RegionSet()
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
        private bool _internalCheck_Map_RegionSet()
        {
            var map = CurrentMap;
            var crSet = CustomRegionNames;
            var player = EntityManager.LocalPlayer;
            var location = player.Location;
            return player.CurrentZoneMapInfo.MapName.Equals(map, StringComparison.Ordinal)
                   && string.IsNullOrEmpty(player.RegionInternalName)
                   && crSet.Within(location);
        }
        private bool _internalCheck_Map_zRange()
        {
            var map = CurrentMap;
            var player = EntityManager.LocalPlayer;
            var location = player.Location;
            return player.CurrentZoneMapInfo.MapName.Equals(map, StringComparison.Ordinal)
                   && string.IsNullOrEmpty(player.RegionInternalName)
                   && _zRange.Within(location.Z);
        }
        private bool _internalCheck_MapRegion_zRange()
        {
            var map = CurrentMap;
            var reg = CurrentRegion;
            var player = EntityManager.LocalPlayer;
            var location = player.Location;
            return player.CurrentZoneMapInfo.MapName.Equals(map, StringComparison.Ordinal)
                   && player.RegionInternalName.Equals(reg, StringComparison.Ordinal)
                   && _zRange.Within(location.Z);
        }
        private bool _internalCheck_MapRegion()
        {
            var map = CurrentMap;
            var reg = CurrentRegion;
            var player = EntityManager.LocalPlayer;
            return player.CurrentZoneMapInfo.MapName.Equals(map, StringComparison.Ordinal)
                   && player.RegionInternalName.Equals(reg, StringComparison.Ordinal);
        }
        private bool _internalCheck_Map()
        {
            var map = CurrentMap;
            var player = EntityManager.LocalPlayer;
            return player.CurrentZoneMapInfo.MapName.Equals(map, StringComparison.Ordinal)
                   && string.IsNullOrEmpty(player.RegionInternalName);
        }
        private bool _internalCheck_zRange()
        {
            var player = EntityManager.LocalPlayer;
            var location = player.Location;
            return _zRange.Within(location.Z);
        }
        #endregion
    }
}
