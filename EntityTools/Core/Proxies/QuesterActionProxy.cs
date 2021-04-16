using System;
using AcTp0Tools.Reflection;
using Astral.Logic.Classes.Map;
using EntityTools.Core.Interfaces;
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
        private Action _action;

        private QuesterActionProxy()
        {
        }

        internal QuesterActionProxy(Action action)
        {
            _action = action ?? throw new ArgumentNullException(nameof(action));
            _internalConditions = _action.GetProperty<bool>("IntenalConditions");
            _internalValidity = _action.GetProperty<ActionValidity>(nameof(InternalValidity));
            _internalDestination = _action.GetProperty<Vector3>(nameof(InternalDestination));
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

        public string ActionLabel
        {
            get
            {
                if (string.IsNullOrEmpty(_label))
                {
                    if (EntityTools.Core.Initialize(_action))
                        _label = _action.ActionLabel;
                    else _label = $"{_action.GetType().Name} [uninitialized]";
                }
                return _label;
            }
        }
        string _label;

        public bool InternalConditions
        {
            get
            {
                if (EntityTools.Core.Initialize(_action))
                {
                    if (_internalConditions.IsValid)
                        return _internalConditions.Value;
                    ETLogger.WriteLine(LogType.Error, $"Invalid 'InternalConditions' accessor in Action {_action.GetType().Name}[{_action.ActionID}]", false);
                    ETLogger.WriteLine(LogType.Error, Environment.StackTrace, false);
                }
                return false;
            }
        }
        PropertyAccessor<bool> _internalConditions;

        public ActionValidity InternalValidity
        {
            get
            {
                if (EntityTools.Core.Initialize(_action))
                {
                    if (_internalValidity.IsValid)
                        return _internalValidity.Value;
                    ETLogger.WriteLine(LogType.Error, $"Invalid 'InternalValidity' accessor in Action {_action.GetType().Name}[{_action.ActionID}]", false);
                    ETLogger.WriteLine(LogType.Error, Environment.StackTrace, false);
                }
                return new ActionValidity($"{_action.GetType().Name} initialization failed");
            }
        }
        PropertyAccessor<ActionValidity> _internalValidity;

        public Vector3 InternalDestination
        {
            get
            {
                if (EntityTools.Core.Initialize(_action))
                {
                    if (_internalDestination.IsValid)
                        return _internalDestination.Value;
                    ETLogger.WriteLine(LogType.Error, $"Invalid 'InternalDestination' accessor in Action {_action.GetType().Name}[{_action.ActionID}]", false);
                    ETLogger.WriteLine(LogType.Error, Environment.StackTrace, false);
                }

                return Vector3.Empty;
            }
        }
        PropertyAccessor<Vector3> _internalDestination;

        public bool UseHotSpots
        {
            get
            {
                if (EntityTools.Core.Initialize(_action))
                    return _action.UseHotSpots;
                return false;
            }
        }

        public void GatherInfos()
        {
            if (EntityTools.Core.Initialize(_action))
                _action.GatherInfos();
        }

        public void InternalReset()
        {
            if (EntityTools.Core.Initialize(_action))
                _action.InternalReset();
        }

        public void OnMapDraw(GraphicsNW graph)
        {
            if (EntityTools.Core.Initialize(_action))
                _action.OnMapDraw(graph);
        }

        public ActionResult Run()
        {
            if (EntityTools.Core.Initialize(_action))
                return _action.Run();

            return ActionResult.Fail;
        }

        public bool Rebase(Action action)
        {
            return EntityTools.Core.Initialize(action);
        }

        public void Dispose()
        {
            if (_action != null)
            {
                ReflectionHelper.SetFieldValue(_action, "Engine", null);
                _action = null;
            }
        }

        public void OnPropertyChanged(Action sender, string propertyName) { }
    }
}
