using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using AcTp0Tools.Reflection;
using MyNW.Classes;
using MyNW.Internals;
using MyNW.Patchables.Enums;

namespace EntityTools.Tools.BuySellItems
{
    /// <summary>
    /// Список сумок
    /// </summary>
    [Serializable]
    public class BagsList : INotifyPropertyChanged, IEnumerable<InvBagIDs>, IXmlSerializable

    {
        #region Предустановленные наборы сумок
        /// <summary>
        /// Только сумки персонажа (без экипированных предметов)
        /// </summary>
        public static BagsList GetPlayerBags()
        {
            //new InvBagIDs[] { InvBagIDs.Inventory, InvBagIDs.PlayerBag1, InvBagIDs.PlayerBag2, InvBagIDs.PlayerBag3, InvBagIDs.PlayerBag4, InvBagIDs.PlayerBag5, InvBagIDs.PlayerBag6, InvBagIDs.PlayerBag7, InvBagIDs.PlayerBag8, InvBagIDs.PlayerBag9, InvBagIDs.Overflow };
            BagsList bags = new BagsList
            {
                _bags =
                {
                    [(int) InvBagIDs.Inventory] = true,
                    [(int) InvBagIDs.PlayerBag1] = true,
                    [(int) InvBagIDs.PlayerBag2] = true,
                    [(int) InvBagIDs.PlayerBag3] = true,
                    [(int) InvBagIDs.PlayerBag4] = true,
                    [(int) InvBagIDs.PlayerBag5] = true,
                    [(int) InvBagIDs.PlayerBag6] = true,
                    [(int) InvBagIDs.PlayerBag7] = true,
                    [(int) InvBagIDs.PlayerBag8] = true,
                    [(int) InvBagIDs.PlayerBag9] = true,
                    [(int) InvBagIDs.Overflow] = true
                }
            };
            return bags;
        }
        /// <summary>
        /// Только сумки персонажа (без экипированных предметов)
        /// </summary>
        public static BagsList GetPlayerBagsAndPotions()
        {
            //new InvBagIDs[] { InvBagIDs.Inventory, InvBagIDs.PlayerBag1, InvBagIDs.PlayerBag2, InvBagIDs.PlayerBag3, InvBagIDs.PlayerBag4, InvBagIDs.PlayerBag5, InvBagIDs.PlayerBag6, InvBagIDs.PlayerBag7, InvBagIDs.PlayerBag8, InvBagIDs.PlayerBag9, InvBagIDs.Overflow };
            BagsList bags = new BagsList
            {
                _bags =
                {
                    [(int) InvBagIDs.Inventory] = true,
                    [(int) InvBagIDs.PlayerBag1] = true,
                    [(int) InvBagIDs.PlayerBag2] = true,
                    [(int) InvBagIDs.PlayerBag3] = true,
                    [(int) InvBagIDs.PlayerBag4] = true,
                    [(int) InvBagIDs.PlayerBag5] = true,
                    [(int) InvBagIDs.PlayerBag6] = true,
                    [(int) InvBagIDs.PlayerBag7] = true,
                    [(int) InvBagIDs.PlayerBag8] = true,
                    [(int) InvBagIDs.PlayerBag9] = true,
                    [(int) InvBagIDs.Overflow] = true,
                    [(int) InvBagIDs.Potions] = true
                }
            };
            return bags;
        }
        /// <summary>
        /// Сумки и экипированные предметы (без предметов профессий и ценностей)
        /// </summary>
        public static BagsList GetFullPlayerInventory()
        {
            BagsList bags = new BagsList
            {
                _bags =
                {
                    [(int) InvBagIDs.AdventuringHead] = true,
                    [(int) InvBagIDs.AdventuringNeck] = true,
                    [(int) InvBagIDs.AdventuringArmor] = true,
                    [(int) InvBagIDs.AdventuringArms] = true,
                    [(int) InvBagIDs.AdventuringWaist] = true,
                    [(int) InvBagIDs.AdventuringFeet] = true,
                    [(int) InvBagIDs.AdventuringHands] = true,
                    [(int) InvBagIDs.AdventuringShirt] = true,
                    [(int) InvBagIDs.AdventuringTrousers] = true,
                    [(int) InvBagIDs.AdventuringRanged] = true,
                    [(int) InvBagIDs.AdventuringRings] = true,
                    [(int) InvBagIDs.AdventuringSurges] = true,
                    [(int) InvBagIDs.ArtifactPrimary] = true,
                    [(int) InvBagIDs.ArtifactSecondary] = true,
                    [(int) InvBagIDs.Inventory] = true,
                    [(int) InvBagIDs.PlayerBag1] = true,
                    [(int) InvBagIDs.PlayerBag2] = true,
                    [(int) InvBagIDs.PlayerBag3] = true,
                    [(int) InvBagIDs.PlayerBag4] = true,
                    [(int) InvBagIDs.PlayerBag5] = true,
                    [(int) InvBagIDs.PlayerBag6] = true,
                    [(int) InvBagIDs.PlayerBag7] = true,
                    [(int) InvBagIDs.PlayerBag8] = true,
                    [(int) InvBagIDs.PlayerBag9] = true,
                    [(int) InvBagIDs.Overflow] = true,
                    [(int) InvBagIDs.Potions] = true
                }
            };
            return bags;
        }
        /// <summary>
        /// Персональный банк
        /// </summary>
        public static BagsList GetBanks()
        {
            BagsList bags = new BagsList
            {
                _bags =
                {
                    [(int) InvBagIDs.Bank] = true,
                    [(int) InvBagIDs.Bank1] = true,
                    [(int) InvBagIDs.Bank2] = true,
                    [(int) InvBagIDs.Bank3] = true,
                    [(int) InvBagIDs.Bank4] = true,
                    [(int) InvBagIDs.Bank5] = true,
                    [(int) InvBagIDs.Bank6] = true,
                    [(int) InvBagIDs.Bank7] = true,
                    [(int) InvBagIDs.Bank8] = true,
                    [(int) InvBagIDs.Bank9] = true
                }
            };
            return bags;
        }

