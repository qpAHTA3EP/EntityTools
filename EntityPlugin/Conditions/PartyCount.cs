using System;
using System.Windows.Forms;
using Astral.Quester.Classes;
using MyNW.Internals;

namespace EntityPlugin.Conditions
{
    public class PartyCount : Condition
    {
        public PartyCount()
        {
            MessageBox.Show($"This Conditions is deprecated. Use '{nameof(TeamMembersCount)}' or '{nameof(TeamMembersCountInCustomRegions)}' instead");
            ;
            this.MemberCount = 0;
            this.Sign = Relation.Superior;
        }

        public override void Reset()
        {
        }

        public override string ToString()
        {
            return $"[{Properties.Resources.CategoryDeprecated}] Members is {Sign} {MemberCount}";
        }

        public int MemberCount { get; set; }

        public override bool IsValid
        {
            get
            {
                uint membersCount = EntityManager.LocalPlayer.PlayerTeam.Team.MembersCount;
                bool result;
                switch (this.Sign)
                {
                    case Relation.Equal:
                        result = ((ulong)membersCount == (ulong)((long)this.MemberCount));
                        break;
                    case Relation.NotEqual:
                        result = ((ulong)membersCount != (ulong)((long)this.MemberCount));
                        break;
                    case Relation.Inferior:
                        result = ((ulong)membersCount < (ulong)((long)this.MemberCount));
                        break;
                    case Relation.Superior:
                        result = ((ulong)membersCount > (ulong)((long)this.MemberCount));
                        break;
                    default:
                        result = false;
                        break;
                }
                return result;
            }
        }

        public Relation Sign { get; set; }
    }
}
