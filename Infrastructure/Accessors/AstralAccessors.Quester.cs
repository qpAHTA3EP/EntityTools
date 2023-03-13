using System;
using System.Reflection;
using Infrastructure.Patches;
using HarmonyLib;
using Infrastructure.Reflection;

// ReSharper disable InconsistentNaming

namespace Infrastructure
{
    /// <summary>
    /// Доступ к закрытым членам и методам Astral'a
    /// </summary>
    public static partial class AstralAccessors
    {
        /// <summary>
        /// Доступ к закрытым членам Astral.Quester
        /// </summary>
        public static partial class Quester
        {
#if false
            public static class Action
            {
                public static readonly InstancePropertyAccessor<Astral.Quester.Classes.Action, Astral.Quester.Classes.ActionDebug> ActionDebug = null;

                static Action()
                {
                    ActionDebug = typeof(Astral.Quester.Classes.Action).GetInstanceProperty<Astral.Quester.Classes.ActionDebug>("Debug");
                }
            } 
#endif
            public static class FSM
            {
                public static class States
                {
                    public static class Combat
                    {
                        static readonly StaticFieldAccessor<int> ignoreCombatMinHP =
                            typeof(Astral.Quester.FSM.States.Combat).GetStaticField<int>("ignoreCombatMinHP");
                        public static int IgnoreCombatMinHP
                        {
                            get => ignoreCombatMinHP.Value;
                            set => ignoreCombatMinHP.Value = value;
                        }

                        static Action<bool, int, int> setIgnoreCombat;
                        /// <summary>
                        /// Установка параметров, управляющих режимом игнорирования боя IgnoreCombat
                        /// </summary>
                        /// <param name="value"></param>
                        /// <param name="minHP">Минимальное значение HP, при котором бой принудительно активируется</param>
                        /// <param name="time">Продолжительность времени в течение которого игнорируются атаки</param>
                        public static void SetIgnoreCombat(bool value, int minHP = -1, int time = 0)
                        {
                            setIgnoreCombat?.Invoke(value, minHP, time);
                        }

                        static Combat()
                        {
                            Type combatType = typeof(Astral.Quester.FSM.States.Combat);
                            setIgnoreCombat = combatType.GetStaticAction<bool, int, int>(nameof(SetIgnoreCombat));
                        }
                    }
                }
            }

#if true
            public static class Entrypoint
            {
#if false
                public static readonly Func<object, Action> OnLoad = typeof(Astral.Quester.Entrypoint).GetAction("OnLoad");
                public static readonly Func<object, Action> OnUnload = typeof(Astral.Quester.Entrypoint).GetAction("OnUnload");
                public static readonly Func<object, Action<GraphicsNW>> OnMapDraw = typeof(Astral.Quester.Entrypoint).GetAction<GraphicsNW>("OnMapDraw");
                public static readonly Func<object, Action<bool>> Start = typeof(Astral.Quester.Entrypoint).GetAction<bool>("Start");
                public static readonly Func<object, Action> Stop = typeof(Astral.Quester.Entrypoint).GetAction("Stop");
                public static readonly Func<object, Action> TooMuchStuckReaction = typeof(Astral.Quester.Entrypoint).GetAction("TooMuchStuckReaction"); 
#endif
#if LoadRoleEvent
                public delegate void LoadRoleEvent(Astral.Quester.Entrypoint entrypoint);

                private static MethodInfo originalOnLoadMethod;
                private static MethodInfo prefixOnLoadMethod;
                private static MethodInfo postfixOnLoadMethod;

                /// <summary>
                /// Событие, вызываемое перед загрузкой квестера
                /// </summary>
                public static event LoadRoleEvent BeforeLoadRole;

                private static bool prefixOnLoad(Astral.Quester.Entrypoint __instance)
                {
                    BeforeLoadRole?.Invoke(__instance);
                    return true;
                }

                /// <summary>
                /// Событие, вызываемое после загрузки квестера
                /// </summary>
                public static event LoadRoleEvent AfterLoadRole;

                private static bool postfixOnLoad(Astral.Quester.Entrypoint __instance)
                {
                    AfterLoadRole?.Invoke(__instance);
                    return true;
                } 
#endif
                /// <summary>
                /// Обновление панели Quester'a в главном окне бота 
                /// </summary>
                public static void RefreshQuesterMainPanel()
                {
                    //if (!((Astral.Forms.BasePanel)Astral.Quester.Entrypoint.lastMain).IsDisposed)
                    //    Astral.Controllers.Forms.InvokeOnMainThread(((Astral.Quester.Forms.Main)Astral.Quester.Entrypoint.lastMain).refreshActions()); 
                    if (!questerMainPanel.IsValid
                        || questerMainPanelRefreshActionAccessor is null)
                        return;
                    var mainPanel = questerMainPanel.Value;
                    if (mainPanel is null
                        || mainPanel.IsDisposed)
                        return;

                    Astral.Controllers.Forms.InvokeOnMainThread(() => questerMainPanelRefreshActionAccessor(mainPanel));
                }
                private static readonly StaticFieldAccessor<Astral.Quester.Forms.Main> questerMainPanel;
                private static readonly Action<object> questerMainPanelRefreshActionAccessor;

                static Entrypoint()
                {
                    var tEntrypoint = typeof(Astral.Quester.Entrypoint);

                    questerMainPanel = tEntrypoint.GetStaticField<Astral.Quester.Forms.Main>("lastMain");
                    questerMainPanelRefreshActionAccessor = typeof(Astral.Quester.Forms.Main).GetAction("refreshActions");
#if LoadRoleEvent
                    var tPatch = typeof(Entrypoint);
                    originalOnLoadMethod = AccessTools.Method(tEntrypoint, "OnLoad");
                    prefixOnLoadMethod = AccessTools.Method(tEntrypoint, nameof(prefixOnLoad));
                    postfixOnLoadMethod = AccessTools.Method(tEntrypoint, nameof(postfixOnLoad));

                    if (originalOnLoadMethod != null
                        && prefixOnLoadMethod != null
                        && postfixOnLoadMethod != null)
                    {
                        ACTP0Patcher.Harmony.Patch(originalOnLoadMethod, new HarmonyMethod(prefixOnLoadMethod), new HarmonyMethod(postfixOnLoadMethod));
                    }
#endif
                }
            }  
#endif

            public static class Forms
            {
                public static class Editor
                {
                    /// <summary>
                    /// Функтор доступа к экземпляру Квестер-редактора
                    /// Astral.Quester.Forms.Main.editorForm
                    /// </summary>
                    static readonly StaticFieldAccessor<Astral.Quester.Forms.Editor> editorForm = typeof(Astral.Quester.Forms.Main).GetStaticField<Astral.Quester.Forms.Editor>("editorForm");
                    public static Astral.Quester.Forms.Editor EditorForm => editorForm.Value;

                    // Метод Astral.Quester.Forms.Editor.RefreshRegions() является публичным
                    //public void RefreshRegions() => editorForm.Value?.RefreshRegions();
                }
            }
        }
    }
}