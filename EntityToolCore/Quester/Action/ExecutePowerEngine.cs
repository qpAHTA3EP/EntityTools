#define DEBUG_POWER

using AcTp0Tools;
using Astral.Logic.Classes.Map;
using Astral.Logic.NW;
using EntityCore.Forms;
using EntityTools;
using EntityTools.Core.Interfaces;
using EntityTools.Patches.Mapper;
using EntityTools.Quester.Actions;
using EntityCore.Tools.Navigation;
using MyNW.Classes;
using MyNW.Internals;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using static Astral.Quester.Classes.Action;

namespace EntityCore.Quester.Action
{
    internal class ExecutePowerEngine : IQuesterActionEngine
    {
        private ExecutePowerExt @this;
        private string label;
        private string actionIDstr;

        internal ExecutePowerEngine(ExecutePowerExt exPow)
        {
            InternalRebase(exPow);
            ETLogger.WriteLine(LogType.Debug, $"{actionIDstr} initialized: {ActionLabel}");
        }
        ~ExecutePowerEngine()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (@this != null)
            {
                power = null;
                @this.Unbind();
                @this = null;
            }
        }

        public bool Rebase(Astral.Quester.Classes.Action action)
        {
            if (action is null)
                return false;
            if (ReferenceEquals(action, @this))
                return true;
            if (action is ExecutePowerExt exPow)
            {
                InternalRebase(exPow);
                ETLogger.WriteLine(LogType.Debug, $"{actionIDstr} reinitialized");
                return true;
            }

            string debugStr = string.Concat("Rebase failed. ", action.GetType().Name, '[', action.ActionID, "] can't be cast to '" + nameof(ExecutePowerExt) + '\'');
            ETLogger.WriteLine(LogType.Error, debugStr);
            throw new InvalidCastException(debugStr);
        }

        private bool InternalRebase(ExecutePowerExt exPow)
        {
            // Убираем привязку к старой команде
            @this?.Unbind();

            @this = exPow;
            power = null;
            label = String.Empty;
            @this.PropertyChanged += OnPropertyChanged;

            internalCheck = initialize_internalCheck;
            actionIDstr = string.Concat(@this.GetType().Name, '[', @this.ActionID, ']');

            @this.Bind(this);

            return true;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(sender as Astral.Quester.Classes.Action, e.PropertyName);
        }

        public void OnPropertyChanged(Astral.Quester.Classes.Action sender, string propertyName)
        {
            if (propertyName == nameof(@this.PowerId))
            {
                power = null;
                label = String.Empty;
            }
            internalCheck = initialize_internalCheck;
        }

        public bool NeedToRun
        {
            get
            {
                var d = MapperHelper.SquareDistance2D(EntityManager.LocalPlayer.Location, @this.InitialPosition);

                if (d > 25 && @this.IgnoreCombat)
                {
                    AstralAccessors.Quester.FSM.States.Combat.SetIgnoreCombat(true, -1, 5000);
                }
                else AstralAccessors.Quester.FSM.States.Combat.SetIgnoreCombat(false);

                return d <= 25 && GetCurrentPower()?.IsValid == true;
            }
        }

