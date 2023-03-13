using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using Astral.Logic.NW;
using Astral.Logic.UCC.Classes;
using Astral.Quester.UIEditors;
using EntityTools.Enums;
using EntityTools.Tools.Powers;
using EntityTools.UCC.Conditions;
using Infrastructure;
using MyNW.Classes;
using MyNW.Internals;
using MyNW.Patchables.Enums;
using PowerResult = EntityTools.Tools.Powers.PowerResult;
using Unit = Astral.Logic.UCC.Ressources.Enums.Unit;

namespace EntityTools.UCC.Actions
{
    [Serializable]
    public class PluggedSkill : UCCAction, INotifyPropertyChanged
    {
        #region Опции команды
        [Editor(typeof(PowerAllIdEditor), typeof(UITypeEditor))]
        [Category("Required")]
        public PluggedSkillSource Source
        {
            get => _source;
            set
            {
                if (_source != value)
                {
                    _source = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private PluggedSkillSource _source = PluggedSkillSource.Mount;

        /// <summary>
        /// Объявление <see cref="CustomConditions"/> для обратной совместимости.
        /// Старый список условий объединен со встроенным <see cref="UCCAction.Conditions"/>.
        /// </summary>
        [Browsable(false)]
        public UCCConditionPack CustomConditions
        {
            get => null;
            set
            {
                if (value != null)
                    Conditions.Add(value);
            }
        }
        public bool ShouldSerializeCustomConditions() => false;


        #region Hide Inherited Properties
        [XmlIgnore]
        [Browsable(false)]
        public new string ActionName { get; set; }

        [XmlIgnore]
        [Browsable(false)]
        public new Unit Target { get => base.Target; set => base.Target = value; }

        #endregion
        #endregion




        #region Взаимодействие с EntityToolsCore
        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            InternalResetOnPropertyChanged(propertyName);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void InternalResetOnPropertyChanged([CallerMemberName] string memberName = default)
        {
            _power = null;
            _label = string.Empty;
            _targetSelector = null;
            _idStr = $"{GetType().Name}[{GetHashCode():X2}]";
        }
        #endregion




        public override UCCAction Clone()
        {
            return BaseClone(new PluggedSkill
            {
                _source = _source
            });
        }

        #region Данные
        private string _label;
        private string _idStr;

        private int attachedGameProcessId;
        private uint characterContainerId;
        private uint powerId;
        private string powerName;
        private bool isIgnoredPower;
        //TODO Заменить на PowerCache
        private Power _power;
        #endregion





        #region IUCCActionEngine
        public override bool NeedToRun
        {
            get
            {
                var currentPower = GetCurrentPower();
                if (currentPower is null)
                    return false;

                //Для умений, целью которых является персонаж, проверку наличия противников вблизи персонажа
                var targetMain = currentPower.PowerDef.TargetMain;
                if (targetMain.Self && Combats.MobCountAround() == 0)
                    return false;

                return !currentPower.IsOnCooldown();
            }
        }

        public override bool Run()
        {
            bool extendedDebugInfo = ExtendedDebugInfo;

            string actionIdStr = extendedDebugInfo
                 ? $"{_idStr}.{MethodBase.GetCurrentMethod()?.Name ?? nameof(Run)}"
                 : string.Empty;

            if (extendedDebugInfo)
                ETLogger.WriteLine(LogType.Debug, $"{actionIdStr}: starts");

            var currentPower = GetCurrentPower();

            if (currentPower is null)
            {
                if (extendedDebugInfo)
                    ETLogger.WriteLine(LogType.Debug, $"{actionIdStr}: Fail to get Artifact's power");

                return false;
            }

            var targetEntity = UnitRef;
            if (targetEntity is null || !targetEntity.IsValid || targetEntity.IsDead)
                return true;

            var powResult = currentPower.ExecutePower(targetEntity, 0, Range, false, extendedDebugInfo);

            switch (powResult)
            {
                case PowerResult.Succeed:
                    if (extendedDebugInfo)
                        ETLogger.WriteLine(LogType.Debug, $"{actionIdStr}: Result => {powResult}");
                    return true;
                default:
                    if (extendedDebugInfo)
                        ETLogger.WriteLine(LogType.Debug, $"{actionIdStr}: Result => {powResult}");
                    return false;
            }
        }

        /// <summary>
        /// Выбор <see cref="Entity"/>, соответствующего дополнительному умению
        /// </summary>
        public Entity UnitRef
        {
            get
            {
                if (_targetSelector is null)
                {
                    var pwr = GetCurrentPower();

                    var targetMain = pwr?.EffectivePowerDef()?.TargetMain;

                    if (targetMain is null || !targetMain.IsValid)
                        _targetSelector = () => Astral.Logic.UCC.Core.CurrentTarget;
                    else if (targetMain.Self)
                        _targetSelector = () => EntityManager.LocalPlayer;
                    else if (targetMain.AffectFoe)
                        _targetSelector = () => ActionsPlayer.AnAdd;
                    else if (targetMain.AffectFriend)
                        _targetSelector = () => ActionsPlayer.StrongestTeamMember;
                    else _targetSelector = () => Astral.Logic.UCC.Core.CurrentTarget;
                }

                return _targetSelector();
            }
        }
        private Func<Entity> _targetSelector;

        /// <summary>
        /// Текстовая метка, соответствующая <see cref="PluggedSkill"/> и отображаемая в GUI
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (string.IsNullOrEmpty(_label))
            {
                var currentPower = GetCurrentPower();

                if (currentPower != null)
                {
                    PowerDef powDef = currentPower.PowerDef;
                    if (powDef != null && powDef.IsValid)
                        _label = $"{_source}Power : {(string.IsNullOrEmpty(powDef.DisplayName) ? powDef.InternalName : powDef.DisplayName)}";
                }
                if (string.IsNullOrEmpty(_label))
                    _label = GetType().Name + " : Indefinite";
            }

            return _label;
        }
        #endregion

        #region Вспомогательные инструменты
        /// <summary>
        /// Метод, проверяющий актуальность кэшированного умения <see cref="_power"/> или выполняющий поиск соответствующего
        /// </summary>
        /// <returns></returns>
        private Power GetCurrentPower()
        {
            var player = EntityManager.LocalPlayer;
            if (player.IsValid)
            {
                // проверяем валидность кэша
                if (attachedGameProcessId == Astral.API.AttachedGameProcess.Id
                       && characterContainerId == player.ContainerId
                       && _power != null
                       && _power.IsValid
                       && _power.PowerId == powerId
                       && string.Equals(_power.PowerDef.InternalName, powerName, StringComparison.Ordinal))
                    goto result;

                // Кэш не валиден и требует обновления
                var isArtifactPower = _source == PluggedSkillSource.Artifact;
                var bagSlots = player.GetInventoryBagById(isArtifactPower
                    ? InvBagIDs.ArtifactPrimary
                    : InvBagIDs.MountEquippedActivePower).Slots;
                // Проверка наличия предмета/маунта в слоте
                if (bagSlots.Count > 0)
                {
                    var itemSlot = bagSlots[0];

                    if (itemSlot.Filled)
                    {
                        var item = itemSlot.Item;
                        if (item != null)
                        {
                            var itemprivateName = item.ItemDef.InternalName;
                            // В бою не применяются Артефактные умения:
                            // - "Каталог "Аврора для всех миров"
                            // - "Молот Гонда"
                            isIgnoredPower = isArtifactPower
                                             && (itemprivateName.StartsWith("Artifact_Auroraswholerealmscatalogue",
                                                     StringComparison.Ordinal)
                                                 || itemprivateName.StartsWith("Artifact_Forgehammer_Of_Gond",
                                                     StringComparison.Ordinal));

                            _power = item.Powers.FirstOrDefault();

                            if (_power != null)
                            {
                                powerId = _power.PowerId;
                                powerName = itemprivateName;
                                _label = string.Empty;
                                attachedGameProcessId = Astral.API.AttachedGameProcess.Id;
                                characterContainerId = player.ContainerId;
                                goto result;
                            }
                        }
                    }
                }
            }
            powerId = 0;
            powerName = string.Empty;
            _label = string.Empty;
            attachedGameProcessId = 0;
            characterContainerId = 0;
            _power = null;

        result:
            return isIgnoredPower ? null : _power;
        }


        /// <summary>
        /// Флаг настроек вывода расширенной отлаточной информации
        /// </summary>
        private bool ExtendedDebugInfo
        {
            get
            {
                var logConf = EntityTools.Config.Logger;
                return logConf.UccActions.DebugChangeTarget && logConf.Active;
            }
        }
        #endregion
    }
}
