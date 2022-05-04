
using AcTp0Tools.Classes.Targeting;
using Astral;
using Astral.Classes.SkillTrain;
using Astral.Logic.UCC.Classes;
using Astral.MultiTasks.Classes;
using Astral.Quester.Classes;
using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Action = Astral.Quester.Classes.Action;
using Memory = MyNW.Memory;

namespace AcTp0Tools.Patches
{
    /// <summary>
    /// Патч метода Astral.Functions.XmlSerializer.GetExtraTypes()
    /// </summary>
    //[HarmonyPatch(typeof(Astral.Functions.XmlSerializer), "GetExtraTypes")] 
    public static class Astral_Functions_XmlSerializer_GetExtraTypes
    {
        internal static List<Type> UccTypes = new List<Type>(20);
        internal static List<Type> QuesterTypes = new List<Type>(100);
        internal static List<Type> MultitaskTypes = new List<Type>(10);
        internal static List<Type> SkillTrainTypes = new List<Type>(50);
        internal static List<Type> UccTargetSelectorTypes = new List<Type>(4);

        internal static Type Before3DDraw_Wrapper;

        static readonly List<Type> emptyTypeList = new List<Type>();
#if false
        internal HarmonyPatch_XmlSerializer_GetExtraTypes() : base(typeof(Astral.Functions.XmlSerializer).GetMethod("GetExtraTypes", ReflectionHelper.DefaultFlags), typeof(HarmonyPatch_XmlSerializer_GetExtraTypes).GetMethod(nameof(GetExtraTypes), ReflectionHelper.DefaultFlags)) { }

        public override bool NeedInjecttion => true;
#endif

        /// <summary>
        /// Получением списка типов, заданных <paramref name="typeNum"/>, и используемых для сериализации
        /// </summary>
        /// <param name="__result"></param>
        /// <param name="typeNum">UCCTypes = 1,
        /// QuesterTypes = 2,
        /// SkillTrainTypes = 3,
        /// MultitaskTypes = 4</param>
        /// <returns></returns>
        [HarmonyPrefix] 
        public static bool GetExtraTypes(out List<Type> __result, int typeNum)
        {
            // До загрузки сборок плагинов в домен приложения (см. Plugins.InitAssemblies) кэшировать списки типов не имеет смысла
            // поэтому постфикс-патч AfterInitAssemblies устанавливает флаг _pluginsAssembliesLoaded,
            // указывающий на то, что сборки плагинов загружены
            if (!_pluginsAssembliesLoaded)
            {
                __result = emptyTypeList;
                // Следующий вызов приводит к переполнению стека
                //  __result = original_GetExtraTypes.Invoke(null, new object[] { typeNum }) as List<Type>;
                return true;
            }

            if (UccTypes.Count == 0 || QuesterTypes.Count == 0 || MultitaskTypes.Count == 0 || SkillTrainTypes.Count == 0)
            {
                try
                {
                    UccTypes.Clear();
                    QuesterTypes.Clear();
                    MultitaskTypes.Clear();
                    SkillTrainTypes.Clear();
                    UccTargetSelectorTypes.Clear();

                    // Проверяем типы, объявленные в Астрале
                    FillTypeLists(Assembly.GetEntryAssembly()?.GetTypes());

                    // Проверяем типы, объявленные в плагинах
                    var types = AstralAccessors.Controllers.Plugins.GetTypes();
                    //if (types != null && types.Count > 0)
                    if (types != null)
                        FillTypeLists(types);
                }
                catch (TypeLoadException ex)
                {
                    Memory.Detach();
                    StringBuilder sb = new StringBuilder(ex.Message).AppendLine();
                    var data = ex.Data;
                    if (data.Count > 0)
                    {
                        sb.AppendLine("\tData: ");
                        foreach (DictionaryEntry dt in data)
                        {
                            sb.AppendFormat("\tKey: {0,-20}      Value: {1}",
                                "'" + dt.Key + "'", dt.Value);
                        }
                    }

                    sb.Append("StackTrace: ").AppendLine(ex.StackTrace);

                    Logger.WriteLine(Logger.LogType.Debug, sb.ToString());
                    throw;
                }
                catch (ReflectionTypeLoadException ex)
                {
                    Memory.Detach();
                    StringBuilder sb = new StringBuilder(ex.Message).AppendLine();
                    if (ex.LoaderExceptions?.Length > 0)
                    {
                        foreach (var lEx in ex.LoaderExceptions)
                        {
                            sb.Append("\tLoaderException: ").AppendLine(lEx.Message);
                            var smg = lEx.TargetSite?.ToString();
                            if (!string.IsNullOrEmpty(smg))
                            {
                                sb.Append("\tTargetSite: ").AppendLine(lEx.TargetSite?.ToString() ?? "NULL");
                            }
                            var data = lEx.Data;
                            if (data.Count > 0)
                            {
                                sb.AppendLine("\tData: ");
                                foreach (DictionaryEntry dt in data)
                                {
                                    sb.AppendFormat("\t\tKey: {0,-20}      Value: {1}",
                                        "'" + dt.Key + "'", dt.Value);
                                } 
                            }
                        }
                    }

                    sb.Append("StackTrace: ").AppendLine(ex.StackTrace);
                    Logger.WriteLine(Logger.LogType.Debug, sb.ToString());
                    throw;
                }
            }

            switch (typeNum)
            {
                case 1: // UCC types
                    __result = UccTypes;
                    return false;
                case 2: // Quester types
                    __result = QuesterTypes;
                    return false;
                case 3: // SkillTrain types
                    __result = SkillTrainTypes;
                    return false;
                case 4: // Multitask types
                    __result = MultitaskTypes;
                    return false;
            }

            __result = null;
            return true;
        }

