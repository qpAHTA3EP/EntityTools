using Astral.Logic.UCC.Classes;
using EntityTools.Editors.TestEditors;
using Infrastructure.Annotations;
using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using EntityTools.Editors;

namespace EntityTools.UCC.Actions
{
    [Serializable]
    public class ExecuteConsoleCommand : UCCAction, INotifyPropertyChanged
    {
        public ExecuteConsoleCommand()
        {
            Timer = 500;
            CoolDown = 2000;
        }

        [Category("Required")]
        [Description("The text of the command to be executed within the game console.")]
        public string Command
        {
            get => _command;
            set
            {
                OnPropertyChanged();
                _command = value;
            }
        }
        private string _command;

        [XmlIgnore]
        [Category("Required")]
        [OnlineHelp("https://neverwinter.fandom.com/wiki/Console_command")]
        [Description("Open online documentation about NeverWinter ConsoleCommand.")]
        [Editor(typeof(OnlineHelpEditor), typeof(UITypeEditor))]
        public string OnlineHelp => "https://neverwinter.fandom.com/wiki/Console_command";

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = default)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        public override UCCAction Clone()
        {
            return BaseClone(new ExecuteConsoleCommand { _command = _command });
        }

        public override bool Run()
        {
            MyNW.Internals.GameCommands.Execute(_command);
            return true;
        }

        public override bool NeedToRun => !string.IsNullOrWhiteSpace(_command);

        public override string ToString()
        {
            return nameof (ExecuteConsoleCommand) + ": " + _command;
        }

        #region Hide Inherited Properties
        [XmlIgnore]
        [Browsable(false)]
        public new string ActionName { get; set; }
        [XmlIgnore]
        [Browsable(false)]
        public new Astral.Logic.UCC.Ressources.Enums.Unit Target  => Astral.Logic.UCC.Ressources.Enums.Unit.Player;
        [XmlIgnore]
        [Browsable(false)]
        public new uint Range { get; set; }
        #endregion
    }
}
