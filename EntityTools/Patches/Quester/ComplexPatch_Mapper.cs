using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Runtime.CompilerServices;
using AStar;
using Astral.Logic.Classes.Map;
using EntityTools.Quester.Mapper;
using HarmonyLib;
using Infrastructure;
using Infrastructure.Quester;
using MyNW.Classes;

// ReSharper disable InconsistentNaming

namespace EntityTools.Patches.Quester
{
    internal static class ComplexPatch_Mapper
    {
        private static MapperFormExt mapperForm;
        public static bool PatchesWasApplied { get; private set; }

        private static MethodInfo original_OpenForm;
        private static MethodInfo prefix_OpenForm;

        private static MethodInfo original_Navmesh_DrawRoad;
        private static MethodInfo prefix_Navmesh_DrawRoad;

        private static MethodInfo original_DrawHotSpots;
        private static MethodInfo prefix_DrawHotSpots;

        private static MethodInfo original_DrawMeshes;
        private static MethodInfo prefix_DrawMeshes;

        public static void Apply()
        {
            if (!EntityTools.Config.Patches.Navigation || PatchesWasApplied) return;

            var tNavmesh = typeof(Astral.Logic.Navmesh);
            var tMapper = typeof(Astral.Quester.Forms.MapperForm);
            var tPatch = typeof(ComplexPatch_Mapper);
            var tPicture = typeof(Astral.Logic.Classes.Map.Functions.Picture);

            original_OpenForm = AccessTools.Method(tMapper, nameof(Astral.Quester.Forms.MapperForm.Open));
            if (original_OpenForm is null)
            {
                ETLogger.WriteLine($@"Patch '{nameof(ComplexPatch_Mapper)}' failed. Method '{original_OpenForm}' not found", true);
                return;
            }
            prefix_OpenForm = AccessTools.Method(tPatch, nameof(OpenMapper));
            if (prefix_OpenForm is null)
            {
                ETLogger.WriteLine($@"Patch '{nameof(ComplexPatch_Mapper)}' failed. Method '{prefix_OpenForm}' not found", true);
                return;
            }
            original_Navmesh_DrawRoad = AccessTools.Method(tNavmesh, "DrawRoad");
            if (original_Navmesh_DrawRoad is null)
            {
                ETLogger.WriteLine($@"Patch '{nameof(ComplexPatch_Mapper)}' failed. Method '{original_Navmesh_DrawRoad}' not found", true);
                return;
            }
            prefix_Navmesh_DrawRoad = AccessTools.Method(tPatch, nameof(PrefixDrawRoad));
            if (prefix_Navmesh_DrawRoad is null)
            {
                ETLogger.WriteLine($@"Patch '{nameof(ComplexPatch_Mapper)}' failed. Method '{prefix_Navmesh_DrawRoad}' not found", true);
                return;
            }
            original_DrawHotSpots = AccessTools.Method(tNavmesh, "DrawHotSpots");
            if (original_DrawHotSpots is null)
            {
                ETLogger.WriteLine($@"Patch '{nameof(ComplexPatch_Mapper)}' failed. Method '{original_DrawHotSpots}' not found", true);
                return;
            }
            prefix_DrawHotSpots = AccessTools.Method(tPatch, nameof(DrawHotSpots));
            if (prefix_DrawHotSpots is null)
            {
                ETLogger.WriteLine($@"Patch '{nameof(ComplexPatch_Mapper)}' failed. Method '{prefix_DrawHotSpots}' not found", true);
                return;
            }
            original_DrawMeshes = AccessTools.Method(tPicture, "DrawMeshes");
            if (original_DrawMeshes is null)
            {
                ETLogger.WriteLine($@"Patch '{nameof(ComplexPatch_Mapper)}' failed. Method '{original_DrawMeshes}' not found", true);
                return;
            }
            prefix_DrawMeshes = AccessTools.Method(tPatch, nameof(DrawMeshes));
            if (prefix_DrawMeshes is null)
            {
                ETLogger.WriteLine($@"Patch '{nameof(ComplexPatch_Mapper)}' failed. Method '{prefix_DrawMeshes}' not found", true);
                return;
            }
            Action unPatch = null;

            try
            {
                Infrastructure.Patches.ACTP0Patcher.Harmony.Patch(original_OpenForm,
                    new HarmonyMethod(prefix_OpenForm));
                unPatch = () =>
                {
                    ETLogger.WriteLine(LogType.Error, $@"Unpatch method '{original_OpenForm}'", true);
                    Infrastructure.Patches.ACTP0Patcher.Harmony.Unpatch(original_OpenForm, prefix_OpenForm);
                };

                Infrastructure.Patches.ACTP0Patcher.Harmony.Patch(original_Navmesh_DrawRoad,
                    new HarmonyMethod(prefix_Navmesh_DrawRoad));
                unPatch += () =>
                {
                    ETLogger.WriteLine(LogType.Error, $@"Unpatch method '{prefix_Navmesh_DrawRoad}'", true);
                    Infrastructure.Patches.ACTP0Patcher.Harmony.Unpatch(original_Navmesh_DrawRoad,
                        prefix_Navmesh_DrawRoad);
                };

                Infrastructure.Patches.ACTP0Patcher.Harmony.Patch(original_DrawMeshes,
                    new HarmonyMethod(prefix_DrawMeshes));
                unPatch += () =>
                {
                    ETLogger.WriteLine(LogType.Error, $@"Unpatch method '{prefix_DrawMeshes}'", true);
                    Infrastructure.Patches.ACTP0Patcher.Harmony.Unpatch(original_DrawMeshes, prefix_DrawMeshes);
                };

#if false
                //BUG вызывает исключение
                //Адресат вызова создал исключение.   в System.RuntimeMethodHandle.InvokeMethod(Object target, Object[] arguments, Signature sig, Boolean constructor)
                //в System.Reflection.RuntimeMethodInfo.UnsafeInvokeInternal(Object obj, Object[] parameters, Object[] arguments)
                //в System.Reflection.RuntimeMethodInfo.Invoke(Object obj, BindingFlags invokeAttr, Binder binder, Object[] parameters, CultureInfo culture)
                //в System.Reflection.MethodBase.Invoke(Object obj, Object[] parameters)
                //в MonoMod.RuntimeDetour.Platforms.DetourRuntimeNETPlatform.GetMethodHandle(MethodBase method)
                //в MonoMod.RuntimeDetour.Platforms.DetourRuntimeILPlatform.<> c__DisplayClass24_0.< Pin > b__0(MethodBase m)
                //в System.Collections.Concurrent.ConcurrentDictionary`2.GetOrAdd(TKey key, Func`2 valueFactory)
                //в MonoMod.RuntimeDetour.Platforms.DetourRuntimeILPlatform.Pin(MethodBase method)
                //в MonoMod.RuntimeDetour.DetourHelper.Pin[T](T method)
                //в HarmonyLib.MethodPatcher.CreateReplacement(Dictionary`2 & finalInstructions)
                //в HarmonyLib.PatchFunctions.UpdateWrapper(MethodBase original, PatchInfo patchInfo)
                //в HarmonyLib.PatchProcessor.Patch()
                //в HarmonyLib.Harmony.Patch(MethodBase original, HarmonyMethod prefix, HarmonyMethod postfix, HarmonyMethod transpiler, HarmonyMethod finalizer)
                //в EntityTools.Patches.Logic.Navmesh.ComplexPatch_Navigation.Patch() в D:\Source\qpAHTA3EP\EntityAddon\EntityTools\Patches\Logic.Navmesh\ComplexPatch_Navigation.cs:строка 111
                //в EntityTools.Patches.ETPatcher.Apply() в D:\Source\qpAHTA3EP\EntityAddon\EntityTools\Patches\Patcher.cs:строка 64
                //в EntityTools.EntityTools.OnLoad() в D:\Source\qpAHTA3EP\EntityAddon\EntityTools\Core\EntityPlugin.cs:строка 133
                //в Astral.Controllers.Plugins.Initialize()
                //в Astral.Core.()
                //в Astral.Core.()
                //в ..(Object )
                //в System.Threading.ThreadHelper.ThreadStart_Context(Object state)
                //в System.Threading.ExecutionContext.RunInternal(ExecutionContext executionContext, ContextCallback callback, Object state, Boolean preserveSyncCtx)
                //в System.Threading.ExecutionContext.Run(ExecutionContext executionContext, ContextCallback callback, Object state, Boolean preserveSyncCtx)
                //в System.Threading.ExecutionContext.Run(ExecutionContext executionContext, ContextCallback callback, Object state)
                //в System.Threading.ThreadHelper.ThreadStart(Object obj)

                AcTp0Tools.Patches.ACTP0Patcher.Harmony.Patch(original_DrawHotSpots,
                    new HarmonyMethod(prefix_DrawHotSpots));
                unPatch += () =>
                    AcTp0Tools.Patches.ACTP0Patcher.Harmony.Unpatch(original_DrawHotSpots, prefix_DrawHotSpots); 
#else
                RuntimeHelpers.PrepareMethod(original_DrawHotSpots.MethodHandle);
                RuntimeHelpers.PrepareMethod(prefix_DrawHotSpots.MethodHandle);

                unsafe
                {
                    long* inj = (long*)prefix_DrawHotSpots.MethodHandle.Value.ToPointer() + 1;
                    long* tar = (long*)original_DrawHotSpots.MethodHandle.Value.ToPointer() + 1;

                    *tar = *inj;
                }
#endif
                ETLogger.WriteLine($@"Patch '{nameof(ComplexPatch_Mapper)}' succeeded", true);
            }
            catch (Exception e)
            {
                ETLogger.WriteLine(LogType.Error, $@"Patch '{nameof(ComplexPatch_Mapper)}' failed", true);
                unPatch?.Invoke();
                ETLogger.WriteLine(LogType.Error, e.ToString(), true);
            }

            PatchesWasApplied = true;
        }

