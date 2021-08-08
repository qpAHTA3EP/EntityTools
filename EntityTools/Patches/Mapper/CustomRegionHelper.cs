using System.Windows.Forms;
using Astral.Quester.Classes;
using EntityTools.Enums;
using EntityTools.Reflection;
using MyNW.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuesterEditorForm = Astral.Quester.Forms.Editor;
using QuesterMainForm = Astral.Quester.Forms.Main;
using Astral.Logic.Classes.Map;
using System.Drawing;

namespace EntityTools.Patches.Mapper
{
#if PATCH_ASTRAL
    /// <summary>
    /// Помощник для добавления CustomRegion
    /// </summary>
    internal static class CustomRegionHelper
    {
        public static CustomRegion Clone(this CustomRegion @this)
        {
            if (@this != null)
                return new CustomRegion()
                {
                    Position = @this.Position.Clone(),
                    Height = @this.Height,
                    Width = @this.Width,
                    Eliptic = @this.Eliptic,
                    Name = @this.Name
                };
            return null;
        }



        private static Vector3 startPoint = null;
        private static Vector3 endPoint = null;
        private static bool isElliptical = false;
#if AstralMapper
        private static Astral.Forms.UserControls.Mapper mapper; 
#else
        private static MapperExt mapper;
#endif

        private static CustomRegion customRegion = null;

        internal static CustomRegion GetCustomRegion()
        {
            if (customRegion != null
                && customRegion.Position.IsValid
                && customRegion.Height != 0
                && customRegion.Width != 0)
                return customRegion;
            else
            {
                if (startPoint != null && startPoint.IsValid
                    && endPoint != null && endPoint.IsValid)
                {
                    Vector3 topLeft = new Vector3(Math.Min(startPoint.X, endPoint.X), Math.Max(startPoint.Y, endPoint.Y), 0f);
                    Vector3 downRight = new Vector3(Math.Max(startPoint.X, endPoint.X), Math.Min(startPoint.Y, endPoint.Y), 0f);
                    return customRegion = new CustomRegion()
                    {
                        Position = topLeft,
                        Eliptic = isElliptical,
                        Height = (int)(downRight.Y - topLeft.Y),
                        Width = (int)(downRight.X - topLeft.X),
                        Name = string.Empty
                    };
                }
            }
            return null;
        }

        private static DragMode dragMode = DragMode.Disabled;
        private static Vector3 anchorPoint = null;
        public static readonly float DefaultAnchorSize = 8;
        private static float anchorSize = 4;

    #region Reflection
        /// <summary>
        /// Функтор доступа к экземпляру Квестер-редактора
        /// Astral.Quester.Forms.Main.editorForm
        /// </summary>
        private static readonly StaticFieldAccessor<QuesterEditorForm> QuesterEditor = typeof(QuesterMainForm).GetStaticField<QuesterEditorForm>("editorForm");

        /// <summary>
        /// Функтор обновления списка CustomRegion'ов в окне Квестер-редактора
        /// </summary>
        private static Func<Object, System.Action> QuesterEditorRefreshRegions = null;
        #endregion

        /// <summary>
        /// Начала процедуры добавления CustomRegion'a
        /// </summary>
        /// <param name="m"></param>
        /// <param name="elliptical"></param>
#if AstralMapper
        public static void BeginAdd(Astral.Forms.UserControls.Mapper m, bool elliptical = false) 
#else
        public static void BeginAdd(MapperExt m, bool elliptical = false)
#endif
        {
            startPoint = null;
            endPoint = null;
            anchorPoint = null;
            customRegion = null;
            isElliptical = elliptical;
            dragMode = DragMode.Disabled;

            mapper = m;
            if (mapper != null && !mapper.IsDisposed)
            {
                mapper.OnClick += handler_MapperClick;
                mapper.CustomDraw += handler_DrawCustomRegion;
            }
        }

        internal static bool IsComplete
        {
            get
            {
                return dragMode == DragMode.None
                       && ((customRegion != null
                                && customRegion.Position.IsValid
                                && customRegion.Height != 0
                                && customRegion.Width != 0)
                            || (startPoint != null && endPoint != null
                                && startPoint.IsValid
                                && startPoint != endPoint));
            }
        }

