using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Reflection;
using AStar;
using Astral.Classes;
using Astral.Classes.ItemFilter;
using Astral.Grinder.Classes;
using Astral.Logic.Classes.Map;
using HarmonyLib;
using MyNW.Classes;

namespace EntityTools.Reflection
{
    /// <summary>
    /// Доступ к закрытым членам и методам Astral'a
    /// </summary>
    public static class AstralAccessors
    {
        public static class Addons
        {
            /// <summary>
            /// доступ к членам и методам класса 
            /// Astral.Addons.Role
            /// </summary>
            public static class Role
            {
                public static readonly Func<object, Action> OnLoad = typeof(Astral.Addons.Role).GetAction("OnLoad");
                public static readonly Func<object, Action> OnUnload = typeof(Astral.Addons.Role).GetAction("OnUnload");
                public static readonly Func<object, Action<GraphicsNW>> OnMapDraw = typeof(Astral.Addons.Role).GetAction<GraphicsNW>("OnMapDraw");
                public static readonly Func<object, Action<bool>> Start = typeof(Astral.Addons.Role).GetAction<bool>("Start");
                public static readonly Func<object, Action> Stop = typeof(Astral.Addons.Role).GetAction("Stop");
                public static readonly Func<object, Action> TooMuchStuckReaction = typeof(Astral.Addons.Role).GetAction("TooMuchStuckReaction");
            }
        }

        /// <summary>
        /// доступ к членам и методам класса 
        /// Astral.Logic.NW.VIP
        /// </summary>
        public static class VIP
        {
            static readonly Func<string, Entity> getNearestEntityByCostume = typeof(Astral.Logic.NW.VIP).GetStaticFunction<string, Entity>("GetNearestEntityByCostume");
            public static Entity GetNearestEntityByCostume(string costume)
            {
                return getNearestEntityByCostume?.Invoke(costume);
            }
        }

        /// <summary>
        /// Доступ к закрытым членам Astral.Quester
        /// </summary>
        public static class Quester
        {
#if false
            public static class Action
            {
                public static readonly InstancePropertyAccessor<Astral.Quester.Classes.Action, Astral.Quester.Classes.ActionDebug> ActionDebug = null;

                static Action()
                {
                    ActionDebug = typeof(Astral.Quester.Classes.Action).GetInstanceProperty<Astral.Quester.Classes.ActionDebug>("Debug");
                }
            } 
#endif
            public static class FSM
            {
                public static class States
                {
                    public static class Combat
                    {
                        static readonly StaticFieldAccessor<int> ignoreCombatMinHP =
                            typeof(Astral.Quester.FSM.States.Combat).GetStaticField<int>("ignoreCombatMinHP");
                        public static int IgnoreCombatMinHP
                        {
                            get => ignoreCombatMinHP.Value;
                            set => ignoreCombatMinHP.Value = value;
                        }

                        static Action<bool, int, int> setIgnoreCombat;
                        /// <summary>
                        /// Установка параметров, управляющих режимом игнорирования боя IgnoreCombat
                        /// </summary>
                        /// <param name="value"></param>
                        /// <param name="minHP">Минимальное значени HP, при котором бой принудительно активируется</param>
                        /// <param name="time">Продолжительность времени в течение которого игнорируются атаки</param>
                        public static void SetIgnoreCombat(bool value, int minHP = -1, int time = 0)
                        {
                            setIgnoreCombat?.Invoke(value, minHP, time);
                        }

                        static Combat()
                        {
                            Type combatType = typeof(Astral.Quester.FSM.States.Combat);
                            if (combatType != null)
                                setIgnoreCombat = combatType.GetStaticAction<bool, int, int>(nameof(SetIgnoreCombat));
                        }
                    }
                }
            }

            /// <summary>
            /// Функтор доступа к графу 
            /// </summary>     
            public static class Core
            {
                //TODO: Пропатчить методы доступа к графу, чтобы геттер устанавливал ReadLock, а Setter - WriteLock
                /// <summary>
                /// Функтор доступа к графу путей (карте) текущего профиля
                /// </summary>
                public static readonly StaticPropertyAccessor<Graph> Meshes = typeof(Astral.Quester.Core).GetStaticProperty<Graph>("Meshes");

                public static Graph UsedMeshes
                {
                    get
                    {
                        if (Meshes.IsValid())
                            return Meshes.Value;
                        return null;
                    }
                }

                /// <summary>
                /// Функтор доступа к коллекции графов путей (карт) текущего профиля
                /// Astral.Quester.Core.MapsMeshes
                /// </summary>
                public static readonly StaticPropertyAccessor<Dictionary<string, Graph>> MapsMeshes = typeof(Astral.Quester.Core).GetStaticProperty<Dictionary<string, Graph>>("MapsMeshes");

