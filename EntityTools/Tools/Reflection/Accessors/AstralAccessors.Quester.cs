using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Reflection;
using AStar;
using Astral.Classes;
using Astral.Classes.ItemFilter;
using Astral.Grinder.Classes;
using Astral.Logic.Classes.Map;
using HarmonyLib;
using MyNW.Classes;
using static Astral.Quester.Classes.Action;

namespace EntityTools.Reflection
{
    /// <summary>
    /// Доступ к закрытым членам и методам Astral'a
    /// </summary>
    public static partial class AstralAccessors
    {
        /// <summary>
        /// Доступ к закрытым членам Astral.Quester
        /// </summary>
        public static class Quester
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
                        /// <param name="minHP">Минимальное значени HP, при котором бой принудительно активируется</param>
                        /// <param name="time">Продолжительность времени в течение которого игнорируются атаки</param>
                        public static void SetIgnoreCombat(bool value, int minHP = -1, int time = 0)
                        {
                            setIgnoreCombat?.Invoke(value, minHP, time);
                        }

                        static Combat()
                        {
                            Type combatType = typeof(Astral.Quester.FSM.States.Combat);
                            if (combatType != null)
                                setIgnoreCombat = combatType.GetStaticAction<bool, int, int>(nameof(SetIgnoreCombat));
                        }
                    }
                }
            }

            /// <summary>
            /// Функтор доступа к графу 
            /// </summary>     
            public static class Core
            {
                //TODO: Пропатчить методы доступа к графу, чтобы геттер устанавливал ReadLock, а Setter - WriteLock
                /// <summary>
                /// Функтор доступа к графу путей (карте) текущего профиля
                /// </summary>
                public static readonly StaticPropertyAccessor<Graph> Meshes = typeof(Astral.Quester.Core).GetStaticProperty<Graph>("Meshes");

                public static Graph UsedMeshes
                {
                    get
                    {
                        if (Meshes.IsValid)
                            return Meshes.Value;
                        return null;
                    }
                }

                /// <summary>
                /// Функтор доступа к коллекции графов путей (карт) текущего профиля
                /// Astral.Quester.Core.MapsMeshes
                /// </summary>
                public static readonly StaticPropertyAccessor<Dictionary<string, Graph>> MapsMeshes = typeof(Astral.Quester.Core).GetStaticProperty<Dictionary<string, Graph>>("MapsMeshes");

                /// <summary>
                /// Функтор доступа к списку названий карт в файле текущего профиля
                /// Astral.Quester.Core.AvailableMeshesFromFile(openFileDialog.FileName)
                /// </summary>
                public static readonly Func<string, List<string>> AvailableMeshesFromFile = typeof(Astral.Quester.Core).GetStaticFunction<string, List<string>>("AvailableMeshesFromFile");

                /// <summary>
                /// Доступ к методу 
                /// Astral.Quester.Core.LoadAllMeshes();
                /// </summary>
                public static readonly Func<int> LoadAllMeshes = typeof(Astral.Quester.Core).GetStaticFunction<int>("LoadAllMeshes");

                /// <summary>
                /// Доступ к методу удаления вершин крафаг путей в области, заданной центром окружности и её радиусом
                /// Astral.Quester.Core.RemoveNodesFrom2DPostion(worldPos, Astral.Controllers.Settings.Get.DeleteNodeRadius);
                /// </summary>                
                public static readonly Action<Vector3, double> RemoveNodesFrom2DPosition = typeof(Astral.Quester.Core).GetStaticAction<Vector3, double>("RemoveNodesFrom2DPostion");
            }

            public static class Entrypoint
            {
                public static readonly Func<object, Action> OnLoad = typeof(Astral.Quester.Entrypoint).GetAction("OnLoad");
                public static readonly Func<object, Action> OnUnload = typeof(Astral.Quester.Entrypoint).GetAction("OnUnload");
                public static readonly Func<object, Action<GraphicsNW>> OnMapDraw = typeof(Astral.Quester.Entrypoint).GetAction<GraphicsNW>("OnMapDraw");
                public static readonly Func<object, Action<bool>> Start = typeof(Astral.Quester.Entrypoint).GetAction<bool>("Start");
                public static readonly Func<object, Action> Stop = typeof(Astral.Quester.Entrypoint).GetAction("Stop");
                public static readonly Func<object, Action> TooMuchStuckReaction = typeof(Astral.Quester.Entrypoint).GetAction("TooMuchStuckReaction");
            }

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

                    /// <summary>
                    /// Функтор обновления списка CustomRegion'ов в окне Квестер-редактора
                    /// </summary>
                    private static Func<object, System.Action> QuesterEditor_RefreshRegions = null;
                    public static void RefreshRegions()
                    {
                        if (editorForm.Value is Astral.Quester.Forms.Editor editor
                            && !editor.IsDisposed)
                        {
                            if (QuesterEditor_RefreshRegions == null)
                            {
                                if ((QuesterEditor_RefreshRegions = typeof(Astral.Quester.Forms.Editor).GetAction("RefreshRegions")) != null)
                                    QuesterEditor_RefreshRegions(editor)();
                            }
                            else QuesterEditor_RefreshRegions(editor)();
                        }
                    }
                }
            }
        }
    }
}