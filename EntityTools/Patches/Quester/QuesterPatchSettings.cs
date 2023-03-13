using System;
using System.ComponentModel;

namespace EntityTools.Settings
{
    /// <summary>
    /// Настройки кэширования Entity
    /// </summary>
    [Serializable]
    public class QuesterPatchSettings : PluginSettingsBase
    {
        /// <summary>
        /// Интервал времени между обновлениями общего кэша Entity
        /// </summary>
        [Bindable(true)]
        [Description("Активация комплексного патча Quester'а")]
        public bool Active
        {
            get => _active;
            set
            {
                if (_active != value)
                {
                    _active = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool _active = true;



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
                    NotifyPropertyChanged();
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
                    NotifyPropertyChanged();
                }
            }
        }
        private bool saveQuesterProfile = true;

        /// <summary>
        /// Активация или деактивация Патча Quester-редактора.
        /// </summary>
        [Bindable(true)]
        [Description("Управление патчем подмены Quester-редактора")]
        public bool ReplaceQuesterEditor
        {
            get => replaceQuesterEditor;
            set
            {
                if (replaceQuesterEditor != value)
                {
                    replaceQuesterEditor = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool replaceQuesterEditor = true;

        public override string ToString()
        {
            if (!_active)
                return "Disabled";

            int num = 0;
            if (getNearestIndexInPositionList) num++;
            if (saveQuesterProfile) num++;
            if (replaceQuesterEditor) num++;
            return string.Concat('(', num, " of 3)");
        }
    }
}
