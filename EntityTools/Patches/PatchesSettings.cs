using System;
using System.ComponentModel;

namespace EntityTools.Settings
{
    /// <summary>
    /// Настройки Патчей
    /// </summary>
    [Serializable]
    public class PatchesSettings : PluginSettingsBase
    {
        /// <summary>
        /// Активация или деактивация Патча методов навигации
        /// </summary>
        [Bindable(true)]
        [Description("Управление патчами, отвечающими за навивацию (Logic.Navmesh)")]
        public bool Navigation
        {
            get => navigation;
            set
            {
                if (navigation != value)
                {
                    navigation = value;
                    NotifyPropertyChanged(nameof(Navigation));
                }
            }
        }
        private bool navigation = true;

        
            /// <summary>
            /// Активация или деактивация Патча методов выбора ближайшей точки
            /// </summary>
            [Bindable(true)]
        [Description("Управление патчем, выбора ближайшей точки пути (Logic.General)")]
        public bool GetNearestIndexInPositionList
        {
            get => getNearestIndexInPositionList;
            set
            {
                if (getNearestIndexInPositionList != value)
                {
                    getNearestIndexInPositionList = value;
                    NotifyPropertyChanged(nameof(GetNearestIndexInPositionList));
                }
            }
        }
        private bool getNearestIndexInPositionList = true;

        /// <summary>
        /// Активация или деактивация Патча методов метода сохранения профилей Quester'a
        /// </summary>
        [Bindable(true)]
        [Description("Управление патчем метода сохранения Quester-профилей")]
        public bool SaveQuesterProfile
        {
            get => saveQuesterProfile;
            set
            {
                if (saveQuesterProfile != value)
                {
                    saveQuesterProfile = value;
                    NotifyPropertyChanged(nameof(SaveQuesterProfile));
                }
            }
        }
        private bool saveQuesterProfile = true;


        /// <summary>
        /// Активация или деактивация Патча ProfessionVendorEntity
        /// </summary>
        [Bindable(true)]
        [Description("Управление патчем призыва Vip-торговца ресурсами профессий")]
        public bool VipProfessionVendorEntity
        {
            get => professionVendorEntity;
            set
            {
                if (professionVendorEntity != value)
                {
                    professionVendorEntity = value;
                    NotifyPropertyChanged(nameof(VipProfessionVendorEntity));
                }
            }
        }
        private bool professionVendorEntity = true;


        /// <summary>
        /// Активация или деактивация Патча SealTraderEntity
        /// </summary>
        [Bindable(true)]
        [Description("Управление патчем призыва Vip-торговца печатями")]
        public bool VipSealTraderEntity
        {
            get => sealTraderEntity;
            set
            {
                if (sealTraderEntity != value)
                {
                    sealTraderEntity = value;
                    NotifyPropertyChanged(nameof(VipSealTraderEntity));
                }
            }
        }
        private bool sealTraderEntity = true;

        public override string ToString()
        {
            int num = 0;
            if (navigation) num++;
            if (saveQuesterProfile) num++;
            if (professionVendorEntity) num++;
            if (sealTraderEntity) num++;
            if (getNearestIndexInPositionList) num++;
            return string.Concat('(', num, " of 5)");
        }
    }
}
