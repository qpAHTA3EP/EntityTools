using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using Astral.Logic.Classes.Map;
using EntityTools.Patches.Mapper;
using EntityTools.Reflection;
using MyNW.Classes;
#if PATCH_ASTRAL && HARMONY

# endif

namespace EntityTools.Patches.Navmesh
{
    internal class Patch_Astral_Logic_Navmesh_DrawHotSpots : Patch
    {
        private static readonly Brush brush = Brushes.Yellow;

        internal Patch_Astral_Logic_Navmesh_DrawHotSpots()
        {
            if (NeedInjecttion)
            {
                MethodInfo mi = typeof(Astral.Logic.Navmesh).GetMethod("DrawHotSpots", ReflectionHelper.DefaultFlags);
                if (mi != null)
                {
                    methodToReplace = mi;
                }
                else throw new Exception("Patch_Astral_Logic_Navmesh_DrawHotSpots: fail to initialize 'methodToReplace'");

                methodToInject = GetType().GetMethod(nameof(DrawHotSpots), ReflectionHelper.DefaultFlags);
            }
        }

        public sealed override bool NeedInjecttion => EntityTools.Config.Mapper.Patch;

#if false
public static void DrawHotSpots(List<Vector3> hotspots, GraphicsNW graph)
{
	foreach (Vector3 vector in hotspots)
	{
		Brush yellow = Brushes.Yellow;
		graph.drawFillEllipse(vector, new Size(12, 12), yellow);
		graph.drawString(vector, hotspots.IndexOf(vector).ToString(), 8, Brushes.Blue, -1, -6);
	}
}
#endif
        /// <summary>
        /// Отрисовка пути <paramref name="waypoints"/> на <paramref name="graphicsNW"/>
        /// </summary>
        public static void DrawHotSpots(List<Vector3> hotspots, GraphicsNW graphicsNW)
        {
            if (graphicsNW is MapperGraphics graphics)
            {
                graphics.GetWorldPosition(0, 0, out double leftX, out double topY);
                graphics.GetWorldPosition(graphics.ImageWidth, graphics.ImageHeight, out double rightX, out double downY);

                for(int i = 0; i < hotspots.Count; i++)
                {
                    Vector3 spot = hotspots[i];
                    double x = spot.X;
                    double y = spot.Y;
                    // Отсеиваем и не отрисовываем HotSpot'ы, расположенные за пределами видимого изображения
                    if (leftX <= x && x <= rightX
                        && downY <= y && y <= topY)
                    {
                        graphics.FillCircleCentered(brush, x, y, 16);
                        //graphics.drawString(spot, i.ToString(), 8, Brushes.Blue, -1, (i < 10) ? -6 : -12);
                        graphics.DrawText(i.ToString(), x, y);
                    }
                } 
            }
            else
            {
                Vector3 startPos = new Vector3();
                foreach (Vector3 vector in hotspots)
                {
                    graphicsNW.drawFillEllipse(vector, MapperHelper.Size_12x12/*new Size(12, 12)*/, Brushes.Blue);
                    graphicsNW.drawString(vector, hotspots.IndexOf(vector).ToString(), 8, Brushes.Blue, -1, -6);
                }
            }
        }
    }
}
