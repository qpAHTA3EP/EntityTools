using System;
using Astral.Logic.NW;
using EntityTools.Enums;
using MyNW.Classes;
using MyNW.Internals;

namespace EntityTools.Tools.BuySellItems
{
    /// <summary>
    /// Класс, идентифицирующий продавца
    /// </summary>
    [Serializable]
    public class VendorInfo
    {
        public VendorInfo() { }
        public VendorInfo(VendorType vndType)
        {
            _vendorType = vndType;
            if (_vendorType == VendorType.VIPProfessionVendor || _vendorType == VendorType.VIPSealTrader || _vendorType == VendorType.ArtifactVendor)
            {
                string str = _vendorType.ToString();
                _displayName = str;
                _costumeName = str;
            }
        }

        #region Данные
        /// <summary>
        /// Тип продавца/магазина
        /// </summary>
        public VendorType VendorType
        {
            get => _vendorType;
            set
            {
                if (_vendorType != value)
                {
                    _vendorType = value;
                    if (_vendorType == VendorType.VIPProfessionVendor || _vendorType == VendorType.VIPSealTrader || _vendorType == VendorType.ArtifactVendor)
                    {
                        string str = _vendorType.ToString();
                        _displayName = str;
                        _costumeName = str;
                        _mapName = string.Empty;
                        _regionName = string.Empty;
                        _position = new Vector3();
                    }
                    else if (_vendorType != VendorType.Normal)
                    {
                        _displayName = string.Empty;
                        _costumeName = string.Empty;
                        _mapName = string.Empty;
                        _regionName = string.Empty;
                        _position = new Vector3();
                    }
                    label = string.Empty;
                }
            }
        }
        private VendorType _vendorType = VendorType.None;

        //Отображаемое имя
        public string DisplayName
        {
            get => _displayName; set
            {
                if (_displayName != value)
                {
                    _displayName = value;
                    label = string.Empty;
                }
            }
        }
        private string _displayName = string.Empty;

        /// <summary>
        /// Место нахождения торговца на карте
        /// </summary>
        public Vector3 Position
        {
            get => _position; set
            {
                if (_position != value)
                {
                    _position = value;
                    label = string.Empty;
                }
            }
        }
        private Vector3 _position = new Vector3();

        /// <summary>
        /// Внутриигровой "костюм" торговца
        /// </summary>
        public string CostumeName
        {
            get => _costumeName; set
            {
                if (_costumeName != value)
                {
                    _costumeName = value;
                    if(_vendorType == VendorType.None)
                    {
                        if (_costumeName == VendorType.ArtifactVendor.ToString())
                            _vendorType = VendorType.ArtifactVendor;
                        else if (_costumeName == VendorType.VIPProfessionVendor.ToString())
                            _vendorType = VendorType.VIPProfessionVendor;
                        else if (_costumeName == VendorType.VIPSealTrader.ToString()
                                || _costumeName == "VIPSummonSealTrader")
                            _vendorType = VendorType.VIPSealTrader;
                        else if (!string.IsNullOrEmpty(_mapName) && _mapName != "All" && !string.IsNullOrEmpty(_costumeName) && _position.IsValid)
                            _vendorType = VendorType.Normal;
                    }
                    label = string.Empty;
                }
            }
        }
        private string _costumeName = string.Empty;

        /// <summary>
        /// Карта, на которой находится торговец
        /// </summary>
        public string MapName
        {
            get => _mapName; set
            {
                if (_mapName != value)
                {
                    _mapName = value;
                    if(_mapName == "All" || string.IsNullOrEmpty(_mapName))
                    {
                        if (_costumeName == VendorType.ArtifactVendor.ToString())
                            _vendorType = VendorType.ArtifactVendor;
                        else if (_costumeName == VendorType.VIPProfessionVendor.ToString())
                            _vendorType = VendorType.VIPProfessionVendor;
                        else if (_costumeName == VendorType.VIPSealTrader.ToString()
                                || _costumeName == "VIPSummonSealTrader")
                            _vendorType = VendorType.VIPSealTrader;
                    }
                    label = string.Empty;
                }
            }
        }
        private string _mapName = string.Empty;

        /// <summary>
        /// Внутриигровой регион карты, в котором находится торговец
        /// </summary>
        public string RegionName
        {
            get => _regionName; set
            {
                if (_regionName != value)
                {
                    _regionName = value;
                    label = string.Empty;
                }
            }
        }
        private string _regionName = string.Empty; 
        #endregion

        /// <summary>
        /// Проверка entity на соответствие параметрам продавца
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool IsMatch(Entity entity)
        {
            bool isMatch = false;
            if (entity != null && entity.IsValid)
            {
                switch (_vendorType)
                {
                    case VendorType.None:
                        isMatch = false;
                        break;
                    case VendorType.Auto:
                        isMatch = false;
                        break;
                    case VendorType.Normal:
                        isMatch = !string.IsNullOrEmpty(_costumeName)
                                    && entity.CostumeRef.CostumeName == _costumeName
                                    && entity.Location.Distance3D(_position) < 1.0;
                        break;
                    case VendorType.ArtifactVendor:
                        isMatch = entity.InternalName == "Vip_Professions_Vendor"
                                    || entity.InternalName.StartsWith("Artifact_Auroraswholerealmscatalogue_Shopkeeper");
                        break;
                    case VendorType.VIPProfessionVendor:
                        isMatch = entity.InternalName == "Vip_Professions_Vendor"
                                  && entity.OwnerRefId == EntityManager.LocalPlayer.RefId;
                        break;
                    case VendorType.VIPSealTrader:
                        isMatch = entity.InternalName == "Vip_Seal_Trader"
                                  && entity.OwnerRefId == EntityManager.LocalPlayer.RefId;
                        break;
                }
            }
            return isMatch;
        }

