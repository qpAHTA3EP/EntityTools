﻿using EntityTools.Quester.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityTools.Core.Proxies
{
    internal class QuesterConditionProxy : IQuesterConditionEngine
    {
        Astral.Quester.Classes.Condition condition;
        internal QuesterConditionProxy(Astral.Quester.Classes.Condition c)
        {
            condition = c ?? throw new ArgumentNullException();

        }
        public bool IsValid
        {
            get
            {
                if (EntityTools.Core.Initialize(condition))
                    return condition.IsValid;

                ETLogger.WriteLine(LogType.Error, "EntityToolsCore is invalid. Stop bot");

                EntityTools.StopBot();

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
                return $"{condition.GetType().Name}: not initialized!";
            }
        }

        public string Label()
        {
            if (EntityTools.Core.Initialize(condition))
                return condition.ToString();
            return condition.GetType().Name;
        }
    }
}
