using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AcTp0Tools.Reflection.ObjectMonitoring
{
    public class PrimitivePropertyMonitor : IPropertyMonitor
    {
        readonly object _owner;
        readonly PropertyInfo _propertyInfo;

        public PrimitivePropertyMonitor(object own, PropertyInfo propertyInfo, int timeStamp = 0)
        {
            _propertyInfo = propertyInfo ?? throw new ArgumentNullException(nameof(propertyInfo));
            if (!_propertyInfo.PropertyType.IsPrimitive)
                throw new ArgumentException("Complex Property type are not alowed", nameof(propertyInfo));
            _owner = own;

            _timeStamp = Environment.TickCount;
            Reread(_timeStamp);
        }

        public string Name => _propertyInfo.Name;

        public Type Type => _propertyInfo.PropertyType;

        /// <summary>
        /// Значение свойства, ассоциированное с метокой времени <see cref="TimeStamp"/>
        /// </summary>
        public object Value
        {
            get
            {
                if(_currentRecord is null)
                {
                    _currentRecord = _history.FindLast(t => t.Item1 <= _timeStamp);
                }
                return _currentRecord?.Item2;
            }
        }
        Tuple<int , object> _currentRecord;
        readonly List<Tuple<int, object>> _history = new List<Tuple<int, object>>();

        public string DisplayedValue => Value.ToString();

        /// <summary>
        /// Метка времени, с которой ассоциировано значение свойства <see cref="Value"/>
        /// </summary>
        public int TimeStamp
        {
            get => _timeStamp;
            set
            {
                _currentRecord = null;
                _timeStamp = value;
            }
        }
        int _timeStamp;

        public ICollection<IPropertyMonitor> Members => ObjectMonitor.EmptyMonitorCollection;

        /// <summary>
        /// Чтение значения свойства и сохранение нового значения во внутреннюю базу с меткой времени <paramref name="timeStamp"/>
        /// </summary>
        public void Reread(int timeStamp)
        {
            var _last = _history.Last();
            var _current = _propertyInfo.GetValue(_owner);
            if (_last == null || !_last.Equals(_current))
            {
                var curRecord = new Tuple<int, object>(timeStamp, _current);
                _history.Add(curRecord);
                _currentRecord = curRecord;
            }
            else _currentRecord = _last;

            _timeStamp = timeStamp;
        }
    }
}