        private static void handler_MapperClick(MouseEventArgs me, GraphicsNW g)
        {
            if (me.Button == MouseButtons.Right)
            {
                Vector3 worldPos = g.getWorldPos(me.Location);
                if (startPoint == null)
                {
                    startPoint = new Vector3(worldPos.X, worldPos.Y, 0f);
                    return;
                }
                if (endPoint == null)
                {
                    endPoint = new Vector3(worldPos.X, worldPos.Y, 0f);
                    dragMode = DragMode.None;
                }

                if (customRegion == null
                    || !customRegion.Position.IsValid)
                {
                    Vector3 topLeft = new Vector3(Math.Min(startPoint.X, endPoint.X), Math.Max(startPoint.Y, endPoint.Y), 0f);
                    Vector3 downRight = new Vector3(Math.Max(startPoint.X, endPoint.X), Math.Min(startPoint.Y, endPoint.Y), 0f);
                    customRegion = new CustomRegion()
                    {
                        Position = topLeft,
                        Eliptic = isElliptical,
                        Height = (int)(downRight.Y - topLeft.Y),
                        Width = (int)(downRight.X - topLeft.X),
                    };
                    dragMode = DragMode.None;
                    return;
                }

                if (dragMode != DragMode.Disabled)
                {
                    if (anchorPoint == null)
                        // вычисление якоря
                        SelectAnchor(customRegion, worldPos, out anchorPoint, out dragMode);
                    else
                    {
                        // вычисление изменений/смещений региона
                        CustomRegion cr = TransformCustomRegion(customRegion, anchorPoint, worldPos, dragMode);
                        if (cr != null)
                        {
                            customRegion = cr;
                            dragMode = DragMode.None;
                            anchorPoint = null;
                        }
                    }
                }
            }
        }

        private static bool SelectAnchor(CustomRegion cr, Vector3 pos, out Vector3 anchor, out DragMode mode)
        {
            if (cr != null
                && cr.Position.IsValid)
            {
                float hulfAnchorEdgeSize = Math.Max(1, Math.Min(Math.Min(Math.Abs(cr.Width / 4), Math.Abs(cr.Height/ 4)), DefaultAnchorSize / 2));

                Vector3 topLeft = cr.Position.Clone();
                // TopLeft
                if (CheckAnchorSelection(topLeft, pos, hulfAnchorEdgeSize))
                {
                    anchor = topLeft;
                    mode = DragMode.TopLeft;
                    return true;
                }

                // TopCenter
                Vector3 topCenter = new Vector3(topLeft.X + cr.Width / 2f, topLeft.Y, 0f);
                if (CheckAnchorSelection(topCenter, pos, hulfAnchorEdgeSize))
                {
                    anchor = topCenter;
                    mode = DragMode.TopCenter;
                    return true;
                }

                // TopRight
                Vector3 topRight = new Vector3(topLeft.X + cr.Width, topLeft.Y, 0f);
                if (CheckAnchorSelection(topRight, pos, hulfAnchorEdgeSize))
                {
                    anchor = topRight;
                    mode = DragMode.TopRight;
                    return true;
                }

                // Left
                Vector3 left = new Vector3(topLeft.X, topLeft.Y + cr.Height / 2f, 0f);
                if (CheckAnchorSelection(left, pos, hulfAnchorEdgeSize))
                {
                    anchor = left;
                    mode = DragMode.Left;
                    return true;
                }

                // Center
                Vector3 center = new Vector3(topLeft.X + cr.Width / 2f, topLeft.Y + cr.Height / 2f, 0f);
                if (CheckAnchorSelection(center, pos, hulfAnchorEdgeSize))
                {
                    anchor = center;
                    mode = DragMode.Center;
                    return true;
                }

                // Right
                Vector3 right = new Vector3(topLeft.X + cr.Width, topLeft.Y + cr.Height / 2f, 0f);
                if (CheckAnchorSelection(right, pos, hulfAnchorEdgeSize))
                {
                    anchor = right;
                    mode = DragMode.Right;
                    return true;
                }

                // DownLeft
                Vector3 downLeft = new Vector3(topLeft.X, topLeft.Y + cr.Height, 0f);
                if (CheckAnchorSelection(downLeft, pos, hulfAnchorEdgeSize))
                {
                    anchor = downLeft;
                    mode = DragMode.DownLeft;
                    return true;
                }

                // DownCenter
                Vector3 downCenter = new Vector3(topLeft.X + cr.Width / 2f, topLeft.Y + cr.Height, 0f);
                if (CheckAnchorSelection(downCenter, pos, hulfAnchorEdgeSize))
                {
                    anchor = downCenter;
                    mode = DragMode.DownCenter;
                    return true;
                }

                // DownRight
                Vector3 downRight = new Vector3(cr.Position.X + cr.Width,
                                                cr.Position.Y + cr.Height, 0f);
                if (CheckAnchorSelection(downRight, pos, hulfAnchorEdgeSize))
                {
                    anchor = downRight;
                    mode = DragMode.DownRight;
                    return true;
                }
            }
            anchor = null;
            mode = DragMode.None;
            return false;
        }

