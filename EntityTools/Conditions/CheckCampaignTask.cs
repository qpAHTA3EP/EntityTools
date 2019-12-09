using Astral.Logic.NW;
using Astral.Quester.Classes;
using EntityTools.Editors;
using MyNW.Classes.GroupProject;
using MyNW.Internals;
using MyNW.Patchables.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Text;

namespace EntityTools.Conditions
{
    [Serializable]
    public enum CampaignTaskState
    {
        Completed,
        NotCompleted
    }

    [Serializable]
    public class CheckCampaignTask : Astral.Quester.Classes.Condition
    {
        public override void Reset() { }

        [Editor(typeof(CampaignTaskEditor), typeof(UITypeEditor))]
        public CampaignTask Task { get; set; } = new CampaignTask();

        public CampaignTaskState Tested { get; set; } = CampaignTaskState.Completed;

        public override bool IsValid
        {
            get
            {
                GroupProjectState groupProjectState = EntityManager.LocalPlayer.Player.GroupProjectContainer.ProjectList.Find(prSlot =>
                                                            prSlot.ProjectDef.Name == Task.GroupProject);

                // Этот вариант не работает, потому что коллекция groupProjectState.TaskSlots содержит "пустые" TaskDef, в которых 
                //      TaskDef.Name == "" и 
                //      TaskDef.State == ""
                //if (groupProjectState.IsValid)
                //{
                //    DonationTaskSlot donationTaskSlot = groupProjectState.TaskSlots.Find(taskSlot => taskSlot.TaskDef.Name == Task.TaskName);
                //    if (donationTaskSlot != null && donationTaskSlot.IsValid)
                //        return donationTaskSlot.State == Tested;
                //}

                if(groupProjectState != null && groupProjectState.IsValid)
                {
                    DonationTaskDefRefContainer taskDef = groupProjectState.CompletedTasks.Find(task => task.TaskDef.Name == Task.TaskName);
                    return (Tested == CampaignTaskState.Completed && taskDef != null && taskDef.IsValid)
                            || (Tested == CampaignTaskState.NotCompleted && (taskDef == null || !taskDef.IsValid));
                }

                return false;
            }
        }

        public override string ToString()
        {
            //return $"{GetType().Name}: {Tested} [{Task.GroupProject}] {Task.TaskName}";
            return $"{GetType().Name}: Is [{Task.TaskName}] {Tested}?";
        }

        public override string TestInfos
        {
            get
            {
                GroupProjectState groupProjectState = EntityManager.LocalPlayer.Player.GroupProjectContainer.ProjectList.Find(prSlot =>
                                                            prSlot.ProjectDef.Name == Task.GroupProject);
                if (groupProjectState != null && groupProjectState.IsValid)
                {
                    //DonationTaskSlot donationTaskSlot = groupProjectState.TaskSlots.Find(taskSlot => taskSlot.TaskDef.Name == Task.TaskName);
                    //if (donationTaskSlot != null && donationTaskSlot.IsValid)
                    //    return $"Task {{[{Task.GroupProject}] {Task.TaskName}}} state is '{donationTaskSlot.State}'";

                    if (groupProjectState != null && groupProjectState.IsValid)
                    {
                        DonationTaskDefRefContainer taskDef = groupProjectState.CompletedTasks.Find(task => task.TaskDef.Name == Task.TaskName);
                        if(taskDef != null && taskDef.IsValid)
                            return $"Task [{Task.TaskName}] completed";
                            //return $"Task {{[{Task.GroupProject}] {Task.TaskName}}} completed";
                        else return $"Task [{Task.TaskName}] NOT completed";
                    }
                }

                return $"Do not found task [{Task.TaskName}]";
            }
        }
    }
}
