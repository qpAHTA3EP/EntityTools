using System;
using System.Reflection;
using Astral.Logic.UCC.Classes;
using EntityTools.Core.Interfaces;
using EntityTools.Reflection;
using MyNW.Classes;

namespace EntityTools.Core.Proxies
{
    public class UCCActionProxy : IUCCActionEngine
    {
        private UCCAction action;

        internal UCCActionProxy(UCCAction a)
        {
            action = a ?? throw new ArgumentNullException();
        }
        public bool NeedToRun
        {
            get
            {
                if (EntityTools.Core.Initialize(action))
                    return action.NeedToRun;

                ETLogger.WriteLine(LogType.Error, "EntityToolsCore is invalid. Stop bot", true);

                EntityTools.StopBot();

                return false;
            }
        }

        public Entity UnitRef
        {
            get
            {
                if (EntityTools.Core.Initialize(action))
                    if (ReflectionHelper.GetPropertyValue(action, "UnitRef", out object result, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                        && result is Entity entity)
                        return entity;
                return new Entity(IntPtr.Zero);
            }
        }

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

            ETLogger.WriteLine(LogType.Error, "EntityToolsCore is invalid. Stop bot", true);

            EntityTools.StopBot();

            return false;
        }
    }
}
