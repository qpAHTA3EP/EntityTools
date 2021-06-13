using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using AcTp0Tools.Annotations;
using Astral.Logic.UCC.Classes;

namespace AcTp0Tools.Classes.UCC
{
    /// <summary>
    /// Базовый класс для классов, инкапсулирующих настройки выбора цели в UCC
    /// </summary>
    [Serializable]
    public abstract class UccTargetSelector : INotifyPropertyChanged
    {
#if false
        /// <summary>
        /// Обработчик, реализующий проверку цели и т.п.
        /// </summary>
        public abstract IUccTargetProcessor Processor { get; set; } 
#else
        /// <summary>
        /// Обработчик, реализующий проверку цели на соответствие текущему объекту
        /// и выдающий целеуказание
        /// </summary>
        /// <returns></returns>
        public virtual UccTargetProcessor GetDefaultProcessor(UCCAction uccAction) => null;
#endif

        public abstract UccTargetSelector Clone();

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        } 
        #endregion
    }
}
