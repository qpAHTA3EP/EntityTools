using System;
using Astral.Quester.Classes;
using EntityTools.Quester.Conditions;
using AcTp0Tools.Reflection;

namespace EntityTools.Core.Proxies
{
    internal class QuesterConditionProxy : IQuesterConditionEngine
    {
        Condition condition;
        internal QuesterConditionProxy(Condition c)
        {
            condition = c ?? throw new ArgumentNullException();
        }
        public bool IsValid
        {
            get
            {
                if (EntityTools.Core.Initialize(condition))
                    return condition.IsValid;

                return false;
            }
        }

        public void Reset()
        {
            if (EntityTools.Core.Initialize(condition))
                condition.Reset();
        }

        public string TestInfos
        {
            get
            {
                if (EntityTools.Core.Initialize(condition))
                    return condition.TestInfos;
                return $"{condition.GetType().Name} initialization failed";
            }
        }

        public string Label()
        {
            if (EntityTools.Core.Initialize(condition))
                return condition.ToString();
            return condition.GetType().Name;
        }

        public bool Rebase(Condition condition)
        {
            return EntityTools.Core.Initialize(condition);
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
