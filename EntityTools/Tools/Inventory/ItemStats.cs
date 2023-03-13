namespace EntityTools.Tools.Inventory
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
            return new ItemStats { MaxItemLevel = MaxItemLevel, TotalItemsCount = TotalItemsCount };
        }
    }
}