        internal static bool OpenMapper()
        {
            if (!EntityTools.Config.Mapper.Patch) return true;

            if (mapperForm is null || mapperForm.IsDisposed)
            {
                mapperForm = new MapperFormExt(AstralAccessors.Quester.Core.CurrentProfile);
                mapperForm.OnDraw += DrawRunningRole;
            }
            mapperForm.ImmutableShow();
            return false;
        }
        internal static void OpenMapper(BaseQuesterProfileProxy profile)
        {
            if (!EntityTools.Config.Mapper.Patch
                || profile is null)
            {
                OpenMapper();
                return;
            }
            var mapper = new MapperFormExt(profile);
            mapper.ImmutableShow();
        }

        internal static void CloseMapper()
        {
            if (mapperForm != null && !mapperForm.IsDisposed)
                mapperForm.Close();
        }



        private static void DrawRunningRole(MapperGraphics graphics)
        {
            if (AstralAccessors.Controllers.Roles.IsRunning)
            {
                var profile = Astral.Quester.API.CurrentProfile;//AstralAccessors.Quester.Core.CurrentProfile;
                if (profile != null)
                {
                    bool hotSpotsNotYetDrawn = true;
                    foreach (var action in profile.GetCurrentActions)
                    {
                        action.OnMapDraw(graphics);
                        if (hotSpotsNotYetDrawn && action.UseHotSpots)
                        {
                            DrawHotSpots(action.HotSpots, graphics);
                            hotSpotsNotYetDrawn = false;
                        }
                    }
                }

                var road = AstralAccessors.Controllers.Engine.MainEngine.Navigation.road;
                if (road != null)
                {
                    var waypoints = road.Waypoints;
                    if (waypoints?.Count > 0)
                    {
                        DrawRoad(waypoints, graphics, defaultPen, defaultBrush);
                    }
                }
            }
        }

