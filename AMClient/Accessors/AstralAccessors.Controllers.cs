using AcTp0Tools.Patches;
using AcTp0Tools.Reflection;
using AStar;
using Astral;
using Astral.Addons;
using Astral.Logic.Classes.Map;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Reflection;

// ReSharper disable once CheckNamespace
namespace AcTp0Tools
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

                static Roles()
                {
                    var assembly = Assembly.GetEntryAssembly();
                    if (assembly != null)
                    {
                        foreach (var type in assembly.GetTypes())
                        {
                            var fullName = type.FullName;
                            if (fullName == typeName_Controler_Roles)
                            {
                                tControler_Roles = type;
                                if (tRole != null)
                                    break;
                            }
                            else if (fullName == typeName_Role)
                            {
                                tRole = type;
                                if (tControler_Roles != null)
                                    break;
                            }
                        }

                        if (tControler_Roles is null)
                        {
                            Logger.WriteLine(Logger.LogType.Debug, $"Does not found type '{typeName_Controler_Roles}'");
                        }
                        else if (tRole is null)
                        {
                            Logger.WriteLine(Logger.LogType.Debug, $"Does not found type '{typeName_Role}'");
                        }
                        else 
                        {
                            currentRoleAccessor = tControler_Roles.GetStaticProperty("CurrentRole", tRole);
                            isRunningAccessor =
                                tControler_Roles.GetStaticProperty<bool>(nameof(Astral.Controllers.Roles.IsRunning));
                        }
                    }
                }

                /// <summary>
                /// Объект, предоставляющий доступ к приватному свойству <see cref="Astral.Controllers.Roles.CurrentRole"/>
                /// </summary>
                private static readonly StaticPropertyAccessor currentRoleAccessor;

                /// <summary>
                /// Полное имя закрытого типа <see cref="Astral.Controllers.Roles"/>
                /// </summary>
                private const string typeName_Controler_Roles = "Astral.Controllers.Roles";
                /// <summary>
                /// Полное имя закрытого типа <see cref="Astral.Addons.Role"/>
                /// </summary>
                private const string typeName_Role = "Astral.Addons.Role";

                /// <summary>
                /// Закрытый тип <see cref="Astral.Controllers.Roles"/>
                /// </summary>
                private static readonly Type tControler_Roles;
                /// <summary>
                /// Закрытый тип <see cref="Astral.Addons.Role"/>
                /// </summary>
                private static readonly Type tRole;

                // Astral.Controllers.Roles.IsRunning

                public static bool IsRunning => isRunningAccessor.Value;
                private static StaticPropertyAccessor<bool> isRunningAccessor;

                public static class CurrentRole
                {
                    /// <summary>
                    /// Вызов метода <see cref="Astral.Controllers.Roles.CurrentRole.OnMapDraw{GraphicsNW}"/> активной роли Астрала. 
                    /// <remarks>Поскольку данный метод является виртуальным, функтор доступа <see cref="_usedMeshes"/>
                    /// необходимо обновлять при смене активной роли</remarks>
                    /// </summary>
                    public static Graph UsedMeshes
                    {
                        get
                        {
#if HARMONY_TRAVERSE
                            object role = currentRole.GetValue();
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
#else

                            var role = currentRoleAccessor.Value;
                            if (_usedMeshes?.Instance != role)
                            {
                                _usedMeshes = role.GetProperty<Graph>(nameof(UsedMeshes));
                            }
                            return _usedMeshes?.Value;

#endif
                        }
                    }
#if HARMONY_TRAVERSE
                    private static Traverse<Graph> _usedMeshes; 
#else
                    private static PropertyAccessor<Graph> _usedMeshes;
#endif

                    /// <summary>
                    /// Вызов метода <see cref="Astral.Controllers.Roles.CurrentRole.OnMapDraw{GraphicsNW}"/> активной роли Астрала
                    /// <remarks>Поскольку данный метод является виртуальным, функтор доступа <see cref="_onMapDraw"/>
                    /// необходимо обновлять при смене активной роли</remarks>
                    /// </summary>
                    /// <param name="graphicsNW"></param>
                    /// <returns>Флаг, указывающий на успешное выполнение метода</returns>
                    public static bool OnMapDraw(GraphicsNW graphicsNW)
                    {
#if HARMONY_TRAVERSE
                        object role = currentRole.GetValue();
                        if (role is null)
                        {
                            ResetTraverses();
                            return false;
                        }
                        if (role != _currentRoleObject)
                        {
                            _currentRoleObject = role;
                            _onMapDraw = Traverse.Create(_currentRoleObject).Method("OnMapDraw", graphicsNW);
                        }
                        if (_currentRoleObject != null && _onMapDraw is null)
                            _onMapDraw = Traverse.Create(_currentRoleObject).Method("OnMapDraw", graphicsNW);

                        if (_onMapDraw != null)
                        {
                            _onMapDraw.GetValue(graphicsNW);
                            return true;
                        } 
                        return false;
#else
                        var role = currentRoleAccessor.Value;

                        if (role != null)
                        {
                            // Поскольку объект Astral.Controllers.Roles.CurrentRole может быть любого из дочерних типов
                            // нужно проверять 
                            if (!ReferenceEquals(role, _onMapDrawTarget))
                            {
                                _onMapDraw = role.GetType().GetAction<GraphicsNW>(nameof(OnMapDraw));
                                _onMapDrawTarget = role;
                            }

                            if (_onMapDraw != null)
                            {
                                _onMapDraw(_onMapDrawTarget, graphicsNW);
                                return true;
                            }
                        } 

                        return false;
#endif
                    }

                    /// <summary>
                    /// Метод заглушка, используемый при ошибке получения доступа к <see cref="Astral.Controllers.Roles.CurrentRole.OnMapDraw{GraphicsNW}"/>
                    /// </summary>
                    /// <param name="o"></param>
                    /// <param name="g"></param>
                    /// <returns></returns>
                    private static void OnMapDraw_Stub(object o, GraphicsNW g)
                    {
#if false
                        Logger.WriteLine(Logger.LogType.Debug, $"Access violation to method 'OnMapDraw' of the object '{o}'\n" +
                                                                               $"{Environment.StackTrace}"); 
#else
                        Logger.WriteLine(Logger.LogType.Debug, $"Accessor to the method 'OnMapDraw' of the object '{o}' is not initialized.\n" +
                                                               $"{Environment.StackTrace}");
#endif
                    }
#if HARMONY_TRAVERSE
                    private static Traverse _onMapDraw; 
#else
                    /// <summary>
                    /// Функтор доступа к функции объекта
                    /// </summary>
                    private static Action<object, GraphicsNW> _onMapDraw = null;
                    /// <summary>
                    /// Кэш объекта, с которым связан делегат <see cref="_onMapDraw"/>
                    /// </summary>
                    private static object _onMapDrawTarget = null;
#endif

                    /// <summary>
                    /// Чтение названия активной роли Астралом (свойство <see cref="Role.Name"/>).
                    /// <remarks>Поскольку данное свойство является виртуальным, функтор доступа <see cref="_nameAccessor"/>
                    /// необходимо обновлять при смене активной роли</remarks>
                    /// </summary>
                    public static string Name
                    {
                        get
                        {
#if HARMONY_TRAVERSE
                            object role = currentRole.GetValue();
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
#else
                            var role = currentRoleAccessor.Value;
                            if (role != null
                                && _nameAccessor?.Instance != null)
                            {
                                _nameAccessor = role.GetProperty<string>(nameof(Role.Name));
                            }
                            return _nameAccessor?.Value;
#endif
                        }
                    }
#if HARMONY_TRAVERSE
                    private static Traverse<string> _name;
#else
                    private static PropertyAccessor<string> _nameAccessor;
#endif

                    static CurrentRole()
                    {
#if HARMONY_TRAVERSE
                        _currentRoleObject = currentRole.GetValue();
                        if (_currentRoleObject != null)
                        {
                            _name = Traverse.Create(_currentRoleObject).Property<string>("Name"); 
                        }
#endif
                        _onMapDraw = OnMapDraw_Stub;
                    }

#if HARMONY_TRAVERSE
                    private static void ResetTraverses()
                    {
                        _currentRoleObject = null;
                        _usedMeshes = null;
                        _onMapDraw = null;
                        _name = null;
                    } 
#endif
                    /// <summary>
                    /// Метод логирования ошибок доступа
                    /// </summary>
                    /// <param name="obj"></param>
                    /// <param name="method"></param>
                    private static void ReportAccessViolation(object obj, MethodBase method)
                    {
                        Logger.WriteLine(Logger.LogType.Debug, $"Access violation to method '{method.Name}' of the object '{obj}'\n" +
                                                               $"{Environment.StackTrace}");
                    }
                }
            }


