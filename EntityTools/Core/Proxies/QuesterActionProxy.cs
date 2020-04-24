using Astral.Logic.Classes.Map;
using EntityTools.Core.Interfaces;
using EntityTools.Reflection;
using System.Reflection;
using MyNW.Classes;
using System;
using static Astral.Quester.Classes.Action;

namespace EntityTools.Core.Proxies
{
    /// <summary>
    /// Класс-заместитель, инициирующий ядро команды Quester.Action 
    /// </summary>
    internal sealed class QuesterActionProxy : IQuesterActionEngine
    {
        private Astral.Quester.Classes.Action action;

        private QuesterActionProxy() { }

        internal QuesterActionProxy(Astral.Quester.Classes.Action a)
        {
            action = a ?? throw new ArgumentNullException();
        }

        public bool NeedToRun
        {
            get
            {
                if (EntityTools.Core.Initialize(action))
                    return action.NeedToRun;

                ETLogger.WriteLine(LogType.Error, $"EntityToolsCore is invalid. Stop bot", true);

                EntityTools.StopBot();

                return false;
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
                    if (ReflectionHelper.GetPropertyValue(action, "InternalConditions", out object result, BindingFlags.Instance | BindingFlags.NonPublic)
                        && result != null)
                        return result.Equals(true);

                ETLogger.WriteLine(LogType.Error, $"EntityToolsCore is invalid. Stop bot", true);

                EntityTools.StopBot();

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

                ETLogger.WriteLine(LogType.Error, $"EntityToolsCore is invalid. Stop bot", true);

                EntityTools.StopBot();

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

            ETLogger.WriteLine(LogType.Error, $"EntityToolsCore is invalid. Stop bot", true);

            EntityTools.StopBot();

            return ActionResult.Fail;
        }
    }
}
