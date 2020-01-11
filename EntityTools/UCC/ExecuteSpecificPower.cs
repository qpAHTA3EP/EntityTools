#if DEBUG
#define DEBUT_ExecuteSpecificPower
#endif
#define REFLECTION_ACCESS
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Xml.Serialization;
using Astral.Classes;
using Astral.Classes.ItemFilter;
using Astral.Controllers;
using Astral.Logic.NW;
using Astral.Logic.UCC;
using Astral.Logic.UCC.Actions;
using Astral.Logic.UCC.Classes;
using Astral.Logic.UCC.Ressources;
using Astral.Quester.FSM.States;
using Astral.Quester.UIEditors;
using EntityTools;
using EntityTools.Editors;
using EntityTools.Tools;
using MyNW.Classes;
using MyNW.Internals;
using MyNW.Patchables.Enums;
using Unit = Astral.Logic.UCC.Ressources.Enums.Unit;


namespace EntityTools.UCC
{
    [Serializable]
    public class ExecuteSpecificPower : UCCAction
    {
        [Editor(typeof(PowerAllIdEditor), typeof(UITypeEditor))]
        [Category("Required")]
        public string PowerId { get; set; } = string.Empty;

        [Category("Required")]
        public bool CheckPowerCooldown { get; set; } = false;

        [Category("Required")]
        public bool CheckInTray { get; set; } = false;

        [XmlIgnore]
        [Browsable(false)]
        public bool Slotted
        {
            get
            {
                if (!Validate(power))
                {
                    attachedGameProcessId = Astral.API.AttachedGameProcess.Id;
                    power = Powers.GetPowerByInternalName(PowerId);
                }
                return power?.IsInTray == true;
            }
        }

        [XmlIgnore]
        [Browsable(false)]
        private Entity TargetEntity
        {
            get
            {
                switch (Target)
                {
                    case Unit.Player:
                        return EntityManager.LocalPlayer;
                    case Unit.MostInjuredAlly:
                        return ActionsPlayer.MostInjuredAlly;
                    case Unit.StrongestAdd:
                        return ActionsPlayer.AnAdd;
                    case Unit.StrongestTeamMember:
                        return ActionsPlayer.StrongestTeamMember;
                    default:
                        return Core.CurrentTarget;
                }
            }
        }

        [XmlIgnore]
        [Browsable(false)]
        public override bool NeedToRun
        {
            get
            {
                Power p = GetCurrentPower();

                return Validate(p)
                        && (!CheckPowerCooldown || !p.IsOnCooldown())
                        && (!CheckInTray || p.IsInTray);
            }
        }

