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
    public enum NodeState
    {
        Exist,
        NotExist,
        Targetable,
        NotTargetable,
        Interactable,
        NotInteractable
    }

    public class CheckNode : Astral.Quester.Classes.Condition
    {
        [NonSerialized]
        private Interact.DynaNode currentNode;

        public NodeState Tested { get; set; }

        [Editor(typeof(NodePositionEditor), typeof(UITypeEditor))]
        [Description("Position of the Node that is checked")]
        public Vector3 Position { get; set; }

        [Description("The maximum distance at which the bot can detect the Node\r\n" +
            "If the distance from the Player to the 'Position' greater then 'VisibilityDistance' then the condition is considered True")]
        public double VisibilityDistance { get; set; }

        public CheckNode() :base()
        {
            Tested = NodeState.Exist;
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
                    case NodeState.Exist:
                        return currentNode != null && currentNode.Node.IsValid;
                    case NodeState.NotExist:
                        return currentNode == null || !currentNode.Node.IsValid;
                    case NodeState.Targetable:
                        return currentNode != null && currentNode.Node.IsValid && currentNode.Node.WorldInteractionNode.IsTargetable();
                    case NodeState.NotTargetable:
                        return currentNode == null || !currentNode.Node.IsValid || !currentNode.Node.WorldInteractionNode.IsTargetable();
                    case NodeState.Interactable:
                        return currentNode != null && currentNode.Node.IsValid && currentNode.Node.WorldInteractionNode.CanInteract();
                    case NodeState.NotInteractable:
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
                strBldr.AppendFormat(" at Position <{0,4:N2};{1,4:N2};{2,4:N2}>", new object[] { Position.X, Position.Y, Position.Z });
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
                        strBldr.AppendFormat("Distance from Player to the Position <{0,4:N2};{1,4:N2};{2,4:N2}> greater then ", new object[] { Position.X, Position.Y, Position.Z  })
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
                            if(Tested == NodeState.Exist || Tested == NodeState.NotExist)
                                strBldr.Append(" 'Exist' ");

                            if (strBldr2.Length > 0)
                                strBldr.Append('{').Append(strBldr2).Append("} ");
                            strBldr.AppendFormat("in Position <{0,4:N2};{1,4:N2};{2,4:N2}> ", new object[] { currentNode.Node.WorldInteractionNode.Location.X,
                                                                                   currentNode.Node.WorldInteractionNode.Location.Y,
                                                                                   currentNode.Node.WorldInteractionNode.Location.Z});

                            if (Tested == NodeState.Targetable || Tested == NodeState.NotTargetable)
                            {
                                if (currentNode.Node.WorldInteractionNode.IsTargetable())
                                    strBldr.Append("is 'Targetable'");
                                else strBldr.Append("is 'NotTargetable'");
                            }
                            else if(Tested == NodeState.Interactable || Tested == NodeState.NotInteractable)
                            {
                                if (currentNode.Node.WorldInteractionNode.CanInteract())
                                    strBldr.Append("is 'Interactable'");
                                else strBldr.Append("is 'NotInteractable'");
                            }
                            strBldr.AppendLine();
                        }
                        else strBldr.AppendFormat("Node does 'NotExist' in Position <{0,4:N2};{1,4:N2};{2,4:N2}>", new object[] { Position.X, Position.Y, Position.Z }).AppendLine();
                        strBldr.AppendLine("Distance from Player is ").Append(Position.Distance3DFromPlayer);
                    }
                }

                return strBldr.ToString();
            }
        }
    }
}
