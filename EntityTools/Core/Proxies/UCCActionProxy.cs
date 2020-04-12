using EntityTools.Core.Interfaces;
using EntityTools.Reflection;
using MyNW.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace EntityTools.Core.Proxies
{
    public class UCCActionProxy : IUCCActionEngine
    {
        private Astral.Logic.UCC.Classes.UCCAction action;

        internal UCCActionProxy(Astral.Logic.UCC.Classes.UCCAction a)
        {
            action = a ?? throw new ArgumentNullException();
        }
        public bool NeedToRun
        {
            get
            {
                if (EntityTools.Core.Initialize(action))
                    return action.NeedToRun;
                else return false;
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
            else return action.GetType().Name;
        }

        public bool Run()
        {
            if (EntityTools.Core.Initialize(action))
                return action.Run();
            else return false;
        }
    }
}
