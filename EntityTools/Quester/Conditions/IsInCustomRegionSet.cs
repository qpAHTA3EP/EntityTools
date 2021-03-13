using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using Astral.Quester.Classes;
using EntityTools.Editors;
using EntityTools.Patches;
using EntityTools.Tools.CustomRegions;
using MyNW.Internals;

namespace EntityTools.Quester.Conditions
{
    [Serializable]
    public class IsInCustomRegionSet : Condition
    {
        public IsInCustomRegionSet()
        {

        }
#if PATCH_ASTRAL
        static IsInCustomRegionSet()
        {
            // Пременение патча на этапе десериализации (до инициализации плагина)
            ETPatcher.Apply();
        }
#endif

        [Description("A set of CustomRegion, the combination of which defines an area on the map")]
        [Editor(typeof(CustomRegionCollectionEditor), typeof(UITypeEditor))]
        //[TypeConverter(typeof(ExpandableObjectConverter))]
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
            get
            {
                return _description;
            }
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
                if (_outside)
                    return !within;
                else return within;
            }
        }

        public override void Reset() => _label = string.Empty;

        public override string ToString()
        {
            if (string.IsNullOrEmpty(_label))
            {
                if(_customRegions.Count > 0)
                {
                    var sb = new StringBuilder("Check Player ");
                    if (_outside)
                        sb.Append("outside ");
                    else sb.Append("within ");

                    if (!string.IsNullOrEmpty(_description))
                        sb.Append('\'').Append(_description).Append("' =>");
                    if (_customRegions.Union.Count > 0)
                        sb.Append(" \x22c3 (").Append(_customRegions.Union.Count).Append(')');
                    if (_customRegions.Intersection.Count > 0)
                        sb.Append(" \x22c2 (").Append(_customRegions.Intersection.Count).Append(')');
                    if (_customRegions.Exclusion.Count > 0)
                        //sb.Append(" \x00ac(").Append(_customRegions.Intersection.Count).Append(')');
                        sb.Append(" \\ (").Append(_customRegions.Exclusion.Count).Append(')');

                    _label = sb.ToString();
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
                    sb.Append("Totaly ").Append(count).AppendLine(" " + nameof(CustomRegions));
                    //sb.Append("Player '").Append(EntityManager.LocalPlayer.InternalName).AppendLine("' located in the following of them:");
#if false
                    using (var enumerator = Astral.Quester.API.CurrentProfile.CustomRegions.Where(cr => cr.IsIn).GetEnumerator()) 
#elif false
                    using (var enumerator = Astral.Quester.API.CurrentProfile.CustomRegions.Where(cr => cr.IsIn && _customRegions.Contains(cr.Name)).GetEnumerator())
#else
                    using (var enumerator = _customRegions.GetEnumerator())
#endif
                    {
                        if (enumerator.MoveNext())
                        {
                            sb.AppendLine("{");
                            var crEntry = enumerator.Current;
                            sb.Append("\t[").Append(crEntry.Inclusion).Append("] ").Append(crEntry.Name);
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
                else return nameof(CustomRegions)+" is empty";
            }
        }
    }
}
