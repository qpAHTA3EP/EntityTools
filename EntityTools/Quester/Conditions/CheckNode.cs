using Astral.Logic.NW;
using EntityTools.Editors;
using MyNW.Classes;
using MyNW.Internals;
using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Text;

namespace EntityTools.Conditions
{
    public enum NodeState2
    {
        Exist,
        NotExist,
        Targetable,
        NotTargetable,
        Interactable,
        NotInteractable
    }

    public class CheckNode2 : Astral.Quester.Classes.Condition
    {
        [NonSerialized]
        private Interact.DynaNode currentNode;

        public NodeState2 Tested { get; set; }

        [Editor(typeof(NodePositionEditor), typeof(UITypeEditor))]
        //[Editor(typeof(PositionNodeEditor), typeof(UITypeEditor))]
        [Description("Position of the Node that is checked.\n" +
            "Координаты проверяемой Ноды.")]
        public Vector3 Position { get; set; }

        [Description("The maximum distance at which the bot can detect the Node\r\n" +
            "If the distance from the Player to the 'Position' greater then 'VisibilityDistance' then the condition is considered True\n" +
            "Максимальное расстояние, в пределах которого от заданной позиции бот будет искать Ноду.\n" +
            "Если расстояние от персонажа до заданной позиции 'Position' будет больше 'VisibilityDistance', тогда условие будет истиным.")]
        public double VisibilityDistance { get; set; }

        public CheckNode2() :base()
        {
            Tested = NodeState2.Exist;
            Position = new Vector3();
            VisibilityDistance = 150;
        }

        private bool getNode()
        {
            TargetableNode targetableNode = EntityManager.LocalPlayer.Player.InteractStatus.TargetableNodes.Find((TargetableNode node) => (node.WorldInteractionNode.Location.Distance3D(Position) < 1.0));
            if (targetableNode != null && targetableNode.IsValid)
            {
                currentNode = new Interact.DynaNode(targetableNode.WorldInteractionNode.Key);
                return currentNode.Node.IsValid;
            }
            return false;
        }
        
        public override bool IsValid
        {
            get
            {
                if (Position.Distance3DFromPlayer > VisibilityDistance)
                    return true;

                if ((currentNode == null || !currentNode.Node.IsValid))
                {
                    TargetableNode targetableNode = EntityManager.LocalPlayer.Player.InteractStatus.TargetableNodes.Find((TargetableNode node) => (node.WorldInteractionNode.Location.Distance3D(Position) < 1.0));
                    if (targetableNode != null && targetableNode.IsValid)
                        currentNode = new Interact.DynaNode(targetableNode.WorldInteractionNode.Key);
                    else currentNode = null;
                }

                switch(Tested)
                {
                    case NodeState2.Exist:
                        return currentNode != null && currentNode.Node.IsValid;
                    case NodeState2.NotExist:
                        return currentNode == null || !currentNode.Node.IsValid;
                    case NodeState2.Targetable:
                        return currentNode != null && currentNode.Node.IsValid && currentNode.Node.WorldInteractionNode.IsTargetable();
                    case NodeState2.NotTargetable:
                        return currentNode == null || !currentNode.Node.IsValid || !currentNode.Node.WorldInteractionNode.IsTargetable();
                    case NodeState2.Interactable:
                        return currentNode != null && currentNode.Node.IsValid && currentNode.Node.WorldInteractionNode.CanInteract();
                    case NodeState2.NotInteractable:
                        return currentNode == null || !currentNode.Node.IsValid || !currentNode.Node.WorldInteractionNode.CanInteract();
                    default:
                        return false;
                }
            }
        }

        public override void Reset()
        {
            currentNode = null;
        }

        public override string ToString()
        {
            StringBuilder strBldr = new StringBuilder();
            strBldr.Append(GetType().Name).Append(' ');

            strBldr.Append(" ").Append(Tested);

            if (Position.IsValid)
            {
                strBldr.AppendFormat(" at Position <{0,4:N2}; {1,4:N2}; {2,4:N2}>", new object[] { Position.X, Position.Y, Position.Z });
            }
            return strBldr.ToString();
        }

        public override string TestInfos
        {
            get
            {
                StringBuilder strBldr = new StringBuilder();

                if (Position.IsValid)
                {
                    if (Position.Distance3DFromPlayer > VisibilityDistance)
                        strBldr.AppendFormat("Distance from Player to the Position <{0,4:N2}; {1,4:N2}; {2,4:N2}> greater then ", new object[] { Position.X, Position.Y, Position.Z  })
                            .Append(nameof(VisibilityDistance)).Append('(').Append(VisibilityDistance).Append(')');
                    else
                    {
                        if ((currentNode == null || !currentNode.Node.IsValid))
                        {
                            TargetableNode targetableNode = EntityManager.LocalPlayer.Player.InteractStatus.TargetableNodes.Find((TargetableNode node) => (node.WorldInteractionNode.Location.Distance3D(Position) < 1.0));
                            if (targetableNode != null && targetableNode.IsValid)
                                currentNode = new Interact.DynaNode(targetableNode.WorldInteractionNode.Key);
                            else currentNode = null;
                        }

                        if (currentNode != null && currentNode.Node.IsValid)
                        {
                            strBldr.Append("Node ");
                            StringBuilder strBldr2 = new StringBuilder();
                            foreach (string caterory in currentNode.Node.Categories)
                            {
                                if (strBldr2.Length > 0)
                                    strBldr2.Append(", ");
                                strBldr2.Append(caterory);
                            }
                            if(Tested == NodeState2.Exist || Tested == NodeState2.NotExist)
                                strBldr.Append(" 'Exist' ");

                            if (strBldr2.Length > 0)
                                strBldr.Append('{').Append(strBldr2).Append("} ");
                            strBldr.AppendFormat("in Position <{0,4:N2}; {1,4:N2}; {2,4:N2}> ", new object[] { currentNode.Node.WorldInteractionNode.Location.X,
                                                                                   currentNode.Node.WorldInteractionNode.Location.Y,
                                                                                   currentNode.Node.WorldInteractionNode.Location.Z});

                            if (Tested == NodeState2.Targetable || Tested == NodeState2.NotTargetable)
                            {
                                if (currentNode.Node.WorldInteractionNode.IsTargetable())
                                    strBldr.Append("is 'Targetable'");
                                else strBldr.Append("is 'NotTargetable'");
                            }
                            else if(Tested == NodeState2.Interactable || Tested == NodeState2.NotInteractable)
                            {
                                if (currentNode.Node.WorldInteractionNode.CanInteract())
                                    strBldr.Append("is 'Interactable'");
                                else strBldr.Append("is 'NotInteractable'");
                            }
                            strBldr.AppendLine();
                        }
                        else strBldr.AppendFormat("Node does 'NotExist' in Position <{0,4:N2}; {1,4:N2}; {2,4:N2}>", new object[] { Position.X, Position.Y, Position.Z }).AppendLine();
                        strBldr.Append("Distance from Player is ").Append(Position.Distance3DFromPlayer.ToString("N4"));
                    }
                }

                return strBldr.ToString();
            }
        }
    }
}
