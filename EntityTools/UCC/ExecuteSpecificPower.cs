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

        [Browsable(false)]
        [XmlIgnore]
        public bool Slotted
        {
            get
            {
                if (!string.IsNullOrEmpty(PowerId) && (power == null || !power.IsValid))
                    power = Powers.GetPowerByInternalName(PowerId);
                return power?.IsInTray == true;
            }
        }

        [Browsable(false)]
        [XmlIgnore]
        private Entity TargetEntity
        {
            get
            {
                switch (Target)
                {
                    case Enums.Unit.Player:
                        return EntityManager.LocalPlayer;
                    case Enums.Unit.MostInjuredAlly:
                        return ActionsPlayer.MostInjuredAlly;
                    case Enums.Unit.StrongestAdd:
                        return ActionsPlayer.AnAdd;
                    case Enums.Unit.StrongestTeamMember:
                        return ActionsPlayer.StrongestTeamMember;
                    default:
                        return Core.CurrentTarget;
                }
            }
        }

        [XmlIgnore]
        private Power power = null;
        [XmlIgnore]
        private static Type movementsType = null;


        [Browsable(false)]
        [XmlIgnore]
        public override bool NeedToRun
        {
            get
            {
                if (power == null || !power.IsValid)
                    power = Powers.GetPowerByInternalName(PowerId);

                //if (!power.IsInTray)
                //{
                //    if (!ActionsPlayer.combatBLActions.Contains(this))
                //    {
                //        ActionsPlayer.combatBLActions.Add(this);
                //    }
                //    return false;
                //}

                return PowerId.Length > 0 
                        && power!= null && power.IsValid 
                        && (!CheckPowerCooldown || power.IsOnCooldown())
                        && (!CheckInTray || power.IsInTray);
            }
        }

        public override bool Run()
        {
            if (movementsType == null)
                movementsType = ReflectionHelper.GetTypeByName("Astral.Logic.UCC.Controllers.Movements", true);

            //if(power == null && !power.IsValid)
            //    power = Powers.GetPowerByInternalName(PowerId);

            //return power.IsValid && Powers.SmartPowerExec(power);
            if(power == null || !power.IsValid)
                power = Powers.GetPowerByInternalName(PowerId);
            if (power == null || !power.IsValid)
                return false;
            Power entActivatedPower = power.EntGetActivatedPower();
            PowerDef powerDef = entActivatedPower.EntGetActivatedPower().EffectivePowerDef();
            if (base.Target != Enums.Unit.Player)
            {
                switch (Target)
                {
                    case Enums.Unit.MostInjuredAlly:
                        ReflectionHelper.SetStaticFieldValue(movementsType, "SpecificTarget", ActionsPlayer.MostInjuredAlly, BindingFlags.Static|BindingFlags.SetField);
                        //Astral.Logic.UCC.Controllers.Movements.SpecificTarget = ActionsPlayer.MostInjuredAlly;
                        break;
                    case Enums.Unit.StrongestAdd:
                        ReflectionHelper.SetStaticFieldValue(movementsType, "SpecificTarget", ActionsPlayer.AnAdd, BindingFlags.Static | BindingFlags.SetField);
                        //Astral.Logic.UCC.Controllers.Movements.SpecificTarget = ActionsPlayer.AnAdd;
                        break;
                    case Enums.Unit.StrongestTeamMember:
                        ReflectionHelper.SetStaticFieldValue(movementsType, "SpecificTarget", ActionsPlayer.StrongestTeamMember, BindingFlags.Static | BindingFlags.SetField);
                        //Astral.Logic.UCC.Controllers.Movements.SpecificTarget = ActionsPlayer.StrongestTeamMember;
                        break;
                    default:
                        ReflectionHelper.SetStaticFieldValue(movementsType, "SpecificTarget", null, BindingFlags.Static | BindingFlags.SetField);
                        //Astral.Logic.UCC.Controllers.Movements.SpecificTarget = null;
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
                    ReflectionHelper.SetStaticFieldValue(typeof(Astral.Logic.UCC.Controllers.Movements), "RequireRange", range - 2, BindingFlags.Static | BindingFlags.SetField);
                    //Astral.Logic.UCC.Controllers.Movements.RequireRange = num - 2;
                    while (ReflectionHelper.GetPropertyValue(movementsType, "RangeIsOk", out object RangeIsOk) && RangeIsOk.Equals(true))
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
            Entity entity = new Entity(TargetEntity.Pointer);
            if (entity.ContainerId != EntityManager.LocalPlayer.ContainerId && !entity.Location.IsInYawFace)
            {
                entity.Location.Face();
                Astral.Classes.Timeout timeout = new Astral.Classes.Timeout(750);
                while (!entity.Location.IsInYawFace && !timeout.IsTimedOut)
                {
                    Thread.Sleep(20);
                }
                Thread.Sleep(100);
            }
            
            try
            {
                if (!powerDef.GroundTargeted && !powerDef.Categories.Contains(PowerCategory.IgnorePitch))
                {
                    entity.Location.Face();
                    Powers.ExecPower(power, entity, true);
                }
                else
                {
                    Vector3 location = entity.Location;
                    location.Z += 3f;
                    location.Face();
                    Powers.ExecPower(power, location, true);
                }                
            }
            catch
            {
            }
            finally
            {
                Powers.ExecPower(power, entity, false);
            }
            return true;
        }

        public ExecuteSpecificPower()
        {
            Target = Astral.Logic.UCC.Ressources.Enums.Unit.Player;
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
            if (! string.IsNullOrEmpty(PowerId) && (power == null || !power.IsValid))
                power = Powers.GetPowerByInternalName(PowerId);

            if (power != null && power.IsValid)
            {
                StringBuilder str = new StringBuilder();
                if (CheckInTray && Slotted)
                    str.Append("[Slotted] ");
                if (power.EffectivePowerDef().DisplayName.Length > 0)
                    str.Append(power.EffectivePowerDef().DisplayName);
                else str.Append(power.EffectivePowerDef().InternalName);

                return str.ToString();
            }

            return "Unknow Power";
        }

        #region Hide Inherited Properties
        [XmlIgnore]
        [Browsable(false)]
        public new Astral.Logic.UCC.Ressources.Enums.Unit Target { get; set; }

        [XmlIgnore]
        [Browsable(false)]
        public new string ActionName { get; set; }
        #endregion
    }
}
