
using System;
using System.Collections.Generic;
using System.Reflection;
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
        internal static List<Type> UCCTypes = new List<Type>(20);
        internal static List<Type> QuesterTypes = new List<Type>(100);
        internal static List<Type> MultitaskTypes = new List<Type>(10);
        internal static List<Type> SkillTrainTypes = new List<Type>(50);
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
            if (UCCTypes.Count == 0 || QuesterTypes.Count == 0 || MultitaskTypes.Count == 0 || SkillTrainTypes.Count == 0)
            {
                UCCTypes.Clear();
                QuesterTypes.Clear();
                MultitaskTypes.Clear();
                SkillTrainTypes.Clear();

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
                    __result = UCCTypes;
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
        

        static readonly Type tUCCAction = typeof(UCCAction);
        static readonly Type tUCCCondition = typeof(UCCCondition);
        static readonly Type tTargetPriorityEntry = typeof(TargetPriorityEntry);
        static readonly Type tQuesterAction = typeof(Action);
        static readonly Type tQuesterCondition = typeof(Condition);
        static readonly Type tSkillTrainAction = typeof(SkillTrainAction);
        static readonly Type tMTAction = typeof(MTAction);

        internal static void FillTypeLists(IEnumerable<Type> types)
        {
            foreach (Type type in types)
            {
                if (type.BaseType == tUCCAction)
                {
                    UCCTypes.Add(type);
                    QuesterTypes.Add(type);
                }
                else if (type.BaseType == tUCCCondition)
                {
                    UCCTypes.Add(type);
                    QuesterTypes.Add(type);
                }
                else if (type.BaseType == tTargetPriorityEntry)
                {
                    UCCTypes.Add(type);
                }
                else if (type.BaseType == tQuesterAction)
                {
                    QuesterTypes.Add(type);
                }
                else if (type.BaseType == tQuesterCondition)
                {
                    UCCTypes.Add(type);
                    QuesterTypes.Add(type);
                }
                else if (type.BaseType == tSkillTrainAction)
                {
                    SkillTrainTypes.Add(type);
                }
                else if (type.BaseType == tMTAction)
                {
                    MultitaskTypes.Add(type);
                }
            }
        }
    } 
}
