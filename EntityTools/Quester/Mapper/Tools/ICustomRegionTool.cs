using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Astral.Quester.Classes;
using DevExpress.XtraVerticalGrid.Internal;

namespace EntityTools.Quester.Mapper.Tools
{
    interface ICustomRegionTool : IMapperTool
    {
        /// <summary>
        /// Флаг, указывающий является ли CustomRegion элиптическим
        /// </summary>
        bool IsElliptical { get; set; }

        bool IsCorrect { get; }

        CustomRegion Apply(string name);

        /// <summary>
        /// Координата <see cref="X"/> верхнего левого угла CustomRegion'a в игровых координата
        /// </summary>
        int X { get; set; }
        /// <summary>
        /// Координата <see cref="Y"/> верхнего левого угла CustomRegion'a в игровых координата
        /// </summary>
        int Y { get; set; }
        /// <summary>
        /// Ширина CustomRegion'a в игровых координата
        /// </summary>
        int Widths { get; set; }
        /// <summary>
        /// Высота CustomRegion'a в игровых координата
        /// </summary>
        int Height { get; set; }
    }
}
