using System.Linq;
using System.Text;
using ACTP0Tools.Classes.Quester;
using ACTP0Tools.Reflection;
using Astral.Quester.Classes.Conditions;
using EntityTools.Extensions;
using MyNW.Internals;
using QuesterCondition = Astral.Quester.Classes.Condition;

namespace EntityCore.Quester.Editor.TreeViewExtension
{
    /// <summary>
    /// Узел дерева, отображающий <see cref="QuesterCondition"/>
    /// </summary>
    internal sealed class ConditionIsInCustomRegionTreeNode : ConditionBaseTreeNode
    {
        public ConditionIsInCustomRegionTreeNode(IsInCustomRegion condition)
        {
            Tag = condition;
            this.condition = condition;
            UpdateView();
        }

        public override QuesterCondition Content => condition;
        private readonly IsInCustomRegion condition;
        
        /*
            foreach (CustomRegion customRegion in Core.Profile.CustomRegions)
            {
                if (customRegion.Name == this.CustomRegionName)
                {
                    Condition.Presence tested = this.Tested;
                    if (tested == Condition.Presence.Equal)
                    {
                        return customRegion.IsIn;
                    }
                    if (tested == Condition.Presence.NotEquel)
                    {
                        return !customRegion.IsIn;
                    }
                }
            }
            return false;
         */
        public override bool IsValid(QuesterProfileProxy profile)
        {
            var crName = condition.CustomRegionName;
            if (string.IsNullOrEmpty(crName))
                return false;
            var player = EntityManager.LocalPlayer;
            if (player.IsLoading)
                return false;
            var customRegion = profile.CustomRegions.FirstOrDefault(cr => cr.Name == crName);
            if (customRegion is null)
                return false;
            bool withing = CustomRegionExtensions.IsInCustomRegion(player.Location, customRegion);
            return condition.Tested == QuesterCondition.Presence.Equal
                ?  withing
                : !withing;
        }

        public override string TestInfo(QuesterProfileProxy profile)
        {
            var crName = condition.CustomRegionName;
            if (string.IsNullOrEmpty(crName))
                return "Property 'CustomRegionName' is empty";
            var player = EntityManager.LocalPlayer;
            if (player.IsLoading)
                return "Player is out of the World";
            var location = player.Location;
            var testInfo = new StringBuilder();
            bool withinTestedCR = false;
            bool foundTestedCR = false;
            int numOfDetectedCR = 0;

            foreach (var customRegion in profile.CustomRegions)
            {
                if (CustomRegionExtensions.IsInCustomRegion(location, customRegion))
                {
                    testInfo.Append('\t').Append(customRegion.Name);
                    if (customRegion.Name == crName)
                    {
                        testInfo.AppendLine(" [Tested]");
                        foundTestedCR = true;
                    }
                    else testInfo.AppendLine();
                    withinTestedCR = true;
                    numOfDetectedCR++;
                }
                else if (customRegion.Name == crName)
                    foundTestedCR = true;
            }


            testInfo.Insert(0, numOfDetectedCR > 0
                               ? $"Player is within {numOfDetectedCR} CustomRegion:\n"
                               : "Player is out of any CustomRegion");
            if (!foundTestedCR)
                testInfo.Append($"CustomRegion named '{crName}' not found");

            return testInfo.ToString();
        }

        public override bool AllowChildren => false;

        public override void UpdateView()
        {
            var txt = condition.ToString();
            var type = condition.GetType();
            if (string.IsNullOrEmpty(txt)
                || txt == type.FullName)
                txt = type.Name;
            Text = txt;
            if (Checked != condition.Locked)
                Checked = condition.Locked;
            ImageKey = "Condition";
            SelectedImageKey = "Condition";
        }

        public override QuesterCondition ReconstructInternal()
        {
            condition.Locked = Checked;
            return condition;
        }

        public override object Clone()
        {
            return new ConditionTreeNode(CopyHelper.CreateDeepCopy(condition));
        }
    }
}