        private static bool CheckAnchorSelection(Vector3 anchor, Vector3 pos, float hulfAnchorEdgeSize)
        {
            float dx = anchor.X - pos.X;
            float dy = anchor.Y - pos.Y;
            return Math.Abs(dx) <= hulfAnchorEdgeSize
                && Math.Abs(dy) <= hulfAnchorEdgeSize;
        }

        public static CustomRegion TransformCustomRegion(CustomRegion cr, Vector3 anchorPoint, Vector3 worldPos, DragMode mode)
        {
            if (cr != null
                && cr.Position.IsValid)
            {
                int dx = (int)Math.Round(worldPos.X - anchorPoint.X);
                int dy = (int)Math.Round(worldPos.Y - anchorPoint.Y);

                switch (mode)
                {
                    case DragMode.TopLeft:
                        cr.Position.X += dx;
                        cr.Position.Y += dy;
                        cr.Width -= dx;
                        cr.Height -= dy;
                        return cr;
                    case DragMode.TopCenter:
                        cr.Position.Y += dy;
                        cr.Height -= dy;
                        return cr;
                    case DragMode.TopRight:
                        cr.Position.Y += dy;
                        cr.Width += dx;
                        cr.Height -= dy;
                        return cr;
                    case DragMode.Left:
                        cr.Position.X += dx;
                        cr.Width -= dx;
                        return cr;
                    case DragMode.Center:
                        cr.Position.X += dx;
                        cr.Position.Y += dy;
                        return cr;
                    case DragMode.Right:
                        cr.Width += dx;
                        return cr;
                    case DragMode.DownLeft:
                        cr.Position.X += dx;
                        cr.Width -= dx;
                        cr.Height += dy;
                        return cr;
                    case DragMode.DownCenter:
                        cr.Height += dy;
                        return cr;
                    case DragMode.DownRight:
                        cr.Width += dx;
                        cr.Height += dy;
                        return cr;
                }
            }
            return null;
        }

