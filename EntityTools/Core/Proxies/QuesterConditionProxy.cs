using System;
using Astral.Quester.Classes;
using EntityTools.Quester.Conditions;
using AcTp0Tools.Reflection;

namespace EntityTools.Core.Proxies
{
    internal class QuesterConditionProxy : IQuesterConditionEngine
    {
        Condition _condition;
        internal QuesterConditionProxy(Condition condition)
        {
            _condition = condition ?? throw new ArgumentNullException(nameof(condition));
        }
        public bool IsValid
        {
            get
            {
                if (EntityTools.Core.Initialize(_condition))
                    return _condition.IsValid;

                return false;
            }
        }

        public void Reset()
        {
            if (EntityTools.Core.Initialize(_condition))
                _condition.Reset();
        }

        public string TestInfos
        {
            get
            {
                if (EntityTools.Core.Initialize(_condition))
                    return _condition.TestInfos;
                return $"{_condition.GetType().Name} initialization failed";
            }
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

        public bool Rebase(Condition condition)
        {
            return EntityTools.Core.Initialize(condition);
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
