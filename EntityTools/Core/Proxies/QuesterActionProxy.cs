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
            _internalConditions = action.GetProperty<bool>(nameof(InternalConditions));
            _internalValidity = action.GetProperty<ActionValidity>(nameof(InternalValidity));
            _internalDestination = action.GetProperty<Vector3>(nameof(InternalDestination));
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
                    if (_internalConditions.IsValid)
                        return _internalConditions.Value;
                    ETLogger.WriteLine(LogType.Error, $"Invalid 'InternalConditions' accessor in Action {action.GetType().Name}[{action.ActionID}]", false);
                    ETLogger.WriteLine(LogType.Error, Environment.StackTrace, false);
                }
                return false;
            }
        }
        InstancePropertyAccessor<bool> _internalConditions;

        public ActionValidity InternalValidity
        {
            get
            {
                if (EntityTools.Core.Initialize(action))
                {
                    if (_internalValidity.IsValid)
                        return _internalValidity.Value;
                    ETLogger.WriteLine(LogType.Error, $"Invalid 'InternalValidity' accessor in Action {action.GetType().Name}[{action.ActionID}]", false);
                    ETLogger.WriteLine(LogType.Error, Environment.StackTrace, false);
                }
                return new ActionValidity($"{action.GetType().Name} initialization failed");
            }
        }
        InstancePropertyAccessor<ActionValidity> _internalValidity;

        public Vector3 InternalDestination
        {
            get
            {
                if (EntityTools.Core.Initialize(action))
                {
                    if (_internalDestination.IsValid)
                        return _internalDestination.Value;
                    ETLogger.WriteLine(LogType.Error, $"Invalid 'InternalDestination' accessor in Action {action.GetType().Name}[{action.ActionID}]", false);
                    ETLogger.WriteLine(LogType.Error, Environment.StackTrace, false);
                }

                return Vector3.Empty;
            }
        }
        InstancePropertyAccessor<Vector3> _internalDestination;

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

            return ActionResult.Fail;
        }

        public bool Rebase(Action action)
        {
            return EntityTools.Core.Initialize(action);
        }

        public void Dispose()
        {
            if (action != null)
            {
                ReflectionHelper.SetFieldValue(action, "Engine", null);
                action = null;
            }
        }

        public void OnPropertyChanged(Action sender, string propertyName) { }
    }
}
