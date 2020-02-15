using Astral;
using Astral.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mount_Tutorial
{
    public class MountTutorialLoaderPlugin : Astral.Addons.Plugin
    {
        public override string Name => GetType().Name;
        public override string Author => "MichaelProg";
        public override System.Drawing.Image Icon => null;
        public override BasePanel Settings => new MainPanel();
        public override void OnBotStart() { }
        public override void OnBotStop() { }
        public override void OnLoad()
        {
            //System.AppDomain.CurrentDomain.AssemblyLoad += CurrentDomain_AssemblyLoad;
            //System.AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        public override void OnUnload() { }

        public static void LoadAssembly()
        {
            // Последовательность инициализации подсистем Астрала
            // Astral.Core.method_2()
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
        //private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        //{
        //    LoadAssembly();
        //    return args.Name.Contains($"{Category}.resources") ? typeof(Astral.Forms.Main).Assembly : typeof(Core).Assembly;
        //}

        internal static bool MountTutorialLoaded = false;
        static MountTutorialLoaderPlugin()
        {
            Type plugins = typeof(Astral.Controllers.Plugins);
            try
            {
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
                            Logger.WriteLine("Load the MountTutorial Assembly");
                            MountTutorialLoaded = true;
                            return;
                        }
                        else throw new Exception("Fail to access to the Astral.Plugins.Assemblies");
                    }
                }
            }
            catch
            {
                Logger.WriteLine("Fail to load the MountTutorial Assembly");
            }
        }
    }
}