                /// <summary>
                /// Функтор доступа к списку названий карт в файле текущего профиля
                /// Astral.Quester.Core.AvailableMeshesFromFile(openFileDialog.FileName)
                /// </summary>
                public static readonly Func<string, List<string>> AvailableMeshesFromFile = typeof(Astral.Quester.Core).GetStaticFunction<string, List<string>>("AvailableMeshesFromFile");

                /// <summary>
                /// Доступ к методу 
                /// Astral.Quester.Core.LoadAllMeshes();
                /// </summary>
                public static readonly Func<int> LoadAllMeshes = typeof(Astral.Quester.Core).GetStaticFunction<int>("LoadAllMeshes");

                /// <summary>
                /// Доступ к методу удаления вершин крафаг путей в области, заданной центром окружности и её радиусом
                /// Astral.Quester.Core.RemoveNodesFrom2DPostion(worldPos, Astral.Controllers.Settings.Get.DeleteNodeRadius);
                /// </summary>                
                public static readonly Action<Vector3, double> RemoveNodesFrom2DPosition = typeof(Astral.Quester.Core).GetStaticAction<Vector3, double>("RemoveNodesFrom2DPostion");
            }

            public static class Entrypoint
            {
                public static readonly Func<object, Action> OnLoad = typeof(Astral.Quester.Entrypoint).GetAction("OnLoad");
                public static readonly Func<object, Action> OnUnload = typeof(Astral.Quester.Entrypoint).GetAction("OnUnload");
                public static readonly Func<object, Action<GraphicsNW>> OnMapDraw = typeof(Astral.Quester.Entrypoint).GetAction<GraphicsNW>("OnMapDraw");
                public static readonly Func<object, Action<bool>> Start = typeof(Astral.Quester.Entrypoint).GetAction<bool>("Start");
                public static readonly Func<object, Action> Stop = typeof(Astral.Quester.Entrypoint).GetAction("Stop");
                public static readonly Func<object, Action> TooMuchStuckReaction = typeof(Astral.Quester.Entrypoint).GetAction("TooMuchStuckReaction");
            }

            public static class Forms
            {
                public static class Editor
                {
                    /// <summary>
                    /// Функтор доступа к экземпляру Квестер-редактора
                    /// Astral.Quester.Forms.Main.editorForm
                    /// </summary>
                    static readonly StaticFieldAccessor<Astral.Quester.Forms.Editor> editorForm = typeof(Astral.Quester.Forms.Main).GetStaticField<Astral.Quester.Forms.Editor>("editorForm");
                    public static Astral.Quester.Forms.Editor EditorForm => editorForm.Value;