        /// <summary>
        /// Экипированные предметы и артифакты
        /// </summary>
        public static BagsList GetEquipments()
        {
            BagsList bags = new BagsList
            {
                _bags =
                {
                    [(int) InvBagIDs.AdventuringHead] = true,
                    [(int) InvBagIDs.AdventuringNeck] = true,
                    [(int) InvBagIDs.AdventuringArmor] = true,
                    [(int) InvBagIDs.AdventuringArms] = true,
                    [(int) InvBagIDs.AdventuringWaist] = true,
                    [(int) InvBagIDs.AdventuringFeet] = true,
                    [(int) InvBagIDs.AdventuringHands] = true,
                    [(int) InvBagIDs.AdventuringShirt] = true,
                    [(int) InvBagIDs.AdventuringTrousers] = true,
                    [(int) InvBagIDs.AdventuringRanged] = true,
                    [(int) InvBagIDs.AdventuringRings] = true,
                    [(int) InvBagIDs.AdventuringSurges] = true,
                    [(int) InvBagIDs.ArtifactPrimary] = true,
                    [(int) InvBagIDs.ArtifactSecondary] = true
                }
            };
            return bags;
        }
        public static bool IsEquipmentsBag(InvBagIDs id)
        {
            return id == InvBagIDs.AdventuringHead
                   || id == InvBagIDs.AdventuringNeck
                   || id == InvBagIDs.AdventuringArmor
                   || id == InvBagIDs.AdventuringArms
                   || id == InvBagIDs.AdventuringWaist
                   || id == InvBagIDs.AdventuringFeet
                   || id == InvBagIDs.AdventuringHands
                   || id == InvBagIDs.AdventuringShirt
                   || id == InvBagIDs.AdventuringTrousers
                   || id == InvBagIDs.AdventuringRanged
                   || id == InvBagIDs.AdventuringRings
                   || id == InvBagIDs.AdventuringSurges
                   || id == InvBagIDs.ArtifactPrimary
                   || id == InvBagIDs.ArtifactSecondary;
        } 
        #endregion

