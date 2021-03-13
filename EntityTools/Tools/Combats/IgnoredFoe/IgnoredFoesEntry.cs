using AStar;
using Astral.Classes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityTools.Tools.Combats.IgnoredFoes
{
    /// <summary>
    /// Экземпляр списка врагов, ассоциированного с профилем <see cref="Profile"/>
    /// </summary>
    public class IgnoredFoes : IEnumerable<string>
    {
        /// <summary>
        /// Полный набор игнорируемых врагов, включающий как временных, так и постоянных врагов из Profile.BlackList
        /// </summary>
        private HashSet<string> _foes = new HashSet<string>();
        long version = 0;
        /// <summary>
        /// Список игнорируемых врагов из Profile.BlackList
        /// </summary>
        private readonly List<string> _profileBlackList = new List<string>();
        /// <summary>
        /// Кэш списка врагов, передаваемых в боевую подсистему
        /// </summary>
        private List<string> _foeList = new List<string>();
        long version_list = -1;

        /// <summary>
        /// Профиль, с которым ассоциирован список врагов
        /// </summary>
        public string Profile
        {
            get => _profile;
            protected set => _profile = value;
        }
        string _profile = string.Empty;

        /// <summary>
        /// Таймер до удаления временно игнорируемых врагов из списка
        /// </summary>
        public Timeout Timeout
        {
            get => _timeout;
            protected set => _timeout = value;
        }
        Timeout _timeout = new Timeout(0);

        /// <summary>
        /// Список врагов, включающий идентификаторы из BlackList профиля, и идентификторы временных врагов
        /// Если <see cref="Timeout"/> истек, то возвращается только BlackList из профиля
        /// </summary>
        public List<string> Foes
        {
            get
            {
                if(version_list < version)
                {
                    _foeList.Clear();
                    if (_timeout.IsTimedOut)
                        _foeList.AddRange(_profileBlackList);
                    else _foeList.AddRange(_foes);
                    version_list = version;
                }
                return _foeList;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="profileName">Текстовый идентификатор профиля, с которым ассоциирован BlackList</param>
        /// <param name="foes">Список постоянно игнорируемых врагов из Profile.BlackList</param>
        /// <param name="tempFoes">Список временно игнорируемых врагов</param>
        /// <param name="time">Период времени (мс) в течении которого игнорируются <paramref name="tempFoes"/></param>
        public IgnoredFoes(string profileName, IEnumerable<string> foes, IEnumerable<string> tempFoes = null, int time = -1)
        {
            _profile = profileName;
            _timeout = new Timeout(time > 0 ? time : int.MaxValue);
            _profileBlackList.AddRange(foes);
            if (tempFoes != null)
            {
                _foes.UnionWith(foes);
                _foes.UnionWith(tempFoes);
            }
            version++;
        }

        /// <summary>
        /// Добавление временно игнорируемого врага <paramref name="foe"/>
        /// </summary>
        public void Add(string foe)
        {
            if (_foes.Add(foe))
                version++;
        }

        /// <summary>
        /// Добавление временно игнорируемых врагов <paramref name="foes"/>
        /// </summary>
        public void AddRange(IEnumerable<string> foes)
        {
            _foes.UnionWith(foes);
            version++;
        }

        /// <summary>
        /// Очистка списка временно игнорируемых врагов
        /// </summary>
        public void Clear()
        {
            _foes.Clear();
            _timeout.ChangeTime(0);
            version++;
        }

        public void Remove(string foe)
        {
            if(_foes.Remove(foe))
                version++;
        }
        public void RemoveRange(IEnumerable<string> foes)
        {
            int oldCount = _foes.Count;
            _foes.ExceptWith(foes);
            if(oldCount < _foes.Count)
                version++;
        }

        #region IEnumerable
        public IEnumerator<string> GetEnumerator()
        {
            return _foes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _foes.GetEnumerator();
        } 
        #endregion
    }
}
