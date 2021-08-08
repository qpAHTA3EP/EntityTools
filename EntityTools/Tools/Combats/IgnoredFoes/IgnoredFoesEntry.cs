using System.Collections;
using System.Collections.Generic;
using Astral.Classes;

namespace EntityTools.Tools.Combats.IgnoredFoes
{
    /// <summary>
    /// Экземпляр списка врагов, ассоциированного с профилем <see cref="Profile"/>
    /// </summary>
    public class IgnoredFoesEntry : IEnumerable<string>
    {
        /// <summary>
        /// Полный набор игнорируемых врагов, включающий как временных, так и постоянных врагов из Profile.BlackList
        /// </summary>
        private HashSet<string> _foes = new HashSet<string>();
        long version;
        /// <summary>
        /// Список игнорируемых врагов из Profile.BlackList
        /// </summary>
        private readonly List<string> _profileBlackList = new List<string>();
        /// <summary>
        /// Кэш списка врагов, передаваемых в боевую подсистему
        /// </summary>
        private List<string> _foesListCache = new List<string>();
        long version_list = -1;

        /// <summary>
        /// Профиль, с которым ассоциирован список врагов
        /// </summary>
        public string Profile
        {
            get => _profile;
            protected set => _profile = value;
        }
        string _profile;

        /// <summary>
        /// Таймер до удаления временно игнорируемых врагов из списка
        /// </summary>
        public Timeout Timeout
        {
            get => _timeout;
            protected set => _timeout = value;
        }
        Timeout _timeout;

        /// <summary>
        /// Список врагов, включающий идентификаторы из BlackList профиля, и идентификторы временных врагов
        /// Если <see cref="Timeout"/> истек, то возвращается только BlackList из профиля
        /// </summary>
        public List<string> Foes
        {
            get
            {
                if(_timeout.IsTimedOut)
                {
                    _foesListCache.Clear();
                    _foesListCache.AddRange(_profileBlackList);
                    version_list = version;
                }
                else if (version_list < version)
                {
                    _foesListCache.Clear();
                    _foesListCache.AddRange(_foes);
                    version_list = version;
                }
                return _foesListCache;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="profileName">Текстовый идентификатор профиля, с которым ассоциирован BlackList</param>
        /// <param name="foes">Список постоянно игнорируемых врагов из Profile.BlackList</param>
        /// <param name="tempFoes">Список временно игнорируемых врагов</param>
        /// <param name="time">Период времени (мс) в течении которого игнорируются <paramref name="tempFoes"/></param>
        public IgnoredFoesEntry(string profileName, IEnumerable<string> foes, IEnumerable<string> tempFoes = null, int time = -1)
        {
            _profile = profileName;
            _timeout = new Timeout(time > 0 ? time : int.MaxValue);
            _profileBlackList.AddRange(foes);
            _foes.UnionWith(_profileBlackList);
            if (tempFoes != null)
                _foes.UnionWith(tempFoes);
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
            _foes.RemoveWhere(foe => !_profileBlackList.Contains(foe));
            _timeout.ChangeTime(0);
            version++;
        }

        public void Remove(string foe)
        {
            if(!_profileBlackList.Contains(foe) 
               && _foes.Remove(foe))
                version++;
        }
        public void RemoveRange(IEnumerable<string> foes)
        {
            int oldCount = _foes.Count;
            foreach (var foe in foes)
            {
                if (!_profileBlackList.Contains(foe))
                    _foes.Remove(foe);
            }
            
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
