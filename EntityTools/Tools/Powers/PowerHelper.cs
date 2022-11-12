using System;
using System.Reflection;
using System.Threading;
using ACTP0Tools;
using EntityTools.Tools.Navigation;
using MyNW.Classes;
using MyNW.Internals;
using MyNW.Patchables.Enums;

namespace EntityTools.Tools.Powers
{
    public enum PowerResult
    {
        Fail,
        Skip,
        Succeed
    }

    public static class PowerHelper
    {
        /// <summary>
        /// Активация умения <paramref name="power"/> на область, заданную координатами <paramref name="targetPosition"/> 
        /// </summary>
        /// <param name="power">Активируемое умение.</param>
        /// <param name="targetPosition">Координаты цели</param>
        /// <param name="castingTime">Время активации умения, мс.</param>
        /// <param name="pause">Время ожидания после активации умения, мс.</param>
        /// <param name="range">Предельное расстояние для применения умения.</param>
        /// <param name="forceMaintain">Серия из умений применяется удержанием ключевой клавиши.</param>
        /// <param name="debugInfo">Флаг активации отладочной информации.</param>
        /// <returns>Результат выполнения умения</returns>
        public static PowerResult ExecutePower(this Power power, Vector3 targetPosition, int castingTime = 500, int pause = 0, int range = 0, bool forceMaintain = false, bool debugInfo = false)
        {
            var methodName = debugInfo 
                                ? MethodBase.GetCurrentMethod()?.Name ?? nameof(ExecutePower)
                                : string.Empty;

            NavigationHelper.StopNavigationCompletely();

            var entActivatedPower = power.EntGetActivatedPower();
            var powerDef = entActivatedPower.EntGetActivatedPower().EffectivePowerDef();

            // Перемещаем персонажа к targetPosition для возможности применения 
            if (range > 1 && targetPosition.Distance3DFromPlayer > range)
            {
                // Вычисляем эффективный радиус действия команды
                int effectiveRange = Math.Max(Astral.Logic.NW.Powers.getEffectiveRange(powerDef), range);

                if (effectiveRange < 7)
                {
                    effectiveRange = 7;
                }

                AstralAccessors.Logic.UCC.Controllers.Movements.RequireRange = effectiveRange - 2;

                // Пытаемся приблизиться к цели
                // Запуск Astral.Logic.UCC.Controllers.Movements.Start()
                // выполняется перед вызовом метода Run() текущей команды в 
                // Astral.Logic.UCC.Classes.ActionsPlayer.playActionList()
                var movingTimeout = new Astral.Classes.Timeout(1050);
                while (!AstralAccessors.Logic.UCC.Controllers.Movements.RangeIsOk)
                {
                    if (movingTimeout.IsTimedOut)
                    {
                        // Завершаем команду, если попытка приблизиться к targetPosition неудачна
                        return PowerResult.Skip;
                    }
                    Thread.Sleep(100);
                }
            }

            if (targetPosition.IsInYawFace)
            {
                targetPosition.Face();
                var timeout = new Astral.Classes.Timeout(750);
                while (!targetPosition.IsInYawFace && !timeout.IsTimedOut)
                {
                    Thread.Sleep(20);
                }
                Thread.Sleep(100);
            }

            castingTime = Math.Max(Astral.Logic.NW.Powers.getEffectiveTimeCharge(powerDef), castingTime);
            var castingTimeout = new Astral.Classes.Timeout(castingTime);

            bool powerActivated = false;
            try
            {
                targetPosition.Face();
                if (debugInfo)
                    ETLogger.WriteLine(LogType.Debug, $"{methodName}: Activating ExecPower '{power.PowerDef.InternalName}' on targetEntity <{targetPosition.X:N2}, {targetPosition.Y:N2}, {targetPosition.Z:N2}>");
                Astral.Logic.NW.Powers.ExecPower(power, targetPosition, true);
                powerActivated = true;
                if (debugInfo)
                    ETLogger.WriteLine(LogType.Debug, $"{methodName}: Waiting casting time ({castingTime} ms)");
                while (!castingTimeout.IsTimedOut && !Astral.Controllers.AOECheck.PlayerIsInAOE)
                {
                    if (power.UseCharges() && !power.ChargeAvailable() || power.IsActive)
                    {
                        break;
                    }
                    Thread.Sleep(20);
                }
                if (pause > 0)
                {
                    if (debugInfo)
                        ETLogger.WriteLine(LogType.Debug, $"{methodName}: Pause after power activation");
                    pause = Math.Max(pause, 500);
                    Thread.Sleep(pause); 
                }
            }
            catch (Exception e)
            {
                if (debugInfo)
                    ETLogger.WriteLine(LogType.Error, $"{methodName}: Catch an exception trying activate power '{power.PowerDef.InternalName}' \n{e.Message}\n{e.StackTrace}");
                return PowerResult.Fail;
            }
            finally
            {
                if (powerActivated)
                {
                    if (debugInfo)
                        ETLogger.WriteLine(LogType.Debug, $"{methodName}: Deactivating ExecPower '{power.PowerDef.InternalName}'");
                    try
                    {
                        Astral.Logic.NW.Powers.ExecPower(power, targetPosition, false);
                    }
                    catch (Exception e)
                    {
                        if (debugInfo)
                            ETLogger.WriteLine(LogType.Debug, $"{methodName}: Catch an exception trying deactivate power '{power.PowerDef.InternalName}'\n{e.Message}\n{e.StackTrace}");
                    }
                }
            }
            if (!forceMaintain && powerActivated)
            {
                double effectiveTimeActivate = Astral.Logic.NW.Powers.getEffectiveTimeActivate(powerDef) * 1.5;
                Astral.Logic.NW.Powers.WaitPowerActivation(power, (int)effectiveTimeActivate);
            }
            return PowerResult.Succeed;
        }

