﻿using Astral.Quester.Classes;
using EntityTools.Editors;
using EntityTools.Tools.CustomRegions;
using MyNW.Internals;
using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Text;

namespace EntityTools.Quester.Conditions
{
    [Serializable]
    public class IsInCustomRegionSet : Condition
    {
        [Description("A set of CustomRegion, the combination of which defines an area on the map")]
        [Editor(typeof(CustomRegionCollectionEditor), typeof(UITypeEditor))]
        public CustomRegionCollection CustomRegions
        {
            get => _customRegions;
            set
            {
                _customRegions = value;
                _label = string.Empty;
            }
        }
        private CustomRegionCollection _customRegions = new CustomRegionCollection();

        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                _label = string.Empty;
            }
        }
        string _description;

        [Description("Check the Player is ouside the area defined by the CustomRegion set")]
        public bool Outside
        {
            get => _outside;
            set
            {
                _outside = value;
                _label = string.Empty;
            }
        }
        private bool _outside;

        public override bool IsValid
        {
            get
            {
                bool within = _customRegions.Within(EntityManager.LocalPlayer.Location);
                return _outside 
                     ? !within 
                     : within;
            }
        }

        public override void Reset()
        {
            _label = string.Empty;
            _customRegions.ResetCache();
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(_label))
            {
                if(_customRegions.Count > 0)
                {
                    int unionCount = _customRegions.Union.Count,
                        intersectionCount = _customRegions.Intersection.Count,
                        exclusionCount = _customRegions.Exclusion.Count;
                    _label = string.Concat("Check Player ",
                                           _outside ? "outside " : "within ",
                                           !string.IsNullOrEmpty(_description) ? $"'{_description}' =>" : string.Empty,
                                           unionCount > 0 ? $" \x22c3 ({unionCount})" : string.Empty,
                                           intersectionCount > 0 ? $" \x22c2 ({intersectionCount})" : string.Empty,
                                           exclusionCount > 0 ? $" \x00ac ({exclusionCount})" : string.Empty);
                }
                else _label = GetType().Name;
            }
            return _label;
        }
        string _label;

        public override string TestInfos
        {
            get
            {
                var count = _customRegions.Count;
                if (count > 0)
                {
                    var sb = new StringBuilder();
                    sb.Append("Total ").Append(count).AppendLine(" " + nameof(CustomRegions));
                    using (var enumerator = _customRegions.GetEnumerator())
                    {
                        if (enumerator.MoveNext())
                        {
                            var crEntry = enumerator.Current;
                            sb.AppendLine("{").Append("\t[").Append(crEntry.Inclusion).Append("] ").Append(crEntry.Name);
                            var cr = crEntry.CustomRegion;
                            if (cr != null)
                                sb.Append(cr.IsIn ? " : within" : " : outside");
                            else sb.Append(" : not exist");
                            while (enumerator.MoveNext())
                            {
                                crEntry = enumerator.Current;
                                sb.AppendLine(",");
                                sb.Append("\t[").Append(crEntry.Inclusion).Append("] ").Append(crEntry.Name);
                                cr = crEntry.CustomRegion;
                                if (cr != null)
                                    sb.Append(cr.IsIn ? " : within" : " : outside");
                                else sb.Append(" : not exist");
                            }
                            sb.AppendLine("\n}");
                        }
                    }
                    return sb.ToString();
                }
                return nameof(CustomRegions)+" is empty";
            }
        }
    }
}
