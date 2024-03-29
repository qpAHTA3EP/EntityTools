﻿using AStar;
using Astral.Logic.Classes.Map;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Reflection;

namespace EntityTools.Reflection
{
    /// <summary>
    /// Доступ к закрытым членам и методам Astral'a
    /// </summary>
    public static partial class AstralAccessors
    {
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

                    object list = _AOElist.PropertyExists() ? _AOElist.GetValue() : null;
                    if (list != null)
                        _AOElist_Enumerator = Traverse.Create(list).Method("GetEnumerator");
                    var enumeratorObj = _AOElist_Enumerator.GetValue();
                    if (enumeratorObj != null && enumeratorObj is IDisposable disposable)
                    {
                        Traverse enumeratorMoveToNext = Traverse.Create(enumeratorObj).Method("MoveToNext");
                        Traverse current = Traverse.Create(enumeratorObj).Property("Current");
                        try
                        {
                            while (enumeratorMoveToNext.GetValue<bool>())
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
                        if (mainEngine.IsValid)
                            return mainEngine.Value;
                        return null;
                    }
                }
            }

            public static class Plugins
            {
                static Plugins()
                {
                    _assemblies = typeof(Astral.Controllers.Plugins).GetStaticProperty<List<Assembly>>(nameof(Assemblies));
                }
                static StaticPropertyAccessor<List<Assembly>> _assemblies;
                static readonly List<Assembly> emptyAssemblyList = new List<Assembly>();
                public static List<Assembly> Assemblies
                {
                    get
                    {
                        return _assemblies.IsValid ? _assemblies.Value : emptyAssemblyList;
                    }
                }
            }
        }

    }
}
