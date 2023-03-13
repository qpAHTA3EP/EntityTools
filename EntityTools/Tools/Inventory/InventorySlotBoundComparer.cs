using MyNW.Classes;
using System.Collections.Generic;
using MyNW.Patchables.Enums;

namespace EntityTools.Tools.Inventory
{
    /// <summary>
    /// Компаратор двух слотов инвентаря с учетом привязки предметов в следующем порядке:<br/>
    /// [Привязка к персонажу] &lt; [Привязка к аккаунту] &lt; [Без привязки]
    /// </summary>
    public class BoundingComparerOfInventorySlot : IComparer<InventorySlot>
    {
        public static readonly BoundingComparerOfInventorySlot Default = new BoundingComparerOfInventorySlot();
        /// <summary>
        /// Сравнение двух слотов инвентаря с учетом привязки в следующем порядке:<br/>
        /// [Привязка к персонажу] &lt; [Привязка к аккаунту] &lt; [Без привязки]
        /// </summary>
        public int Compare(InventorySlot x, InventorySlot y)
        {
            bool xFilled = x?.Filled ?? false;
            bool yFilled = y?.Filled ?? false;
            if (xFilled)
            {
                if (yFilled)
                {
                    var xFlag = (ItemFlags)x.Item.Flags;
                    var yFlag = (ItemFlags)y.Item.Flags;

                    if ((xFlag & ItemFlags.Bound) > 0)
                    {
                        // x привязан к персонажу
                        if ((yFlag & ItemFlags.Bound) > 0)
                            // х и у привязаны к персонажу
                            return 0;
                        
                        // у не привязан или привязаны к аккаунту
                        return -1;
                    }
                    if ((xFlag & ItemFlags.BoundToAccount) > 0)
                    {
                        // x привязан к аккаунту
                        if ((yFlag & ItemFlags.Bound) > 0)
                            // у привязаны к персонажу
                            return 1;
                        if ((yFlag & ItemFlags.BoundToAccount) > 0)
                            // х и у привязаны к аккаунту
                            return 0;
                        // у не привязан
                        return -1;
                    }

                    if ((yFlag & (ItemFlags.Bound | ItemFlags.BoundToAccount)) > 0)
                        // х и у НЕ привязаны
                        return 0;
                    return 1;
                }
                return 1;
            }
            if (yFilled)

                return 1;
            return 0;
        }
    }
}
