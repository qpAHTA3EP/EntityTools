using System;
using System.ComponentModel;

namespace EntityTools.Settings
{
    /// <summary>
    /// Настройки кэширования Entity
    /// </summary>
    [Serializable]
    public class EntityCacheSettings : PluginSettingsBase
    {
        /// <summary>
        /// Интервал времени между обновлениями общего кэша Entity
        /// </summary>
        [Bindable(true)]
        public int GlobalCacheTime
        {
            get => _cacheTime;
            set
            {
                if (_cacheTime != value)
                {
                    _cacheTime = Math.Max(value, 500);
                    base.NotifyPropertyChanged(nameof(GlobalCacheTime));
                }
            }
        }
        private static int _cacheTime = 500;

        /// <summary>
        /// Интервал времени между обновлениями кэша во время боя
        /// </summary>
        [Bindable(true)]
        public int CombatCacheTime
        {
            get => _combatCacheTime;
            set
            {
                if(_combatCacheTime != value)
                {

                    _combatCacheTime = Math.Max(value, 100);
                    base.NotifyPropertyChanged(nameof(CombatCacheTime));
                }
            }
        }
        private int _combatCacheTime = 200;

        /// <summary>
        /// Интервал времени обновления Entity, кэшированного в командах и условиях локально
        /// </summary>
        [Bindable(true)]
        public int LocalCacheTime
        {
            get => _localCacheTime;
            set
            {
                if (_localCacheTime != value)
                {
                    _localCacheTime = Math.Max(value, 100);
                    base.NotifyPropertyChanged(nameof(LocalCacheTime));
                }
            }
        }
        private int _localCacheTime = 500;
    }
}