#if AOECheck // Ошибка доступа времени исполнения поскольку тип Astral.Controllers.AOECheck.AOE является приватным
            public static class AOECheck
            {
#if false
                public static readonly StaticPropertyAccessor<List<Astral.Controllers.AOECheck.AOE>> List = typeof(Astral.Controllers.Roles).GetStaticProperty<List<Astral.Controllers.AOECheck.AOE>>("List"); 
#endif
                public static object List => aoeList.GetValue();
                private static readonly Traverse aoeList;
                public static IEnumerable<object> GetAOEList()
                {

                    object list = aoeList.PropertyExists() ? aoeList.GetValue() : null;
                    if (list != null)
                        aoeList_Enumerator = Traverse.Create(list).Method("GetEnumerator");
                    var enumeratorObj = aoeList_Enumerator.GetValue();
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
                private static Traverse aoeList_Enumerator;

                static AOECheck()
                {
                    Type type = Assembly.GetEntryAssembly()?.GetType("Astral.Controllers.Roles");
                    if (type != null)
                        aoeList = Traverse.Create(type).Property("List");
                }
            } 
#endif

            public static class BotComs
            {
                public static class BotClient
                {
                    public static Astral.Functions.TCP.Client.Client Client => client.Value;

                    private static readonly StaticPropertyAccessor<Astral.Functions.TCP.Client.Client> client =
                        typeof(Astral.Controllers.BotComs.BotClient)
                            .GetStaticProperty<Astral.Functions.TCP.Client.Client>("Client");


                    private static readonly FieldAccessor<TcpClient> tcpClientAccessor 
                        = typeof(Astral.Functions.TCP.Client.Client).GetField<TcpClient>("\u0002");

                    public static TcpClient Client_TcpClient => tcpClientAccessor[client.Value];
                }

                public static class BotServer
                {
                    public static Astral.Functions.TCP.Server.Server Server
                    {
                        get => server.Value;
                        set => server.Value = value;
                    }
                    private static readonly StaticPropertyAccessor<Astral.Functions.TCP.Server.Server> server;

                    public static readonly Action SendQuesterProfileInfos;

                    public static bool ForceRefreshTasks
                    {
                        get => forceRefreshTasks.Value;
                        set => forceRefreshTasks.Value = value;
                    }
                    private static readonly StaticPropertyAccessor<bool> forceRefreshTasks;

                    static BotServer()
                    {
                        var tBotServer = typeof(Astral.Controllers.BotComs.BotServer);

                        server = tBotServer.GetStaticProperty<Astral.Functions.TCP.Server.Server>(nameof(Astral.Controllers.BotComs.BotServer.Server));
                        forceRefreshTasks = tBotServer.GetStaticProperty<bool>(nameof(ForceRefreshTasks));

                        SendQuesterProfileInfos = tBotServer.GetStaticAction(nameof(Astral.Controllers.BotComs.BotServer.SendQuesterProfileInfos));

                    }
                }
            }

            public static class Engine
            {
                private static readonly StaticPropertyAccessor<Astral.Logic.Classes.FSM.Engine> mainEngine =
                    typeof(Astral.Controllers.Engine).GetStaticProperty<Astral.Logic.Classes.FSM.Engine>("MainEngine");

                public static Astral.Logic.Classes.FSM.Engine MainEngine => mainEngine.IsValid ? mainEngine.Value : null;
            }

            public static class Plugins
            {
                /// <summary>
                /// Коллекция сборок плагинов
                /// </summary>
                public static List<Assembly> Assemblies
                {
                    get 
                    { 
                        if(assemblies.IsValid) return assemblies.Value;
                        
                        emptyAssemblyList.Clear();
                        return emptyAssemblyList;
                        
                    }
                }
                private static readonly StaticPropertyAccessor<List<Assembly>> assemblies = 
                    typeof(Astral.Controllers.Plugins).GetStaticProperty<List<Assembly>>(nameof(Assemblies));
                private static readonly List<Assembly> emptyAssemblyList = new List<Assembly>();

                public static IEnumerable<Type> UccTargetSelectors =>
                    Astral_Functions_XmlSerializer_GetExtraTypes.UccTargetSelectorTypes;

                //static Func<List<Type>> getTypes = typeof(Plugins).GetStaticFunction<List<Type>>("GetTypes");

                /// <summary>
                /// Перечисление типов, объявленных в плагинах <see cref="Astral.Controllers.Plugins"/>
                /// </summary>
                /// <returns></returns>
                public static IEnumerable<Type> GetTypes()
                {
                    //return getTypes();
                    foreach (var assembly in assemblies.Value)
                    {
                        foreach (var type in assembly.GetTypes())
                        {
                            yield return type;
                        }
                    }
                }

                /// <summary>
                /// Перечисление типов, производных от типа <typeparam name="TBase"/>,
                /// объявленных в сборках-плагинах
                /// </summary>
                /// <typeparam name="TBase">Базовый тип, производные которого необходимо получить</typeparam>
                /// <param name="includeBase">True, если в перечисление нужно включить базовый тип <typeparam name="TBase"/></param>
                /// <returns></returns>
                public static IEnumerable<Type> GetPluginTypesDerivedFrom<TBase>(bool includeBase = false)
                {
                    var baseType = typeof(TBase);
                    if (includeBase)
                        yield return baseType;
                    foreach (var type in GetTypes())
                    {
                        if (baseType.IsAssignableFrom(type))
                            yield return type;
                    }
                }


#if false       // Во время выполнения вызывает ошибку доступа при перечислении системных типов
                /// <summary>
                /// Перечисление всех типов, производных от типа <typeparamref name="TBase"/>,
                /// объявленных во всех загруженных сборка
                /// </summary>
                /// <typeparam name="TBase">Базовый тип, производные которого необходимо получить</typeparam>
                /// <param name="includeBase">True, если в перечисление нужно включить базовый тип <typeparamref name="TBase"/></param>
                /// <returns></returns>
                public static IEnumerable<Type> GetAllTypesDerivedFrom<TBase>(bool includeBase = true)
                {
                    var baseType = typeof(TBase);
                    List<Type> typeList;

                    if (!typeTreeDictionary.ContainsKey(baseType))
                    {
                        // Вариант с Linq
                        //typeList = AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly =>
                        //    assembly.GetTypes().Where(t => baseType.IsAssignableFrom(t))).ToList();

                        // Вариант с перебором типов в цикле
                        typeList = new List<Type>(8);
                        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                        {
                            foreach (var type in assembly.GetTypes())
                            {
                                if (baseType.IsAssignableFrom(type))
                                    typeList.Add(type);
                            }
                        }

                        // Добавление списка в коллекцию (кэширование)
                        typeTreeDictionary.Add(baseType, typeList);
                    }
                    else typeList = typeTreeDictionary[baseType];

                    if (includeBase)
                        yield return baseType;
                    if (typeTreeDictionary.ContainsKey(baseType))
                    {
                        foreach (var type in typeList)
                        {
                            yield return type;
                        }
                    }
                } 

                /// <summary>
                /// Коллекция (кэш) поддерева типов, производных от типа, являющегося ключом словаря.
                /// </summary>
                private static readonly Dictionary<Type, List<Type>> typeTreeDictionary = new Dictionary<Type, List<Type>>();
#endif
            }
        }
    }
}
