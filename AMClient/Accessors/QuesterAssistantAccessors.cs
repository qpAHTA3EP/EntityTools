using System;
using System.Linq;
using System.Reflection;
using ACTP0Tools.Reflection;

namespace ACTP0Tools
{
    public static class QuesterAssistantAccessors
    {
        public static class Classes
        {
            public static class Monitoring
            {
                public static class Frames
                {
                    static Frames()
                    {
                        foreach (var frames in from assembly in AstralAccessors.Controllers.Plugins.Assemblies 
                            where assembly.GetName().Name.StartsWith("QuesterAssistant") 
                            select assembly.GetType("QuesterAssistant.Classes.Monitoring.Frames"))
                        {
                            if (frames != null)
                            {
                                sleep = frames.GetStaticAction<int>(nameof(Sleep));
                                sleepLeft = frames.GetStaticProperty<int>(nameof(SleepLeft));
                            }
                            break;
                        }
                    }

                    private static readonly Action<int> sleep;
                    public static void Sleep(int time)
                    {
                        sleep?.Invoke(time);
                    }

                    private static readonly StaticPropertyAccessor<int> sleepLeft;
                    public static int SleepLeft => sleepLeft?.Value ?? 0;
                }
            }
        }
    }
}