        /// <summary>
        /// Отслеживание момента загрузки плагинов, после чего можно кэшировать списки типов
        /// </summary>
        private static void AfterInitAssemblies()
        {
            _pluginsAssembliesLoaded = true;
        }
        /// <summary>
        /// Флаг, указывающий на загрузку плагинов
        /// </summary>
        private static bool _pluginsAssembliesLoaded;

        static readonly Type tQuesterAction = typeof(Action);
        static readonly Type tQuesterCondition = typeof(Condition);
        
        static readonly Type tSkillTrainAction = typeof(SkillTrainAction);
        static readonly Type tMTAction = typeof(MTAction);
        
        static readonly Type tUccAction = typeof(UCCAction);
        static readonly Type tUccCondition = typeof(UCCCondition);
        static readonly Type tUccTargetSelector = typeof(TargetSelector);
        static readonly Type tUccTargetProcessor = typeof(TargetProcessor);
        static readonly Type tTargetPriorityEntry = typeof(TargetPriorityEntry);


        internal static void FillTypeLists(IEnumerable<Type> types)
        {
            if(types is null)
                return;
            
            foreach (Type type in types)
            {
                if (tUccAction.IsAssignableFrom(type) ||
                    tUccCondition.IsAssignableFrom(type) ||
                    tQuesterCondition.IsAssignableFrom(type) ||
                    tUccTargetProcessor.IsAssignableFrom(type))
                {
                    UccTypes.Add(type);
                    QuesterTypes.Add(type);
                }
                else if(tUccTargetSelector.IsAssignableFrom(type))
                {
                    UccTypes.Add(type);
                    QuesterTypes.Add(type);
                    UccTargetSelectorTypes.Add(type);
                }
                else if (tQuesterAction.IsAssignableFrom(type))
                    QuesterTypes.Add(type);
                else if (tTargetPriorityEntry.IsAssignableFrom(type))
                    UccTypes.Add(type);
                else if(tMTAction.IsAssignableFrom(type))
                    MultitaskTypes.Add(type);
                else if (tSkillTrainAction.IsAssignableFrom(type))
                {
                    SkillTrainTypes.Add(type);
                }
                else if (type.FullName == "\u0001.\u0002")
                {
                    Before3DDraw_Wrapper = type;
                }
            }
        }

        /// <summary>
        /// Статический метод, вызов которого влечет вызов статического конструктора и применение патей
        /// </summary>
        public static void ApplyPatches() {}

        private static readonly Type tPlugins;
        private static readonly Type tXmlSerializer;
        private static readonly Type tPatch;

        private static readonly MethodInfo original_InitAssemblies;
        private static readonly MethodInfo postfix_InitAssemblies;

        private static readonly MethodInfo original_GetExtraTypes;
        private static readonly MethodInfo prefix_GetExtraTypes;

        static Astral_Functions_XmlSerializer_GetExtraTypes()
        {
            tPlugins = typeof(Astral.Controllers.Plugins);
            tPatch = typeof(Astral_Functions_XmlSerializer_GetExtraTypes);

            if (tPlugins != null && tPatch != null)
            {
                original_InitAssemblies = AccessTools.Method(tPlugins, "InitAssemblies");
                postfix_InitAssemblies = AccessTools.Method(tPatch, nameof(AfterInitAssemblies));

                if (original_InitAssemblies != null
                    && postfix_InitAssemblies != null)
                {
                    AcTp0Patcher.Harmony.Patch(original_InitAssemblies, null,
                        new HarmonyMethod(postfix_InitAssemblies));
                    Astral.Logger.WriteLine(Logger.LogType.Debug, $"Patch of 'Astral.Controllers.Plugins.InitAssemblies()' succeeded");
                }
                else Astral.Logger.WriteLine(Logger.LogType.Debug, $"Patch of 'Astral.Controllers.Plugins.InitAssemblies()' failed");
            }

            tXmlSerializer = typeof(Astral.Functions.XmlSerializer);

            if (tXmlSerializer != null && tPatch != null)
            {
                original_GetExtraTypes = AccessTools.Method(tXmlSerializer, nameof(GetExtraTypes));
                prefix_GetExtraTypes = AccessTools.Method(tPatch, nameof(GetExtraTypes));

                if (original_GetExtraTypes != null
                    && prefix_GetExtraTypes != null)
                {
                    AcTp0Patcher.Harmony.Patch(original_GetExtraTypes, 
                        new HarmonyMethod(prefix_GetExtraTypes));
                    Astral.Logger.WriteLine(Logger.LogType.Debug, $"Patch of 'Astral.Functions.XmlSerializer.GetExtraTypes()' succeeded");
                }
                else Astral.Logger.WriteLine(Logger.LogType.Debug, $"Patch of 'Astral.Functions.XmlSerializer.GetExtraTypes()' failed");
            }
        }
    } 
}
