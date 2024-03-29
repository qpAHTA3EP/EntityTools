﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using ACTP0Tools;
using Astral.Logic.Classes.Map;
using EntityTools.Core.Interfaces;
using EntityTools.Editors;
using EntityTools.Forms;
using EntityTools.Patches.Mapper;
using EntityTools.Tools.Navigation;
using EntityTools.Tools.Powers;
using MyNW.Classes;
using MyNW.Internals;
using static Astral.Quester.Classes.Action;

namespace EntityTools.Quester.Actions.Engines
{
    internal class ExecutePowerEngine : IQuesterActionEngine
    {
        private ExecutePowerExt @this;
        private string label;
        private string actionIDstr;
        private readonly PowerCache powerCache = new PowerCache(string.Empty);

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
                @this.Unbind();
                @this = null;
            }
            powerCache.Reset();
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
            powerCache.Reset(@this.PowerId);
            label = string.Empty;
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
                powerCache.Reset(@this.PowerId);
                label = string.Empty;
            }
            internalCheck = initialize_internalCheck;
        }

        public bool NeedToRun
        {
            get
            {
                bool debugInfo = global::EntityTools.EntityTools.Config.Logger.QuesterActions.DebugExecutePowerExt;
                string currentMethodName = debugInfo ? string.Concat(actionIDstr, '.', MethodBase.GetCurrentMethod()?.Name ?? nameof(Run)) : string.Empty;

                if (!InternalConditions)
                {
                    // Чтобы прервать цикл обработки команды нужно, чтобы Engine перешел к обработке Run()
                    if (debugInfo)
                        ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: {nameof(InternalConditions)} failed.");
                    return true;
                }

                var d = MapperHelper.SquareDistance2D(EntityManager.LocalPlayer.Location, @this.InitialPosition);

                if (d > 25 
                    && @this.IgnoreCombat
                    && CheckingIgnoreCombatCondition())
                {
                    AstralAccessors.Quester.FSM.States.Combat.SetIgnoreCombat(true, @this.IgnoreCombatMinHP, 5_000);
                    if (debugInfo)
                        ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: Set ignore combat.");
                    return false;
                }
                AstralAccessors.Quester.FSM.States.Combat.SetIgnoreCombat(false);

                return d <= 25 && powerCache.GetPower() != null;
            }
        }

        public ActionResult Run()
        {
            bool debugInfo = global::EntityTools.EntityTools.Config.Logger.QuesterActions.DebugExecutePowerExt;
            string currentMethodName = debugInfo ? string.Concat(actionIDstr, '.', MethodBase.GetCurrentMethod()?.Name ?? nameof(Run)) : string.Empty;


            if (!InternalConditions)
            {
                if (debugInfo)
                    ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: {nameof(InternalConditions)} failed. ActionResult={ActionResult.Fail}.");
                return ActionResult.Fail;
            }


            if (debugInfo)
                ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: starts");
            NavigationHelper.StopNavigationCompletely();

            var currentPower = powerCache.GetPower();

            if (currentPower is null)
            {
                if (debugInfo)
                    ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: Fail to get Power '{@this.PowerId}' by '" + nameof(@this.PowerId) + "'");
                return ActionResult.Fail;
            }

            if (debugInfo)
                ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: Activating Power '{@this.PowerId}'");

            var powResult = currentPower.ExecutePower(@this.TargetPosition, @this.CastingTime, @this.Pause, 0, false, debugInfo);

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
                    if (debugInfo)
                        ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: Current player position: <{pos.X:N2}, {pos.Y:N2}, {pos.Z:N2}>. Distance to {nameof(@this.TargetPosition)} is {Math.Sqrt(dist2)}.\n" +
                                                          $"InternalConditions '{intCheck}' => {actRes}");
                    // Расстояние от персонажа до TargetPosition больше TargetRadius,
                    // поэтому выполнение команды нужно повторить
                    return actRes;
                }
                if (debugInfo)
                    ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: Current player position: <{pos.X:N2}, {pos.Y:N2}, {pos.Z:N2}>. Distance to {nameof(@this.TargetPosition)} is {Math.Sqrt(dist2)} => {ActionResult.Completed}");
            }
            else if (debugInfo)
                ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: ActionResult => {ActionResult.Completed}");
            
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
            powerCache.Reset(@this.PowerId);
            internalCheck = initialize_internalCheck;
        }

        public void GatherInfos()
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
                            else @this.TargetPosition = node.Position;

                            @this.CurrentMap = player.MapState.MapName;
                            @this.CurrentRegion = player.RegionInternalName;

                            var zi = iniPos.Z;
                            var zRange = @this.ZRange;
                            var zDev = @this.ZDeviation;
                            if (zDev > 0)
                            {
                                zRange.Min = (float)Math.Floor(zi - zDev);
                                zRange.Max = (float)Math.Round(zi + zDev);
                            }
                            var pwId = PowerIdEditor.GetPowerId(@this.PowerId);
                            if (string.IsNullOrEmpty(pwId))
                            {
                                @this.PowerId = PowerIdEditor.GetPowerId(@this.PowerId);
                            }
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

        /// <summary>
        /// Проверка условия отключения боя <see cref="ExecutePowerExt.IgnoreCombatCondition"/>
        /// </summary>
        /// <returns>Результат проверки <see cref="ExecutePowerExt.IgnoreCombatCondition"/> либо True, если оно не задано.</returns>
        private bool CheckingIgnoreCombatCondition()
        {
            var check = @this.IgnoreCombatCondition;
            if (check != null)
                return check.IsValid;
            return true;
        }


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
