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
using System.Reflection;

namespace EntityTools.Patches.Mapper
{
#if PATCH_ASTRAL
    /// <summary>
    /// Помощник для добавления CustomRegion
    /// </summary>
    internal static class MapperHelper_CustomRegion
    {
#if true
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
#endif
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

        private static RegionTransformMode dragMode = RegionTransformMode.Disabled;
        private static Vector3 anchorPoint = null;
        public static readonly float DefaultAnchorSize = 8.5f;

    #region Reflection
        /// <summary>
        /// Функтор доступа к экземпляру Квестер-редактора
        /// Astral.Quester.Forms.Main.editorForm
        /// </summary>
        private static readonly StaticFieldAccessor<QuesterEditorForm> QuesterEditor = typeof(QuesterMainForm).GetStaticField<QuesterEditorForm>("editorForm");

        /// <summary>
        /// Функтор обновления списка CustomRegion'ов в окне Квестер-редактора
        /// </summary>
        private static Func<object, System.Action> QuesterEditor_RefreshRegions = null;
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
            dragMode = RegionTransformMode.Disabled;

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
                return dragMode == RegionTransformMode.None
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
                    dragMode = RegionTransformMode.None;
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
                    dragMode = RegionTransformMode.None;
                    return;
                }

                if (dragMode != RegionTransformMode.Disabled)
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
                            dragMode = RegionTransformMode.None;
                            anchorPoint = null;
                        }
                    }
                }
            }
        }

        private static bool SelectAnchor(CustomRegion cr, Vector3 pos, out Vector3 anchor, out RegionTransformMode mode)
        {
            if (cr != null
                && cr.Position.IsValid)
            {
                float hulfAnchorEdgeSize = Math.Max(1.5f, Math.Min(Math.Min(Math.Abs(cr.Width / 4), Math.Abs(cr.Height/ 4)), DefaultAnchorSize / 2));

                Vector3 topLeft = cr.Position.Clone();
                // TopLeft
                if (CheckAnchorSelection(topLeft, pos, hulfAnchorEdgeSize))
                {
                    anchor = topLeft;
                    mode = RegionTransformMode.TopLeft;
                    return true;
                }

                // TopCenter
                Vector3 topCenter = new Vector3(topLeft.X + cr.Width / 2f, topLeft.Y, 0f);
                if (CheckAnchorSelection(topCenter, pos, hulfAnchorEdgeSize))
                {
                    anchor = topCenter;
                    mode = RegionTransformMode.TopCenter;
                    return true;
                }

                // TopRight
                Vector3 topRight = new Vector3(topLeft.X + cr.Width, topLeft.Y, 0f);
                if (CheckAnchorSelection(topRight, pos, hulfAnchorEdgeSize))
                {
                    anchor = topRight;
                    mode = RegionTransformMode.TopRight;
                    return true;
                }

                // Left
                Vector3 left = new Vector3(topLeft.X, topLeft.Y + cr.Height / 2f, 0f);
                if (CheckAnchorSelection(left, pos, hulfAnchorEdgeSize))
                {
                    anchor = left;
                    mode = RegionTransformMode.Left;
                    return true;
                }

                // Center
                Vector3 center = new Vector3(topLeft.X + cr.Width / 2f, topLeft.Y + cr.Height / 2f, 0f);
                if (CheckAnchorSelection(center, pos, hulfAnchorEdgeSize))
                {
                    anchor = center;
                    mode = RegionTransformMode.Center;
                    return true;
                }

                // Right
                Vector3 right = new Vector3(topLeft.X + cr.Width, topLeft.Y + cr.Height / 2f, 0f);
                if (CheckAnchorSelection(right, pos, hulfAnchorEdgeSize))
                {
                    anchor = right;
                    mode = RegionTransformMode.Right;
                    return true;
                }

                // DownLeft
                Vector3 downLeft = new Vector3(topLeft.X, topLeft.Y + cr.Height, 0f);
                if (CheckAnchorSelection(downLeft, pos, hulfAnchorEdgeSize))
                {
                    anchor = downLeft;
                    mode = RegionTransformMode.DownLeft;
                    return true;
                }

                // DownCenter
                Vector3 downCenter = new Vector3(topLeft.X + cr.Width / 2f, topLeft.Y + cr.Height, 0f);
                if (CheckAnchorSelection(downCenter, pos, hulfAnchorEdgeSize))
                {
                    anchor = downCenter;
                    mode = RegionTransformMode.DownCenter;
                    return true;
                }

                // DownRight
                Vector3 downRight = new Vector3(cr.Position.X + cr.Width,
                                                cr.Position.Y + cr.Height, 0f);
                if (CheckAnchorSelection(downRight, pos, hulfAnchorEdgeSize))
                {
                    anchor = downRight;
                    mode = RegionTransformMode.DownRight;
                    return true;
                }
            }
            anchor = null;
            mode = RegionTransformMode.None;
            return false;
        }

        private static bool CheckAnchorSelection(Vector3 anchor, Vector3 pos, float hulfAnchorEdgeSize)
        {
            float dx = anchor.X - pos.X;
            float dy = anchor.Y - pos.Y;
            return Math.Abs(dx) <= hulfAnchorEdgeSize
                && Math.Abs(dy) <= hulfAnchorEdgeSize;
        }

        public static CustomRegion TransformCustomRegion(CustomRegion cr, Vector3 anchorPoint, Vector3 worldPos, RegionTransformMode mode)
        {
            if (cr != null
                && cr.Position.IsValid)
            {
                int dx = (int)Math.Round(worldPos.X - anchorPoint.X);
                int dy = (int)Math.Round(worldPos.Y - anchorPoint.Y);

                switch (mode)
                {
                    case RegionTransformMode.TopLeft:
                        cr.Position.X += dx;
                        cr.Position.Y += dy;
                        cr.Width -= dx;
                        cr.Height -= dy;
                        return cr;
                    case RegionTransformMode.TopCenter:
                        cr.Position.Y += dy;
                        cr.Height -= dy;
                        return cr;
                    case RegionTransformMode.TopRight:
                        cr.Position.Y += dy;
                        cr.Width += dx;
                        cr.Height -= dy;
                        return cr;
                    case RegionTransformMode.Left:
                        cr.Position.X += dx;
                        cr.Width -= dx;
                        return cr;
                    case RegionTransformMode.Center:
                        cr.Position.X += dx;
                        cr.Position.Y += dy;
                        return cr;
                    case RegionTransformMode.Right:
                        cr.Width += dx;
                        return cr;
                    case RegionTransformMode.DownLeft:
                        cr.Position.X += dx;
                        cr.Width -= dx;
                        cr.Height += dy;
                        return cr;
                    case RegionTransformMode.DownCenter:
                        cr.Height += dy;
                        return cr;
                    case RegionTransformMode.DownRight:
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
                if (dragMode == RegionTransformMode.None)
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
            float hulfAnchorEdgeSize = Math.Max(1.5f, Math.Min(Math.Min(Math.Abs(width / 4), Math.Abs(height / 4)), DefaultAnchorSize / 2));

            // Отрисовака опорного прямоугольника якоря topLeft
            g.drawRectangle(topLeft, new Vector3(width, height, 0), penRect);
            if (drawEdgeAnchors)
            {
                top = new Vector3(topLeft.X + width / 2, topLeft.Y, 0f);
                down = new Vector3(topLeft.X + width / 2, topLeft.Y + height, 0f);
                g.drawLine(top, down, penRect);
                right = new Vector3(topLeft.X + width, topLeft.Y + height / 2, 0f);
                left = new Vector3(topLeft.X, topLeft.Y + height / 2, 0f);
                g.drawLine(left, right, penRect);
                
                // Отрисовака якоря top
                DrawAnchor(g, top, hulfAnchorEdgeSize, brushCR);
                // Отрисовака якоря left
                DrawAnchor(g, left, hulfAnchorEdgeSize, brushCR);
                // Отрисовака якоря right
                DrawAnchor(g, right, hulfAnchorEdgeSize, brushCR);
                // Отрисовака якоря down
                DrawAnchor(g, down, hulfAnchorEdgeSize, brushCR); 
            }

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
                if (QuesterEditor_RefreshRegions == null)
                {
                    if ((QuesterEditor_RefreshRegions = typeof(QuesterEditorForm).GetAction("RefreshRegions")) != null)
                        QuesterEditor_RefreshRegions(editor)();
                }
                else QuesterEditor_RefreshRegions(editor)();
            }
        }

        /// <summary>
        /// Завершение процедуры добавления CustomRegion'a
        /// </summary>
        internal static bool Finish(string crName)
        {
            if (dragMode == RegionTransformMode.None)
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
#if false
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
#endif
        }
    } 
#endif
}
