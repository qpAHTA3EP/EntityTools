using System;
using Astral.Logic.UCC.Classes;
using EntityTools.Core.Interfaces;
using AcTp0Tools.Reflection;
using EntityTools.UCC.Conditions;

namespace EntityTools.Core.Proxies
{
    public class UccConditionProxy : IUccConditionEngine
    {
        private UCCCondition condition;

        public UccConditionProxy(UCCCondition c)
        {
            condition = c ?? throw new ArgumentNullException();
        }

        public bool IsOK(UCCAction refAction)
        {
            if (EntityTools.Core.Initialize(condition))
                return condition.IsOK(refAction);

#if false
            ETLogger.WriteLine(LogType.Error, "EntityToolsCore is invalid. Stop bot", true);

            EntityTools.StopBot(); 
#endif

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

        public bool Rebase(UCCCondition uccCondition)
        {
            return EntityTools.Core.Initialize(uccCondition);
        }

        public void Dispose()
        {
            if (condition != null)
            {
                ReflectionHelper.SetFieldValue(condition, "Engine", null);
                condition = null;
            }
        }
    }
}