        public ActionResult Run()
        {
            bool debugInfo = EntityTools.EntityTools.Config.Logger.QuesterActions.DebugExecutePowerExt;

#if DEBUG_POWER
            if(debugInfo)
                ETLogger.WriteLine(LogType.Debug, $"{actionIDstr}::Run() starts");
#endif
            NavigationHelper.StopNavigationCompletly();

            var currentPower = GetCurrentPower();

            if (currentPower is null)
            {
#if DEBUG_POWER
                if (debugInfo)
                    ETLogger.WriteLine(LogType.Debug, $"{actionIDstr}: Fail to get Power '{@this.PowerId}' by 'PowerId'");
#endif
                return ActionResult.Fail;
            }
            var entActivatedPower = currentPower.EntGetActivatedPower();
            var powerDef = entActivatedPower.EntGetActivatedPower().EffectivePowerDef();

            var tarPos = @this.TargetPosition;
            if (tarPos.IsInYawFace)
            {
                tarPos.Face();
                var timeout = new Astral.Classes.Timeout(750);
                while (!tarPos.IsInYawFace && !timeout.IsTimedOut)
                {
                    Thread.Sleep(20);
                }
                Thread.Sleep(100);
            }

            int castingTime = Math.Max(Powers.getEffectiveTimeCharge(powerDef), 500);
            var castingTimeout = new Astral.Classes.Timeout(castingTime);

            bool powerActivated = false;
            try
            {
                tarPos.Face();
#if DEBUG_POWER
                if (debugInfo)
                    ETLogger.WriteLine(LogType.Debug, $"{actionIDstr}: Activating ExecPower '{currentPower.PowerDef.InternalName}' on target <{tarPos.X:N2}, {tarPos.Y:N2}, {tarPos.Z:N2}>");
#endif
                Powers.ExecPower(currentPower, tarPos, true);
                powerActivated = true;
#if DEBUG_POWER
                if (debugInfo)
                    ETLogger.WriteLine(LogType.Debug, $"{actionIDstr}: Waiting casting time ({castingTime} ms)");
#endif
                while (!castingTimeout.IsTimedOut && !Astral.Controllers.AOECheck.PlayerIsInAOE)
                {
                    if (currentPower.UseCharges() && !currentPower.ChargeAvailable() || currentPower.IsActive)
                    {
                        break;
                    }
                    Thread.Sleep(20);
                }
#if DEBUG_POWER
                if (debugInfo)
                    ETLogger.WriteLine(LogType.Debug, $"{actionIDstr}: Pause after power activation");
#endif
                var pause = Math.Max(@this.Pause, 500);
                Thread.Sleep(pause);

                // Проверяем необходимость и возможность повторения команды
                var rad = @this.TargetRadius;
                if (rad > 0)
                {
                    rad = Math.Max(rad, 5);
                    rad *= rad;
                    var pos = EntityManager.LocalPlayer.Location.Clone();
                    var dist2 = MapperHelper.SquareDistance3D(pos, @this.TargetPosition);
                    if (dist2 > rad)
                    {
                        bool intCheck = internalCheck();
                        var actRes = intCheck
                            ? ActionResult.Running
                            : ActionResult.Fail;
#if DEBUG_POWER
                        if (debugInfo)
                            ETLogger.WriteLine(LogType.Debug, $"{actionIDstr}: Current player position: <{pos.X:N2}, {pos.Y:N2}, {pos.Z:N2}>. Distance to {nameof(@this.TargetPosition)} is {Math.Sqrt(dist2)}.\n" +
                                                              $"InternalConditions '{intCheck}' => {actRes}");
#endif
                        // Расстояние от персонажа до TargetPosition больше TargetRadius,
                        // поэтому выполнение команды нужно повторить
                        return actRes;
                    }
#if DEBUG_POWER
                    if (debugInfo)
                        ETLogger.WriteLine(LogType.Debug, $"{actionIDstr}: Current player position: <{pos.X:N2}, {pos.Y:N2}, {pos.Z:N2}>. Distance to {nameof(@this.TargetPosition)} is {Math.Sqrt(dist2)} => {ActionResult.Completed}");
#endif
                }
            }
#if DEBUG_POWER
            catch (Exception e)
            {
                if (debugInfo)
                    ETLogger.WriteLine(LogType.Error, $"{actionIDstr}: Catch an exception trying activate power '{currentPower.PowerDef.InternalName}' \n{e.Message}\n{e.StackTrace}");
                return ActionResult.Fail;
            }
#endif
            finally
            {
                if (powerActivated)
                {
#if DEBUG_POWER
                    if (debugInfo)
                        ETLogger.WriteLine(LogType.Debug, $"{actionIDstr}: Deactivating ExecPower '{currentPower.PowerDef.InternalName}'");
#endif
                    try
                    {
                        Powers.ExecPower(currentPower, tarPos, false);
                    }
                    catch (Exception e)
                    {
#if DEBUG_POWER
                        if (debugInfo)
                            ETLogger.WriteLine(LogType.Debug, $"{actionIDstr}: Catch an exception trying deactivate power '{currentPower.PowerDef.InternalName}'\n{e.Message}\n{e.StackTrace}");
#endif
                    } 
                }
            }
            return ActionResult.Completed;
        }

