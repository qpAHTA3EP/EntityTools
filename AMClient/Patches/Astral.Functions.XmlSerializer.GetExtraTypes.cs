
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AcTp0Tools.Classes.UCC;
using Astral.Classes.SkillTrain;
using Astral.Controllers;
using Astral.Logic.UCC.Classes;
using Astral.MultiTasks.Classes;
using Astral.Quester.Classes;
using AcTp0Tools.Reflection;
using Action = Astral.Quester.Classes.Action;

using HarmonyLib; 

namespace AcTp0Tools.Patches
{
    /// <summary>
    /// Патч метода Astral.Functions.XmlSerializer.GetExtraTypes()
    /// </summary>
    [HarmonyPatch(typeof(Astral.Functions.XmlSerializer), "GetExtraTypes")] 
    public class Astral_Functions_XmlSerializer_GetExtraTypes
    {
        static Func<List<Type>> GetPluginTypes = typeof(Plugins).GetStaticFunction<List<Type>>("GetTypes");
        internal static List<Type> UccTypes = new List<Type>(20);
        internal static List<Type> QuesterTypes = new List<Type>(100);
        internal static List<Type> MultitaskTypes = new List<Type>(10);
        internal static List<Type> SkillTrainTypes = new List<Type>(50);
        internal static List<Type> UccTargetSelectorTypes = new List<Type>(4);

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
            if (UccTypes.Count == 0 || QuesterTypes.Count == 0 || MultitaskTypes.Count == 0 || SkillTrainTypes.Count == 0)
            {
                UccTypes.Clear();
                QuesterTypes.Clear();
                MultitaskTypes.Clear();
                SkillTrainTypes.Clear();
                UccTargetSelectorTypes.Clear();
                
                // Проверяем типы, объявленные в Астрале
                FillTypeLists(Assembly.GetEntryAssembly().GetTypes());

                // Проверяем типы, объявленные в плагинах
                var types = GetPluginTypes();
                if (types != null && types.Count > 0)
                    FillTypeLists(types); 
            }

            switch (typeNum)
            {
                case 1: // UCC types
                    __result = UccTypes;
                    break;
                case 2: // Quester types
                    __result = QuesterTypes;
                    break;
                case 3: // SkillTrain types
                    __result = SkillTrainTypes;
                    break;
                case 4: // Multitask types
                    __result = MultitaskTypes;
                    break;
                default:
                    emptyTypeList.Clear();
                    __result = emptyTypeList;
                break;
            }
            return false;
        }
        

        static readonly Type tQuesterAction = typeof(Action);
        static readonly Type tQuesterCondition = typeof(Condition);
        
        static readonly Type tSkillTrainAction = typeof(SkillTrainAction);
        static readonly Type tMTAction = typeof(MTAction);
        
        static readonly Type tUccAction = typeof(UCCAction);
        static readonly Type tUccCondition = typeof(UCCCondition);
        static readonly Type tUccTargetSelector = typeof(UccTargetSelector);
        static readonly Type tUccTargetProcessor = typeof(UccTargetProcessor);
        static readonly Type tTargetPriorityEntry = typeof(TargetPriorityEntry);


        internal static void FillTypeLists(IEnumerable<Type> types)
        {
            foreach (Type type in types)
            {
#if true
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
#else
                var baseType = type.BaseType;
                if (baseType == tUCCAction)
                {
                    UCCTypes.Add(type);
                    QuesterTypes.Add(type);
                }
                else if (baseType == tUCCCondition)
                {
                    UCCTypes.Add(type);
                    QuesterTypes.Add(type);
                }
                else if (baseType == tTargetPriorityEntry)
                {
                    UCCTypes.Add(type);
                }
                else if (baseType == tQuesterAction)
                {
                    QuesterTypes.Add(type);
                }
                else if (baseType == tQuesterCondition)
                {
                    UCCTypes.Add(type);
                    QuesterTypes.Add(type);
                }
                else if (baseType == tSkillTrainAction)
                {
                    SkillTrainTypes.Add(type);
                }
                else if (type.BaseType == tMTAction)
                {
                    MultitaskTypes.Add(type);
                }
                else if (baseType == tUccTargetSelector)
                {
                    UCCTypes.Add(type);
                    QuesterTypes.Add(type);
                }
                else if (baseType == tUccTargetProcessor)
                {
                    UCCTypes.Add(type);
                    QuesterTypes.Add(type);
                } 
#endif
            }
        }
    } 
}
