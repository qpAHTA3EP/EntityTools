using Astral.Logic.Classes.Map;
using Astral.Quester.Classes;
using EntityTools.Core.Interfaces;
using EntityTools.Reflection;
using System.Reflection;
using MyNW.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Astral.Quester.Classes.Action;

namespace EntityTools.Core.Facades
{
    /// <summary>
    /// Класс-заглушка, инициирующий ядро команды Quester.Action 
    /// </summary>
    internal class QuesterActionInitializer : IQuesterActionEngine
    {
        private Astral.Quester.Classes.Action action;

        internal QuesterActionInitializer(Astral.Quester.Classes.Action a)
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

        public string ActionLabel
        {
            get
            {
                if (EntityTools.Core.Initialize(action))
                    return action.ActionLabel;
                else return action.GetType().Name;
            }
        }

        public bool InternalConditions
        {
            get
            {
                if (EntityTools.Core.Initialize(action))
                    if(ReflectionHelper.GetPropertyValue(action, "InternalConditions", out object result, BindingFlags.Instance | BindingFlags.NonPublic)
                        && result != null)
                        return result.Equals(true);
                return false;
            }
        }

        public ActionValidity InternalValidity
        {
            get
            {
                if (EntityTools.Core.Initialize(action))
                    if (ReflectionHelper.GetPropertyValue(action, "InternalValidity", out object result, BindingFlags.Instance | BindingFlags.NonPublic)
                        && result != null)
                        return result as ActionValidity;
                return new ActionValidity($"{action.GetType().Name} not valid");
            }
        }

        public Vector3 InternalDestination
        {
            get
            {
                if (EntityTools.Core.Initialize(action))
                    if (ReflectionHelper.GetPropertyValue(action, "InternalDestination", out object result, BindingFlags.Instance | BindingFlags.NonPublic)
                        && result != null)
                        return result as Vector3;
                return new Vector3();
            }
        }
        public bool UseHotSpots
        {
            get
            {
                if (EntityTools.Core.Initialize(action))
                    return action.UseHotSpots;
                else return false;
            }
        }

        public void GatherInfos()
        {
            if (EntityTools.Core.Initialize(action))
                action.GatherInfos();
        }

        public void InternalReset()
        {
            if (EntityTools.Core.Initialize(action))
                action.InternalReset();
        }

        public void OnMapDraw(GraphicsNW graph)
        {
            if (EntityTools.Core.Initialize(action))
                action.OnMapDraw(graph);
        }

        public ActionResult Run()
        {
            if (EntityTools.Core.Initialize(action))
                return action.Run();
            else return ActionResult.Fail;
        }
    }
}
