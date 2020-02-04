#if DEBUG
#define DEBUT_ExecuteSpecificPower
#endif
#define REFLECTION_ACCESS
using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Text;
using System.Threading;
using System.Xml.Serialization;
using Astral.Classes.ItemFilter;
using Astral.Controllers;
using Astral.Logic.NW;
using Astral.Logic.UCC;
using Astral.Logic.UCC.Classes;
using Astral.Quester.UIEditors;
using EntityTools.Editors;
using EntityTools.Enums;
using EntityTools.Tools;
using EntityTools.Tools.Entities;
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

        [Category("Optional")]
        public bool CheckPowerCooldown { get; set; } = false;

        [Category("Optional")]
        public bool CheckInTray { get; set; } = false;

        [Category("Optional")]
        [DisplayName("CastingTime (ms)")]
        public int CastingTime { get; set; } = 0;

        [Category("Optional")]
        public bool ForceMaintain { get; set; } = false;

        [Description("ID of the Entity that is preferred to attack\n" +
            "If Entity does not exist or EntityID is empty then the Target option is used")]
        [Editor(typeof(EntityIdEditor), typeof(UITypeEditor))]
        [Category("TargetEntity")]
        public string EntityID
        {
            get => entityId;
            set
            {
                if (entityId != value)
                {
                    entityId = value;
                    if (!string.IsNullOrEmpty(entityId))
                        Comparer = new EntityComparerToPattern(entityId, entityIdType, entityNameType);
                    else Comparer = null;
                }
            }
        }

        [Description("Type of and EntityID:\n" +
            "Simple: Simple text string with a wildcard at the beginning or at the end (char '*' means any symbols)\n" +
            "Regex: Regular expression")]
        [Category("TargetEntity")]
        public ItemFilterStringType EntityIdType
        {
            get => entityIdType;
            set
            {
                if (entityIdType != value)
                {
                    entityIdType = value;
                    if (!string.IsNullOrEmpty(entityId))
                        Comparer = new EntityComparerToPattern(entityId, entityIdType, entityNameType);
                    else Comparer = null;
                }
            }
        }

        [Description("The switcher of the Entity filed which compared to the property EntityID")]
        [Category("TargetEntity")]
        public EntityNameType EntityNameType
        {
            get => entityNameType;
            set
            {
                if (entityNameType != value)
                {
                    entityNameType = value;
                    if (!string.IsNullOrEmpty(entityId))
                        Comparer = new EntityComparerToPattern(entityId, entityIdType, entityNameType);
                    else Comparer = null;
                }
            }
        }

        [Description("Check Entity's Ingame Region (Not CustomRegion):\n" +
            "True: Only Entities located in the same Region as Player are detected\n" +
            "False: Entity's Region does not checked during search")]
        [Category("TargetEntity (Optional)")]
        public bool RegionCheck { get; set; } = true;

        [Description("Check if Entity's health greater than zero:\n" +
            "True: Only Entities with nonzero health are detected\n" +
            "False: Entity's health does not checked during search")]
        [Category("TargetEntity (Optional)")]
        public bool HealthCheck { get; set; } = true;

        [Description("Aura which checked on the Entity")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [Category("TargetEntity (Optional)")]
        public AuraOption Aura { get; set; } = new AuraOption();

        [Description("The maximum distance from the character within which the Entity is searched\n" +
            "The default value is 0, which disables distance checking")]
        [Category("TargetEntity (Optional)")]
        public float ReactionRange { get; set; } = 60;

        [Description("The maximum ZAxis difference from the withing which the Entity is searched\n" +
            "The default value is 0, which disables ZAxis checking")]
        [Category("TargetEntity (Optional)")]
        public float ReactionZRange { get; set; } = 0;

        [XmlIgnore]
        [Editor(typeof(EntityTestEditor), typeof(UITypeEditor))]
        [Description("Нажми на кнопку '...' чтобы увидеть тестовую информацию")]
        //[Category("Entity")]
        public string TestInfo { get; } = "Нажми '...' =>";


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

        //[XmlIgnore]
        //[Browsable(false)]
        //private Entity TargetEntity
        //{
        //    get
        //    {
        //        if (!string.IsNullOrEmpty(entityId))
        //        {
        //            if (Validate(entity))
        //                return entity;
        //            else entity = SearchCached.FindClosestEntity(EntityID, EntityIdType, EntityNameType, EntitySetType.Complete,
        //                                                        HealthCheck, ReactionRange, ReactionRange, RegionCheck, null, Aura.Checker);
        //            if (Validate(entity))
        //                return entity;
        //        }
        //        switch (Target)
        //        {
        //            case Unit.Player:
        //                return EntityManager.LocalPlayer;
        //            case Unit.MostInjuredAlly:
        //                return ActionsPlayer.MostInjuredAlly;
        //            case Unit.StrongestAdd:
        //                return ActionsPlayer.AnAdd;
        //            case Unit.StrongestTeamMember:
        //                return ActionsPlayer.StrongestTeamMember;
        //            default:
        //                return Core.CurrentTarget;
        //        }
        //    }
        //}

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
                int effectiveRange = Powers.getEffectiveRange(powerDef);

                if (Range > 0)
                    effectiveRange = Range;

                if (effectiveRange > 1)
                {
                    if (effectiveRange < 7)
                    {
                        effectiveRange = 7;
                    }
#if REFLECTION_ACCESS
                    if (!ReflectionHelper.SetStaticPropertyValue(movementsType, "RequireRange", effectiveRange - 2/*, BindingFlags.Static | BindingFlags.SetProperty*/))
                    {
#if DEBUT_ExecuteSpecificPower
                        Astral.Logger.WriteLine(Astral.Logger.LogType.Debug, $"ExecuteSpecificPower: Fail to set UCC.Controllers.Movements.RequireRange to '{effectiveRange - 2}'");
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
            int castingTime;
            if (this.CastingTime > 0)
            {
                castingTime = this.CastingTime;
            }
            else
            {
                castingTime = Powers.getEffectiveTimeCharge(powerDef);
            }
            Entity target = new Entity(UnitRef.Pointer);//new Entity(TargetEntity.Pointer);
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
            double effectiveTimeActivate = (double)Powers.getEffectiveTimeActivate(powerDef) * 1.5;
            Astral.Classes.Timeout castingTimeout = new Astral.Classes.Timeout(castingTime);

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
#if DEBUT_ExecuteSpecificPower
                Astral.Logger.WriteLine(Astral.Logger.LogType.Debug, $"ExecuteSpecificPower: Wait casting time ({castingTime} ms)");
#endif
                while (!castingTimeout.IsTimedOut && !AOECheck.PlayerIsInAOE)
                {
                    if (Core.CurrentTarget.IsDead)
                    {
                        return true;
                    }
                    if (!this.ForceMaintain && ((currentPower.UseCharges() && !currentPower.ChargeAvailable()) || currentPower.IsActive))
                    {
                        break;
                    }
                    Thread.Sleep(20);
                }
            }
            catch { }
            finally
            {
#if DEBUT_ExecuteSpecificPower
                Astral.Logger.WriteLine(Astral.Logger.LogType.Debug, $"ExecuteSpecificPower: Deactivate ExecPower '{currentPower.PowerDef.InternalName}' on target {target.Name}[{target.InternalName}]");
#endif
                Powers.ExecPower(currentPower, target, false);
            }
            if (!this.ForceMaintain)
            {
                Powers.WaitPowerActivation(currentPower, (int)effectiveTimeActivate);
            }
            return true;
        }

        public ExecuteSpecificPower()
        {
            Target = Unit.Target;
            movementsType = typeof(Astral.Logic.UCC.Controllers.Movements);
        }

        public override UCCAction Clone()
        {
            return base.BaseClone(new ExecuteSpecificPower
            {
                PowerId = this.PowerId,
                CheckPowerCooldown = this.CheckPowerCooldown,
                CheckInTray = this.CheckInTray,
                CastingTime = this.CastingTime,
                ForceMaintain = this.ForceMaintain,
                entityId = this.entityId,
                entityIdType = this.entityIdType,
                entityNameType = this.entityNameType,
                RegionCheck = this.RegionCheck,
                HealthCheck = this.HealthCheck,
                Aura = new AuraOption
                {
                    AuraName = this.Aura.AuraName,
                    AuraNameType = this.Aura.AuraNameType,
                    Sign = this.Aura.Sign,
                    Stacks = this.Aura.Stacks
                }
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
        private bool Validate(Entity e)
        {
            return e != null && e.IsValid && Comparer.Check(e);
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

        [XmlIgnore]
        [Browsable(false)]
        internal EntityComparerToPattern Comparer { get; private set; } = null;

        [XmlIgnore]
        [Browsable(false)]
        public Entity UnitRef
        {
            get
            {
                if (!string.IsNullOrEmpty(entityId))
                {
                    if (Comparer == null)
                        Comparer = new EntityComparerToPattern(entityId, entityIdType, entityNameType);
                    if (Validate(entity))
                        return entity;
                    else entity = SearchCached.FindClosestEntity(entityId, entityIdType, entityNameType, EntitySetType.Complete,
                                                                HealthCheck, ReactionRange,
                                                                (ReactionZRange > 0) ? ReactionZRange : Astral.Controllers.Settings.Get.MaxElevationDifference,
                                                                RegionCheck, null, Aura.Checker);

                    if (Validate(entity))
                        return entity;
                }
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

        [NonSerialized]
        private string entityId = string.Empty;
        [NonSerialized]
        private ItemFilterStringType entityIdType = ItemFilterStringType.Simple;
        [NonSerialized]
        private EntityNameType entityNameType = EntityNameType.NameUntranslated;
        [NonSerialized]
        protected Entity entity = new Entity(IntPtr.Zero);


#region Hide Inherited Properties
        [XmlIgnore]
        [Browsable(false)]
        public new string ActionName { get; set; }
#endregion
    }
}
