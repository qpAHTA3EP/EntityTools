#define PROFILING
using AStar;
using Astral;
using EntityTools.Patches.Mapper;
using EntityTools.Tools;
using EntityTools.Tools.Reflection;
using MyNW.Classes;
using MyNW.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
//using NodeDistPair = EntityTools.Tools.Pair<AStar.Node, double>;

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
        //private static StaticMethodInvoker<Vector3> GetNearesNodes = typeof(Astral.Quester.Core).GetInvoker<Vector3, Vector3, bool>("GetNearesNodetPosition");
        private static readonly Func<Vector3, bool, Vector3> GetNearesNodes = typeof(Astral.Quester.Core).GetStaticFunction<Vector3, bool, Vector3>("GetNearesNodetPosition");

        /// <summary>
        /// Функтор доступа к графу 
        /// </summary>
        private static readonly StaticPropertyAccessor<AStar.Graph> Meshes = typeof(Astral.Quester.Core).GetStaticProperty<Graph>("Meshes");

        /// <summary>
        /// Кэш вершин крафа
        /// </summary>
        private static MapperGraphCache graphCache;

        /// <summary>
        /// Патч для метода Astral.Quester.Forms.MapperForm.Mapper()
        /// Штатная реализация метода добавления пути
        /// </summary>
        internal static void Mapper()
        {
            try
            {
#if PROFILING && DEBUG
                AddNavigationNodeDirect.ResetWatch();
#endif
                if (graphCache.FullGraph.Nodes.Count == 0)
                {
                    Vector3 location = EntityManager.LocalPlayer.Location;
                    new Mapper.AddNavigationNodeDirect(location.X, location.Y, location.Z, Meshes.Value);
                }

                for (; ; )
                {
                    if (GetNearesNodes.Invoke(EntityManager.LocalPlayer.Location, false).Distance3DFromPlayer >= EntityTools.PluginSettings.Mapper.WaypointDistance
                        || Meshes.Value.Nodes.Count == 0)
                    {
                        Vector3 location = EntityManager.LocalPlayer.Location;
                        new Mapper.AddNavigationNodeDirect(location.X, location.Y, location.Z, Meshes.Value);
                    }
                    Thread.Sleep(100);
                }                
            }
            catch (ThreadAbortException)
            {
#if PROFILING && DEBUG
                AddNavigationNodeDirect.LogWatch();
                Logger.WriteLine(Logger.LogType.Debug, $"MapperWithCache:: Graph Nodes: {Meshes.Value.Nodes.Count}");
#endif
            }
            catch (Exception ex)
            {
#if PROFILING && DEBUG
                AddNavigationNodeChached.LogWatch();
                Logger.WriteLine(Logger.LogType.Debug, $"MapperWithCache:: Graph Nodes: {Meshes.Value.Nodes.Count}");
#endif
                Logger.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// Патч для метода Astral.Quester.Forms.MapperForm.Mapper()
        /// Реализация, использующая кэширование вершин и асинхронное добавление новых вершин
        /// </summary>
        internal static void MapperWithCache()
        {
            try
            {
                graphCache = new MapperGraphCache(Meshes.Value);
#if PROFILING && DEBUG
                AddNavigationNodeChached.ResetWatch();
#endif
                if (graphCache.FullGraph.Nodes.Count == 0)
                    new Mapper.AddNavigationNodeChached(EntityManager.LocalPlayer.Location.Clone(), graphCache);
                
                for (; ; )
                {
                    if (graphCache.ClosestNode(EntityManager.LocalPlayer.Location, out double distance) == null
                        || distance >= EntityTools.PluginSettings.Mapper.WaypointDistance)
                    {
                        new Mapper.AddNavigationNodeChached(EntityManager.LocalPlayer.Location.Clone(), graphCache);
                    }
                    Thread.Sleep(100);
                }
            }
            catch (ThreadAbortException)
            {
#if PROFILING && DEBUG
                AddNavigationNodeChached.LogWatch();
                Logger.WriteLine(Logger.LogType.Debug, $"MapperWithCache:: Graph Nodes: {Meshes.Value.Nodes.Count}");
#endif
            }
            catch (Exception ex)
            {
#if PROFILING && DEBUG
                AddNavigationNodeChached.LogWatch();
                Logger.WriteLine(Logger.LogType.Debug, $"MapperWithCache:: Graph Nodes: {Meshes.Value.Nodes.Count}");
#endif
                Logger.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// Патч для метода Astral.Quester.Forms.MapperForm.Mapper()
        /// Реализация, использующая кэширование вершин и синхронный метод добавления узлов (без дочернего процесса)
        /// Релизована опция связи с предыдущим узлом
        /// </summary>
        internal static void MapperExtWithCache()
        {
            if (EntityTools.PluginSettings.Mapper.LinearPath)
                MakeBidirectionalLinearPath();
            else MakeBidirectionalPath();
        }

        /// <summary>
        /// Двунаправленный путь
        /// </summary>
        private static void MakeBidirectionalPath()
        {
            NodeDetail nodeDetail = null;
            try
            {
                if (graphCache == null)
                    graphCache = new MapperGraphCache(Meshes);
                else graphCache.RegenCache(EntityManager.LocalPlayer.Location.Clone(), true);

#if PROFILING && DEBUG
                AddNavigationNodeChached.ResetWatch();
#endif
                if (graphCache.FullGraph.Nodes.Count == 0)
                    nodeDetail = AddNavigationNodeStatic.LinkComplex(EntityManager.LocalPlayer.Location.Clone(), graphCache);

                for (; ; )
                {
                    /*/ 1. Вариант реализации с поиском ближайших узлов
                    if (graphCache.ClosestNode(EntityManager.LocalPlayer.Location, out double distance) == null
                        || distance >= EntityTools.PluginSettings.Mapper.WaipointDistance) //*/
                    /*/ 2. Вариант реализации с проверкой расстояния до последнего добавленного узла
                    Vector3 location = EntityManager.LocalPlayer.Location;
                    double distance = (lastNode != null) ? Math.Sqrt(Math.Pow(lastNode.X - location.X, 2) + Math.Pow(lastNode.Y - location.Y, 2) + Math.Pow(lastNode.Z - location.Z, 2))
                                                         // Уловка для прохождения условия, поскольку узел lastNode отсутствует
                                                         : EntityTools.PluginSettings.Mapper.WaypointDistance + 1; 
                    if (distance >= EntityTools.PluginSettings.Mapper.WaypointDistance)//*/
                    /* 3. Вариант реализации с проверкой расстояния только до предыдущего узла*/
                    if (nodeDetail != null)
                        nodeDetail.Rebase(EntityManager.LocalPlayer.Location);

                    if (nodeDetail == null || nodeDetail.Distance > EntityTools.PluginSettings.Mapper.WaypointDistance)
                    {
                        if (EntityTools.PluginSettings.Mapper.ForceLinkingWaypoint)
                            nodeDetail = AddNavigationNodeStatic.LinkComplex(EntityManager.LocalPlayer.Location.Clone(), graphCache, nodeDetail) ?? nodeDetail;
                        else nodeDetail = AddNavigationNodeStatic.LinkComplex(EntityManager.LocalPlayer.Location.Clone(), graphCache) ?? nodeDetail;
                    }
                    Thread.Sleep(100);
                }
            }
            catch (ThreadAbortException)
            {
                try
                {
                    AddNavigationNodeStatic.LinkComplex(EntityManager.LocalPlayer.Location.Clone(), graphCache, nodeDetail);
                }
                catch { }

#if PROFILING && DEBUG
                AddNavigationNodeStatic.LogWatch();
                Logger.WriteLine(Logger.LogType.Debug, $"MapperExtWithCache:: Graph Nodes: {Meshes.Value.Nodes.Count}");
#endif
            }
            catch (Exception ex)
            {
#if PROFILING && DEBUG
                AddNavigationNodeStatic.LogWatch();
                Logger.WriteLine(Logger.LogType.Debug, $"MapperExtWithCache:: Graph Nodes: {Meshes.Value.Nodes.Count}");
#endif
                Logger.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// Двунаправленный линейный путь
        /// </summary>
        private static void MakeBidirectionalLinearPath()
        {
            NodeDetail nodeDetail = null;
            try
            {
                if (graphCache == null)
                    graphCache = new MapperGraphCache(Meshes);
                else graphCache.RegenCache(EntityManager.LocalPlayer.Location.Clone(), true);
#if PROFILING && DEBUG
                AddNavigationNodeChached.ResetWatch();
#endif
                nodeDetail = AddNavigationNodeStatic.LinkNearest(EntityManager.LocalPlayer.Location.Clone(), graphCache);

                for (; ; )
                {
                    /*/ 1. Вариант реализации с поиском ближайших узлов
                    if (graphCache.ClosestNode(EntityManager.LocalPlayer.Location, out double distance) == null
                        || distance >= EntityTools.PluginSettings.Mapper.WaipointDistance) //*/
                    /*/ 2. Вариант реализации с проверкой расстояния до последнего добавленного узла
                    Vector3 location = EntityManager.LocalPlayer.Location;
                    double distance = (lastNode != null) ? Math.Sqrt(Math.Pow(lastNode.X - location.X, 2) + Math.Pow(lastNode.Y - location.Y, 2) + Math.Pow(lastNode.Z - location.Z, 2))
                                                         // Уловка для прохождения условия, поскольку узел lastNode отсутствует
                                                         : EntityTools.PluginSettings.Mapper.WaypointDistance + 1; 
                    if (distance >= EntityTools.PluginSettings.Mapper.WaypointDistance)//*/
                    /* 3. Вариант реализации с проверкой расстояния до предыдущего узла*/
                    if (nodeDetail != null)
                        nodeDetail.Rebase(EntityManager.LocalPlayer.Location);

                    if (nodeDetail == null || nodeDetail.Distance > EntityTools.PluginSettings.Mapper.WaypointDistance)
                    {
                        if (EntityTools.PluginSettings.Mapper.ForceLinkingWaypoint)
                            nodeDetail = AddNavigationNodeStatic.LinkLast(EntityManager.LocalPlayer.Location.Clone(), graphCache, nodeDetail) ?? nodeDetail;
                        else nodeDetail = AddNavigationNodeStatic.LinkLast(EntityManager.LocalPlayer.Location.Clone(), graphCache) ?? nodeDetail;
                    }
                    Thread.Sleep(100);
                }
            }
            catch (ThreadAbortException)
            {
                try
                {
                    AddNavigationNodeStatic.LinkNearest(EntityManager.LocalPlayer.Location.Clone(), graphCache);
                }
                catch { }

#if PROFILING && DEBUG
                AddNavigationNodeStatic.LogWatch();
                Logger.WriteLine(Logger.LogType.Debug, $"MapperExtWithCache:: Graph Nodes: {Meshes.Value.Nodes.Count}");
#endif
            }
            catch (Exception ex)
            {
#if PROFILING && DEBUG
                AddNavigationNodeStatic.LogWatch();
                Logger.WriteLine(Logger.LogType.Debug, $"MapperExtWithCache:: Graph Nodes: {Meshes.Value.Nodes.Count}");
#endif
                Logger.WriteLine(ex.ToString());
            }
        }
    }
}