        #region DrawRoad
#if false   // public static void Astral.Logic.Navmesh.DrawRoad(List<Vector3> waypoints, graph graph)
public static void Astral.Logic.Navmesh.DrawRoad(List<Vector3> waypoints, graph graph)
{
	Vector3 vector = Vector3.Empty;
	foreach (Vector3 vector2 in waypoints)
	{
		Brush blue = Brushes.Blue;
		graph.drawFillEllipse(vector2, new Size(4, 4), blue);
		if (vector.IsValid)
		{
			graph.drawLine(vector, vector2, Pens.Blue);
		}
		vector = vector2;
	}
}
#endif
        private static readonly Brush defaultBrush = Brushes.Blue;
        private static readonly Pen defaultPen = new Pen(Color.Blue, 2);

        private static bool PrefixDrawRoad(List<Vector3> waypoints, GraphicsNW graph)
        {
            if (graph is MapperGraphics mapGraphics)
            {
                DrawRoad(waypoints, mapGraphics, defaultPen, defaultBrush);
            }
            else
            {
                Brush blue = Brushes.Blue;
                Vector3 startPos = Vector3.Empty;
                foreach (Vector3 endPos in waypoints)
                {
                    graph.drawFillEllipse(endPos, MapperHelper.Size_4x4, blue);
                    if (startPos.IsValid)
                        graph.drawLine(startPos, endPos, Pens.Blue);
                    startPos = endPos;
                }
            }
            return false;
        }

