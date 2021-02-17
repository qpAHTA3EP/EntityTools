#define BagsList_Enumerable
//#define BagsList_Items
#define BagsList_IXmlSerializable

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using EntityTools.Reflection;
using MyNW.Classes;
using MyNW.Internals;
using MyNW.Patchables.Enums;

namespace EntityTools.Tools.BuySellItems
{
    /// <summary>
    /// Список сумок
    /// </summary>
    [Serializable]
    public class BagsList : INotifyPropertyChanged
#if BagsList_Enumerable
                        , IEnumerable<InvBagIDs>
#if BagsList_IXmlSerializable
                        , IXmlSerializable
#endif
#elif BagsList_IXmlSerializable
        : IXmlSerializable
#endif
    {
        #region Предустановленные наборы сумок
        /// <summary>
        /// Только сумки персонажа (без экипированных предметов)
        /// </summary>
        public static BagsList GetPlayerBags()
        {
            //new InvBagIDs[] { InvBagIDs.Inventory, InvBagIDs.PlayerBag1, InvBagIDs.PlayerBag2, InvBagIDs.PlayerBag3, InvBagIDs.PlayerBag4, InvBagIDs.PlayerBag5, InvBagIDs.PlayerBag6, InvBagIDs.PlayerBag7, InvBagIDs.PlayerBag8, InvBagIDs.PlayerBag9, InvBagIDs.Overflow };
            BagsList bags = new BagsList();
            bags._bags[(int)InvBagIDs.Inventory] = true;
            bags._bags[(int)InvBagIDs.PlayerBag1] = true;
            bags._bags[(int)InvBagIDs.PlayerBag2] = true;
            bags._bags[(int)InvBagIDs.PlayerBag3] = true;
            bags._bags[(int)InvBagIDs.PlayerBag4] = true;
            bags._bags[(int)InvBagIDs.PlayerBag5] = true;
            bags._bags[(int)InvBagIDs.PlayerBag6] = true;
            bags._bags[(int)InvBagIDs.PlayerBag7] = true;
            bags._bags[(int)InvBagIDs.PlayerBag8] = true;
            bags._bags[(int)InvBagIDs.PlayerBag9] = true;
            bags._bags[(int)InvBagIDs.Overflow] = true;
            return bags;
        }
        /// <summary>
        /// Только сумки персонажа (без экипированных предметов)
        /// </summary>
        public static BagsList GetPlayerBagsAndPotions()
        {
            //new InvBagIDs[] { InvBagIDs.Inventory, InvBagIDs.PlayerBag1, InvBagIDs.PlayerBag2, InvBagIDs.PlayerBag3, InvBagIDs.PlayerBag4, InvBagIDs.PlayerBag5, InvBagIDs.PlayerBag6, InvBagIDs.PlayerBag7, InvBagIDs.PlayerBag8, InvBagIDs.PlayerBag9, InvBagIDs.Overflow };
            BagsList bags = new BagsList();
            bags._bags[(int)InvBagIDs.Inventory] = true;
            bags._bags[(int)InvBagIDs.PlayerBag1] = true;
            bags._bags[(int)InvBagIDs.PlayerBag2] = true;
            bags._bags[(int)InvBagIDs.PlayerBag3] = true;
            bags._bags[(int)InvBagIDs.PlayerBag4] = true;
            bags._bags[(int)InvBagIDs.PlayerBag5] = true;
            bags._bags[(int)InvBagIDs.PlayerBag6] = true;
            bags._bags[(int)InvBagIDs.PlayerBag7] = true;
            bags._bags[(int)InvBagIDs.PlayerBag8] = true;
            bags._bags[(int)InvBagIDs.PlayerBag9] = true;
            bags._bags[(int)InvBagIDs.Overflow] = true;
            bags._bags[(int)InvBagIDs.Potions] = true;
            return bags;
        }
        /// <summary>
        /// Сумки и экипированные предметы (без предметов профессий и ценностей)
        /// </summary>
        public static BagsList GetFullPlayerInventory()
        {
            //new InvBagIDs[] { InvBagIDs.Inventory, InvBagIDs.PlayerBag1, InvBagIDs.PlayerBag2, InvBagIDs.PlayerBag3, InvBagIDs.PlayerBag4, InvBagIDs.PlayerBag5, InvBagIDs.PlayerBag6, InvBagIDs.PlayerBag7, InvBagIDs.PlayerBag8, InvBagIDs.PlayerBag9, InvBagIDs.Overflow,
            //                  InvBagIDs.AdventuringHead, InvBagIDs.AdventuringNeck, InvBagIDs.AdventuringArmor, InvBagIDs.AdventuringArms, InvBagIDs.AdventuringWaist, InvBagIDs.AdventuringFeet, InvBagIDs.AdventuringHands, InvBagIDs.AdventuringShirt, InvBagIDs.AdventuringTrousers, InvBagIDs.AdventuringRanged, InvBagIDs.AdventuringRings, InvBagIDs.AdventuringSurges, InvBagIDs.ArtifactPrimary, InvBagIDs.ArtifactSecondary };
            BagsList bags = new BagsList();
            bags._bags[(int)InvBagIDs.AdventuringHead] = true;
            bags._bags[(int)InvBagIDs.AdventuringNeck] = true;
            bags._bags[(int)InvBagIDs.AdventuringArmor] = true;
            bags._bags[(int)InvBagIDs.AdventuringArms] = true;
            bags._bags[(int)InvBagIDs.AdventuringWaist] = true;
            bags._bags[(int)InvBagIDs.AdventuringFeet] = true;
            bags._bags[(int)InvBagIDs.AdventuringHands] = true;
            bags._bags[(int)InvBagIDs.AdventuringShirt] = true;
            bags._bags[(int)InvBagIDs.AdventuringTrousers] = true;
            bags._bags[(int)InvBagIDs.AdventuringRanged] = true;
            bags._bags[(int)InvBagIDs.AdventuringRings] = true;
            bags._bags[(int)InvBagIDs.AdventuringSurges] = true;
            bags._bags[(int)InvBagIDs.ArtifactPrimary] = true;
            bags._bags[(int)InvBagIDs.ArtifactSecondary] = true;
            bags._bags[(int)InvBagIDs.Inventory] = true;
            bags._bags[(int)InvBagIDs.PlayerBag1] = true;
            bags._bags[(int)InvBagIDs.PlayerBag2] = true;
            bags._bags[(int)InvBagIDs.PlayerBag3] = true;
            bags._bags[(int)InvBagIDs.PlayerBag4] = true;
            bags._bags[(int)InvBagIDs.PlayerBag5] = true;
            bags._bags[(int)InvBagIDs.PlayerBag6] = true;
            bags._bags[(int)InvBagIDs.PlayerBag7] = true;
            bags._bags[(int)InvBagIDs.PlayerBag8] = true;
            bags._bags[(int)InvBagIDs.PlayerBag9] = true;
            bags._bags[(int)InvBagIDs.Overflow] = true;
            bags._bags[(int)InvBagIDs.Potions] = true;
            return bags;
        }
        /// <summary>
        /// Персональный банк
        /// </summary>
        public static BagsList GetBanks()
        {
            //new InvBagIDs[] { InvBagIDs.Bank, InvBagIDs.Bank1, InvBagIDs.Bank2, InvBagIDs.Bank3, InvBagIDs.Bank4, InvBagIDs.Bank5, InvBagIDs.Bank6, InvBagIDs.Bank7, InvBagIDs.Bank8, InvBagIDs.Bank9 };
            BagsList bags = new BagsList();
            bags._bags[(int)InvBagIDs.Bank] = true;
            bags._bags[(int)InvBagIDs.Bank1] = true;
            bags._bags[(int)InvBagIDs.Bank2] = true;
            bags._bags[(int)InvBagIDs.Bank3] = true;
            bags._bags[(int)InvBagIDs.Bank4] = true;
            bags._bags[(int)InvBagIDs.Bank5] = true;
            bags._bags[(int)InvBagIDs.Bank6] = true;
            bags._bags[(int)InvBagIDs.Bank7] = true;
            bags._bags[(int)InvBagIDs.Bank8] = true;
            bags._bags[(int)InvBagIDs.Bank9] = true;
            return bags;
        }

        /// <summary>
        /// Экипированные предметы и артифакты
        /// </summary>
        public static BagsList GetEquipments()
        {
            //new InvBagIDs[] { InvBagIDs.AdventuringHead, InvBagIDs.AdventuringNeck, InvBagIDs.AdventuringArmor, InvBagIDs.AdventuringArms, InvBagIDs.AdventuringWaist, InvBagIDs.AdventuringFeet, InvBagIDs.AdventuringHands, InvBagIDs.AdventuringShirt, InvBagIDs.AdventuringTrousers, InvBagIDs.AdventuringRanged, InvBagIDs.AdventuringRings, InvBagIDs.AdventuringSurges, InvBagIDs.AdventuringWaist, InvBagIDs.ArtifactPrimary, InvBagIDs.ArtifactSecondary };
            BagsList bags = new BagsList();
            bags._bags[(int)InvBagIDs.AdventuringHead] = true;
            bags._bags[(int)InvBagIDs.AdventuringNeck] = true;
            bags._bags[(int)InvBagIDs.AdventuringArmor] = true;
            bags._bags[(int)InvBagIDs.AdventuringArms] = true;
            bags._bags[(int)InvBagIDs.AdventuringWaist] = true;
            bags._bags[(int)InvBagIDs.AdventuringFeet] = true;
            bags._bags[(int)InvBagIDs.AdventuringHands] = true;
            bags._bags[(int)InvBagIDs.AdventuringShirt] = true;
            bags._bags[(int)InvBagIDs.AdventuringTrousers] = true;
            bags._bags[(int)InvBagIDs.AdventuringRanged] = true;
            bags._bags[(int)InvBagIDs.AdventuringRings] = true;
            bags._bags[(int)InvBagIDs.AdventuringSurges] = true;
            bags._bags[(int)InvBagIDs.ArtifactPrimary] = true;
            bags._bags[(int)InvBagIDs.ArtifactSecondary] = true;
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
                if (_bags[(int)bagId] != value)
                {
                    if (_bags[(int)bagId])
                        _count--;
                    else _count++;

                    _bags[(int)bagId] = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(bagId.ToString()));
                }
            }
        }

