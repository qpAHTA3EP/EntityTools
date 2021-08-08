using EntityTools.Tools.CustomRegions;
using MyNW.Classes;

namespace EntityTools.Core.Interfaces
{
    /// <summary>
    /// Интерфейс, опередляющий расширенный набор свойств и атрибутов, идентифицирующих <seealso cref="Entity"/>
    /// </summary>
    public interface IEntityDescriptor : IEntityIdentifier
    {
        /// <summary>
        /// Флаг проверки совпадения <seealso cref="Entity.RegionInternalName"/> у игрока и <see cref="Entity"/>
        /// </summary>
        bool RegionCheck { get; set; }

        /// <summary>
        /// Флаг проверки положительного количества здоровья (НР) и без флага <see cref="Entity.IsDead"/>
        /// </summary>
        bool HealthCheck { get; set; }

        /// <summary>
        /// Предельное расстояние, дальше которого <see cref="Entity"/> игнорируются
        /// </summary>
        float ReactionRange { get; set; }

        /// <summary>
        /// Предельная разница высот относительно персонажа, за пределами которого <see cref="Entity"/> игнорируются
        /// </summary>
        float ReactionZRange { get; set; }

#if false
        /// <summary>
        /// Набор <seealso cref="CustomRegion"/>, задаюций область, за пределами которой <seealso cref="Entity"/> игнорируются
        /// </summary>
        CustomRegionCollection CustomRegionNames { get; set; } 
#endif
    }
}