        /// <summary>
        /// Отрисовка пути <paramref name="waypoints"/> на <paramref name="mapGraphics"/>
        /// </summary>
        internal static void DrawRoad(List<Vector3> waypoints, MapperGraphics mapGraphics, Pen pen, Brush brush)
        {
            mapGraphics.GetWorldPosition(0, 0, out double topLeftX, out double topLeftY);
            mapGraphics.GetWorldPosition(mapGraphics.ImageWidth, mapGraphics.ImageHeight, out double downRightX, out double downRightY);

            using (var wp = waypoints.GetEnumerator())
            {
                if (wp.MoveNext())
                {
#if Labeling_Waypoints
                        int i = 0; 
#endif
                    Vector3 startPos = wp.Current;
                    bool startVisible = false,
                         endVisible;
                    double x = startPos.X,
                           y = startPos.Y;
                    // Отсеиваем и не отрисовываем точки пути, расположенные за пределами видимого изображения
                    if (topLeftX <= x && x <= downRightX
                        && downRightY <= y && y <= topLeftY)
                    {
                        mapGraphics.FillCircleCentered(brush, x, y, 6);
#if Labeling_Waypoints
                            mapGraphics.DrawText(i.ToString(), x, y, Alignment.TopLeft, SystemFonts.DefaultFont, Brushes.White); 
#endif

                        startVisible = true;
                    }
#if Labeling_Waypoints
                        i++; 
#endif
                    while (wp.MoveNext())
                    {
                        Vector3 endPos = wp.Current;
                        x = endPos.X;
                        y = endPos.Y;
                        // Отсеиваем и не отрисовываем точки пути, расположенные за пределами видимого изображения
                        if (topLeftX <= x && x <= downRightX
                            && downRightY <= y && y <= topLeftY)
                        {
                            mapGraphics.FillCircleCentered(brush, x, y, 6);
#if Labeling_Waypoints
                                mapGraphics.DrawText(i.ToString(), x, y, Alignment.TopLeft, SystemFonts.DefaultFont, Brushes.White); 
#endif
                            endVisible = true;
                        }
                        else endVisible = false;
#if Labeling_Waypoints
                            i++; 
#endif
                        if (startVisible || endVisible)
                            mapGraphics.DrawLine(pen, startPos.X, startPos.Y, endPos.X, endPos.Y);
                        startPos = endPos;
                        startVisible = endVisible;
                    }
                }
            }
        }
        #endregion