        public string ActionLabel
        {
            get
            {
                if (string.IsNullOrEmpty(label))
                {
                    var id = @this.PowerId;
                    if(string.IsNullOrEmpty(id))
                        label = @this.GetType().Name;
                    else label = $@"{@this.GetType().Name}: [{id}]";
                }
                return label;
            }
        }

        public bool InternalConditions => internalCheck();

        public ActionValidity InternalValidity
        {
            get
            {
                var iniPos = @this.InitialPosition;
                if (!iniPos.IsValid)
                    return new ActionValidity($"{nameof(@this.InitialPosition)} is invalid");
                if (!@this.TargetPosition.IsValid)
                    return new ActionValidity($"{nameof(@this.TargetPosition)} is invalid");
                if (string.IsNullOrEmpty(@this.PowerId))
                    return new ActionValidity($"{nameof(@this.PowerId)} is invalid");
                var zRange = @this.ZRange;
                if(zRange.IsValid && zRange.Outside(iniPos.Z))
                    return new ActionValidity($"{nameof(@this.InitialPosition)} is out of {nameof(@this.ZRange)}");
                return new ActionValidity();
            }
        }

        public bool UseHotSpots => false;

        public Vector3 InternalDestination
        {
            get
            {
                if(!internalCheck())
                    return Vector3.Empty;

                var d = MapperHelper.SquareDistance2D(EntityManager.LocalPlayer.Location, @this.InitialPosition);
                if (d > 25)
                    return @this.InitialPosition;
                
                return Vector3.Empty;
            }
        }

        public void InternalReset()
        {
            power = null;
            internalCheck = initialize_internalCheck;
        }

        public void GatherInfos()
        {
            if (TargetSelectForm.GUIRequest("Set InitialPosition", 
                "Move the Character to the position where the Power could be activated and press F12") == 
                DialogResult.OK)
            {
                var player = EntityManager.LocalPlayer;
                var iniPos = player.Location;
                if (iniPos.IsValid)
                {
                    var node = AstralAccessors.Quester.Core.Meshes.ClosestNode(iniPos.X, iniPos.Y, iniPos.Z, out double dist, false);

                    if (node is null
                        || dist > 10)
                    {
                        @this.InitialPosition = iniPos;
                    }
                    else iniPos = @this.InitialPosition = node.Position;

                    if (TargetSelectForm.GUIRequest("Set TargetPosition",
                            "Move the Character to the position on which the Power should be target before activated and press F12") ==
                        DialogResult.OK)
                    {
                        var tarPos = EntityManager.LocalPlayer.Location;
                        if (tarPos.IsValid)
                        {
                             node = AstralAccessors.Quester.Core.Meshes.ClosestNode(tarPos.X, tarPos.Y, tarPos.Z,
                                out dist, false);

                            if (node is null
                                || dist > 10)
                            {
                                @this.TargetPosition = tarPos;
                            }
                            else tarPos = @this.TargetPosition = node.Position;

                            @this.CurrentMap = player.MapState.MapName;
                            @this.CurrentRegion = player.RegionInternalName;

#if true
                            var zi = iniPos.Z;
                            var zRange = @this.ZRange;
                            var zDev = @this.ZDeviation;
                            if (zDev > 0)
                            {
                                zRange.Min = (float)Math.Floor(zi - zDev);
                                zRange.Max = (float)Math.Round(zi + zDev);
                                return;
                            }
#else
                            var zRange = @this.ZRange;

                            if (zRange.IsValid && zRange.Within(iniPos.Z))
                                return;

                            var zi = iniPos.Z;
                            var zt = tarPos.Z;
                            var dz = zt - zi;

                            var zDev = @this.ZDeviation;
                            if (zDev > 0)
                            {
                                zRange.Min = (float)Math.Floor(zi - zDev);
                                zRange.Max = (float)Math.Round(zi + zDev);
                                return;
                            }

                            if (dz > 0)
                            {
                                zRange.Min = (float)Math.Floor(Math.Min(zRange.Min, zi - 10));
                                zRange.Max = (float)Math.Round(Math.Max(zRange.Max, zi + 1 + dz / 2));
                            }
                            else
                            {
                                zRange.Min = (float)Math.Round(Math.Min(zRange.Min, zi - 1 + dz / 2));
                                zRange.Max = (float)Math.Round(Math.Max(zRange.Max, zi + 10));
                            } 
#endif
                        }
                    }
                }
            }
        }

