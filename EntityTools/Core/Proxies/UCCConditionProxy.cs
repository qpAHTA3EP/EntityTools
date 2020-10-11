using System;
using Astral.Logic.UCC.Classes;
using EntityTools.Core.Interfaces;
using EntityTools.UCC.Conditions;

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

            ETLogger.WriteLine(LogType.Error, "EntityToolsCore is invalid. Stop bot", true);

            EntityTools.StopBot();

            return false;
        }

        public string Label()
        {
            if (EntityTools.Core.Initialize(condition))
                return condition.ToString();
            return condition.GetType().Name;
        }

        public string TestInfos(UCCAction refAction)
        {
            if (EntityTools.Core.Initialize(condition))
            {
                if (condition is ICustomUCCCondition iCond)
                    return iCond.TestInfos(refAction);
                return $"{condition.GetType().Name} | Result: {condition.IsOK(refAction)}!";
            }

            return $"{condition.GetType().Name}: not initialized!";
        }
    }
}
