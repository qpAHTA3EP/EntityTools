using Astral;
using Astral.Forms;
using EntityTools.Core.Interfaces;
using System;
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
        internal static IEntityToolsCore Core
        {
            get
            {
                if(_core == null)
                {
                    using (FileStream file = FileStreamHelper.OpenWithStream(Assembly.GetExecutingAssembly().Location, "Core", System.IO.FileMode.Open, System.IO.FileAccess.Read))
                    {
                        byte[] buffer = new byte[file.Length];
                        if (file.Read(buffer, 0, (int)file.Length) > 0)
                        {
                            Assembly assembly = Assembly.Load(buffer);

                            _core = assembly.CreateInstance("Mount_Tutorial_Core.Engine") as IEntityToolsCore;
                        }
                    }
                    //Assembly assembly = Assembly.Load(Properties.Resources.Mount_Tutorial_Core);
                    //foreach(Type type in assembly.GetTypes())
                    //{
                    //    if(type.GetInterfaces().Contains(typeof(IEntityToolsCore)))
                    //    {
                    //        _core = Activator.CreateInstance(type) as IEntityToolsCore;
                    //    }
                    //}
                }
                return _core;
            }
        }
        internal static IEntityToolsCore _core = null;

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
private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
{
    //LoadAssembly();
    //return args.Name.Contains($"{Category}.resources") ? typeof(Astral.Forms.Main).Assembly : typeof(MountTutorialLoaderPlugin).Assembly;
    if (Regex.IsMatch(args.Name, $"^{Assembly.GetExecutingAssembly().GetName().Name}\\W"))
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
    }
}