        public BagsList()
        {
            _bags = new BitArray(Enum.GetValues(typeof(InvBagIDs)).Length, false);// { InvBagIDs.Inventory, InvBagIDs.PlayerBag1, InvBagIDs.PlayerBag2, InvBagIDs.PlayerBag3, InvBagIDs.PlayerBag4, InvBagIDs.PlayerBag5, InvBagIDs.PlayerBag6, InvBagIDs.PlayerBag7, InvBagIDs.PlayerBag8, };
        }
        public BagsList(InvBagIDs[] bagsList)
        {
            if (bagsList != null)
                foreach (var id in bagsList)
                {
                    _bags[(int)id] = true;
                    _count++;
                }
        }

        /// <summary>
        /// Список флагов-сумок
        /// </summary>
        [XmlIgnore]
        BitArray _bags = new BitArray(Enum.GetValues(typeof(InvBagIDs)).Length, false);

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChange([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Флаги сумок
        /// </summary>
        /// <param name="bagId"></param>
        /// <returns></returns>
        public bool this[InvBagIDs bagId]
        {
            get
            {
                return _bags[(int)bagId];
            }
            set
            {
                var bagInclusion = _bags[(int) bagId];
                if (bagInclusion != value)
                {
                    if (bagInclusion)
                        _count--;
                    else _count++;

                    _bags[(int)bagId] = value;
                    NotifyPropertyChange(nameof(Count));
                }
            }
        }


        /// <summary>
        /// Перечисление идентификаторов выбранных сумок
        /// </summary>
        /// <returns></returns>
        public IEnumerable<InvBagIDs> GetSelectedBagsId()
        {
            for (int i = 0; i < _bags.Count; i++)
            {
                if(_bags[i])
                    yield return (InvBagIDs)i;
            }
        }

        /// <summary>
        /// Перечисление выбранных сумок
        /// </summary>
        /// <returns></returns>
        public IEnumerable<InventoryBag> GetSelectedBags()
        {
            for (int i = 0; i < _bags.Count; i++)
            {
                if (_bags[i])
                    yield return EntityManager.LocalPlayer.GetInventoryBagById((InvBagIDs)i);
            }
        }

        /// <summary>
        /// Перечисление слотов в выбранных сумках
        /// </summary>
        /// <returns></returns>
        public IEnumerable<InventorySlot> GetItems()
        {
            for (int i = 0; i < _bags.Count; i++)
            {
                if (_bags[i])
                    foreach(var item in EntityManager.LocalPlayer.GetInventoryBagById((InvBagIDs)i).GetItems)
                        yield return item;
            }
        }

        /// <summary>
        /// Количество выбранных сумок
        /// </summary>
        public uint Count => _count;
        uint _count;

        /// <summary>
        /// Сброс всех сумок
        /// </summary>
        public void Clear()
        {
            _bags.SetAll(false);
            _count = 0;
            NotifyPropertyChange(nameof(Count));
        }

        //создание копии списка
        public BagsList Clone()
        {
            BagsList newBags = new BagsList();
            newBags._bags = CopyHelper.CreateDeepCopy(_bags);
            return newBags;
        }

        /// <summary>
        /// добавление сумки в список
        /// </summary>
        /// <param name="obj"></param>
        public void Add(object obj)
        {
            if(obj is InvBagIDs id)
            {
                if (!_bags[(int)id])
                    _count++;
                _bags[(int)id] = true;
                NotifyPropertyChange(nameof(Count));
            }
        }

        /// <summary>
        /// Добавление сумки <param name="id"/> в список
        /// </summary>
        /// <param name="id">Идентификатор добавляемой сумки</param>
        public void Add(InvBagIDs id)
        {
            if (!_bags[(int)id])
                _count++;
            _bags[(int)id] = true;
            NotifyPropertyChange(nameof(Count));
        }

        public IEnumerator<InvBagIDs> GetEnumerator()
        {
            return new BagsListEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new BagsListEnumerator(this);
        } 

        public override string ToString()
        {
            return string.Concat(nameof(BagsList), " (", _count, ')');
        }

        /// <summary>
        /// Перечислитель
        /// </summary>
        public class BagsListEnumerator : IEnumerator<InvBagIDs>
        {
            BagsList _bags;
            int ind = -1;

            public BagsListEnumerator(BagsList bags)
            {
                _bags = bags;
            }
            public InvBagIDs Current
            {
                get
                {
                    if (ind >= 0 && ind < _bags._bags.Count)
                    {
                        return (InvBagIDs)ind;
                    }

                    throw new IndexOutOfRangeException();
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    if (ind >= 0 && ind < _bags._bags.Count)
                    {
                        return (InvBagIDs)ind;
                    }

                    throw new IndexOutOfRangeException();
                }
            }

            public void Dispose()
            {
                _bags = null;
            }

            public bool MoveNext()
            {
                //TODO: Проверить корректность состояния "до первого вызова MoveNext()", чтобы не получилось пропуска нулевой сумки
                while (ind < _bags._bags.Count)
                {
                    ind++;
                    if (ind < _bags._bags.Count && _bags._bags[ind])
                        return true;
                }
                return false;
            }

            public void Reset()
            {
                ind = -1;
            }
        }

        #region IXmlSerializable
        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            if (_bags == null)
                _bags = new BitArray(Enum.GetValues(typeof(InvBagIDs)).Length, false);
            else _bags.SetAll(false);

            if(reader.IsStartElement())
            {
                string name = reader.Name;

                {
                    if (reader.IsEmptyElement)
                    {
                        reader.ReadStartElement(name);
                    }
                    else
                    {
                        reader.ReadStartElement(name);
                        ReadInnerXml(reader, name);
                        reader.ReadEndElement();
                    }
                }
            }
        }

        /// <summary>
        ///  Рекурсивное считывание поддерева xml до обнаружения xmlEndElement
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="xmlEndElement"></param>
        private void ReadInnerXml(XmlReader reader, string xmlEndElement)
        {
            while (reader.ReadState == ReadState.Interactive)
            {
                string name = reader.Name;
                if (name == nameof(InvBagIDs))
                {
                    string bagName = reader.ReadElementContentAsString(name, string.Empty);
                    if (Enum.TryParse(bagName, out InvBagIDs bagId))
                    {
                        _bags[(int)bagId] = true;
                        _count++;
                    }
                    else ETLogger.WriteLine(LogType.Error, $"{MethodBase.GetCurrentMethod().Name}: Expect the {nameof(InvBagIDs)} but has the value '{bagName}'", true);
                }
                else if (name == xmlEndElement)
                {
                    if (!reader.IsStartElement())
                        return;
                    throw new XmlException($"{MethodBase.GetCurrentMethod().Name}: Unexpected XmlStartElement '{name}' while there are should be the XmlEndElement '{xmlEndElement}'");
                }
                else if (name == nameof(BagsList)
                         || name == "Items"
                         || name == "Bags")
                {
                    if (reader.IsStartElement())
                    {
                        if (reader.IsEmptyElement)
                            reader.ReadStartElement(name);
                        else
                        {
                            reader.ReadStartElement(name);
                            ReadInnerXml(reader, name);
                            reader.ReadEndElement();
                        }
                    }
                    else throw new XmlException($"{MethodBase.GetCurrentMethod().Name}: Unexpected XmlStartElement '{name}' while there are should be the XmlEndElement '{xmlEndElement}'");
                }
                else reader.Skip();
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            foreach(InvBagIDs id in GetSelectedBagsId())
            {
                writer.WriteStartElement(nameof(InvBagIDs), "");
                writer.WriteString(id.ToString());
                writer.WriteEndElement();
            }
        } 
        #endregion
    }
}