        public override bool Run()
        {
#if DEBUT_ExecuteSpecificPower
            Astral.Logger.WriteLine(Astral.Logger.LogType.Debug, $"ExecuteSpecificPower::Run() starts");
#endif
#if REFLECTION_ACCESS
            if (movementsType == null)
                movementsType = typeof(Astral.Logic.UCC.Controllers.Movements);//ReflectionHelper.GetTypeByName("Astral.Logic.UCC.Controllers.Movements", true);
#endif

            Power currentPower = GetCurrentPower();

            if (!Validate(currentPower))
            {
#if DEBUT_ExecuteSpecificPower
                Astral.Logger.WriteLine(Astral.Logger.LogType.Debug, $"ExecuteSpecificPower: Fail to get Power '{PowerId}' by 'PowerId'");
#endif
                return false;
            }
            Power entActivatedPower = currentPower.EntGetActivatedPower();
            PowerDef powerDef = entActivatedPower.EntGetActivatedPower().EffectivePowerDef();
            if (base.Target != Unit.Player)
            {
                switch (Target)
                {
                    case Unit.MostInjuredAlly:
#if REFLECTION_ACCESS
                        if (!ReflectionHelper.SetStaticPropertyValue(movementsType, "SpecificTarget", ActionsPlayer.MostInjuredAlly/*, BindingFlags.Static | BindingFlags.NonPublic*/))
                        {
#if DEBUT_ExecuteSpecificPower
                            Astral.Logger.WriteLine(Astral.Logger.LogType.Debug, $"ExecuteSpecificPower: Fail to set UCC.Controllers.Movements.SpecificTarget to 'MostInjuredAlly'");
#endif
                            return false;
                        }
#else
                        Astral.Logic.UCC.Controllers.Movements.SpecificTarget = ActionsPlayer.MostInjuredAlly;
#endif
                        break;
                    case Unit.StrongestAdd:
#if REFLECTION_ACCESS
                        if (!ReflectionHelper.SetStaticPropertyValue(movementsType, "SpecificTarget", ActionsPlayer.AnAdd/*, BindingFlags.Static | BindingFlags.NonPublic*/))
                        {
#if DEBUT_ExecuteSpecificPower
                            Astral.Logger.WriteLine(Astral.Logger.LogType.Debug, $"ExecuteSpecificPower: Fail to set UCC.Controllers.Movements.SpecificTarget to 'AnAdd'");
#endif
                            return false;
                        }
#else
                        Astral.Logic.UCC.Controllers.Movements.SpecificTarget = ActionsPlayer.AnAdd;
#endif
                        break;
                    case Unit.StrongestTeamMember:
#if REFLECTION_ACCESS
                        if (!ReflectionHelper.SetStaticPropertyValue(movementsType, "SpecificTarget", ActionsPlayer.StrongestTeamMember/*, BindingFlags.Static | BindingFlags.NonPublic*/))
                        {
#if DEBUG
                            Astral.Logger.WriteLine(Astral.Logger.LogType.Debug, $"ExecuteSpecificPower: Fail to set UCC.Controllers.Movements.SpecificTarget to 'StrongestTeamMember'");
#endif
                            return false;
                        }
#else
                        Astral.Logic.UCC.Controllers.Movements.SpecificTarget = ActionsPlayer.StrongestTeamMember;
#endif
                        break;
                    default:
#if REFLECTION_ACCESS
                        if (!ReflectionHelper.SetStaticPropertyValue(movementsType, "SpecificTarget", null/*, BindingFlags.Static | BindingFlags.NonPublic*/))
                        {
#if DEBUT_ExecuteSpecificPower
                            Astral.Logger.WriteLine(Astral.Logger.LogType.Debug, $"ExecuteSpecificPower: Fail to set UCC.Controllers.Movements.SpecificTarget to 'null'");
#endif
                            return false;
                        }
#else
                        Astral.Logic.UCC.Controllers.Movements.SpecificTarget = null;
#endif
                        break;
                }
                int range = Powers.getEffectiveRange(powerDef);

                if (Range > 0)
                    range = Range;

                if (range > 1)
                {
                    if (range < 7)
                    {
                        range = 7;
                    }
#if REFLECTION_ACCESS
                    if (!ReflectionHelper.SetStaticPropertyValue(movementsType, "RequireRange", range - 2/*, BindingFlags.Static | BindingFlags.SetProperty*/))
                    {
#if DEBUT_ExecuteSpecificPower
                        Astral.Logger.WriteLine(Astral.Logger.LogType.Debug, $"ExecuteSpecificPower: Fail to set UCC.Controllers.Movements.RequireRange to '{range - 2}'");
#endif
                        return false;
                    }
#else
                    Astral.Logic.UCC.Controllers.Movements.RequireRange = range - 2;
#endif
#if REFLECTION_ACCESS
                    while (ReflectionHelper.GetPropertyValue(movementsType, "RangeIsOk", out object RangeIsOk) && RangeIsOk.Equals(true))
#else
                    while (Astral.Logic.UCC.Controllers.Movements.RangeIsOk)
#endif
                    {
                        if (Core.CurrentTarget.IsDead)
                        {
                            return true;
                        }
                        Thread.Sleep(100);
                        //if (Spell.stopMoveDodgeTO.IsTimedOut && AOECheck.PlayerIsInAOE)
                        //{
                        //    Spell.stopMoveDodgeTO.ChangeTime(3500);
                        //    return true;
                        //}
                    }
                }
            }
            //int milseconds;
            //if (this.CastingTime > 0)
            //{
            //    milseconds = this.CastingTime;
            //}
            //else
            //{
            //    milseconds = Powers.getEffectiveTimeCharge(powerDef);
            //}
            Entity target = new Entity(TargetEntity.Pointer);
            if (target.ContainerId != EntityManager.LocalPlayer.ContainerId && !target.Location.IsInYawFace)
            {
                target.Location.Face();
                Astral.Classes.Timeout timeout = new Astral.Classes.Timeout(750);
                while (!target.Location.IsInYawFace && !timeout.IsTimedOut)
                {
                    Thread.Sleep(20);
                }
                Thread.Sleep(100);
            }
            
            try
            {
                if (!powerDef.GroundTargeted && !powerDef.Categories.Contains(PowerCategory.IgnorePitch))
                {
                    target.Location.Face();
#if DEBUT_ExecuteSpecificPower
                    Astral.Logger.WriteLine(Astral.Logger.LogType.Debug, $"ExecuteSpecificPower: Activate ExecPower '{currentPower.PowerDef.InternalName}' on target {target.Name}[{target.InternalName}]");
#endif
                    Powers.ExecPower(currentPower, target, true);
                }
                else
                {
                    Vector3 location = target.Location;
                    location.Z += 3f;
                    location.Face();
#if DEBUT_ExecuteSpecificPower
                    Astral.Logger.WriteLine(Astral.Logger.LogType.Debug, $"ExecuteSpecificPower: Activate ExecPower '{currentPower.PowerDef.InternalName}' on location <{location.X.ToString("0,4:N2")}, {location.Y.ToString("0,4:N2")}, {location.Z.ToString("0,4:N2")}>");
#endif
                    Powers.ExecPower(currentPower, location, true);
                }                
            }
            catch
            {
            }
            finally
            {
#if DEBUT_ExecuteSpecificPower
                Astral.Logger.WriteLine(Astral.Logger.LogType.Debug, $"ExecuteSpecificPower: Deactivate ExecPower '{currentPower.PowerDef.InternalName}' on target {target.Name}[{target.InternalName}]");
#endif
                Powers.ExecPower(currentPower, target, false);
            }
            return true;
        }

