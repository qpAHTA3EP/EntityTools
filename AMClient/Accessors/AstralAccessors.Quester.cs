using System;
using System.Collections.Generic;
using AStar;
using HarmonyLib;
using AcTp0Tools.Patches;
using AcTp0Tools.Reflection;

namespace AcTp0Tools
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
                private static readonly StaticPropertyAccessor<Graph> _meshes = typeof(Astral.Quester.Core).GetStaticProperty<Graph>("Meshes");
                public static Graph Meshes
                {
                    get
                    {
                        if (_meshes.IsValid)
                            return _meshes.Value;
                        return null;
                    }
                }

                /// <summary>
                /// Функтор доступа к коллекции графов путей (карт) текущего профиля
                /// Astral.Quester.Core.MapsMeshes
                /// </summary>
                private static readonly StaticPropertyAccessor<Dictionary<string, Graph>> _mapsMeshes = typeof(Astral.Quester.Core).GetStaticProperty<Dictionary<string, Graph>>("MapsMeshes");
                public static Dictionary<string, Graph> MapsMeshes
                {
                    get
                    {
                        if (_mapsMeshes.IsValid)
                            return _mapsMeshes.Value;
                        return null;
                    }
                }

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

#if disabled_20210321
                /// <summary>
                /// Доступ к методу удаления вершин графа путей в области, заданной центром окружности и её радиусом
                /// Astral.Quester.Core.RemoveNodesFrom2DPostion(worldPos, Astral.Controllers.Settings.Get.DeleteNodeRadius);
                /// </summary>                
                public static readonly Action<Vector3, double> RemoveNodesFrom2DPosition = typeof(Astral.Quester.Core).GetStaticAction<Vector3, double>("RemoveNodesFrom2DPostion"); 
#endif

                #region Events
                /// <summary>
                /// Делегат, вызываемый перед вызовом <seealso cref="Astral.Quester.Core.Load(string Path, bool savePath = true)"/>,
                /// то есть перед загрузкой Quester-профиля
                /// </summary>
                public delegate void BeforeLoadEvent(ref string Path);
                public static event BeforeLoadEvent BeforeLoad;
                private static bool prefixLoad(ref string Path, bool savePath)
                {
                    BeforeLoad?.Invoke(ref Path);
                    return true;
                }

                /// <summary>
                /// Делегат, вызываемый после вызова <seealso cref="Astral.Quester.Core.Load(string Path, bool savePath = true)"/>,
                /// то есть после загрузки Quester-профиля
                /// </summary>
                public delegate void AfterLoadEvent(string path);
                public static event AfterLoadEvent AfterLoad;
                private static void postfixLoad(string Path, bool savePath)
                {
                    AfterLoad?.Invoke(Path);
                }

                /// <summary>
                /// Делегат, вызываемый перед вызовом <seealso cref="Astral.Quester.Core.New()"/>,
                /// то есть перед созданием нового Quester-профиля
                /// </summary>
                public delegate void BeforeNewEvent();
                public static event BeforeNewEvent BeforeNew;
                private static bool prefixNew()
                {
                    BeforeNew?.Invoke();
                    return true;
                }

                /// <summary>
                /// Делегат, вызываемый после вызова <seealso cref="Astral.Quester.Core.New()"/>,
                /// то есть после создания нового Quester-профиля
                /// </summary>
                public delegate void AfterNewEvent();
                public static event AfterNewEvent AfterNew;
                private static void postfixNew()
                {
                    AfterNew?.Invoke();
                }
                #endregion

                static Core()
                {
                    var originalLoad = AccessTools.Method(typeof(Astral.Quester.Core), nameof(Astral.Quester.Core.Load));//typeof(Astral.Quester.Core).GetMethod(nameof(Astral.Quester.Core.Load));
                    var prefixLoad = AccessTools.Method(typeof(Core), nameof(Core.prefixLoad));// typeof(Core).GetMethod(nameof(Core.prefixLoad));
                    var postfixLoad = AccessTools.Method(typeof(Core), nameof(Core.postfixLoad));//typeof(Core).GetMethod(nameof(Core.postfixLoad));

                    if (originalLoad != null)
                        AcTp0Patcher.Harmony.Patch(originalLoad, new HarmonyMethod(prefixLoad), new HarmonyMethod(postfixLoad));

                    var originalNew = AccessTools.Method(typeof(Astral.Quester.Core), nameof(Astral.Quester.Core.New));//typeof(Astral.Quester.Core).GetMethod(nameof(Astral.Quester.Core.New));
                    var prefixNew = AccessTools.Method(typeof(Core), nameof(Core.prefixNew));//typeof(Core).GetMethod(nameof(Core.prefixNew));
                    var postfixNew = AccessTools.Method(typeof(Core), nameof(Core.postfixNew));//typeof(Core).GetMethod(nameof(Core.postfixNew));

                    if (originalNew != null)
                        AcTp0Patcher.Harmony.Patch(originalLoad, new HarmonyMethod(prefixNew), new HarmonyMethod(postfixNew));
                }
            }

#if false
            public static class Entrypoint
            {
                public static readonly Func<object, Action> OnLoad = typeof(Astral.Quester.Entrypoint).GetAction("OnLoad");
                public static readonly Func<object, Action> OnUnload = typeof(Astral.Quester.Entrypoint).GetAction("OnUnload");
                public static readonly Func<object, Action<GraphicsNW>> OnMapDraw = typeof(Astral.Quester.Entrypoint).GetAction<GraphicsNW>("OnMapDraw");
                public static readonly Func<object, Action<bool>> Start = typeof(Astral.Quester.Entrypoint).GetAction<bool>("Start");
                public static readonly Func<object, Action> Stop = typeof(Astral.Quester.Entrypoint).GetAction("Stop");
                public static readonly Func<object, Action> TooMuchStuckReaction = typeof(Astral.Quester.Entrypoint).GetAction("TooMuchStuckReaction");
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

#if false // Метод Astral.Quester.Forms.Editor.RefreshRegions() является публичным
                    /// <summary>
                    /// Функтор обновления списка CustomRegion'ов в окне Квестер-редактора
                    /// </summary>
                    private static Func<Astral.Quester.Forms.Editor, System.Action> QuesterEditor_RefreshRegions = null;
                    public static void RefreshRegions()
                    {
                        //TODO: Разобраться почему не видит метод Disposed
#if false
                        if (editorForm.Value is Astral.Quester.Forms.Editor editor && !editor.IsDisposed) 
#else
                        if (editorForm.Value is Astral.Quester.Forms.Editor editor)
#endif
                        {
                            if (QuesterEditor_RefreshRegions == null)
                            {
                                if ((QuesterEditor_RefreshRegions = typeof(Astral.Quester.Forms.Editor).GetAction("RefreshRegions")) != null)
                                    QuesterEditor_RefreshRegions(editor)();
                            }
                            else QuesterEditor_RefreshRegions(editor)();
                        }
                    } 
#endif
                }
            }
        }
    }
}