        #region DrawHotSpots
#if false   // public static void Astral.Logic.Navmesh.DrawHotSpots(List<Vector3> hotspots, graph graph)
public static void Astral.Logic.Navmesh.DrawHotSpots(List<Vector3> hotspots, graph graph)
{
foreach (Vector3 vector in hotspots)
{
	Brush yellow = Brushes.Yellow;
	graph.drawFillEllipse(vector, new Size(12, 12), yellow);
	graph.drawString(vector, hotspots.IndexOf(vector).ToString(), 8, Brushes.Blue, -1, -6);
}
#endif
        private static readonly Brush hotSpotBrush = Brushes.Yellow;

        /// <summary>
        /// Отрисовка точек <paramref name="hotspots"/> на <paramref name="graph"/>
        /// </summary>
        //[HarmonyPatch(typeof(Astral.Logic.Navmesh), "DrawHotSpots"), HarmonyPrefix]
        public static bool DrawHotSpots(List<Vector3> hotspots, GraphicsNW graph)
        {
            if (graph is MapperGraphics graphics)
            {
                graphics.GetWorldPosition(0, 0, out double leftX, out double topY);
                graphics.GetWorldPosition(graphics.ImageWidth, graphics.ImageHeight, out double rightX, out double downY);

                for (int i = 0; i < hotspots.Count; i++)
                {
                    var spot = hotspots[i];
                    double x = spot.X;
                    double y = spot.Y;

                    // Отсеиваем и не отрисовываем HotSpot'ы, расположенные за пределами видимого изображения
                    if (leftX <= x && x <= rightX
                        && downY <= y && y <= topY)
                    {
                        graphics.FillCircleCentered(hotSpotBrush, x, y, 16);
                        graphics.DrawText(i.ToString(), x, y);
                    }
                }
            }
            else
            {
                for (var i = 0; i < hotspots.Count; i++)
                {
                    var vector = hotspots[i];
                    graph.drawFillEllipse(vector, MapperHelper.Size_12x12, Brushes.Blue);
                    graph.drawString(vector, i.ToString(), 8, Brushes.Blue, -1, -6);
                }
            }
            return false;
        }
        #endregion

        #region DrawMeshes
#if false   // public static void Astral.Logic.Classes.Map.Functions.Picture.DrawMeshes(GraphicsNW graph, Graph meshes)
public static void Astral.Logic.Classes.Map.Functions.Picture.DrawMeshes(GraphicsNW graph, Graph meshes)
{
	string obj = "AddPointWork";
	lock (obj)
	{
		double num = graph.getWorldPos(new Point(graph.ImageWidth, graph.ImageHeight)).Distance2D(graph.CenterPosition);
		foreach (object obj2 in meshes.Arcs)
		{
			Arc arc = (Arc)obj2;
			if (arc.StartNode.Passable && arc.EndNode.Passable)
			{
				Vector3 vector = new Vector3((float)arc.StartNode.X, (float)arc.StartNode.Y, (float)arc.StartNode.Z);
				Vector3 worldPos = new Vector3((float)arc.EndNode.X, (float)arc.EndNode.Y, (float)arc.EndNode.Z);
				if (vector.Distance2D(graph.CenterPosition) < num)
				{
					graph.drawLine(vector, worldPos, Pens.Red);
				}
			}
		}
		foreach (object obj3 in meshes.Nodes)
		{
			Node node = (Node)obj3;
			if (node.Passable)
			{
				Brush brush = Brushes.Red;
				int count = node.IncomingArcs.Count;
				int count2 = node.OutgoingArcs.Count;
				if (count == 1 && count2 == 1)
				{
					brush = Brushes.SkyBlue;
				}
				Vector3 vector2 = new Vector3((float)node.X, (float)node.Y, (float)node.Z);
				if (vector2.Distance2D(graph.CenterPosition) < num)
				{
					graph.drawFillEllipse(vector2, new Size(5, 5), brush);
				}
			}
		}
	}
}
#endif
        /// <summary>
        /// Отрисовка графа путей <paramref name="meshes"/> на <paramref name="graph"/>
        /// </summary>
        public static bool DrawMeshes(GraphicsNW graph, IGraph meshes)
        {
            if (graph is null || meshes is null)
                return false;
            if (graph is MapperGraphics mapGraphics)
            {
                var bidirBrush = mapGraphics.DrawingTools.BidirectionalPathBrush;
                var unidirBrush = mapGraphics.DrawingTools.UnidirectionalPathBrush;
                var bidirPen = mapGraphics.DrawingTools.BidirectionalPathPen;
                var unidirPen = mapGraphics.DrawingTools.UnidirectionalPathPen;

                //var graphCache = mapGraphics.GraphCache;
                using (meshes.ReadLock())
                {
                    mapGraphics.GetWorldPosition(0, 0, out double left, out double top);
                    mapGraphics.GetWorldPosition(mapGraphics.ImageWidth, mapGraphics.ImageHeight, out double right, out double down);

                    foreach (Node node in meshes.NodesCollection)
                    {
                        if (node.Passable)
                        {
                            // Отсеиваем и не отрисовываем вершины и ребра, расположенные за пределами видимого изображения
                            if (left <= node.X && node.X <= right
                                && down <= node.Y && node.Y <= top)
                            {
                                bool uniPath = node.IncomingArcsCount <= 1 && node.OutgoingArcsCount <= 1;
                                Brush brush = uniPath ? unidirBrush : bidirBrush;
                                Pen pen = uniPath ? unidirPen : bidirPen;

                                foreach (Arc arc in node.OutgoingArcs)
                                {
                                    if (arc.Passable)
                                        mapGraphics.DrawLine(pen, node.X, node.Y, arc.EndNode.X, arc.EndNode.Y);
                                }
                                mapGraphics.FillCircleCentered(brush, node.Position, 5);
                            }
                        }
                    }
                }
            }
            else
            {
                using (meshes.ReadLock())
                {
                    double num = graph.getWorldPos(new Point(graph.ImageWidth, graph.ImageHeight)).Distance2D(graph.CenterPosition);
                    var nodeSize = new Size(5, 5);
                    foreach (Node node in meshes.NodesCollection)
                    {
                        if (node.Passable)
                        {
                            Brush brush = Brushes.Red;
                            Vector3 nodePos = new Vector3((float)node.X, (float)node.Y, (float)node.Z);
                            if (nodePos.Distance2D(graph.CenterPosition) < num)
                            {
                                int inCount = node.IncomingArcsCount;
                                int outCount = node.OutgoingArcsCount;
                                if (inCount == 1 && outCount == 1)
                                    brush = Brushes.SkyBlue;

                                foreach (Arc arc in node.OutgoingArcs)
                                {
                                    if (arc.EndNode.Passable)
                                    {
                                        Vector3 endPos = new Vector3((float)arc.EndNode.X, (float)arc.EndNode.Y, (float)arc.EndNode.Z);
                                        graph.drawLine(nodePos, endPos, Pens.Red);
                                    }
                                }
                                foreach (Arc arc in node.IncomingArcs)
                                {
                                    if (arc.StartNode.Passable)
                                    {
                                        Vector3 startPos = new Vector3((float)arc.StartNode.X, (float)arc.StartNode.Y, (float)arc.StartNode.Z);
                                        graph.drawLine(startPos, nodePos, Pens.Red);
                                    }
                                }
                                graph.drawFillEllipse(nodePos, nodeSize, brush);
                            }
                        }
                    }
                }
            }

            return false;
        }
        #endregion
    }
}
