using System;
using System.Reflection;
using EntityTools.Reflection;

namespace EntityTools.Reflection
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
#if false
                        foreach (var assambly in AppDomain.CurrentDomain.GetAssemblies()) 
#else
                        foreach (var assambly in AstralAccessors.Controllers.Plugins.Assemblies)
#endif
                        {
                            if (assambly.GetName().Name.StartsWith("QuesterAssistant"))
                            {
                                Type frames = assambly.GetType("QuesterAssistant.Classes.Monitoring.Frames");
                                if (frames != null)
                                {
                                    _sleep = frames.GetStaticAction<int>(nameof(Sleep));
                                    _sleepLeft = frames.GetStaticProperty<int>(nameof(SleepLeft));
                                }
                                break;
                            }
                        }
                    }

                    static Action<int> _sleep;
                    public static void Sleep(int time)
                    {
                        _sleep?.Invoke(time);
                    }
                    static StaticPropertyAccessor<int> _sleepLeft;
                    public static int SleepLeft
                    {
                        get
                        {
                            return _sleepLeft?.Value ?? 0;
                        }
                    }
                }
            }
        }
    }
}
