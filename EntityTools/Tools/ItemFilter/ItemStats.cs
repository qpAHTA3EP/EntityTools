using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityTools.Tools.BuySellItems
{
    /// <summary>
    /// Статистические данные по предметам
    /// </summary>
    public class ItemStats
    {
        public uint MaxItemLevel;
        public uint TotalItemsCount;

        public ItemStats() { }
        public ItemStats(uint maxLvl, uint cnt)
        {
            MaxItemLevel = maxLvl;
            TotalItemsCount = cnt;
        }

        public ItemStats Clone()
        {
            return new ItemStats() { MaxItemLevel = this.MaxItemLevel, TotalItemsCount = this.TotalItemsCount };
        }
    }
}
