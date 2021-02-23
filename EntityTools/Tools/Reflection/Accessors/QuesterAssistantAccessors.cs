using System;
using System.Reflection;
using EntityTools.Reflection;

namespace EntityTools.Reflection
{
    public static class QuesterAssistantAccessors
    {
        public static class Classes
        {
            public static class Pause
            {
                static Pause()
                {
                    foreach (var assambly in AppDomain.CurrentDomain.GetAssemblies())
                    {
                        if(assambly.GetName().Name.StartsWith("QuesterAssistant"))
                        {
                            Type pause = assambly.GetType("Pause");
                            if(pause != null)
                            {
                                sleep = pause.GetStaticAction<int>(nameof(Sleep));
                                randomSleep = pause.GetStaticAction<int, int>(nameof(RandomSleep));
                            }
                            break;
                        }
                    }
                }

                static Action<int> sleep;
                public static void Sleep(int time)
                {
                    sleep?.Invoke(time);
                }
                static Action<int, int> randomSleep;
                public static void RandomSleep(int min, int max)
                {
                    randomSleep?.Invoke(min, max);
                }
            }
        }
    }
}
