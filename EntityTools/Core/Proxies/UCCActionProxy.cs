using System;
using System.Reflection;
using Astral.Logic.UCC.Classes;
using EntityTools.Core.Interfaces;
using AcTp0Tools.Reflection;
using MyNW.Classes;

namespace EntityTools.Core.Proxies
{
    public class UccActionProxy : IUccActionEngine
    {
        private UCCAction _action;

        internal UccActionProxy(UCCAction uccAction)
        {
            _action = uccAction ?? throw new ArgumentNullException(nameof(uccAction));
            _unitRef = uccAction.GetProperty<Entity>(nameof(UnitRef));
        }
        public bool NeedToRun
        {
            get
            {
                if (EntityTools.Core.Initialize(_action))
                    return _action.NeedToRun;

                return false;
            }
        }

        public Entity UnitRef
        {
            get
            {
                if (EntityTools.Core.Initialize(_action))
                {
                    if (_unitRef.IsValid)
                        return _unitRef.Value;
                    ETLogger.WriteLine(LogType.Error, "Invalid 'UnitRef' accessor");
                    ETLogger.WriteLine(LogType.Error, Environment.StackTrace);
                }
                return Empty.Entity;
            }
        }
        PropertyAccessor<Entity> _unitRef;

        public string Label()
        {
            if (string.IsNullOrEmpty(_label))
            {
                if (EntityTools.Core.Initialize(_action))
                    _label = _action.ToString();
                else _label = $"{_action.GetType().Name} [uninitialized]"; 
            }
            return _label;
        }
        string _label;

        public bool Run()
        {
            if (EntityTools.Core.Initialize(_action))
                return _action.Run();

            return false;
        }

        public bool Rebase(UCCAction uccAction)
        {
            return EntityTools.Core.Initialize(uccAction);
        }

        public void Dispose()
        {
            if (_action != null)
            {
                ReflectionHelper.SetFieldValue(_action, "Engine", null);
                _action = null;
            }
        }
    }
}
