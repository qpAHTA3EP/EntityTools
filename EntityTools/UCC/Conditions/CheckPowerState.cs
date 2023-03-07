using System.ComponentModel;
using System.Drawing.Design;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using Astral.Logic.UCC.Classes;
using EntityTools.Editors;
using EntityTools.Enums;
using EntityTools.Tools.Powers;

namespace EntityTools.UCC.Conditions
{
    public class CheckPowerState : UCCCondition, ICustomUCCCondition
    {
        #region Опции
        [Editor(typeof(PowerIdEditor), typeof(UITypeEditor))]
        [Category("Required")]
        public string PowerId
        {
            get => _powerId;
            set
            {
                if (_powerId != value)
                {
                    _powerId = value;
                    powerCache.PowerIdPattern = PowerId;
                    NotifyPropertyChanged();
                }
            }
        }
        private string _powerId = string.Empty;

        [Category("Required")]
        public PowerState CheckState
        {
            get => checkState;
            set
            {
                if (checkState != value)
                {
                    checkState = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private PowerState checkState;


        #region Hide Inherited Properties
        [XmlIgnore]
        [Browsable(false)]
        public new string Value { get; set; }

        [XmlIgnore]
        [Browsable(false)]
        public new Astral.Logic.UCC.Ressources.Enums.Unit Target { get; set; }

        [XmlIgnore]
        [Browsable(false)]
        public new Astral.Logic.UCC.Ressources.Enums.ActionCond Tested { get; set; }

        [XmlIgnore] [Browsable(false)] public new Astral.Logic.UCC.Ressources.Enums.Sign Sign { get; set; }

        #endregion
        #endregion




        public new ICustomUCCCondition Clone()
        {
            return new CheckPowerState
            {
                _powerId = _powerId,
                checkState = checkState,
                Sign = Sign,
                Locked = base.Locked,
                Target = Target,
                Value = Value
            };
        }

        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = default)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion



        #region Данные
        private readonly PowerCache powerCache = new PowerCache(string.Empty);
        #endregion

        


        #region Вспомогательные методы
        public new bool IsOK(UCCAction refAction)
        {
            var pwr = powerCache.GetPower();

            switch (CheckState)
            {
                case PowerState.HasPower:
                    return pwr != null && pwr.IsValid;
                case PowerState.HasntPower:
                    return pwr == null || !pwr.IsValid;
            }

            return false;
        }

        public string TestInfos(UCCAction refAction)
        {
            var pwr = powerCache.GetPower();

            if (pwr != null && pwr.IsValid)
            {
                var pwrDef = pwr.PowerDef;
                return $"Character has Power {pwrDef.DisplayName}[{pwrDef.InternalName}].";
            }

            return $"No Power [{PowerId}] was found.";
        }

        public override string ToString()
        {
            return $"CheckPowerState : {CheckState} [{PowerId}]";
        }
        #endregion
    }
}
