using Astral.Logic.UCC.Classes;
using EntityTools.Core.Interfaces;
using EntityTools.UCC.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityTools.Core.Proxies
{
    public class UCCConditionProxy : IUCCConditionEngine
    {
        private UCCCondition condition;

        public UCCConditionProxy(UCCCondition c)
        {
            condition = c ?? throw new ArgumentNullException();
        }

        public bool IsOK(UCCAction refAction)
        {
            if (EntityTools.Core.Initialize(condition))
                return condition.IsOK(refAction);

            ETLogger.WriteLine(LogType.Error, $"EntityToolsCore is invalid. Stop bot", true);

            EntityTools.StopBot();

            return false;
        }

        public string Label()
        {
            if (EntityTools.Core.Initialize(condition))
                return condition.ToString();
            else return condition.GetType().Name;
        }

        public string TestInfos(UCCAction refAction)
        {
            if (EntityTools.Core.Initialize(condition))
            {
                if (condition is ICustomUCCCondition iCond)
                    return iCond.TestInfos(refAction);
                else return $"{condition.GetType().Name} | Result: {condition.IsOK(refAction)}!";
            }
            else return $"{condition.GetType().Name}: not initialized!";
        }
    }
}
