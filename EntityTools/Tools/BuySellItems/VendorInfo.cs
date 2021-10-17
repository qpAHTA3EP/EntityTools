using System;
using System.ComponentModel;
using System.Drawing.Design;
using Astral.Logic.NW;
using Astral.Quester.UIEditors;
using EntityTools.Enums;
using EntityTools.Tools.Navigation;
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
                        _position = Vector3.Empty;
                    }
                    else if (_vendorType != VendorType.Normal)
                    {
                        _displayName = string.Empty;
                        _costumeName = string.Empty;
                        _mapName = string.Empty;
                        _regionName = string.Empty;
                        _position = Vector3.Empty;
                    }
                    label = string.Empty;
                }
            }
        }
        private VendorType _vendorType = VendorType.None;

        //Отображаемое имя
        public string DisplayName
        {
            get => _displayName; 
            set
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
        private Vector3 _position = Vector3.Empty;

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
        [Editor(typeof(CurrentMapEdit), typeof(UITypeEditor))]
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
        [Editor(typeof(CurrentRegionEdit), typeof(UITypeEditor))]
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
#if false
                    case VendorType.None:
                        isMatch = false;
                        break; 
#endif
#if VendorType_Auto
                    case VendorType.Auto:
                        isMatch = false;
                        break; 
#endif
                    case VendorType.Normal:
                        isMatch = !string.IsNullOrEmpty(_costumeName)
                                    && entity.CostumeRef.CostumeName == _costumeName
                                    && entity.Location.Distance3D(_position) < 1.0;
                        break;
                    case VendorType.ArtifactVendor:
                        isMatch = entity.InternalName == "Vip_Professions_Vendor"
                                  || entity.InternalName.StartsWith("Artifact_Auroraswholerealmscatalogue_Shopkeeper", StringComparison.Ordinal);
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

        public bool IsMatch(WorldInteractionNode node)
        {
            var player = EntityManager.LocalPlayer;
            return _vendorType == VendorType.Node
                   && string.Equals(_regionName, player.RegionInternalName, StringComparison.Ordinal)
                   && string.Equals(_mapName, player.MapState.MapName, StringComparison.Ordinal)
                   && NavigationHelper.SquareDistance3D(_position, node.Location) <= 1.0;
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
                    case VendorType.Node:
                        valid = !string.IsNullOrEmpty(_mapName)
                                && _position.IsValid;
                        break;
                    case VendorType.Normal:
                        valid = !(string.IsNullOrEmpty(_costumeName)
                                    || string.IsNullOrEmpty(_mapName)
                                  //|| string.IsNullOrEmpty(_regionName) <- может быть пустым
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
                var player = EntityManager.LocalPlayer;
                switch (_vendorType)
                {
                    case VendorType.None:
                        if (_mapName == "All" || string.IsNullOrEmpty(_mapName))
                        {
                            switch (_costumeName)
                            {
                                case nameof(VendorType.ArtifactVendor):
                                    _vendorType = VendorType.ArtifactVendor;
                                    available = SpecialVendor.IsAvailable() || SpecialVendor.VendorEntity.IsValid;
                                    break;
                                case nameof(VendorType.VIPProfessionVendor):
                                    _vendorType = VendorType.VIPProfessionVendor;
                                    available = VIP.CanSummonProfessionVendor;
                                    break;
                                case nameof(VendorType.VIPSealTrader):
                                case "VIPSummonSealTrader":
                                    _vendorType = VendorType.VIPSealTrader;
                                    available = VIP.CanSummonSealTrader;
                                    break;
                            }
                        }
                        else if (!string.IsNullOrEmpty(_costumeName)
                                 && _position.IsValid)
                        {
                            _vendorType = VendorType.Normal;
                            available = true;
                        }
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
                    case VendorType.Node:
                        available = _position.IsValid
                                    && _mapName == player.MapState.MapName
                                    && _regionName == player.RegionInternalName;
                        break;
                    case VendorType.Normal:
                        available = !string.IsNullOrEmpty(_costumeName)
                                    && _mapName == player.MapState.MapName
                                    && _regionName == player.RegionInternalName;
                        break;
                }

                return available;
            }
        }

        public double Distance
        {
            get
            {
                if (_vendorType == VendorType.Normal
                    || _vendorType == VendorType.Node)
                {
                    if (_position != null && _position.IsValid && IsAvailable)
                        return _position.Distance3DFromPlayer;
                    return double.MaxValue;
                }
                return 0;
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
                            switch (_costumeName)
                            {
                                case nameof(VendorType.ArtifactVendor):
                                    _vendorType = VendorType.ArtifactVendor;
                                    break;
                                case nameof(VendorType.VIPProfessionVendor):
                                    _vendorType = VendorType.VIPProfessionVendor;
                                    break;
                                case nameof(VendorType.VIPSealTrader):
                                case "VIPSummonSealTrader":
                                    _vendorType = VendorType.VIPSealTrader;
                                    break;
                                default:
                                    return label = "Not set";
                            }

                            label = ToString();
                        }
                        else if (!string.IsNullOrEmpty(_costumeName)
                                 && _position.IsValid)
                        {
                            _vendorType = VendorType.Normal;
                            label = ToString();
                        }
                        else label = "Not set";
                        break;
                    case VendorType.Normal:
                        if (!string.IsNullOrEmpty(_mapName))
                        {
                            label = string.Concat(_displayName, " (", _mapName, (_regionName.Length > 0) ? ("/" + _regionName) : string.Empty, ")");
                        }
                        else label = "Not set";
                        break;
                    case VendorType.Node:
                        if (!string.IsNullOrEmpty(_mapName))
                        {
                            label = string.Concat("Node <", _position.X.ToString("N1"), ", ", _position.Y.ToString("N1"), ", ", _position.X.ToString("N1"), "> (", _mapName, string.IsNullOrEmpty(_regionName) ? string.Empty : "/" + _regionName, ")");
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

