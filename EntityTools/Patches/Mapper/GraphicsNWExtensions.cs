using Astral.Logic.Classes.Map;
using MyNW.Classes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityTools.Patches.Mapper
{
    public static class GraphicsNWExtensions
    {
        static float cos30 = (float)Math.Cos(Math.PI / 3);
        static float tg30 = (float)Math.Tan(Math.PI / 3);
        static float sin30 = 0.5f;

        /// <summary>
        /// Отрисовка заполненного равностороннего треугольника с основанием <paramref name="edge"/> внизу
        /// </summary>
        public static void drawFillTriangle(this GraphicsNW graphicsNW, Vector3 location, float edge, Brush brush)
        {
            float x = location.X,
                  y = location.Y,
                  r = edge / cos30,
                  dx = edge / 2,
                  dy = dx * tg30;

            List<Vector3> coords = new List<Vector3>() {
                    new Vector3(x,      y + r,  0),
                    new Vector3(x - dx, y - dy, 0),
                    new Vector3(x + dx, y - dy, 0)
                };
            graphicsNW.drawFillPolygon(coords, brush);
        }
        /// <summary>
        /// Отрисовка перевернутого заполненного равностороннего треугольника с основанием <paramref name="edge"/> вверху
        /// </summary>
        public static void drawFillUpsideTriangle(this GraphicsNW graphicsNW, Vector3 location, float edge, Brush brush)
        {
            float x = location.X,
                  y = location.Y,
                  r = edge / cos30,
                  dx = edge / 2,
                  dy = dx * tg30;

            List<Vector3> coords = new List<Vector3>() {
                    new Vector3(x,      y - r,  0),
                    new Vector3(x - dx, y + dy, 0),
                    new Vector3(x + dx, y + dy, 0)
                };
            graphicsNW.drawFillPolygon(coords, brush);
        }
        /// <summary>
        /// Отрисовка заполненного квадрата со стороной <paramref name="edge"/>
        /// </summary>
        public static void drawFillSquare(this GraphicsNW graphicsNW, Vector3 location, float edge, Brush brush)
        {
            float x = location.X,
                  y = location.Y,
                  halfEdge = edge / 2;

            List<Vector3> coords = new List<Vector3>() {
                    new Vector3(x - halfEdge, y - halfEdge, 0),
                    new Vector3(x - halfEdge, y + halfEdge, 0),
                    new Vector3(x + halfEdge, y + halfEdge, 0),
                    new Vector3(x + halfEdge, y - halfEdge, 0)
                };
            graphicsNW.drawFillPolygon(coords, brush);
        }
        /// <summary>
        /// Отрисовка заполненого ромба с диагоналями <paramref name="diagonalX"/> и <paramref name="diagonalY"/>
        /// </summary>
        /// <param name="graphicsNW"></param>
        /// <param name="location"></param>
        /// <param name="diagonalX"></param>
        /// <param name="diagonalY"></param>
        /// <param name="brush"></param>
        public static void drawFillRhomb(this GraphicsNW graphicsNW, Vector3 location, float diagonalX, float diagonalY, Brush brush)
        {
            float x = location.X,
                  y = location.Y,
                  dx = diagonalX / 2,
                  dy = diagonalY / 2;

            List<Vector3> coords = new List<Vector3>() {
                    new Vector3(x + dx, y,      0),
                    new Vector3(x,      y + dy, 0),
                    new Vector3(x - dx, y,      0),
                    new Vector3(x,      y - dy, 0)
                };
            graphicsNW.drawFillPolygon(coords, brush);
        }
    }
}