        /// <summary>
        /// Активация умения <paramref name="power"/> на сущность <paramref name="targetEntity"/> 
        /// </summary>
        /// <param name="power">Активируемое умение.</param>
        /// <param name="targetEntity">Цель, на которую применяется умение.</param>
        /// <param name="castingTime">Время активации умения, мс.</param>
        /// <param name="range">Предельное расстояние для применения умения.</param>
        /// <param name="forceMaintain">Серия из умений применяется удержанием ключевой клавиши.</param>
        /// <param name="debugInfo">Флаг активации отладочной информации.</param>
        /// <returns></returns>
        public static PowerResult ExecutePower(this Power power, Entity targetEntity, int castingTime = 500, 
                                               int range = 0, bool forceMaintain = false, bool debugInfo = false)
        {
            var methodName = debugInfo
                ? MethodBase.GetCurrentMethod()?.Name ?? nameof(ExecutePower)
                : string.Empty;

            var entActivatedPower = power.EntGetActivatedPower();
            var powerDef = entActivatedPower.EntGetActivatedPower().EffectivePowerDef();

            var player = EntityManager.LocalPlayer;

            // Устанавливаем цель для перемещения персонажа к ней
            if (targetEntity.ContainerId != player.ContainerId)
            {
                // Вычисляем эффективный радиус действия команды
                int effectiveRange = Astral.Logic.NW.Powers.getEffectiveRange(powerDef);

                if (range > 0)
                    effectiveRange = range;

                if (effectiveRange > 1)
                {
                    if (effectiveRange < 7)
                    {
                        effectiveRange = 7;
                    }

                    AstralAccessors.Logic.UCC.Controllers.Movements.RequireRange = effectiveRange - 2;

                    // Пытаемся приблизиться к цели
                    // Запуск Astral.Logic.UCC.Controllers.Movements.Start()
                    // выполняется перед вызовом метода Run() текущей команды в 
                    // Astral.Logic.UCC.Classes.ActionsPlayer.playActionList()
                    var movingTimeout = new Astral.Classes.Timeout(1050);
                    while (!AstralAccessors.Logic.UCC.Controllers.Movements.RangeIsOk)
                    {
                        if (Astral.Logic.UCC.Core.CurrentTarget.IsDead || movingTimeout.IsTimedOut)
                        {
                            // Завершаем команду, если цель мертва, или попытка приблизиться к ней неудачна
                            return PowerResult.Skip;
                        }
                        Thread.Sleep(100);
                    }
                }
            }

            castingTime = castingTime > 0
                ? castingTime
                : Astral.Logic.NW.Powers.getEffectiveTimeCharge(powerDef);

            if (targetEntity.ContainerId != EntityManager.LocalPlayer.ContainerId && !targetEntity.Location.IsInYawFace)
            {
                targetEntity.Location.Face();
                var timeout = new Astral.Classes.Timeout(750);
                while (!targetEntity.Location.IsInYawFace && !timeout.IsTimedOut)
                {
                    Thread.Sleep(20);
                }
                Thread.Sleep(100);
            }

            var castingTimeout = new Astral.Classes.Timeout(castingTime);
            bool powerActivated = false;
            try
            {
                if (!powerDef.GroundTargeted && !powerDef.Categories.Contains(PowerCategory.Ignorepitch))
                {
                    targetEntity.Location.Face();
                    if (debugInfo)
                        ETLogger.WriteLine(LogType.Debug, $"{methodName}: Activate ExecPower '{power.PowerDef.InternalName}' on targetEntity {targetEntity.Name}[{targetEntity.InternalName}]");

                    Astral.Logic.NW.Powers.ExecPower(power, targetEntity, true);
                    powerActivated = true;
                }
                else
                {
                    Vector3 location = targetEntity.Location;
                    location.Z += 3f;
                    location.Face();
                    if (debugInfo)
                        ETLogger.WriteLine(LogType.Debug, $"{methodName}: Activate ExecPower '{power.PowerDef.InternalName}' on location <{location.X.ToString("0,4:N2")}, {location.Y.ToString("0,4:N2")}, {location.Z.ToString("0,4:N2")}>");
                    Astral.Logic.NW.Powers.ExecPower(power, location, true);
                    powerActivated = true;
                }
                if (debugInfo)
                    ETLogger.WriteLine(LogType.Debug, $"{methodName}: Wait casting time ({castingTime} ms)");
                while (!castingTimeout.IsTimedOut && !Astral.Controllers.AOECheck.PlayerIsInAOE)
                {
                    if (Astral.Logic.UCC.Core.CurrentTarget.IsDead)
                    {
                        return PowerResult.Succeed;
                    }
                    if (!forceMaintain && (power.UseCharges() && !power.ChargeAvailable() || power.IsActive))
                    {
                        break;
                    }
                    Thread.Sleep(20);
                }
            }
            catch (Exception e)
            {
                if (debugInfo)
                    ETLogger.WriteLine(LogType.Debug, $"{methodName}: Catch an exception trying activate power '{power.PowerDef.InternalName}' \n\r{e.Message}");
            }
            finally
            {
                if (powerActivated)
                {
                    if (debugInfo)
                        ETLogger.WriteLine(LogType.Debug, $"{methodName}: Deactivate ExecPower '{power.PowerDef.InternalName}' on targetEntity {targetEntity.Name}[{targetEntity.InternalName}]");
                    try
                    {
                        Astral.Logic.NW.Powers.ExecPower(power, targetEntity, false);
                    }
                    catch (Exception e)
                    {
                        if (debugInfo)
                            ETLogger.WriteLine(LogType.Debug, $"{methodName}: Catch an exception trying deactivate power '{power.PowerDef.InternalName}'\n\r {e.Message}");
                    } 
                }
            }
            if (!forceMaintain && powerActivated)
            {
                double effectiveTimeActivate = Astral.Logic.NW.Powers.getEffectiveTimeActivate(powerDef) * 1.5;
                Astral.Logic.NW.Powers.WaitPowerActivation(power, (int)effectiveTimeActivate);
            }
            return PowerResult.Succeed;
        }
    }
}
