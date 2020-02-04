using Astral.Logic.UCC.Classes;
using Astral.Logic.UCC.Ressources;
using System.ComponentModel;
using System.Xml.Serialization;
using Astral.Quester.Classes;
using EntityTools.Editors;
using System.Drawing.Design;
using System;
using Sign = Astral.Logic.UCC.Ressources.Enums.Sign;
using EntityTools.Enums;
using EntityTools.Tools;

namespace EntityTools.UCC.Conditions
{
    public class UCCCheckTimer : UCCCondition, ICustomUCCCondition
    {
        [Category("Timer")]
        public string TimerName { get; set; } = string.Empty;

        [Category("Timer")]
        [Description("Time (ms) compared to Timer starts time\n" +
            "Left: Check the amount of time remaining before the timer expiration\n" +
            "Passed: Check the time passed from the timer starts")]
        public TestTimer TestTimer { get; set; } = TestTimer.Left;

        [Category("Timer")]
        [Description("Time (ms) compared to Timer starts time")]
        public uint Time { get; set; } = 0;

        [Category("Timer")]
        [Description("Comparison type for Time")]
        public new Sign Sign { get; set; }


        #region ICustomUCCCondition
        bool ICustomUCCCondition.IsOK(UCCAction refAction = null)
        {
            if(!string.IsNullOrEmpty(TimerName) && UCCTools.SpecialTimers.ContainsKey(TimerName))
            {
                switch (TestTimer)
                {
                    case TestTimer.Left:
                        int endTickCount = UCCTools.SpecialTimers[TimerName].Second;
                        int left = (endTickCount > Environment.TickCount) ? endTickCount - Environment.TickCount : 0;

                        switch (Sign)
                        {
                            case Sign.Equal:
                                return left == Time;
                            case Sign.NotEqual:
                                return left != Time;
                            case Sign.Inferior:
                                return left < Time;
                            case Sign.Superior:
                                return left > Time;
                        }
                        break;
                    case TestTimer.Passed:
                        int passed = UCCTools.SpecialTimers[TimerName].First - Environment.TickCount;

                        switch (Sign)
                        {
                            case Sign.Equal:
                                return passed == Time;
                            case Sign.NotEqual:
                                return passed != Time;
                            case Sign.Inferior:
                                return passed < Time;
                            case Sign.Superior:
                                return passed > Time;
                        }
                        break;
                }
            }
            else
            {
                // Поскольку таймер на задан, принимаем, что таймаут истек (до окончания 0 мс), а
                // время с момента старта таймера 0 мс
                switch (Sign)
                {
                    case Sign.Equal:
                        return Time == 0;
                    case Sign.NotEqual:
                        return Time != 0;
                    case Sign.Inferior:
                        return false;
                    case Sign.Superior:
                        return true;
                }
            }
            return false;
        }

        bool ICustomUCCCondition.Loked { get => base.Locked; set => base.Locked = value; }

        string ICustomUCCCondition.TestInfos(UCCAction refAction)
        {
            if (!string.IsNullOrEmpty(TimerName) && UCCTools.SpecialTimers.ContainsKey(TimerName))
            {
                Pair<int, int> p = UCCTools.SpecialTimers[TimerName];
                int left = (p.Second > Environment.TickCount) ? p.Second - Environment.TickCount : 0;
                int passed = p.First - Environment.TickCount;

                return $"Timer '{TimerName}' started at {DateTime.Now.Subtract(new TimeSpan(left)).ToLongTimeString()}\n" +
                    $"\tLeft: {left} ms\n" +
                    $"\tPassed: {passed} ms";
            }
            else return $"Timer '{TimerName}' was not initialized";
        }
        #endregion

        public override string ToString()
        {
            return string.Concat("CheckTimer '", TimerName, '\'');
        }

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
        #endregion
    }
}