#if BagsList_Items
        [Browsable(false)]
        public IEnumerable<InvBagIDs> Items
        {
            get
            {
                return GetSelectedBagsId();
            }
            set
            {
                _bags.SetAll(false);
                if(value != null)
                {
                    foreach (InvBagIDs id in value)
                        _bags[(int)id] = true;
                }
            }
        }
#endif

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
        public uint Count { get => _count; }
        uint _count;

        /// <summary>
        /// Сброс всех сумок
        /// </summary>
        public void Clear()
        {
            _bags.SetAll(false);
            _count = 0;
        }

        //создание копии списка
        public BagsList Clone()
        {
            BagsList newBags = new BagsList();
            newBags._bags = CopyHelper.CreateDeepCopy(_bags);
            return newBags;
        }

#if BagsList_Enumerable
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
                
            }
        }

        /// <summary>
        /// добавление сумки в список
        /// </summary>
        /// <param name="obj"></param>
        public void Add(InvBagIDs id)
        {
            if (!_bags[(int)id])
                _count++;
            _bags[(int)id] = true;
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
#endif
        #region IXmlSerializable
#if BagsList_IXmlSerializable
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
#if false
                if (name == nameof(BagsList)
            || name == "Bags") 
#endif
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
#if false
                while (reader.ReadState == ReadState.Interactive)
                {
                    string name = reader.Name;
                    if (name == nameof(InvBagIDs))
                    {
                        string bagName = reader.ReadElementContentAsString(name, string.Empty);
                        if (Enum.TryParse(bagName, out InvBagIDs bagId))
                            _bags[(int)bagId] = true;
                        else ETLogger.WriteLine(LogType.Error, $"{MethodBase.GetCurrentMethod().Name}: Expect the {nameof(InvBagIDs)} but has the value '{bagName}'", true);
                    }
                    else if (name == nameof(BagsList)
#if BagsList_Items
                             || name == nameof(Items)
#else
                             || name == "Items"
#endif
                             || name == "Bags")
                    {
                        if (reader.IsStartElement())
                        {
                            reader.ReadStartElement(name);
                            ReadInnerXml(reader, name);
                            reader.ReadEndElement();
                        }
                    }
                    else reader.Skip();
                } 
#endif
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
                        _bags[(int)bagId] = true;
                    else ETLogger.WriteLine(LogType.Error, $"{MethodBase.GetCurrentMethod().Name}: Expect the {nameof(InvBagIDs)} but has the value '{bagName}'", true);
                }
                else if (name == xmlEndElement)
                {
                    if (!reader.IsStartElement())
                        return;
                    throw new XmlException($"{MethodBase.GetCurrentMethod().Name}: Unexpected XmlStartElement '{name}' while there are should be the XmlEndElement '{xmlEndElement}'");
                }
                else if (name == nameof(BagsList)
#if BagsList_Items
                            || name == nameof(Items)
#else
                            || name == "Items"
#endif
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
#endif
        #endregion
    }
}
