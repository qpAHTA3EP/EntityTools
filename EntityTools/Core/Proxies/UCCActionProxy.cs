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
        private UCCAction action;

        internal UccActionProxy(UCCAction a)
        {
            action = a ?? throw new ArgumentNullException();
            _unitRef = a.GetProperty<Entity>(nameof(UnitRef));
        }
        public bool NeedToRun
        {
            get
            {
                if (EntityTools.Core.Initialize(action))
                    return action.NeedToRun;

                return false;
            }
        }

        public Entity UnitRef
        {
            get
            {
                if (EntityTools.Core.Initialize(action))
                {
                    if (_unitRef.IsValid)
                        return _unitRef.Value;
                    ETLogger.WriteLine(LogType.Error, "Invalid 'UnitRef' accessor");
                    ETLogger.WriteLine(LogType.Error, Environment.StackTrace);
                }
                return Empty.Entity;
            }
        }
        Property<Entity> _unitRef;

        public string Label()
        {
            if (EntityTools.Core.Initialize(action))
                return action.ToString();
            return action.GetType().Name;
        }

        public bool Run()
        {
            if (EntityTools.Core.Initialize(action))
                return action.Run();

            return false;
        }

        public bool Rebase(UCCAction uccAction)
        {
            return EntityTools.Core.Initialize(uccAction);
        }

        public void Dispose()
        {
            if (action != null)
            {
                ReflectionHelper.SetFieldValue(action, "Engine", null);
                action = null;
            }
        }
    }
}
