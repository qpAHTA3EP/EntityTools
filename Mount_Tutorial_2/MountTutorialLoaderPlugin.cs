using Astral;
using Astral.Forms;
using Astral.Logic.UCC.Classes;
using Astral.Professions.Classes;
using EntityTools.Core.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("Mount_Tutorial_Core")]

namespace Mount_Tutorial
{
    public class MountTutorialLoaderPlugin : Astral.Addons.Plugin
    {
        internal static IEntityToolsCore Core { get; private set; }  = new CoreProxy();
        internal static ConcurrentQueue<IQuesterActionRequest> Requests_QuesterActions { get; } = new ConcurrentQueue<IQuesterActionRequest>();

        async static internal TResult RequestCore<TResult>(IQuesterActionRequest request)
        {
            Requests_QuesterActions.Add(request);

            while(request.Ready != )
        }

        public override string Name => GetType().Name;
        public override string Author => "MichaelProg";
        public override System.Drawing.Image Icon => null;
        public override BasePanel Settings => new MainPanel();
        public override void OnBotStart() { }
        public override void OnBotStop() { }
        public override void OnLoad()
        {
            //System.AppDomain.CurrentDomain.AssemblyLoad += CurrentDomain_AssemblyLoad;
            System.AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            assemblyResolve_Deletage_Binded = true;
        }

        public override void OnUnload() { }

        public static void LoadAssembly()
        {
            // Последовательность инициализации подсистем Астрала
            // Astral.Core.\u0003()
            //typeof(Astral.Controllers.CustomClasses);

            /*FieldInfo reloggerField = typeof(Astral.Controllers.Relogger).GetField("checkThread", BindingFlags.Static | BindingFlags.NonPublic);
            if (reloggerField != null)
            {
                object reloggerChecker = reloggerField.GetValue(null);
                // проверяем загрузку "релоггера" Астрала, который запускается после инициализации плагинов
                // Таймаут ожидания 5 минут (300 с)
                Astral.Classes.Timeout timeout = new Astral.Classes.Timeout(300000);
                while(reloggerChecker == null && !timeout.IsTimedOut)
                {
                    Thread.Sleep(100);
                    reloggerChecker = reloggerField.GetValue(null);
                }
                if (reloggerChecker != null)
                {
                    // Релоггер Астрала запустился
                    try
                    {
                        Type plugins = typeof(Astral.Controllers.Plugins);
                        PropertyInfo pi = plugins.GetProperty("Assemblies", BindingFlags.Static | BindingFlags.NonPublic);
                        if (pi != null)
                        {
                            MethodInfo getter = pi.GetGetMethod(true);
                            if (getter != null)
                            {
                                object[] arg = new object[] { };
                                List<Assembly> Assemblies = getter.Invoke(null, arg) as List<Assembly>;
                                if (Assemblies != null)
                                {
                                    Assembly mountTutorialAssembly = Assembly.Load(Properties.Resources.Mount_Tutorial);
                                    Assemblies.Add(mountTutorialAssembly);
                                    return;
                                }
                                else throw new Exception("Fail access to Astral.Plugins.Assemblies");
                            }
                        }
                    }
                    catch { }
                }
            }

            Logger.WriteLine("Fail to load MountTutorial Assembly");*/
        }

        //private static void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
        //{
        //    LoadAssembly();
        //}
        private static bool assemblyResolve_Deletage_Binded = false;

        private static readonly string assemblyResolve_Name = $"^{Assembly.GetExecutingAssembly().GetName().Name}\\W";

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (Regex.IsMatch(args.Name, assemblyResolve_Name))
                    return typeof(MountTutorialLoaderPlugin).Assembly;
            return null;
        }

        //internal static bool MountTutorialLoaded = false;
        //static MountTutorialLoaderPlugin()
        //{
        //    Type plugins = typeof(Astral.Controllers.Plugins);
        //    try
        //    {
        //        PropertyInfo pi = plugins.GetProperty("Assemblies", BindingFlags.Static | BindingFlags.NonPublic);
        //        if (pi != null)
        //        {
        //            MethodInfo getter = pi.GetGetMethod(true);
        //            if (getter != null)
        //            {
        //                object[] arg = new object[] { };
        //                List<Assembly> Assemblies = getter.Invoke(null, arg) as List<Assembly>;
        //                if (Assemblies != null)
        //                {
        //                    Assembly mountTutorialAssembly = Assembly.Load(Properties.Resources.Mount_Tutorial_Core);
        //                    Assemblies.Add(mountTutorialAssembly);
        //                    Logger.WriteLine("Load the MountTutorial Assembly");
        //                    MountTutorialLoaded = true;
        //                    return;
        //                }
        //                else throw new Exception("Fail to access to the Astral.Plugins.Assemblies");
        //            }
        //        }
        //    }
        //    catch
        //    {
        //        Logger.WriteLine("Fail to load the MountTutorial Assembly");
        //    }
        //}
        internal class CoreProxy : IEntityToolsCore
        {
            static Func<bool> InternalInitialize = LoadCore;
            public string EntityDiagnosticInfos(object obj)
            {
                if (InternalInitialize())
                    return MountTutorialLoaderPlugin.Core.EntityDiagnosticInfos(obj);
                return string.Empty;
            }

            public bool Initialize(object obj)
            {
                if (InternalInitialize())
                    return MountTutorialLoaderPlugin.Core.Initialize(obj);
                return false;
            }

            public bool Initialize(Astral.Quester.Classes.Action action)
            {
                if (InternalInitialize())
                    return MountTutorialLoaderPlugin.Core.Initialize(action);
                return false;
            }

            public bool Initialize(Astral.Quester.Classes.Condition condition)
            {
                if (InternalInitialize())
                    return MountTutorialLoaderPlugin.Core.Initialize(condition);
                return false;
            }

            public bool Initialize(UCCAction action)
            {
                if (InternalInitialize())
                    return MountTutorialLoaderPlugin.Core.Initialize(action);
                return false;
            }

            public bool Initialize(UCCCondition condition)
            {
                if (InternalInitialize())
                    return MountTutorialLoaderPlugin.Core.Initialize(condition);
                return false;
            }

            private static bool LoadCore()
            {
                if (assemblyResolve_Deletage_Binded)
                {
                    // Попытка загрузки ядра производится только после привязки делегата

                    try
                    {
                        using (FileStream file = FileStreamHelper.OpenWithStream(Assembly.GetExecutingAssembly().Location, "Core", System.IO.FileMode.Open, System.IO.FileAccess.Read))
                        {
                            byte[] buffer = new byte[file.Length];
                            if (file.Read(buffer, 0, (int)file.Length) > 0)
                            {
                                Assembly assembly = Assembly.Load(buffer);

                                if (assembly != null)
                                    foreach (Type type in assembly.GetTypes())
                                    {
                                        if (type.GetInterfaces().Contains(typeof(IEntityToolsCore)))
                                        {
                                            IEntityToolsCore core = Activator.CreateInstance(type) as IEntityToolsCore;
                                            if (core != null)
                                            {
                                                MountTutorialLoaderPlugin.Core = core;
                                                return true;
                                            }
                                        }
                                    }
                                //if(assembly.CreateInstance("Mount_Tutorial_Core.Engine") is IEntityToolsCore core)
                                //{
                                //    MountTutorialLoaderPlugin.Core = core;
                                //    return true;
                                //}
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Astral.Logger.WriteLine(Logger.LogType.Debug, e.ToString());
                    }
                    finally
                    {
                        InternalInitialize = DoNothing;
                    }
                }
                return false;
            }
            private static bool DoNothing() => false;
        }
    }
}
