#if PATCH_ASTRAL && HARMONY
using HarmonyLib;
# endif

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Reflection;
using Astral.Logic.Classes.Map;
using AStar;
using System.Drawing;
using MyNW.Classes;
using EntityTools.Reflection;

namespace EntityTools.Patches.Mapper
{
    internal class Patch_Astral_Navmesh_DrawHotSpots : Patch
    {
        internal Patch_Astral_Navmesh_DrawHotSpots()
        {
            MethodInfo mi = typeof(Astral.Logic.Navmesh).GetMethod("DrawHotSpots", ReflectionHelper.DefaultFlags);
            if (mi != null)
            {
                methodToReplace = mi;
            }
            else throw new Exception("Patch_Astral_Navmesh_DrawHotSpots: fail to initialize 'methodToReplace'");

            methodToInject = GetType().GetMethod(nameof(DrawHotSpots), ReflectionHelper.DefaultFlags);
        }


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

                Brush brush = Brushes.Yellow;
                for(int i = 0; i < hotspots.Count; i++)
                {
                    Vector3 spot = hotspots[i];

                    // Отсеиваем и не отрисовываем HotSpot'ы, расположенные за пределами видимого изображения
                    if (leftX <= spot.X && spot.X <= rightX
                        && downY <= spot.Y && spot.Y <= topY)
                    {
                        graphics.FillCircleCentered(brush, spot, 12);
                        graphics.drawString(spot, i.ToString(), 8, Brushes.Blue, -1, (i < 10) ? -6 : -12);
                    }
                } 
            }
            else
            {
                Brush blue = Brushes.Yellow;
                Vector3 startPos = new Vector3();
                foreach (Vector3 vector in hotspots)
                {
                    graphicsNW.drawFillEllipse(vector, new Size(12, 12), blue);
                    graphicsNW.drawString(vector, hotspots.IndexOf(vector).ToString(), 8, Brushes.Blue, -1, -6);
                }
            }
        }
    }
}
