using System;
using System.Reflection;
using Astral.Logic.Classes.Map;
using Astral.Quester.Classes;
using EntityTools.Core.Interfaces;
using EntityTools.Reflection;
using MyNW.Classes;
using static Astral.Quester.Classes.Action;
using Action = Astral.Quester.Classes.Action;

namespace EntityTools.Core.Proxies
{
    /// <summary>
    /// Класс-заместитель, инициирующий ядро команды Quester.Action 
    /// </summary>
    internal sealed class QuesterActionProxy : IQuesterActionEngine
    {
        private Action action;

        private QuesterActionProxy() { }

        internal QuesterActionProxy(Action a)
        {
            action = a ?? throw new ArgumentNullException();
            _internalConditions = action.GetInstanceProperty<Action, bool>(nameof(InternalConditions));
            _internalValidity = action.GetInstanceProperty<Action, ActionValidity>(nameof(InternalValidity));
            _internalDestination = action.GetInstanceProperty<Action, Vector3>(nameof(InternalDestination));
        }

        public bool NeedToRun
        {
            get
            {
                if (EntityTools.Core.Initialize(action))
                    return action.NeedToRun;

#if false
                ETLogger.WriteLine(LogType.Error, "EntityToolsCore is invalid. Stop bot", true);

                EntityTools.StopBot(); 
#endif

                return false;
            }
        }

        public string ActionLabel
        {
            get
            {
                if (EntityTools.Core.Initialize(action))
                    return action.ActionLabel;
                return action.GetType().Name;
            }
        }

        public bool InternalConditions
        {
            get
            {
                if (EntityTools.Core.Initialize(action))
                {
#if false
                    if (ReflectionHelper.GetPropertyValue(action, "InternalConditions", out object result))
                        return result.Equals(true); 
#else
                    if (_internalConditions.IsValid)
                        return _internalConditions.Value;
                    ETLogger.WriteLine(LogType.Error, $"Invalid 'InternalConditions' accessor in Action {action.GetType().Name}[{action.ActionID}]", false);
                    ETLogger.WriteLine(LogType.Error, Environment.StackTrace, false);
#endif
                }
#if false
                else ETLogger.WriteLine(LogType.Error, "EntityToolsCore is invalid. Stop bot", true);
                EntityTools.StopBot(); 
#endif
                return false;
            }
        }
        InstancePropertyAccessor<Action, bool> _internalConditions;

        public ActionValidity InternalValidity
        {
            get
            {
                if (EntityTools.Core.Initialize(action))
                {
#if false
                    if (ReflectionHelper.GetPropertyValue(action, "InternalValidity", out object result))
                        return result as ActionValidity; 
#else
                    if (_internalValidity.IsValid)
                        return _internalValidity.Value;
                    ETLogger.WriteLine(LogType.Error, $"Invalid 'InternalValidity' accessor in Action {action.GetType().Name}[{action.ActionID}]", false);
                    ETLogger.WriteLine(LogType.Error, Environment.StackTrace, false);
#endif
                }
#if false
                else ETLogger.WriteLine(LogType.Error, "EntityToolsCore is invalid. Stop bot", true); 
#endif

                return new ActionValidity($"{action.GetType().Name} initialization failed");
            }
        }
        InstancePropertyAccessor<Action, ActionValidity> _internalValidity;

        public Vector3 InternalDestination
        {
            get
            {
                if (EntityTools.Core.Initialize(action))
                {
#if false
                    if (ReflectionHelper.GetPropertyValue(action, "InternalDestination", out object result))
                        return result as Vector3; 
#else
                    if (_internalDestination.IsValid)
                        return _internalDestination.Value;
                    ETLogger.WriteLine(LogType.Error, $"Invalid 'InternalDestination' accessor in Action {action.GetType().Name}[{action.ActionID}]", false);
                    ETLogger.WriteLine(LogType.Error, Environment.StackTrace, false);

#endif
                }
#if false
                else ETLogger.WriteLine(LogType.Error, "EntityToolsCore is invalid. Stop bot", true);

                EntityTools.StopBot(); 
#endif

                return Vector3.Empty;
            }
        }
        InstancePropertyAccessor<Action, Vector3> _internalDestination;

        public bool UseHotSpots
        {
            get
            {
                if (EntityTools.Core.Initialize(action))
                    return action.UseHotSpots;
                return false;
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
#if false
            ETLogger.WriteLine(LogType.Error, "EntityToolsCore is invalid. Stop bot", true);

            EntityTools.StopBot(); 
#endif
            return ActionResult.Fail;
        }

        public bool Rebase(Action action)
        {
            return EntityTools.Core.Initialize(action);
        }
    }
}
