using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace EntityTools.States
{

    public class SlideMonitorSettings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private bool active;
        public bool Active
        {
            get => active;
            set
            {
                if (active != value)
                {
                    active = value;
                    NotifyPropertyChanged("Active");
                }
            }
        }

        // Приоритет
        private int priority = 100;
        public int Priority
        {
            get
            {
                return priority;
            }
            set
            {
                if (value != priority)
                {
                    value = priority;
                    NotifyPropertyChanged("Priority");
                }
            }
        }

        /// <summary>
        /// Интервал между проверками с аурой скольжения
        /// </summary>
        private int checkTimeSlide = 100;
        public int CheckTimeSlide
        {
            get => checkTimeSlide;
            set
            {
                if (value != checkTimeSlide)
                {
                    value = checkTimeSlide;
                    NotifyPropertyChanged("CheckTimeSlide");
                }
            }
        }

        /// <summary>
        /// Интервал между проверками без ауры скольжения
        /// </summary>
        private int checkTimeNotSlide = 3000;
        public int CheckTimeNotSlide
        {
            get => checkTimeNotSlide;
            set
            {
                if (value != checkTimeNotSlide)
                {
                    value = checkTimeNotSlide;
                    NotifyPropertyChanged("CheckTimeNotSlide");
                }
            }
        }

        /// <summary>
        /// Расстояние между последовательными точками пути при скольжении
        /// </summary>
        private float filter = 60;
        public float Filter
        {
            get => filter;
            set
            {
                if (value != filter)
                {
                    value = filter;
                    NotifyPropertyChanged("Filter");
                }
            }
        }

        /// <summary>
        /// Списко скользящий аур
        /// </summary>
        public BindingList<string> SlidingAuras { get; } = new BindingList<string>();

        private void NotifyPropertyChanged(/*[CallerMemberName]*/ string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(propertyName)));
        }
    }
}