        public ExecuteSpecificPower()
        {
            Target = Unit.Player;
            movementsType = typeof(Astral.Logic.UCC.Controllers.Movements);
        }

        public override UCCAction Clone()
        {
            return base.BaseClone(new ExecuteSpecificPower
            {
                PowerId = this.PowerId,
                CheckPowerCooldown = this.CheckPowerCooldown,
                CheckInTray = this.CheckInTray
            });
        }
        public override string ToString()
        {
            Power currentPower = GetCurrentPower();

            if (Validate(currentPower))
            {
                StringBuilder str = new StringBuilder();
                if (CheckInTray && Slotted)
                    str.Append("[Slotted] ");
                if (currentPower.EffectivePowerDef().DisplayName.Length > 0)
                    str.Append(currentPower.EffectivePowerDef().DisplayName);
                else str.Append(currentPower.EffectivePowerDef().InternalName);

                return str.ToString();
            }

            return "Unknow Power";
        }

        private bool Validate(Power p)
        {
            return attachedGameProcessId == Astral.API.AttachedGameProcess.Id
                && p != null && p.IsValid && p.PowerDef.InternalName == PowerId;
        }

        private Power GetCurrentPower()
        {
            if (!Validate(power))
            {
                attachedGameProcessId = Astral.API.AttachedGameProcess.Id;
                power = Powers.GetPowerByInternalName(PowerId);
            }
            return power;
        }

        [NonSerialized]
        int attachedGameProcessId = 0;

        [NonSerialized]
        private Power power = null;
#if REFLECTION_ACCESS
        [NonSerialized]
        private static Type movementsType = null;
#endif

        #region Hide Inherited Properties
        //[XmlIgnore]
        //[Browsable(false)]
        //public new Unit Target { get; set; }

        [XmlIgnore]
        [Browsable(false)]
        public new string ActionName { get; set; }
#endregion
    }
}