        /// <summary>
        /// Указывает на корректность сведений, идентифицирующих продавца
        /// </summary>
        public bool IsValid
        {
            get
            {
                bool valid = true;
                switch (_vendorType)
                {
                    case VendorType.None:
                        valid = false;
                        if (_mapName == "All" || string.IsNullOrEmpty(_mapName))
                        {
                            if (_costumeName == VendorType.ArtifactVendor.ToString())
                            {
                                _vendorType = VendorType.ArtifactVendor;
                                valid = true;
                            }
                            else if (_costumeName == VendorType.VIPProfessionVendor.ToString())
                            {
                                _vendorType = VendorType.VIPProfessionVendor;
                                valid = true;
                            }
                            else if (_costumeName == VendorType.VIPSealTrader.ToString()
                                    || _costumeName == "VIPSummonSealTrader")
                            {
                                _vendorType = VendorType.VIPSealTrader;
                                valid = true;
                            }
                        }
                        else if (!string.IsNullOrEmpty(_costumeName)
                                 && _position.IsValid)
                        {
                            _vendorType = VendorType.Normal;
                            valid = true;
                        }
                        break;
                    case VendorType.Auto:
                        valid = false;
                        break;
#if false
                    case VendorType.ArtifactVendor:
                        valid = SpecialVendor.IsAvailable();
                        break;
                    case VendorType.VIPProfessionVendor:
                        valid = VIP.CanSummonProfessionVendor;
                        break;
                    case VendorType.VIPSealTrader:
                        valid = VIP.CanSummonSealTrader;
                        break; 
#endif
                    case VendorType.Normal:
                        valid = !(string.IsNullOrEmpty(_costumeName)
                                    || string.IsNullOrEmpty(_mapName)
                                  //|| string.IsNullOrEmpty(_regionName) <- может быть пустым
                                    || string.IsNullOrEmpty(_costumeName)
                                    || !_position.IsValid);
                        break;
                }

                return valid;
            }
        }

        /// <summary>
        /// Указывает на доступность продавца (находится на той же карте, либо можно призвать)
        /// </summary>
        public bool IsAvailable
        {
            get
            {
                bool available = false;
                switch (_vendorType)
                {
                    case VendorType.None:
                        available = false;
                        if (_mapName == "All" || string.IsNullOrEmpty(_mapName))
                        {
                            if (_costumeName == VendorType.ArtifactVendor.ToString())
                            {
                                _vendorType = VendorType.ArtifactVendor;
                                available = true;
                            }
                            else if (_costumeName == VendorType.VIPProfessionVendor.ToString())
                            {
                                _vendorType = VendorType.VIPProfessionVendor;
                                available = true;
                            }
                            else if (_costumeName == VendorType.VIPSealTrader.ToString()
                                    || _costumeName == "VIPSummonSealTrader")
                            {
                                _vendorType = VendorType.VIPSealTrader;
                                available = true;
                            }
                        }
                        else if (!string.IsNullOrEmpty(_costumeName)
                                 && _position.IsValid)
                        {
                            _vendorType = VendorType.Normal;
                            available = true;
                        }
                        break;
                    case VendorType.Auto:
                        available = false;
                        break;
                    case VendorType.ArtifactVendor:
                        // Если торговец уже призван, то IsAvailable() возвращает false
                        available = SpecialVendor.IsAvailable() || SpecialVendor.VendorEntity.IsValid;
                        break;
                    case VendorType.VIPProfessionVendor:
                        available = VIP.CanSummonProfessionVendor;
                        break;
                    case VendorType.VIPSealTrader:
                        available = VIP.CanSummonSealTrader;
                        break;
                    case VendorType.Normal:
                        available = !string.IsNullOrEmpty(_costumeName)
                                    && _mapName == EntityManager.LocalPlayer.MapState.MapName
                                    && _regionName == EntityManager.LocalPlayer.RegionInternalName;
                        break;
                }

                return available;
            }
        }

        string label = string.Empty;
        public override string ToString()
        {
            if (string.IsNullOrEmpty(label))
            {
                switch (_vendorType)
                {
                    case VendorType.None:
                        if (_mapName == "All" || string.IsNullOrEmpty(_mapName))
                        {
                            if (_costumeName == VendorType.ArtifactVendor.ToString())
                            {
                                _vendorType = VendorType.ArtifactVendor;
                                label = ToString();
                            }
                            else if (_costumeName == VendorType.VIPProfessionVendor.ToString())
                            {
                                _vendorType = VendorType.VIPProfessionVendor;
                                label = ToString();
                            }
                            else if (_costumeName == VendorType.VIPSealTrader.ToString()
                                    || _costumeName == "VIPSummonSealTrader")
                            {
                                _vendorType = VendorType.VIPSealTrader;
                                label = ToString();
                            }
                            
                            else label = "Not set";
                        }
                        else if (!string.IsNullOrEmpty(_costumeName)
                                 && _position.IsValid)
                        {
                            _vendorType = VendorType.Normal;
                            label = ToString();
                        }
                        else label = "Not set";
                        break;
                    case VendorType.Auto:
                        label = "Auto";
                        break;
                    case VendorType.Normal:
                        if (_mapName.Length > 0)
                        {
                            label = string.Concat(_displayName, " (", _mapName, (_regionName.Length > 0) ? ("/" + _regionName) : string.Empty, ")");
                        }
                        else label = "Not set";
                        break;
                    default:
                        label = string.Concat(_vendorType, " (All)");
                        break;
                }
            }
            return label;
        }
    }
}

