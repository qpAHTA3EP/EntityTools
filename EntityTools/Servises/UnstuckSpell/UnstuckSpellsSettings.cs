﻿using EntityTools.Services;
using System;
using System.ComponentModel;

namespace EntityTools.Settings
{
    /// <summary>
    /// Настройки службы UnstuckSpell
    /// Выключающей залипшие умения
    /// </summary>
    [Serializable]
    public class UnstuckSpellsSettings : PluginSettingsBase
    {
        /// <summary>
        /// Активация или деактивация умения
        /// </summary>
        [Bindable(true)]
        public bool Active
        {
            get => active;
            set
            {
                if (active != value)
                {
                    active = value;
                    base.NotifyPropertyChanged(nameof(Active));
                }
            }
        }
        //[NonSerialized]
        private bool active = true;

        /// <summary>
        /// Интервал проверки залипания умений (милисекунд)
        /// </summary>
        [Bindable(true)]
        public int CheckInterval
        {
            get => checkInterval;
            set
            {
                if (checkInterval != value)
                {
                    checkInterval = Math.Max(value, 500);
                    base.NotifyPropertyChanged(nameof(Active));
                }
            }
        }
        //[NonSerialized]
        private int checkInterval = 500;
    }
}