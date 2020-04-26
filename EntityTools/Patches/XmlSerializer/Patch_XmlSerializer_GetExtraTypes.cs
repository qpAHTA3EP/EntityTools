#if HARMONY
using HarmonyLib; 
#endif

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Astral.Classes.SkillTrain;
using Astral.Logic.UCC.Classes;
using Astral.MultiTasks.Classes;
using EntityTools.Reflection;

namespace EntityTools.Patches
{
#if DEVELOPER
    /// <summary>
    /// Патч метода Astral.Functions.XmlSerializer.GetExtraTypes()
    /// </summary>
#if HARMONY
    [HarmonyPatch(typeof(Astral.Functions.XmlSerializer), "GetExtraTypes")] 
#endif
    internal class Patch_XmlSerializer_GetExtraTypes : Patch
    {
        static Func<List<Type>> GetPluginTypes = typeof(Astral.Controllers.Plugins).GetStaticFunction<List<Type>>("GetTypes");
        static List<Type> UCCTypes = new List<Type>(20);
        static List<Type> QuesterTypes = new List<Type>(100);
        static List<Type> MultitaskTypes = new List<Type>(10);
        static List<Type> SkillTrainTypes = new List<Type>(50);

        internal Patch_XmlSerializer_GetExtraTypes() : base(typeof(Astral.Functions.XmlSerializer).GetMethod("GetExtraTypes", ReflectionHelper.DefaultFlags), typeof(Patch_XmlSerializer_GetExtraTypes).GetMethod(nameof(GetExtraTypes), ReflectionHelper.DefaultFlags))
        { }

#if HARMONY
        [HarmonyPrefix] 
        internal static bool GetExtraTypes(out List<Type> __result, int typeNum)
        {
            if (UCCTypes.Count == 0 || QuesterTypes.Count == 0 || MultitaskTypes.Count == 0 || SkillTrainTypes.Count == 0)
            {
                UCCTypes.Clear();
                QuesterTypes.Clear();
                MultitaskTypes.Clear();
                SkillTrainTypes.Clear();
                // Заполняем все списки одновременно
                
                // Проверяем типы, объявленные в Астрале
                FillTypeLists(Assembly.GetEntryAssembly().GetTypes());
                
                // Проверяем типы, объявленные в плагинах
                if (GetPluginTypes != null)
                    FillTypeLists(GetPluginTypes());
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
                    __result = new List<Type>();
                break;
            }
            return false;
        }
#else
        internal static List<Type> GetExtraTypes(int typeNum)
        {
            if (UCCTypes.Count == 0 || QuesterTypes.Count == 0 || MultitaskTypes.Count == 0 || SkillTrainTypes.Count == 0)
            {
                UCCTypes.Clear();
                QuesterTypes.Clear();
                MultitaskTypes.Clear();
                SkillTrainTypes.Clear();
                // Заполняем все списки одновременно

                // Проверяем типы, объявленные в Астрале
                FillTypeLists(Assembly.GetEntryAssembly().GetTypes());

                // Проверяем типы, объявленные в плагинах
                if (GetPluginTypes != null)
                    FillTypeLists(GetPluginTypes());
            }

            switch (typeNum)
            {
                case 1: // UCC types
                    return UCCTypes;
                case 2: // Quester types
                    return QuesterTypes;
                case 3: // SkillTrain types
                    return MultitaskTypes;
                case 4: // Multitask types
                    return SkillTrainTypes;
                default:
                    return new List<Type>();
            }
        }
#endif

        internal static void FillTypeLists(IEnumerable<Type> types)
        {
            // перебираем типы, объявленные в Астрале
            foreach (Type type in types)
            {
                if (type.BaseType == typeof(UCCAction))
                {
                    UCCTypes.Add(type);
                    QuesterTypes.Add(type);
                }
                else if (type.BaseType == typeof(UCCCondition))
                {
                    UCCTypes.Add(type);
                    QuesterTypes.Add(type);
                }
                else if (type.BaseType == typeof(TargetPriorityEntry))
                {
                    UCCTypes.Add(type);
                }
                else if (type.BaseType == typeof(Astral.Quester.Classes.Action))
                {
                    QuesterTypes.Add(type);
                }
                else if (type.BaseType == typeof(Astral.Quester.Classes.Condition))
                {
                    UCCTypes.Add(type);
                    QuesterTypes.Add(type);
                }
                else if (type.BaseType == typeof(UCCAction))
                {
                    QuesterTypes.Add(type);
                }
                else if (type.BaseType == typeof(UCCCondition))
                {
                    QuesterTypes.Add(type);
                }
                else if (type.BaseType == typeof(SkillTrainAction))
                {
                    SkillTrainTypes.Add(type);
                }
                else if (type.BaseType == typeof(MTAction))
                {
                    MultitaskTypes.Add(type);
                }
            }
        }
    } 
#endif
}
