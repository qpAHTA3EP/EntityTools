using Astral;
using EntityTools.Tools.Reflection;
using MyNW.Classes;
using MyNW.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace EntityTools.Patches
{
    /// <summary>
    /// Патч для Astral.Quester.Forms.MapperForm 
    /// </summary>
    internal class Patch_Mapper
    {
        /// <summary>
        /// Функтор получения ближайшего узла
        /// </summary>
        private static StaticMethodInvoker<Vector3> GetNearesNodes = typeof(Astral.Quester.Core).GetStaticMethodInvoker<MyNW.Classes.Vector3>("GetNearesNodetPosition", new Type[] { typeof(MyNW.Classes.Vector3), typeof(bool) });

        /// <summary>
        /// Функтор доступа к графу 
        /// </summary>
        private static StaticPropertyAccessor<AStar.Graph> Meshes = typeof(Astral.Quester.Core).GetStaticPropertyAccessor<AStar.Graph>("Meshes");
        
        /// <summary>
        /// Патч для метода Astral.Quester.Forms.MapperForm.Mapper()
        /// Штатная реализация метода добавления пути
        /// </summary>
        internal static void Mapper()
        {
            for (; ; )
            {
                try
                {
                    if (GetNearesNodes.Invoke(EntityManager.LocalPlayer.Location, false).Distance3DFromPlayer >= EntityTools.PluginSettings.Mapper.WaipointDistance
                        || Meshes.Value.Nodes.Count == 0)
                    {
                        Vector3 location = EntityManager.LocalPlayer.Location;
                        new Mapper.AddNavigationNode(location.X, location.Y, location.Z, Meshes.Value);
                    }
                    Thread.Sleep(100);
                    continue;
                }
                catch (ThreadAbortException) { }
                catch (Exception ex)
                {
                    Logger.WriteLine(ex.ToString());
                }
                break;
            }
        }

        internal static void SmartMapper()
        {

        }
    }
}
