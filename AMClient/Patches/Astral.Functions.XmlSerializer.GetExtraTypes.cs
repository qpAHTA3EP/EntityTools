
using Astral;
using Astral.Classes.SkillTrain;
using Astral.Logic.UCC.Classes;
using Astral.MultiTasks.Classes;
using Astral.Quester.Classes;
using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;
using Action = Astral.Quester.Classes.Action;
using Memory = MyNW.Memory;

namespace ACTP0Tools.Patches
{
    /// <summary>
    /// Патч метода <see cref="Astral.Functions.XmlSerializer.GetExtraTypes"/> 
    /// </summary>
    //[HarmonyPatch(typeof(Astral.Functions.XmlSerializer), "GetExtraTypes")] 
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static class ACTP0Serializer
    {
        public static List<Type> UccTypes
        {
            get
            {
                if (!_uccTypes.Any())
                    AnalizeAssemblyTypes();
                return _uccTypes;
            }
        }
        private static readonly List<Type> _uccTypes = new List<Type>(20);
        public static List<Type> QuesterTypes
        {
            get
            {
                if (!_questerTypes.Any())
                    AnalizeAssemblyTypes();
                return _questerTypes;
            }
        }
        private static readonly List<Type> _questerTypes = new List<Type>(100);

        public static List<Type> QuesterActions
        {
            get
            {
                if (!_questerActions.Any())
                    AnalizeAssemblyTypes();
                return _questerActions;
            }
        }
        private static readonly List<Type> _questerActions = new List<Type>(100);

        public static List<Type> QuesterConditions
        {
            get
            {
                if (!_questerConditions.Any())
                    AnalizeAssemblyTypes();
                return _questerConditions;
            }
        }
        private static readonly List<Type> _questerConditions = new List<Type>(100);

        public static List<Type> MultitaskTypes
        {
            get
            {
                if (!_multitaskTypes.Any())
                    AnalizeAssemblyTypes();
                return _multitaskTypes;
            }
        }
        private static readonly List<Type> _multitaskTypes = new List<Type>(10);

        public static List<Type> SkillTrainTypes
        {
            get
            {
                if (!_skillTrainTypes.Any())
                    AnalizeAssemblyTypes();
                return _skillTrainTypes;
            }
        }
        private static readonly List<Type> _skillTrainTypes = new List<Type>(50);
        public static List<Type> UccTargetSelectorTypes
        {
            get
            {
                if (!_uccTargetSelectorTypes.Any())
                    AnalizeAssemblyTypes();
                return _uccTargetSelectorTypes;
            }
        }
        private static readonly List<Type> _uccTargetSelectorTypes = new List<Type>(4);

        public static Type QuesterConditionPack => _questerConditionPack;
        private static Type _questerConditionPack;
        public static Type PushProfileToStackAndLoad => _pushProfileToStackAndLoad;
        private static Type _pushProfileToStackAndLoad;

        internal static Type Before3DDraw_Wrapper;

        static readonly List<Type> emptyTypeList = new List<Type>();


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

            if (_uccTypes.Count == 0 || _questerTypes.Count == 0 || _multitaskTypes.Count == 0 ||
                _skillTrainTypes.Count == 0)
                AnalizeAssemblyTypes();

            switch (typeNum)
            {
                case 1: // UCC types
                    __result = _uccTypes;
                    return false;
                case 2: // Quester types
                    __result = _questerTypes;
                    return false;
                case 3: // SkillTrain types
                    __result = _skillTrainTypes;
                    return false;
                case 4: // Multitask types
                    __result = _multitaskTypes;
                    return false;
            }

            __result = null;
            return false;
        }

