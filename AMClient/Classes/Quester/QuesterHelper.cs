using ACTP0Tools.Reflection;
using Astral.Quester.Classes;
using System;
using System.Collections.Generic;
using QuesterAction = Astral.Quester.Classes.Action;

namespace ACTP0Tools.Classes.Quester
{
    public static class QuesterHelper
    {
        private static readonly PropertyAccessor<ActionPlayer> GetActionsPlayer = typeof(ActionPack).GetProperty<ActionPlayer>("AP");
        private static readonly PropertyAccessor<List<QuesterAction>> GetCurrentActions = typeof(ActionPlayer).GetProperty<List<QuesterAction>>("CurrentActions");

        public static void SetStartPoint(ActionPack actionPack, Guid actionId)
        {
            if (actionPack is null)
                return;
            actionPack.Reset();
            SetStartPointInternal(actionPack, actionId);
        }
        private static QuesterAction SetStartPointInternal(ActionPack actionPack, Guid actionId)
        {
            /*
                pack.AP.CurrentActions.Clear();
	            foreach (Action action in pack.Actions)
	            {
		            if (action == startAction)
		            {
			            pack.AP.CurrentActions.Add(action);
			            return action;
		            }
		            if (action is ActionPack)
		            {
			            ActionPack pack2 = action as ActionPack;
			            Action action2 = this.SetStartPoint(pack2, startAction);
			            if (action2 != null)
			            {
				            pack.AP.CurrentActions.Add(action);
				            return action2;
			            }
		            }
		            action.SetCompleted(true, "");
	            }
	            pack.SetCompleted(true, "");
	            return null;
             */

            if (actionPack is null)
                return null;
            var actionPlayerCurrentList = GetCurrentActions[GetActionsPlayer[actionPack]];
            actionPlayerCurrentList.Clear();
            foreach (var action in actionPack.Actions)
            {
                if (action.ActionID == actionId)
                {
                    actionPlayerCurrentList.Add(action);
                    return action;
                }

                if (action is ActionPack innerActionPack)
                {
                    var innerAction = SetStartPointInternal(innerActionPack, actionId);
                    if (innerAction != null)
                    {
                        actionPlayerCurrentList.Add(innerAction);
                        return action;
                    }
                }
                action.SetCompleted(true, "");
            }
            actionPack.SetCompleted(true, "");
            return null;
        }
    }
}
