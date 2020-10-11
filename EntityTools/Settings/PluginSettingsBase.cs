using System;
using System.ComponentModel;

namespace EntityTools.Settings
{
    [Serializable]
    public class PluginSettingsBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(/*[CallerMemberName]*/ string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