        private static void AnalizeAssemblyTypes()
        {
            try
            {
                _uccTypes.Clear();
                _questerTypes.Clear();
                _multitaskTypes.Clear();
                _skillTrainTypes.Clear();
                _uccTargetSelectorTypes.Clear();

                // Проверяем типы, объявленные в Астрале
                CategorizeTypeLists(Assembly.GetEntryAssembly()?.GetTypes());

                // Проверяем типы, объявленные в плагинах
                var types = AstralAccessors.Controllers.Plugins.GetTypes();
                //if (types != null && types.Count > 0)
                if (types != null)
                    CategorizeTypeLists(types);
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

        private static readonly Type tQuesterProfile = typeof(Profile);
        private static readonly Type tQuesterAction = typeof(Action);
        private static readonly Type tQuesterCondition = typeof(Condition);

        private static readonly Type tSkillTrainAction = typeof(SkillTrainAction);
        private static readonly Type tMTAction = typeof(MTAction);

        private static readonly Type tUccProfile = typeof(Profil);
        private static readonly Type tUccAction = typeof(UCCAction);
        private static readonly Type tUccCondition = typeof(UCCCondition);
        private static readonly Type tTargetPriorityEntry = typeof(TargetPriorityEntry);

        public static XmlSerializer QuesterProfileSerializer => _questerProfileSerializer ?? (_questerProfileSerializer = new XmlSerializer(tQuesterProfile, QuesterTypes.ToArray()));
        private static XmlSerializer _questerProfileSerializer;
        public static XmlSerializer QuesterActionSerializer => _questerActionSerializer ?? (_questerActionSerializer = new XmlSerializer(tQuesterAction, QuesterTypes.ToArray()));
        private static XmlSerializer _questerActionSerializer;
        public static XmlSerializer QuesterConditionSerializer => _questerConditionSerializer ?? (_questerConditionSerializer = new XmlSerializer(tQuesterCondition, QuesterTypes.ToArray()));
        private static XmlSerializer _questerConditionSerializer;

        public static XmlSerializer UccProfileSerializer => _uccProfileSerializer ?? (_uccProfileSerializer = new XmlSerializer(tUccProfile, UccTypes.ToArray()));
        private static XmlSerializer _uccProfileSerializer;
        public static XmlSerializer UccActionSerializer => _uccActionSerializer ?? (_uccActionSerializer = new XmlSerializer(tUccAction, UccTypes.ToArray()));
        private static XmlSerializer _uccActionSerializer;
        public static XmlSerializer UccConditionSerializer => _uccConditionSerializer ?? (_uccConditionSerializer = new XmlSerializer(tUccCondition, UccTypes.ToArray()));
        private static XmlSerializer _uccConditionSerializer;
        public static XmlSerializer UccTargetPrioritySerializer => _uccTargetPrioritySerializer ?? (_uccTargetPrioritySerializer = new XmlSerializer(tTargetPriorityEntry, UccTypes.ToArray()));
        private static XmlSerializer _uccTargetPrioritySerializer;

        internal static void CategorizeTypeLists(IEnumerable<Type> types)
        {
            if(types is null)
                return;
            Type qaConditionPackType = null;
            Type etConditionPackType = null;
            foreach (Type type in types)
            {
                switch (type.FullName)
                {
                    case "QuesterAssistant.Conditions.ConditionPack":
                        qaConditionPackType = type;
                        continue;
                    case "EntityTools.Quester.Conditions.ConditionPack":
                        etConditionPackType = type;
                        continue;
                    case "QuesterAssistant.Actions.PushProfileToStackAndLoad":
                        _pushProfileToStackAndLoad = type;
                        break;
                }

                if (tUccAction.IsAssignableFrom(type) 
                    || tUccCondition.IsAssignableFrom(type) 
                    //|| tUccTargetProcessor.IsAssignableFrom(type)
                    )
                {
                    _uccTypes.Add(type);
                    _questerTypes.Add(type);
                }
                else if (tQuesterCondition.IsAssignableFrom(type))
                {
                    _uccTypes.Add(type);
                    _questerTypes.Add(type);
                    _questerConditions.Add(type);
                }
                else if (tQuesterAction.IsAssignableFrom(type))
                {
                    _questerTypes.Add(type);
                    _questerActions.Add(type);
                }
                else if (tTargetPriorityEntry.IsAssignableFrom(type))
                {
                    if (tTargetPriorityEntry == type)
                        continue;
                    _questerTypes.Add(type);
                    _uccTypes.Add(type);
                }
                else if(tMTAction.IsAssignableFrom(type))
                    _multitaskTypes.Add(type);
                else if (tSkillTrainAction.IsAssignableFrom(type))
                {
                    _skillTrainTypes.Add(type);
                }
                else if (type.FullName == "\u0001.\u0002")
                {
                    Before3DDraw_Wrapper = type;
                }
            }
            if (etConditionPackType != null)
            {
                // Используется реализация ConditionPack из EntityTools
                _questerConditionPack = etConditionPackType;
                _questerTypes.Add(_questerConditionPack);
                _uccTypes.Add(_questerConditionPack);
                _questerConditions.Add(_questerConditionPack);
            }
            else if (qaConditionPackType != null)
            {
                // Если тип ConditionPack не найден в EntityTools, используется реализация из QuesterAssistant
                _questerConditionPack = qaConditionPackType;
                if (_questerConditionPack != null)
                {
                    _questerTypes.Add(_questerConditionPack);
                    _uccTypes.Add(_questerConditionPack);
                    _questerConditions.Add(_questerConditionPack);
                }
            }
        }

        /// <summary>
        /// Статический метод, вызов которого влечет вызов статического конструктора и применение патчей
        /// </summary>
        public static void ApplyPatches() {}

        static ACTP0Serializer()
        {
            var tPlugins = typeof(Astral.Controllers.Plugins);
            var tPatch = typeof(ACTP0Serializer);
            var originalInitAssemblies = AccessTools.Method(tPlugins, "InitAssemblies");
            var postfixInitAssemblies = AccessTools.Method(tPatch, nameof(AfterInitAssemblies));

            if (originalInitAssemblies != null
                && postfixInitAssemblies != null)
            {
                ACTP0Patcher.Harmony.Patch(originalInitAssemblies, null,
                    new HarmonyMethod(postfixInitAssemblies));
                Logger.WriteLine(Logger.LogType.Debug, $"Patch of 'Astral.Controllers.Plugins.InitAssemblies()' succeeded");
            }
            else Logger.WriteLine(Logger.LogType.Debug, $"Patch of 'Astral.Controllers.Plugins.InitAssemblies()' failed");

            var tXmlSerializer = typeof(Astral.Functions.XmlSerializer);
            var originalGetExtraTypes = AccessTools.Method(tXmlSerializer, nameof(GetExtraTypes));
            var prefixGetExtraTypes = AccessTools.Method(tPatch, nameof(GetExtraTypes));

            if (originalGetExtraTypes != null
                && prefixGetExtraTypes != null)
            {
                ACTP0Patcher.Harmony.Patch(originalGetExtraTypes, 
                    new HarmonyMethod(prefixGetExtraTypes));
                Logger.WriteLine(Logger.LogType.Debug, $"Patch of 'Astral.Functions.XmlSerializer.GetExtraTypes()' succeeded");
            }
            else Logger.WriteLine(Logger.LogType.Debug, $"Patch of 'Astral.Functions.XmlSerializer.GetExtraTypes()' failed");
        }
    } 
}
