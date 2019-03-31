using Astral.Logic.NW;
using Astral.Quester.UIEditors;
using EntityPlugin.Editors;
using MyNW.Classes;
using MyNW.Internals;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Text;

namespace EntityPlugin.Conditions
{
    public class IsNodeTargetable : Astral.Quester.Classes.Condition
    {
        [NonSerialized]
        private Interact.DynaNode currentNode;

        //public string NodeCategory { get; set; }

        //public string LocalizedName { get; set; }

        [Editor(typeof(NodePositionEditor), typeof(UITypeEditor))]
        public Vector3 NodePosition { get; set; }

        public double VisibilityDistance { get; set; }

        public IsNodeTargetable() :base()
        {
            NodePosition = new Vector3();
            VisibilityDistance = 150;
        }

        private bool getNode()
        {
            //foreach (TargetableNode targetableNode in EntityManager.LocalPlayer.Player.InteractStatus.TargetableNodes)
            //{
            //    if (targetableNode.WorldInteractionNode.Location.Distance3D(NodePosition) < 1.0)
            //    {
            //        currentNode = new Interact.DynaNode(targetableNode.WorldInteractionNode.Key);
            //        return true;
            //    }
            //}
            TargetableNode targetableNode = EntityManager.LocalPlayer.Player.InteractStatus.TargetableNodes.Find((TargetableNode node) => (node.WorldInteractionNode.Location.Distance3D(NodePosition) < 1.0));
            currentNode = new Interact.DynaNode(targetableNode.WorldInteractionNode.Key);
            return currentNode.Node.IsValid;
        }

        public override bool IsValid
        {
            get
            {
                if (NodePosition.Distance3DFromPlayer > VisibilityDistance)
                    return true;

                if((currentNode == null || !currentNode.Node.IsValid))
                {
                    TargetableNode targetableNode = EntityManager.LocalPlayer.Player.InteractStatus.TargetableNodes.Find((TargetableNode node) => (node.WorldInteractionNode.Location.Distance3D(NodePosition) < 1.0));
                    if (targetableNode != null && targetableNode.IsValid)
                        currentNode = new Interact.DynaNode(targetableNode.WorldInteractionNode.Key);
                    else currentNode = null;
                }
                return currentNode != null && currentNode.Node.IsValid && currentNode.Node.WorldInteractionNode.IsTargetable();
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

            if (NodePosition.IsValid)
            {
                strBldr.Append("at Position <").Append(NodePosition).Append("> ");
            }

            return strBldr.ToString();
        }

        public override string TestInfos
        {
            get
            {
                StringBuilder strBldr = new StringBuilder();

                if (NodePosition.IsValid)
                {
                    if (NodePosition.Distance3DFromPlayer > VisibilityDistance)
                        strBldr.Append("Distance from Player to the Position <").Append(NodePosition).Append("> greater then ").Append(nameof(VisibilityDistance)).Append('(').Append(VisibilityDistance).Append(')');
                    else
                    {
                        if ((currentNode == null || !currentNode.Node.IsValid))
                        {
                            TargetableNode targetableNode = EntityManager.LocalPlayer.Player.InteractStatus.TargetableNodes.Find((TargetableNode node) => (node.WorldInteractionNode.Location.Distance3D(NodePosition) < 1.0));
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
                            if (strBldr2.Length > 0)
                                strBldr.Append('{').Append(strBldr2).Append("} ");
                            strBldr.Append("in Position <").Append(currentNode.Node.WorldInteractionNode.Location).Append("> ");
                            if (currentNode.Node.WorldInteractionNode.IsTargetable())
                                strBldr.Append("is 'Targetable'");
                            else strBldr.Append("is NOT 'Targetable'");
                        }
                        else strBldr.Append("Nodes does not found in Position <").Append(NodePosition).Append('>');
                        strBldr.AppendLine("Distance from Player is ").Append(NodePosition.Distance3DFromPlayer);
                    }
                }

                return strBldr.ToString();
            }
        }
    }
}
