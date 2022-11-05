using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace EntityTools.Settings
{
    [Serializable]
    public class QuesterPatchSettings : PluginSettingsBase
    {
        /// <summary>
        /// Управление патчем замены штатного Quester-редактора
        /// </summary>
        [Bindable(true)]
        [Description("Управление патчем замены штатного Quester-редактора")]
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

        /// <summary>
        /// Управление патчем замены штатного  AuraDetector
        /// </summary>
        [Bindable(true)]
        [Description("Управление патчем замены штатного AuraDetector")]
        public bool AuraDetector
        {
            get => auraDetector;
            set
            {
                if (auraDetector != value)
                {
                    auraDetector = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool auraDetector = true;

        /// <summary>
        /// Управление патчем замены штатного  AuraDetector
        /// </summary>
        [Bindable(true)]
        [Description("Управление патчем замены редактора quester-команды AddUCCAction")]
        public bool ReplaceEditorForAddUccActions
        {
            get => replaceEditorForAddUccActions;
            set
            {
                if (replaceEditorForAddUccActions != value)
                {
                    replaceEditorForAddUccActions = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool replaceEditorForAddUccActions = true;

        [XmlIgnore]
        [Browsable(false)]
        public int Count
        {
            get
            {

                int num = 0;
                if (replaceQuesterEditor) num++;
                if (replaceEditorForAddUccActions) num++;
                if (auraDetector) num++;
                return num;
            }
        }
        
        public override string ToString() => string.Concat('(', Count, " of 3)");
    }
}