        public void OnMapDraw(GraphicsNW graphics)
        {
            var iniPos = @this.InitialPosition;
            var tarPos = @this.TargetPosition;
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
                else if(tarPos.IsValid)
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

        #region CurrentPower
        private int attachedGameProcessId;
        private uint characterContainerId;
        private uint powerId;
        private Power power;
        private Power GetCurrentPower()
        {
            var player = EntityManager.LocalPlayer;
            var processId = Astral.API.AttachedGameProcess.Id;
            var powId = @this.PowerId;

            if (!(attachedGameProcessId == processId
                  && characterContainerId == player.ContainerId
                  && power != null
                  && (power.PowerId == powerId
                      || string.Equals(power.PowerDef.InternalName, powId, StringComparison.Ordinal)
                      || string.Equals(power.EffectivePowerDef().InternalName, powId, StringComparison.Ordinal))))
            {
                power = Powers.GetPowerByInternalName(powId);
                if (power != null)
                {
                    powerId = power.PowerId;
                    label = string.Empty;
                    attachedGameProcessId = processId;
                    characterContainerId = player.ContainerId;
                }
                else
                {
                    powerId = 0;
                    label = string.Empty;
                    attachedGameProcessId = 0;
                    characterContainerId = 0;
                }
            }
            return power;
        }
        #endregion

        #region internalCheck
        /// <summary>
        /// Функтор проверки <see cref="InternalConditions"/>
        /// </summary>
        private Func<bool> internalCheck;

        /// <summary>
        /// Функтор инициализирующий предикат проверки <see cref="InternalConditions"/>
        /// </summary>
        /// <returns></returns>
        private bool initialize_internalCheck()
        {
            Func<bool> check;

            var iniPos = @this.InitialPosition;
            var tarPos = @this.TargetPosition;
            var zRange = @this.ZRange;
            var powId = @this.PowerId;
            if (!iniPos.IsValid || !tarPos.IsValid || string.IsNullOrEmpty(powId) 
                || zRange.IsValid && zRange.Outside(EntityManager.LocalPlayer.Location.Z))
            {
                internalCheck = () => false;
                return false;
            }

            var crSet = @this.CustomRegionNames;
            if (crSet.Count > 0)
            {
                if (zRange.IsValid)
                {
                    var map = @this.CurrentMap;
                    if (string.IsNullOrEmpty(map))
                    {
                        check = internalCheck_RegionSet_zRange;
                    }
                    else
                    {
                        var reg = @this.CurrentRegion;
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
                    var map = @this.CurrentMap;
                    if (string.IsNullOrEmpty(map))
                    {
                        check = internalCheck_RegionSet;
                    }
                    else
                    {
                        var reg = @this.CurrentRegion;
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
                    var map = @this.CurrentMap;
                    if (string.IsNullOrEmpty(map))
                    {
                        check = internalCheck_zRange;
                    }
                    else
                    {
                        var reg = @this.CurrentRegion;
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
                    var map = @this.CurrentMap;
                    if (string.IsNullOrEmpty(map))
                    {
                        check = internalCheck_true;
                    }
                    else
                    {
                        var reg = @this.CurrentRegion;
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
            var crSet = @this.CustomRegionNames;
            var player = EntityManager.LocalPlayer;
            var location = player.Location;
            return crSet.Within(location);
        }
        private bool internalCheck_RegionSet_zRange()
        {
            var zRange = @this.ZRange;
            var crSet = @this.CustomRegionNames;
            var player = EntityManager.LocalPlayer;
            var location = player.Location;
            return crSet.Within(location)
                   && zRange.Within(location.Z);
        }
        private bool internalCheck_MapRegion_RegionSet_zRange()
        {
            var map = @this.CurrentMap;
            var reg = @this.CurrentRegion;
            var zRange = @this.ZRange;
            var crSet = @this.CustomRegionNames;
            var player = EntityManager.LocalPlayer;
            var location = player.Location;
            return player.CurrentZoneMapInfo.MapName.Equals(map, StringComparison.Ordinal)
                   && player.RegionInternalName.Equals(reg, StringComparison.Ordinal)
                   && crSet.Within(location)
                   && zRange.Within(location.Z);
        }
        private bool internalCheck_Map_RegionSet_zRange()
        {
            var map = @this.CurrentMap;
            var zRange = @this.ZRange;
            var crSet = @this.CustomRegionNames;
            var player = EntityManager.LocalPlayer;
            var location = player.Location;
            return player.CurrentZoneMapInfo.MapName.Equals(map, StringComparison.Ordinal)
                   && string.IsNullOrEmpty(player.RegionInternalName)
                   && crSet.Within(location)
                   && zRange.Within(location.Z);
        }
        private bool internalCheck_MapRegion_RegionSet()
        {
            var map = @this.CurrentMap;
            var reg = @this.CurrentRegion;
            var crSet = @this.CustomRegionNames;
            var player = EntityManager.LocalPlayer;
            var location = player.Location;
            return player.CurrentZoneMapInfo.MapName.Equals(map, StringComparison.Ordinal)
                   && player.RegionInternalName.Equals(reg, StringComparison.Ordinal)
                   && crSet.Within(location);
        }
        private bool internalCheck_Map_RegionSet()
        {
            var map = @this.CurrentMap;
            var crSet = @this.CustomRegionNames;
            var player = EntityManager.LocalPlayer;
            var location = player.Location;
            return player.CurrentZoneMapInfo.MapName.Equals(map, StringComparison.Ordinal)
                   && string.IsNullOrEmpty(player.RegionInternalName)
                   && crSet.Within(location);
        }
        private bool internalCheck_Map_zRange()
        {
            var map = @this.CurrentMap;
            var zRange = @this.ZRange;
            var player = EntityManager.LocalPlayer;
            var location = player.Location;
            return player.CurrentZoneMapInfo.MapName.Equals(map, StringComparison.Ordinal)
                   && string.IsNullOrEmpty(player.RegionInternalName)
                   && zRange.Within(location.Z);
        }
        private bool internalCheck_MapRegion_zRange()
        {
            var map = @this.CurrentMap;
            var reg = @this.CurrentRegion;
            var zRange = @this.ZRange;
            var player = EntityManager.LocalPlayer;
            var location = player.Location;
            return player.CurrentZoneMapInfo.MapName.Equals(map, StringComparison.Ordinal)
                   && player.RegionInternalName.Equals(reg, StringComparison.Ordinal)
                   && zRange.Within(location.Z);
        }
        private bool internalCheck_MapRegion()
        {
            var map = @this.CurrentMap;
            var reg = @this.CurrentRegion;
            var player = EntityManager.LocalPlayer;
            return player.CurrentZoneMapInfo.MapName.Equals(map, StringComparison.Ordinal)
                   && player.RegionInternalName.Equals(reg, StringComparison.Ordinal);
        }
        private bool internalCheck_Map()
        {
            var map = @this.CurrentMap;
            var player = EntityManager.LocalPlayer;
            return player.CurrentZoneMapInfo.MapName.Equals(map, StringComparison.Ordinal)
                   && string.IsNullOrEmpty(player.RegionInternalName);
        }
        private bool internalCheck_zRange()
        {
            var zRange = @this.ZRange;
            var player = EntityManager.LocalPlayer;
            var location = player.Location;
            return zRange.Within(location.Z);
        }
        #endregion
    }
}
