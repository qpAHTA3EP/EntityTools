using System;
using System.Windows.Forms;
using Astral.Quester.Classes;
using MyNW.Internals;

namespace EntityTools.Quester.Conditions
{
    public class PartyCount : Condition
    {
        public int MemberCount { get; set; }

        public Relation Sign { get; set; }

        public PartyCount()
        {
            MemberCount = 0;
            Sign = Relation.Superior;
        }

        public override void Reset()
        {
        }

        public override string ToString()
        {
            return $"[Deprecated] Members is {Sign} {MemberCount}";
        }

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
    }
}