        private static void handler_DrawCustomRegion(GraphicsNW g)
        {
            Vector3 topLeft, top, topRight, left, center, right, downLeft, down, downRight;
            float width, height;

            if (customRegion == null)
            {
                if (startPoint != null)
                {
                    if (endPoint != null)
                    {
                        topLeft = new Vector3(Math.Min(startPoint.X, endPoint.X),
                                              Math.Max(startPoint.Y, endPoint.Y), 0f);
                        downRight = new Vector3(Math.Max(startPoint.X, endPoint.X),
                                                Math.Min(startPoint.Y, endPoint.Y), 0f);

                        height = downRight.Y - topLeft.Y; // Math.Max(startPoint.Y, endPoint.Y) - Math.Min(startPoint.Y, endPoint.Y)
                        width = downRight.X - topLeft.X;  // Math.Max(startPoint.X, endPoint.X) - Math.Min(startPoint.X, endPoint.X)

                        customRegion = new CustomRegion()
                        {
                            Position = topLeft.Clone(),
                            Height = (int)height,
                            Width = (int)width,
                            Eliptic = isElliptical
                        };
                    }
                    else
                    {
                        Vector3 worldPos = g.getWorldPos(mapper.RelativeMousePosition);
                        topLeft = new Vector3(Math.Min(startPoint.X, worldPos.X),
                                              Math.Max(startPoint.Y, worldPos.Y), 0f);
                        downRight = new Vector3(Math.Max(startPoint.X, worldPos.X),
                                                Math.Min(startPoint.Y, worldPos.Y), 0f);//*/

                        height = downRight.Y - topLeft.Y; // Math.Max(startPoint.Y, endPoint.Y) - Math.Min(startPoint.Y, endPoint.Y)
                        width = downRight.X - topLeft.X;  // Math.Max(startPoint.X, endPoint.X) - Math.Min(startPoint.X, endPoint.X)
                    }
                }
                else return;
            }
            else
            {
                CustomRegion cr;
                if (dragMode == DragMode.None)
                    cr = customRegion;
                else
                    cr = TransformCustomRegion(customRegion.Clone(), anchorPoint, g.getWorldPos(mapper.RelativeMousePosition), dragMode);

                if (cr == null)
                    return;

                topLeft = cr.Position;
                downRight = new Vector3(cr.Position.X + cr.Width,
                                        cr.Position.Y + cr.Height, 0);

                height = cr.Height; // Math.Max(startPoint.Y, endPoint.Y) - Math.Min(startPoint.Y, endPoint.Y)
                width = cr.Width;  // Math.Max(startPoint.X, endPoint.X) - Math.Min(startPoint.X, endPoint.X)
            }

            Pen penRect = (isElliptical) ? Pens.DarkOliveGreen : Pens.LimeGreen;
            Pen penCR = Pens.LimeGreen;
            Brush brushRect = (isElliptical) ? Brushes.DarkOliveGreen : Brushes.LimeGreen;
            Brush brushCR = Brushes.LimeGreen;
            bool drawEdgeAnchors = Math.Abs(height) > DefaultAnchorSize * 3 && Math.Abs(width) > DefaultAnchorSize * 3;
            bool drawCornerAnchors = Math.Abs(height) > DefaultAnchorSize && Math.Abs(width) > DefaultAnchorSize;
            float hulfAnchorEdgeSize = Math.Max(1, Math.Min(Math.Min(Math.Abs(width / 4), Math.Abs(height / 4)), DefaultAnchorSize / 2));

            // Отрисовака опорного прямоугольника якоря topLeft
            g.drawRectangle(topLeft, new Vector3(width, height, 0), penRect);
            top = new Vector3(topLeft.X + width / 2, topLeft.Y, 0f);
            down = new Vector3(topLeft.X + width / 2, topLeft.Y + height, 0f);
            g.drawLine(top, down, penRect);
            right = new Vector3(topLeft.X + width, topLeft.Y + height / 2, 0f);
            left = new Vector3(topLeft.X, topLeft.Y + height / 2, 0f);
            g.drawLine(left, right, penRect);

            if (drawCornerAnchors)
            {
                DrawAnchor(g, topLeft, hulfAnchorEdgeSize, brushRect);
                DrawAnchor(g, downRight, hulfAnchorEdgeSize, brushRect);
                topRight = new Vector3(topLeft.X + width, topLeft.Y, 0f);
                DrawAnchor(g, topRight, hulfAnchorEdgeSize, brushRect);
                downLeft = new Vector3(topLeft.X, topLeft.Y + height, 0f);
                DrawAnchor(g, downLeft, hulfAnchorEdgeSize, brushRect);
                center = new Vector3(topLeft.X + width / 2, topLeft.Y + height / 2, 0f);
                DrawAnchor(g, center, hulfAnchorEdgeSize, brushRect); 
            }

            // Отрисовка Эллипса
            if (isElliptical)
                g.drawComplexeEllipse(topLeft, new Vector3(width, height, 0), penCR);

            if (drawEdgeAnchors)
            {
                // Отрисовака якоря top
                DrawAnchor(g, top, hulfAnchorEdgeSize, brushCR);
                // Отрисовака якоря left
                DrawAnchor(g, left, hulfAnchorEdgeSize, brushCR);
                // Отрисовака якоря right
                DrawAnchor(g, right, hulfAnchorEdgeSize, brushCR);
                // Отрисовака якоря down
                DrawAnchor(g, down, hulfAnchorEdgeSize, brushCR); 
            }
        }

