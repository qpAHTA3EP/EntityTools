using System;
using Astral.Logic.UCC.Classes;
using EntityTools.Core.Interfaces;
using AcTp0Tools.Reflection;
using EntityTools.UCC.Conditions;

namespace EntityTools.Core.Proxies
{
    public class UccConditionProxy : IUccConditionEngine
    {
        private UCCCondition _condition;

        public UccConditionProxy(UCCCondition uccCondition)
        {
            _condition = uccCondition ?? throw new ArgumentNullException(nameof(uccCondition));
        }

        public bool IsOK(UCCAction refAction)
        {
            if (EntityTools.Core.Initialize(_condition))
                return _condition.IsOK(refAction);

#if false
            ETLogger.WriteLine(LogType.Error, "EntityToolsCore is invalid. Stop bot", true);

            EntityTools.StopBot(); 
#endif

            return false;
        }

        public string Label()
        {
            if (string.IsNullOrEmpty(_label))
            {
                if (EntityTools.Core.Initialize(_condition))
                    _label = _condition.ToString();
                else _label = $"{_condition.GetType().Name} [uninitialized]"; 
            }
            return _label;
        }
        string _label;

        public string TestInfos(UCCAction refAction)
        {
            if (EntityTools.Core.Initialize(_condition))
            {
                if (_condition is ICustomUCCCondition iCond)
                    return iCond.TestInfos(refAction);
                return $"{_condition.GetType().Name} | Result: {_condition.IsOK(refAction)}!";
            }

            return $"{_condition.GetType().Name}: not initialized!";
        }

        public bool Rebase(UCCCondition uccCondition)
        {
            return EntityTools.Core.Initialize(uccCondition);
        }

        public void Dispose()
        {
            if (_condition != null)
            {
                ReflectionHelper.SetFieldValue(_condition, "Engine", null);
                _condition = null;
            }
        }
    }
}
