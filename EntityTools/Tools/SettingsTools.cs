using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace EntityTools.Tools
{
    public class EntityPluginSettings : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        private string aurasExportFile;
        public string AurasExportFile
        {
            get => aurasExportFile;
            set
            {
                if (aurasExportFile != value)
                {
                    aurasExportFile = value;
                    NotifyPropertyChanged("AurasExportFile");
                }
            }
        }

        private void NotifyPropertyChanged(/*[CallerMemberName]*/ string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(propertyName)));
        }
    }

}
