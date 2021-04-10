
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

#if PATCH_ASTRAL && HARMONY
using HarmonyLib; 
#endif

namespace EntityTools.Patches
{
#if PATCH_ASTRAL && HARMONY
    /// <summary>
    /// Патч метода Astral.Functions.XmlSerializer.GetExtraTypes()
    /// </summary>
    [HarmonyPatch(typeof(Astral.Functions.XmlSerializer), "GetExtraTypes")] 
    internal class HarmonyPatch_XmlSerializer_GetExtraTypes
    {
        static Func<List<Type>> GetPluginTypes = typeof(Plugins).GetStaticFunction<List<Type>>("GetTypes");
        internal static List<Type> UCCTypes = new List<Type>(20);
        internal static List<Type> QuesterTypes = new List<Type>(100);
        internal static List<Type> MultitaskTypes = new List<Type>(10);
        internal static List<Type> SkillTrainTypes = new List<Type>(50);

#if false
        internal HarmonyPatch_XmlSerializer_GetExtraTypes() : base(typeof(Astral.Functions.XmlSerializer).GetMethod("GetExtraTypes", ReflectionHelper.DefaultFlags), typeof(HarmonyPatch_XmlSerializer_GetExtraTypes).GetMethod(nameof(GetExtraTypes), ReflectionHelper.DefaultFlags)) { }

        public override bool NeedInjecttion => true;
#endif

        [HarmonyPrefix] 
        internal static bool GetExtraTypes(out List<Type> __result, int typeNum)
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
                    __result = new List<Type>();
                break;
            }
            return false;
        }

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
                else if (type.BaseType == typeof(Action))
                {
                    QuesterTypes.Add(type);
                }
                else if (type.BaseType == typeof(Condition))
                {
                    UCCTypes.Add(type);
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
#if false
        internal static void FillTypeLists()
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {// перебираем типы, объявленные в Астрале
                foreach (Type type in assembly.GetTypes())
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
                    else if (type.BaseType == typeof(Action))
                    {
                        QuesterTypes.Add(type);
                    }
                    else if (type.BaseType == typeof(Condition))
                    {
                        UCCTypes.Add(type);
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
#endif
}