                    /// <summary>
                    /// Функтор обновления списка CustomRegion'ов в окне Квестер-редактора
                    /// </summary>
                    private static Func<object, System.Action> QuesterEditor_RefreshRegions = null;
                    public static void RefreshRegions()
                    {
                        if (editorForm.Value is Astral.Quester.Forms.Editor editor
                            && !editor.IsDisposed)
                        {
                            if (QuesterEditor_RefreshRegions == null)
                            {
                                if ((QuesterEditor_RefreshRegions = typeof(Astral.Quester.Forms.Editor).GetAction("RefreshRegions")) != null)
                                    QuesterEditor_RefreshRegions(editor)();
                            }
                            else QuesterEditor_RefreshRegions(editor)();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Доступ к закрытым членам Astral.Grinder
        /// </summary>
        public static class Grinder
        {
            public static class Core
            {
                static readonly StaticPropertyAccessor<GrinderProfile> profile = typeof(Astral.Grinder.Core).GetStaticProperty<GrinderProfile>("Profile");
                public static GrinderProfile Profile => profile.Value;
            }
        }

        /// <summary>
        /// Доступ к закрытым членам Astral.Controllers
        /// </summary>
        public static class Controllers
        {
            // Ошибка доступа времени исполнения
            public static class Roles
            {
#if false
                public static readonly StaticPropertyAccessor<Astral.Addons.Role> CurrentRole;
                static Roles()
                {
                    Type type = Assembly.GetEntryAssembly()?.GetType("Astral.Controllers.Roles");//typeof(Astral.Controllers.Roles);
                    if (type != null)
                        CurrentRole = type.GetStaticProperty<Astral.Addons.Role>("CurrentRole");
                } 
#endif

                /// <summary>
                /// Объект, соответствующий текущей роли Астрала
                /// </summary>
                private static Traverse _currentRole;
                private static object _currentRoleObject;

#if false
                /// <summary>
                /// Вызов метода CurrentRole.OnMapDraw(GraphicsNW graphicsNW)
                /// </summary>
                /// <param name="graphicsNW"></param>
                public static bool CurrentRole_OnMapDraw(GraphicsNW graphicsNW)
                {
                    object role = _currentRole.GetValue();
                    if (role is null)
                    {
                        _currentRoleObject = null;
                        _currentRole_OnMapDraw = null;
                        return false;
                    }
                    if (role != _currentRoleObject)
                    {
                        _currentRoleObject = role;
                        _currentRole_OnMapDraw = Traverse.Create(_currentRoleObject).Method("OnMapDraw", graphicsNW);
                    }
                    if (_currentRoleObject != null && _currentRole_OnMapDraw is null)
                        _currentRole_OnMapDraw = Traverse.Create(_currentRoleObject).Method("OnMapDraw", graphicsNW);

                    if (_currentRole_OnMapDraw != null)
                    {
                        _currentRole_OnMapDraw.GetValue(graphicsNW);
                        return true;
                    }
                    return false;
                }
                private static Traverse _currentRole_OnMapDraw;

                /// <summary>
                /// Чтение имени роли, выполняемой Астралом
                /// </summary>
                public static string CurrentRole_Name
                {
                    get
                    {
                        object role = _currentRole.GetValue();
                        if (role is null)
                        {
                            _currentRoleObject = null;
                            _currentRole_Name = null;
                            return string.Empty;
                        }
                        if (role != _currentRoleObject || _currentRole_Name == null)
                        {
                            _currentRoleObject = role;
                            _currentRole_Name = Traverse.Create(_currentRoleObject).Property<string>("Name");
                        }
                        if (_currentRole_Name != null)
                            return _currentRole_Name.Value;
                        return string.Empty;
                    }
                }
                private static Traverse<string> _currentRole_Name; 
#endif

                public static class CurrentRole
                {
                    public static Graph UsedMeshes
                    {
                        get
                        {
                            object role = _currentRole.GetValue();
                            if (role is null)
                            {
                                ResetTraverses();
                                return null;
                            }
                            if (role != _currentRoleObject || _usedMeshes == null)
                            {
                                _currentRoleObject = role;
                                _usedMeshes = Traverse.Create(_currentRoleObject).Property<Graph>("UsedMeshes");
                            }

                            return _usedMeshes?.Value;
                        }
                    }
                    private static Traverse<Graph> _usedMeshes;

                    /// <summary>
                    /// Вызов метода CurrentRole.OnMapDraw(GraphicsNW graphicsNW)
                    /// </summary>
                    /// <param name="graphicsNW"></param>
                    public static bool OnMapDraw(GraphicsNW graphicsNW)
                    {
                        object role = _currentRole.GetValue();
                        if (role is null)
                        {
                            ResetTraverses();
                            return false;
                        }
                        if (role != _currentRoleObject)
                        {
                            _currentRoleObject = role;
                            _OnMapDraw = Traverse.Create(_currentRoleObject).Method("OnMapDraw", graphicsNW);
                        }
                        if (_currentRoleObject != null && _OnMapDraw is null)
                            _OnMapDraw = Traverse.Create(_currentRoleObject).Method("OnMapDraw", graphicsNW);

                        if (_OnMapDraw != null)
                        {
                            _OnMapDraw.GetValue(graphicsNW);
                            return true;
                        }
                        return false;
                    }
                    private static Traverse _OnMapDraw;

                    /// <summary>
                    /// Чтение имени роли, выполняемой Астралом
                    /// </summary>
                    public static string Name
                    {
                        get
                        {
                            object role = _currentRole.GetValue();
                            if (role is null)
                            {
                                ResetTraverses();
                                return string.Empty;
                            }
                            if (role != _currentRoleObject || _name == null)
                            {
                                _currentRoleObject = role;
                                _name = Traverse.Create(_currentRoleObject).Property<string>("Name");
                            }
                            if (_name != null)
                                return _name.Value;
                            return string.Empty;
                        }
                    }
                    private static Traverse<string> _name;

                    static CurrentRole()
                    {
                        _currentRoleObject = _currentRole.GetValue();
                        if (_currentRoleObject != null)
                        {
                            _name = Traverse.Create(_currentRoleObject).Property<string>("Name");
                        }
                    }

                    private static void ResetTraverses()
                    {
                        _currentRoleObject = null;
                        _usedMeshes = null;
                        _OnMapDraw = null;
                        _name = null;
                    }
                }

                static Roles()
                {
                    Type type = Assembly.GetEntryAssembly()?.GetType("Astral.Controllers.Roles");
                    if (type != null)
                    {
                        _currentRole = Traverse.Create(type).Property("CurrentRole");
                        _currentRoleObject = _currentRole.GetValue();
                    }
                }
            }

            // Ошибка доступа времени исполнения
            public static class AOECheck
            {
#if false
                public static readonly StaticPropertyAccessor<List<Astral.Controllers.AOECheck.AOE>> List = typeof(Astral.Controllers.Roles).GetStaticProperty<List<Astral.Controllers.AOECheck.AOE>>("List"); 
#endif
                public static object List => _AOElist.GetValue();
                private static readonly Traverse _AOElist;
                public static IEnumerable<object> GetAOEList()
                {
                    
                    object list = _AOElist.PropertyExists() ? _AOElist.GetValue()  : null;
                    if (list != null)
                        _AOElist_Enumerator = Traverse.Create(list).Method("GetEnumerator");
                    var enumeratorObj = _AOElist_Enumerator.GetValue();
                    if (enumeratorObj != null && enumeratorObj is IDisposable disposable)
                    {
                        Traverse enumeratorMoveToNext = Traverse.Create(enumeratorObj).Method("MoveToNext");
                        Traverse current = Traverse.Create(enumeratorObj).Property("Current");
                        try
                        {
                            while(enumeratorMoveToNext.GetValue<bool>())
                            {
                                yield return current.GetValue();
                            }
                        }
                        finally
                        {
                            disposable.Dispose();
                        }
                    }
                }
                private static Traverse _AOElist_Enumerator;

                static AOECheck()
                {
                    Type type = Assembly.GetEntryAssembly()?.GetType("Astral.Controllers.Roles");
                    if (type != null)
                        _AOElist = Traverse.Create(type).Property("List");
                }
            }

            public static class BotComs
            {
                public static class BotClient
                {
                    public static readonly StaticPropertyAccessor<Astral.Functions.TCP.Client.Client> Client =
                        typeof(Astral.Controllers.BotComs.BotClient)
                            .GetStaticProperty<Astral.Functions.TCP.Client.Client>("Client");

                    private static readonly Func<Astral.Functions.TCP.Client.Client, System.Net.Sockets.TcpClient>
                        _tcpClientAccessor = InstanceFieldAccessorFactory
                            .GetInstanceFieldAccessor<Astral.Functions.TCP.Client.Client, System.Net.Sockets.TcpClient>("\u0002");

                    public static TcpClient Client_TcpClient => _tcpClientAccessor(Client);
                }
            }

            public static class Engine
            {
                private static StaticPropertyAccessor<Astral.Logic.Classes.FSM.Engine> mainEngine =
                    typeof(Astral.Controllers.Engine).GetStaticProperty<Astral.Logic.Classes.FSM.Engine>("MainEngine");

                public static Astral.Logic.Classes.FSM.Engine MainEngine
                {
                    get
                    {
                        if(mainEngine.IsValid())
                            return mainEngine.Value;
                        return null;
                    }
                }
            }
        }

        public  static class Logic
        {
            public  static class NW
            {
                public static class Movements
                {
                    public static readonly StaticPropertyAccessor<List<Astral.Logic.NW.Movements.DodgeLosTestResult>> LastValidPoses 
                        = typeof(Astral.Logic.NW.Movements).GetStaticProperty<List<Astral.Logic.NW.Movements.DodgeLosTestResult>>("LastValidPoses");
                    public static readonly StaticFieldAccessor<Timeout> LastValidPosesTimeout 
                        = typeof(Astral.Logic.NW.Movements).GetStaticField<Timeout>("lastvlidposto");
                }

                public static class Combats
                {
                    static readonly Action<bool> abortCombat = typeof(Astral.Logic.NW.Combats).GetStaticAction<bool>(nameof(AbordCombat));

                    /// <summary>
                    /// Прерывание потока, управляющего персонажем в режиме боя
                    /// </summary>
                    /// <param name="stopMove"></param>
                    public static void AbordCombat(bool stopMove)
                    {
                        abortCombat?.Invoke(stopMove);
                    }
                }
            }
        }

        /// <summary>
        /// Доступ к членам и методам класса
        /// Astral.Classes.ItemFilter
        /// </summary>
        public static class ItemFilter
        {
            public static readonly Func<ItemFilterCore, Func<Item, bool>> IsMatch = InstanceAccessor<ItemFilterCore>.GetFunction<Item, bool>("\u0001");
        }
    }
}
