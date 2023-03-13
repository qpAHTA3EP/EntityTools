using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Infrastructure.Annotations;

namespace EntityTools.Settings
{
    [Serializable]
    public class PluginSettingsBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
