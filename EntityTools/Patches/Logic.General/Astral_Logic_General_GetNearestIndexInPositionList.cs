using ACTP0Tools.Reflection;
using MyNW.Classes;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace EntityTools.Patches.Logic.General
{
    /// <summary>
    /// Патч метода <see cref="Astral.Logic.General.GetNearestIndexInPositionList(List{Vector3}, Vector3)"/>
    /// </summary>
    // ReSharper disable once InconsistentNaming
    internal class Astral_Logic_General_GetNearestIndexInPositionList : Patch
    {
        internal Astral_Logic_General_GetNearestIndexInPositionList()
        {
            if (NeedInjection)
            {
                MethodInfo mi = typeof(Astral.Logic.General).GetMethod("GetNearestIndexInPositionList", ReflectionHelper.DefaultFlags);
                methodToReplace = mi != null
                    ? mi
                    : throw new Exception(
                        "Astral_Logic_General_GetNearestIndexInPositionList: fail to initialize 'methodToReplace'");

                methodToInject = GetType().GetMethod(nameof(GetNearestIndexInPositionList), ReflectionHelper.DefaultFlags);
            }
        }

        public sealed override bool NeedInjection => EntityTools.Config.Patches.GetNearestIndexInPositionList;

#if false
// Astral.Logic.General
public static int GetNearestIndexInPositionList(List<Vector3> waypoints, Vector3 pos)
{
	int result = 0;
	double num = -1.0;
	int num2 = 0;
	foreach (Vector3 vector in waypoints)
	{
		double num3 = vector.Distance3D(pos);
		if (num == -1.0 || num3 < num)
		{
			result = num2;
			num = num3;
		}
		num2++;
	}
	return result;

}
#endif
        internal static int GetNearestIndexInPositionList(List<Vector3> waypoints, Vector3 pos)
        {
            int result = -1;

            int count = waypoints.Count;
            int current = 0;

            if(count > 0
               && pos != null && pos.IsValid)
            {
                result = current;
                if(count > 1)
                {
                    // в списке минимум 2 точки
                    var wp0 = waypoints[current];

                    float x = pos.X, // координаты pos - смещенного "начала координат"
                          y = pos.Y,
                          z = pos.Z,
                          // нормализованные координаты вектора {pos -> wp0} и квадрат расстояния до первой точки wp0
                          // координаты  
                          x0 = wp0.X - x,
                          y0 = wp0.Y - y,
                          z0 = wp0.Z - z,
                          dist0 = x0 * x0 + y0 * y0 + z0 * z0;

                    float minDist = dist0;

                    current++;
                    do
                    {
                        var wp1 = waypoints[current];
                        // нормализованные координаты вектора {pos -> wp1} и квадрат расстояния до второй точки wp1
                        float x1 = wp1.X - x,
                              y1 = wp1.Y - y,
                              z1 = wp1.Z - z,
                              dist1 = x1 * x1 + y1 * y1 + z1 * z1;

                        if (dist1 < minDist)
                        {
                            // wp1 ближе чем wp0
                            minDist = dist1;
                            result = current;
                        }
#if false
                        // Данная оптимизация приводит к некорректному поведению
                        else
                        {
                            // Вычисляем косинус угла между векторами (pos -> wp0) и (pos -> wp1)
                            // из формулы скалярного произведения векторов
                            double cos = (x0 * x1 + y0 * y1 + z0 * z1) / Math.Sqrt(dist0 * dist1);

                            if (cos < 0)
                            {
                                // угол между векторами (pos -> wp0) и (pos -> wp1) больше 90 градусов,
                                // значит wp0 и wp1 лежат в разных полуплоскостях
                                // При этом dist1 больше минимального расстояния minDist
                                // следовательно wp1 дальше wp0 (который находится позади персонажа) и поиск закончен
                                result = current;
                            }
                            else
                            {
                                // угол между векторами (pos -> wp0) и (pos -> wp1) меньше 90 градусов,
                                // значит wp0 и wp1 лежат в одной полуплоскости
                                // При этом dist1 больше минимального расстояния minDist
                                // следовательно wp1 дальше wp0, обе точки находятся впереди персонажа, но точки начали от него "удаляться"
                                // поиск завершен на предыдущем шаге
                            }
                            break;
                        } 
#endif

                        current++;
                        // сохраняем информацию о wp1, чтобы не повторять вычисления
                        //wp0 = wp1;
                        x0 = x1;
                        y0 = y1;
                        z0 = z1;
                        dist0 = dist1;
                        
                    } while (current < count);
                }
            }

            return result;
        }
    }
}
