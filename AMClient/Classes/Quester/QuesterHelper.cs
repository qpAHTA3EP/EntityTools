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

        /// <summary>
        /// Поиск в <paramref name="actionPack"/> команды с идентификатором <paramref name="searchedActionId"/>
        /// и установка её в качестве текущей исполняемой команды.
        /// </summary>
        public static void SetStartPoint(ActionPack actionPack, Guid searchedActionId)
        {
            if (actionPack is null)
                return;
            actionPack.Reset();
            SetStartPointInternal(actionPack, searchedActionId);
        }
        private static QuesterAction SetStartPointInternal(ActionPack actionPack, Guid searchedActionId)
        {
            /*
                private Action SetStartPoint(ActionPack pack, Action startAction)
                {
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
                }
             */

            if (actionPack is null)
                return null;
            var actionPlayer = GetActionsPlayer[actionPack];
            if (actionPlayer is null)
                return null;
            var playingActionList = actionPlayer.CurrentActions;
            playingActionList.Clear();
            foreach (var action in actionPack.Actions)
            {
                if (action.ActionID == searchedActionId)
                {
                    playingActionList.Add(action);
                    return action;
                }

                if (action is ActionPack innerActionPack)
                {
                    var foundedAction = SetStartPointInternal(innerActionPack, searchedActionId);
                    if (foundedAction != null)
                    {
                        playingActionList.Add(action);
                        return foundedAction;
                    }
                }
                action.SetCompleted(true, "");
            }
            actionPack.SetCompleted(true, "");
            return null;
        }




        public static void ResetActionPlayer(ActionPack actionPack)
        {
            var actionPlayer = GetActionsPlayer[actionPack];
            if (actionPlayer is null)
                return;
            var playingActionList = actionPlayer.CurrentActions;
            playingActionList.Clear();
        }
    }
}