        public static void DrawAnchor(GraphicsNW g, Vector3 anchor, float hulfAnchorEdgeSize, Brush brush = null)
        {
            if (brush == null)
                brush = Brushes.Green;
#if true
            var list = new List<Vector3>() { new Vector3(anchor.X - hulfAnchorEdgeSize, anchor.Y - hulfAnchorEdgeSize, 0),
                                             new Vector3(anchor.X + hulfAnchorEdgeSize, anchor.Y - hulfAnchorEdgeSize, 0),
                                             new Vector3(anchor.X + hulfAnchorEdgeSize, anchor.Y + hulfAnchorEdgeSize, 0),
                                             new Vector3(anchor.X - hulfAnchorEdgeSize, anchor.Y + hulfAnchorEdgeSize, 0) };

            g.drawFillPolygon(list, brush);
#elif false
            g.drawFillPolygon(new List<Vector3>() { new Vector3(anchor.X - anchorSize, anchor.Y - anchorSize, 0),
                                                    new Vector3(anchor.X + anchorSize, anchor.Y - anchorSize, 0),
                                                    new Vector3(anchor.X + anchorSize, anchor.Y + anchorSize, 0),
                                                    new Vector3(anchor.X - anchorSize, anchor.Y + anchorSize, 0),
                                                  }, brush); 
#else
            float halfSize = (float)(anchorSize * g.Zoom);
            g.drawFillPolygon(new List<Vector3>() { new Vector3(anchor.X - halfSize, anchor.Y - halfSize, 0),
                                                    new Vector3(anchor.X + halfSize, anchor.Y - halfSize, 0),
                                                    new Vector3(anchor.X + halfSize, anchor.Y + halfSize, 0),
                                                    new Vector3(anchor.X - halfSize, anchor.Y + halfSize, 0),
                                                  }, brush);
#endif
        }

        internal static void RefreshQuesterEditorForm()
        {
            if (QuesterEditor.Value is QuesterEditorForm editor
                && !editor.IsDisposed)
            {
                if (QuesterEditorRefreshRegions == null)
                {
                    if ((QuesterEditorRefreshRegions = typeof(QuesterEditorForm).GetAction("RefreshRegions")) != null)
                        QuesterEditorRefreshRegions(editor)();
                }
                else QuesterEditorRefreshRegions(editor)();
            }
        }

        /// <summary>
        /// Завершение процедуры добавления CustomRegion'a
        /// </summary>
        internal static bool Finish(string crName)
        {
            if (dragMode == DragMode.None)
            {
                CustomRegion cr = GetCustomRegion();
                if (cr != null
                    && !string.IsNullOrEmpty(crName))
                {
                    if (customRegion == null
                        || !customRegion.Position.IsValid)
                    {
                        Vector3 topLeft = new Vector3(Math.Min(startPoint.X, endPoint.X), Math.Max(startPoint.Y, endPoint.Y), 0f);
                        Vector3 downRight = new Vector3(Math.Max(startPoint.X, endPoint.X), Math.Min(startPoint.Y, endPoint.Y), 0f);
                        customRegion = new CustomRegion()
                        {
                            Position = topLeft,
                            Eliptic = isElliptical,
                            Height = (int)(downRight.Y - topLeft.Y),
                            Width = (int)(downRight.X - topLeft.X),
                            Name = crName
                        };

                    }
                    else customRegion.Name = crName;

                    Astral.Quester.API.CurrentProfile.CustomRegions.Add(customRegion);
                    RefreshQuesterEditorForm();
                    Reset();

                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Сброс всех сохраненных значений
        /// </summary>
        internal static void Reset()
        {
            startPoint = null;
            endPoint = null;
            anchorPoint = null;
            customRegion = null;
            dragMode = DragMode.Disabled;

            if (mapper != null && !mapper.IsDisposed)
            {
                mapper.OnClick -= handler_MapperClick;
                mapper.CustomDraw -= handler_DrawCustomRegion;
                mapper = null;
            }
        }
    } 
#endif
}
