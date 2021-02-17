
using System;
using MyNW.Internals;
using Astral.Classes;
using MyNW.Classes;
using EntityTools.Enums;

#if PATCH_ASTRAL && HARMONY
using HarmonyLib; 
#endif

namespace EntityTools.Patches.Logic.Classes.FSM.Navigation
{
#if false && HARMONY
    /// <summary>
    /// Патч метода Astral.Logic.Classes.FSM.Navigation.SetNextWaypoint()
    /// </summary>
    [HarmonyPatch(typeof(Astral.Logic.Classes.FSM.Navigation), "SetNextWaypoint")] 
    internal class HarmonyPatch_Astral_Logic_Classes_FSM_Navigation_SetNextWaypoint
    {

#if false
// Astral.Logic.Classes.FSM.Navigation
public void SetNextWaypoint()
{
	if (!this.IsLastWaypoints)
	{
		this.CurrentWaypointIndex++;
		return;
	}
	if (this.Loop)
	{
		this.CurrentWaypointIndex = 0;
	}
}
#endif
        [HarmonyPrefix] 
        internal static bool SetNextWaypoint()
        {

            int count = waypoints.Count;
            int current = 0;

            if (count > 0
               && pos != null && pos.IsValid)
            {
                result = current;
                if (count > 1)
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
            return false;
        }

        #region Данные
        #endregion
    }
#endif
}
