using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.IO;
using System.Linq;

namespace ClassPatcher
{
    class Program
    {
        static void Main(string[] args)
        {
            AssemblyDefinition assembly;

            if (args.Length > 0)
            {
                switch (args[0])
                {
                    case "-dump":
                        {
                            string fileName = args[1];

                            if (File.Exists(fileName))
                            {
                                string className = args[2];

                                assembly = AssemblyDefinition.ReadAssembly(fileName);
                                StreamWriter dump = File.CreateText(className + ".dump");

                                foreach (var modDef in assembly.Modules)
                                {
                                    foreach (var tDef in modDef.GetTypes())
                                    {
                                        if (tDef.Name == className)
                                        {
                                            foreach (var mDef in tDef.Methods)
                                            {
                                                dump.WriteLine();
                                                dump.WriteLine("Method: " + mDef);
                                                dump.WriteLine();
                                                dump.WriteLine("Variables:");
                                                foreach (var varDef in mDef.Body.Variables)
                                                {
                                                    dump.WriteLine('\t' + varDef.ToString());
                                                }
                                                dump.WriteLine();
                                                foreach (var instruction in mDef.Body.Instructions)
                                                {
                                                    dump.WriteLine(instruction.ToString());
                                                }
                                            }
                                        }
                                    }
                                }

                                dump.Close();
                            }
                        }
                        break;
                    #region Experiments
                    //case "-patch_ucc":
                    //    {

                    //        string fileName;
                    //        if (args.Length > 1)
                    //            fileName = args[1];
                    //        else fileName = "Astral.exe";

                    //        if (File.Exists(fileName))
                    //        {
                    //            assembly = AssemblyDefinition.ReadAssembly(fileName);
                    //            foreach (var modDef in assembly.Modules)
                    //            {
                    //                // Ищем тип AddClass
                    //                var tDef = modDef.GetTypes().FirstOrDefault(td => td.Name == "AddClass");
                    //                if(tDef != null)
                    //                {
                    //                    // Ищем конструктор, который нужно изменить
                    //                    var ctorDef = tDef.Methods.FirstOrDefault(md => md.Name == ".ctor" && md.IsConstructor);
                    //                    if(ctorDef != null)
                    //                    {
                    //                        PatchUCCAddClass(assembly, tDef, ctorDef);
                    //                    }
                    //                    break;
                    //                }
                    //            }

                    //            assembly.Write(Path.Combine(Path.GetDirectoryName(fileName),Path.GetFileNameWithoutExtension(fileName)+"_patched"+Path.GetExtension(fileName)));
                    //            Console.ReadKey();
                    //        }                           
                    //    }
                    //    break;
                    //case "-patch_login":
                    //    {

                    //        string fileName;
                    //        if (args.Length > 1)
                    //            fileName = args[1];
                    //        else fileName = "MyNW.dll";

                    //        if (File.Exists(fileName))
                    //        {
                    //            assembly = AssemblyDefinition.ReadAssembly(fileName);
                    //            // Ищем тип MyNW.Internals.Game
                    //            var tDef = assembly.MainModule.GetTypes().FirstOrDefault(td => td.FullName == "MyNW.Internals.Game");
                    //            if (tDef != null)
                    //            {
                    //                // Ищем метод AccountLogin(string login, string password)
                    //                var accountLogin = tDef.Methods.FirstOrDefault(md => md.Name == "AccountLogin");
                    //                if (accountLogin != null)
                    //                {
                    //                    var writeLineRef = assembly.MainModule.ImportReference(typeof(Astral.Logger).GetMethod("WriteLine", new Type[] { typeof(string) }));

                    //                    accountLogin.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Ldstr, "Inject!"));
                    //                    accountLogin.Body.Instructions.Insert(1, Instruction.Create(OpCodes.Call, writeLineRef));
                    //                }
                    //                break;
                    //            }

                    //            assembly.Write(Path.Combine(Path.GetDirectoryName(fileName), Path.GetFileNameWithoutExtension(fileName) + "_patched" + Path.GetExtension(fileName)));
                    //            Console.ReadKey();
                    //        }
                    //    }
                    //    break;
                    #endregion
                    default /*"-patch"*/:
                        {

                            string fileName;
                            if (args.Length > 1)
                                fileName = args[1];
                            else fileName = "Astral.exe";
                            patch_astral(fileName);
                        }
                        break;
                }
            }
            else patch_astral("Astral.exe");            
        }

        private static bool patch_astral(string fileName)
        {
            bool result = false;
            if (File.Exists(fileName))
            {
                AssemblyDefinition assembly = AssemblyDefinition.ReadAssembly(fileName);
                foreach (var tDef in assembly.MainModule.GetTypes())
                {
                    if (tDef.Name == "ItemIdFilterEditor" ||
                        tDef.Name == "ItemIdEditor" ||
                        tDef.Name == "GetMailItems" ||
                        tDef.Name == "Roles" ||
                        tDef.Name == "Entrypoint" ||
                        tDef.Name == "Engine" ||
                        tDef.Name == "Profile" ||
                        tDef.Name == "Settings" ||
                        tDef.Name == "NPCVendorInfos" ||
                        tDef.Name == "BuyOptionsEditor" ||
                        tDef.Name == "DialogKeyEditor" ||
                        tDef.Name == "DialogEditor" ||
                        tDef.Name == "Core" ||
                        tDef.Name == "ProcessInfos" ||
                        tDef.Name == "RemoteContactEditor" ||
                        tDef.Name == "Movements" ||
                        tDef.Name == "MainMissionEditor" ||
                        tDef.Name == "AuraEditor" ||
                        tDef.Name == "PowerAllIdEditor" ||
                        tDef.Name == "AuraEditor" ||
                        tDef.Name == "NPCInfos" ||
                        tDef.FullName == "Astral.Addons.Role" ||
                        tDef.FullName == "Astral.Controllers.Plugins" ||
                        tDef.FullName == "Astral.Controllers.Relogger" ||
                        tDef.FullName == "Astral.Controllers.CustomClasses" ||
                        tDef.FullName == "Astral.Controllers.AOECheck" ||
                        tDef.FullName == "Astral.Controllers.BotComs.BotClient" ||
                        tDef.Name == "AOE" || // "Astral.Controllers.AOECheck.AOE"
                        tDef.FullName == "Astral.Controllers.AOECheck.AOE" ||
                        tDef.FullName == "Astral.Logic.Classes.Map.Functions.Picture" ||
                        tDef.FullName == "Astral.Logic.UCC.Controllers.Movements" ||
                        tDef.Name == "DodgeLosTestResult" || //Astral.Logic.NW.Movements.DodgeLosTestResult
                        tDef.FullName == "Astral.Quester.FSM.States.Combat" ||
                        tDef.FullName == "Astral.Quester.Controllers.Road"||
                        tDef.Name == "Road")

                    {
                        tDef.IsPublic = true;
                        if (tDef.IsPublic) Console.WriteLine(tDef.ToString());
                        result = true;
                    }
                    if (tDef.FullName == "Astral.Logic.UCC.Controllers.Movements")
                    {
                        foreach (var mDef in tDef.Methods)
                        {
                            mDef.IsPublic = true;
                            if (mDef.IsPublic) Console.WriteLine(mDef.FullName);
                            result = true;
                        }
                    }
                }

                assembly.Write(Path.Combine(Path.GetDirectoryName(fileName), Path.GetFileNameWithoutExtension(fileName) + "_patched" + Path.GetExtension(fileName)));
                Console.ReadKey();
                return result;
            }

            return result;
        }
        //private static void PatchUCCAddClass(AssemblyDefinition assembly, TypeDefinition uccAddClassType, MethodDefinition ctorDef)
        //{
        //    var ilProc = ctorDef.Body.GetILProcessor();

        //    // Удаляем старый код конструктора
        //    ctorDef.Body.Instructions.Clear();
        //    ctorDef.Body.Variables.Clear();
        //    ctorDef.Body.ExceptionHandlers.Clear();

        //    // ссылка на GetCurrentMethod()
        //    var getCurrentMethodRef = assembly.MainModule.ImportReference(typeof(System.Reflection.MethodBase).GetMethod("GetCurrentMethod"));
        //    // ссылка на Attribute.GetCustomAttribute()
        //    //var getCustomAttributeRef = assembly.MainModule.ImportReference(typeof(System.Attribute).GetMethod("GetCustomAttribute", new Type[] { typeof(System.Reflection.MethodInfo), typeof(Type) }));
        //    // ссылка на Type.GetTypeFromHandle() - аналог typeof()
        //    var getTypeFromHandleRef = assembly.MainModule.ImportReference(typeof(Type).GetMethod("GetTypeFromHandle"));
        //    // ссылка на тип MethodBase 
        //    var methodBaseRef = assembly.MainModule.ImportReference(typeof(System.Reflection.MethodBase));

        //    // ссылка на тип List<Type>
        //    var listTypeType = Type.GetType("System.Collections.Generic.List[System.Type]");

        //    // ссылка на тип Dictionary<string,object>
        //    //System.Collections.Generic.Dictionary`2 < System.String,System.Type >
        //    var dictionaryType = Type.GetType("System.Collections.Generic.Dictionary`2[System.String,System.String]");
        //    var dictStringObjectRef = assembly.MainModule.ImportReference(dictionaryType);
        //    //var dictConstructorRef = assembly.MainModule.ImportReference(dictionaryType.GetConstructor(Type.EmptyTypes));
        //    var dictMethodAddRef = assembly.MainModule.ImportReference(dictionaryType.GetMethod("Add"));

        //    // ссылка на типы переменных

        //    var enumeratorListTypeType = Type.GetType("System.Collections.Generic.IEnumerator[System.Type]");
        //    var enumeratorListTypeRef = assembly.MainModule.ImportReference(enumeratorListTypeType);
        //    //var assemblyArrayType = Type.GetType("System.Reflection.Assembly[]");
        //    //var assemblyArrayRef = assembly.MainModule.ImportReference(assemblyArrayType);
        //    //var assemblyType = Type.GetType("System.Reflection.Assembly");
        //    //var assemblyRef = assembly.MainModule.ImportReference(assemblyType);
        //    //var int32Type = Type.GetType("int32");
        //    //var int32Ref = assembly.MainModule.ImportReference(int32Type);
        //    //var boolType = Type.GetType("bool");
        //    //var boolRef = assembly.MainModule.ImportReference(boolType);
        //    //var typeArrayType = Type.GetType("System.Type[]");
        //    //var typeArrayRef = assembly.MainModule.ImportReference(typeArrayType);
        //    var typeType = Type.GetType("System.Type");
        //    var typeRef = assembly.MainModule.ImportReference(typeType);

        //    // необходимо установить InitLocals в true, так как если он находился в false (в методе изначально не было локальных переменных)
        //    // а теперь локальные переменные появятся - верификатор IL кода выдаст ошибку.
        //    ctorDef.Body.InitLocals = true;

        //    // создаем локальные переменные 
        //    var v_0_enumeratorListTypeVar = new VariableDefinition(enumeratorListTypeRef);
        //    ilProc.Body.Variables.Add(v_0_enumeratorListTypeVar);
        //    var v_1_typeVar = new VariableDefinition(typeRef);
        //    ilProc.Body.Variables.Add(v_1_typeVar);
        //    var v_2_uccActionTypeVar = new VariableDefinition(typeRef);
        //    ilProc.Body.Variables.Add(v_2_uccActionTypeVar);            

        //    // добавляем код
        //    //IL_0000: ldarg.0
        //    ilProc.Append(Instruction.Create(OpCodes.Ldarg_0));
        //    //IL_0001: call System.Void System.Object::.ctor()
        //    ilProc.Append(Instruction.Create(OpCodes.Call, methodBaseRef));
        //    //IL_0006: ldarg.0
        //    ilProc.Append(Instruction.Create(OpCodes.Ldarg_0));
        //    //IL_0007: call System.Void TestClassLibrary.TestClass::InitializeComponent()
        //    var initializeComponentRef = assembly.MainModule.ImportReference(uccAddClassType.GetType().GetMethod("InitializeComponent"));
        //    ilProc.Append(Instruction.Create(OpCodes.Call, initializeComponentRef));
        //    //IL_000c: ldc.i4.2
        //    ilProc.Append(Instruction.Create(OpCodes.Ldc_I4_2));
        //    //IL_000d: call System.Collections.Generic.List`1 < System.Type > Astral.Functions.XmlSerializer::GetExtraTypes(System.Int32)
        //    var getExtraTypesRef = assembly.MainModule.ImportReference(typeof(Astral.Functions.XmlSerializer).GetMethod("GetExtraTypes"));
        //    ilProc.Append(Instruction.Create(OpCodes.Call, getExtraTypesRef));
        //    //IL_0012: callvirt System.Collections.Generic.List`1 / Enumerator < !0 > System.Collections.Generic.List`1 < System.Type >::GetEnumerator()
        //    var getEnumeratorListTypeRef = assembly.MainModule.ImportReference(listTypeType.GetMethod("GetEnumerator"));
        //    ilProc.Append(Instruction.Create(OpCodes.Call, getEnumeratorListTypeRef));
        //    //IL_0017: stloc.0
        //    ilProc.Append(Instruction.Create(OpCodes.Stloc_0));
        //    //IL_0018: br.s IL_0064
        //    ilProc.Append(Instruction.Create(OpCodes.Br_S, IL_0064));
        //    //IL_001a: ldloca.s V_0
        //    ilProc.Append(Instruction.Create(OpCodes.Ldloca_S, v_0_enumeratorListTypeVar));
        //    //IL_001c: call !0 System.Collections.Generic.List`1 / Enumerator < System.Type >::get_Current()
        //    var get_CurrentEnumeratorListTypeRef = assembly.MainModule.ImportReference(enumeratorListTypeType.GetMethod("get_Current"));
        //    ilProc.Append(Instruction.Create(OpCodes.Call, get_CurrentEnumeratorListTypeRef));
        //    //IL_0021: stloc.1
        //    ilProc.Append(Instruction.Create(OpCodes.Stloc_1));
        //    //IL_0022: ldloc.1
        //    ilProc.Append(Instruction.Create(OpCodes.Ldloc_1));
        //    //IL_0023: callvirt System.Type System.Type::get_BaseType()
        //    var get_BaseTypeTypeRef = assembly.MainModule.ImportReference(typeType.GetMethod("get_BaseType"));
        //    ilProc.Append(Instruction.Create(OpCodes.Callvirt, get_BaseTypeTypeRef));
        //    //IL_0028: ldtoken Astral.Logic.UCC.Classes.UCCAction
        //    ilProc.Append(Instruction.Create(OpCodes.Ldtoken, Astral.Logic.UCC.Classes.UCCAction));
        //    //IL_002d: call System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle)
        //    ilProc.Append(Instruction.Create(OpCodes.Call, getTypeFromHandleRef));
        //    //IL_0032: stloc.2
        //    ilProc.Append(Instruction.Create(OpCodes.Stloc_2));
        //    //IL_0033: ldloc.2
        //    ilProc.Append(Instruction.Create(OpCodes.Ldloc_2));
        //    //IL_0034: call System.Boolean System.Type::op_Equality(System.Type, System.Type)
        //    var op_EqualityTypeRef = assembly.MainModule.ImportReference(typeType.GetMethod("op_Equality"));
        //    ilProc.Append(Instruction.Create(OpCodes.Call, op_EqualityTypeRef));
        //    //IL_0039: brfalse.s IL_0064
        //    ilProc.Append(Instruction.Create(OpCodes.Br_S, IL_0064));
        //    //IL_003b: ldarg.0
        //    ilProc.Append(Instruction.Create(OpCodes.Ldarg_0));
        //    //IL_003c: ldfld System.Collections.Generic.Dictionary`2 < System.String,System.Type > TestClassLibrary.TestClass::actions
        //    //IL_0041: ldloc.1
        //    //IL_0042: callvirt System.String System.Reflection.MemberInfo::get_Name()
        //    //IL_0047: ldloc.1
        //    //IL_0048: callvirt System.Void System.Collections.Generic.Dictionary`2 < System.String,System.Type >::Add(!0, !1)
        //    //IL_004d: ldarg.0
        //    //IL_004e: ldfld DevExpress.XtraEditors.ListBoxControl TestClassLibrary.TestClass::typesList
        //    //IL_0053: callvirt DevExpress.XtraEditors.Controls.ListBoxItemCollection DevExpress.XtraEditors.ListBoxControl::get_Items()
        //    //IL_0058: ldloc.1
        //    //IL_0059: callvirt System.String System.Reflection.MemberInfo::get_Name()
        //    //IL_005e: callvirt System.Int32 DevExpress.XtraEditors.Controls.ListBoxItemCollection::Add(System.Object)
        //    //IL_0063: pop
        //    //IL_0064: ldloca.s V_0
        //    //IL_0066: call System.Boolean System.Collections.Generic.List`1 / Enumerator < System.Type >::MoveNext()
        //    //IL_006b: brtrue.s IL_001a
        //    //IL_006d: leave.s IL_007d
        //    //IL_006f: ldloca.s V_0
        //    //IL_0071: constrained.System.Collections.Generic.List`1 / Enumerator < System.Type >
        //    //IL_0077: callvirt System.Void System.IDisposable::Dispose()
        //    //IL_007c: endfinally
        //    //IL_007d: ret
        //}

        //private static void PatchUCCAddClass_Full(AssemblyDefinition assembly, TypeDefinition uccAddClassType, MethodDefinition ctorDef)
        //{
        //    var ilProc = ctorDef.Body.GetILProcessor();

        //    // Удаляем старый код конструктора
        //    ctorDef.Body.Instructions.Clear();
        //    ctorDef.Body.Variables.Clear();
        //    ctorDef.Body.ExceptionHandlers.Clear();

        //    // ссылка на GetCurrentMethod()
        //    var getCurrentMethodRef = assembly.MainModule.ImportReference(typeof(System.Reflection.MethodBase).GetMethod("GetCurrentMethod"));
        //    // ссылка на Attribute.GetCustomAttribute()
        //    //var getCustomAttributeRef = assembly.MainModule.ImportReference(typeof(System.Attribute).GetMethod("GetCustomAttribute", new Type[] { typeof(System.Reflection.MethodInfo), typeof(Type) }));
        //    // ссылка на Type.GetTypeFromHandle() - аналог typeof()
        //    var getTypeFromHandleRef = assembly.MainModule.ImportReference(typeof(Type).GetMethod("GetTypeFromHandle"));
        //    // ссылка на тип MethodBase 
        //    var methodBaseRef = assembly.MainModule.ImportReference(typeof(System.Reflection.MethodBase));

        //    // ссылка на тип Dictionary<string,object>
        //    //var dictionaryType = Type.GetType("System.Collections.Generic.Dictionary`2[System.String,System.Object]");
        //    //var dictStringObjectRef = assembly.MainModule.ImportReference(dictionaryType);
        //    //var dictConstructorRef = assembly.MainModule.ImportReference(dictionaryType.GetConstructor(Type.EmptyTypes));
        //    //var dictMethodAddRef = assembly.MainModule.ImportReference(dictionaryType.GetMethod("Add"));

        //    // ссылка на типы переменных
        //    var assemblyArrayType = Type.GetType("System.Reflection.Assembly[]");
        //    var assemblyArrayRef = assembly.MainModule.ImportReference(assemblyArrayType);
        //    var assemblyType = Type.GetType("System.Reflection.Assembly");
        //    var assemblyRef = assembly.MainModule.ImportReference(assemblyType);
        //    var int32Type = Type.GetType("int32");
        //    var int32Ref = assembly.MainModule.ImportReference(int32Type);
        //    var boolType = Type.GetType("bool");
        //    var boolRef = assembly.MainModule.ImportReference(boolType);
        //    var typeArrayType = Type.GetType("System.Type[]");
        //    var typeArrayRef = assembly.MainModule.ImportReference(typeArrayType);
        //    var typeType = Type.GetType("System.Type");
        //    var typeRef = assembly.MainModule.ImportReference(typeType);

        //    // необходимо установить InitLocals в true, так как если он находился в false (в методе изначально не было локальных переменных)
        //    // а теперь локальные переменные появятся - верификатор IL кода выдаст ошибку.
        //    ctorDef.Body.InitLocals = true;

        //    // создаем локальные переменные 
        //    var v_0_assambliesVar = new VariableDefinition(assemblyArrayRef);
        //    ilProc.Body.Variables.Add(v_0_assambliesVar);
        //    var v_1_int = new VariableDefinition(int32Ref);
        //    ilProc.Body.Variables.Add(v_1_int);
        //    var v_2_assamblyVar = new VariableDefinition(assemblyRef);
        //    ilProc.Body.Variables.Add(v_2_assamblyVar);
        //    var v_3_typeArrayVar = new VariableDefinition(typeArrayRef);
        //    ilProc.Body.Variables.Add(v_3_typeArrayVar);
        //    var v_4_int = new VariableDefinition(int32Ref);
        //    ilProc.Body.Variables.Add(v_4_int);
        //    var v_5_typeVar = new VariableDefinition(typeArrayRef);
        //    ilProc.Body.Variables.Add(v_5_typeVar);
        //    var v_6_bool = new VariableDefinition(int32Ref);
        //    ilProc.Body.Variables.Add(v_6_bool);

        //    // добавляем код
        //    Instruction firstInstruction = ilProc.Body.Instructions[0];
        //    //IL_0000: ldarg.0
        //    ilProc.InsertBefore(firstInstruction, Instruction.Create(OpCodes.Ldarg_0));
        //    //IL_0001: call System.Void System.Object::.ctor()
        //    ilProc.InsertBefore(firstInstruction, Instruction.Create(OpCodes.Call, methodBaseRef));
        //    //IL_0006: nop
        //    ilProc.InsertBefore(firstInstruction, Instruction.Create(OpCodes.Nop));
        //    //IL_0007: nop
        //    ilProc.InsertBefore(firstInstruction, Instruction.Create(OpCodes.Nop));
        //    //IL_0008: ldarg.0
        //    ilProc.InsertBefore(firstInstruction, Instruction.Create(OpCodes.Ldarg_0));
        //    //IL_0009: call System.Void TestClassLibrary.TestClass::InitializeComponent()
        //    var initializeComponentRef = assembly.MainModule.ImportReference(uccAddClassType.GetType().GetMethod("InitializeComponent"));
        //    ilProc.InsertBefore(firstInstruction, Instruction.Create(OpCodes.Call, initializeComponentRef));
        //    //IL_000e: nop
        //    ilProc.InsertBefore(firstInstruction, Instruction.Create(OpCodes.Nop));
        //    //IL_000f: nop
        //    ilProc.InsertBefore(firstInstruction, Instruction.Create(OpCodes.Nop));
        //    //IL_0010: call System.AppDomain System.AppDomain::get_CurrentDomain()
        //    var get_CurrentDomainRef = assembly.MainModule.ImportReference(typeof(System.AppDomain).GetMethod("get_CurrentDomain"));
        //    ilProc.InsertBefore(firstInstruction, Instruction.Create(OpCodes.Call, get_CurrentDomainRef));
        //    //IL_0015: callvirt System.Reflection.Assembly[] System.AppDomain::GetAssemblies()
        //    var getAssembliesRef = assembly.MainModule.ImportReference(typeof(System.AppDomain).GetMethod("GetAssemblies"));
        //    ilProc.InsertBefore(firstInstruction, Instruction.Create(OpCodes.Call, initializeComponentRef));
        //    //IL_001a: stloc.0
        //    ilProc.InsertBefore(firstInstruction, Instruction.Create(OpCodes.Stloc));
        //    //IL_001b: ldc.i4.0
        //    ilProc.InsertBefore(firstInstruction, Instruction.Create(OpCodes.Ldc_I4_0));
        //    //IL_001c: stloc.1
        //    ilProc.InsertBefore(firstInstruction, Instruction.Create(OpCodes.Stloc_1));
        //    //IL_001d: br.s IL_0096
        //    ilProc.InsertBefore(firstInstruction, Instruction.Create(OpCodes.Br_S, IL_0096));
        //    //IL_001f: ldloc.0
        //    ilProc.InsertBefore(firstInstruction, Instruction.Create(OpCodes.Ldloc_0));
        //    //IL_0020: ldloc.1
        //    ilProc.InsertBefore(firstInstruction, Instruction.Create(OpCodes.Ldloc_1));
        //    //IL_0021: ldelem.ref
        //    ilProc.InsertBefore(firstInstruction, Instruction.Create(OpCodes.Ldelem_Ref));
        //    //IL_0022: stloc.2
        //    ilProc.InsertBefore(firstInstruction, Instruction.Create(OpCodes.Stloc_2));
        //    //IL_0023: nop
        //    ilProc.InsertBefore(firstInstruction, Instruction.Create(OpCodes.Nop));
        //    //IL_0024: nop
        //    ilProc.InsertBefore(firstInstruction, Instruction.Create(OpCodes.Nop));
        //    //IL_0025: ldloc.2
        //    ilProc.InsertBefore(firstInstruction, Instruction.Create(OpCodes.Ldloc_2));
        //    //IL_0026: callvirt System.Type[] System.Reflection.Assembly::GetTypes()
        //    var getTypesRef = assembly.MainModule.ImportReference(typeof(System.Reflection.Assembly).GetMethod("GetTypes"));
        //    ilProc.InsertBefore(firstInstruction, Instruction.Create(OpCodes.Callvirt, getTypesRef));
        //    //IL_002b: stloc.3
        //    ilProc.InsertBefore(firstInstruction, Instruction.Create(OpCodes.Stloc_3));
        //    //IL_002c: ldc.i4.0
        //    ilProc.InsertBefore(firstInstruction, Instruction.Create(OpCodes.Ldc_I4_0));
        //    //IL_002d: stloc.s V_4
        //    ilProc.InsertBefore(firstInstruction, Instruction.Create(OpCodes.Stloc_s, v_4_int));
        //    //IL_002f: br.s IL_008a
        //    ilProc.InsertBefore(firstInstruction, Instruction.Create(OpCodes.Br_S, IL_008a));
        //    //IL_0031: ldloc.3
        //    ilProc.InsertBefore(firstInstruction, Instruction.Create(OpCodes.Stloc_3));
        //    //IL_0032: ldloc.s V_4
        //    ilProc.InsertBefore(firstInstruction, Instruction.Create(OpCodes.Stloc_s, v_4_int));
        //    //IL_0034: ldelem.ref
        //    ilProc.InsertBefore(firstInstruction, Instruction.Create(OpCodes.Ldelem_Ref));
        //    //IL_0035: stloc.s V_5
        //    ilProc.InsertBefore(firstInstruction, Instruction.Create(OpCodes.Stloc_s, v_5_typeVar));
        //    //IL_0037: nop
        //    ilProc.InsertBefore(firstInstruction, Instruction.Create(OpCodes.Nop));
        //    //IL_0038: ldloc.s V_5
        //    ilProc.InsertBefore(firstInstruction, Instruction.Create(OpCodes.Stloc_s, v_5_typeVar));
        //    //IL_003a: callvirt System.Type System.Type::get_BaseType()
        //    var get_BaseTypeRef = assembly.MainModule.ImportReference(typeof(System.Reflection.Assembly).GetMethod("get_BaseType"));
        //    ilProc.InsertBefore(firstInstruction, Instruction.Create(OpCodes.Callvirt, get_BaseTypeRef));
        //    //IL_003f: ldtoken Astral.Logic.UCC.Classes.UCCAction
        //    ilProc.InsertBefore(firstInstruction, Instruction.Create(OpCodes.Ldtoken, Astral.Logic.UCC.Classes.UCCAction));
        //    //IL_0044: call System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle)
        //    //IL_0049: call System.Boolean System.Type::op_Equality(System.Type, System.Type)
        //    //IL_004e: stloc.s V_6
        //    //IL_0050: ldloc.s V_6
        //    //IL_0052: brfalse.s IL_0083
        //    //IL_0054: nop
        //    //IL_0055: ldarg.0
        //    //IL_0056: ldfld System.Collections.Generic.Dictionary`2 < System.String,System.Type > TestClassLibrary.TestClass::actions
        //    //IL_005b: ldloc.s V_5
        //    //IL_005d: callvirt System.String System.Reflection.MemberInfo::get_Name()
        //    //IL_0062: ldloc.s V_5
        //    //IL_0064: callvirt System.Void System.Collections.Generic.Dictionary`2 < System.String,System.Type >::Add(!0, !1)
        //    //IL_0069: nop
        //    //IL_006a: ldarg.0
        //    //IL_006b: ldfld DevExpress.XtraEditors.ListBoxControl TestClassLibrary.TestClass::typesList
        //    //IL_0070: callvirt DevExpress.XtraEditors.Controls.ListBoxItemCollection DevExpress.XtraEditors.ListBoxControl::get_Items()
        //    //IL_0075: ldloc.s V_5
        //    //IL_0077: callvirt System.String System.Reflection.MemberInfo::get_Name()
        //    //IL_007c: callvirt System.Int32 DevExpress.XtraEditors.Controls.ListBoxItemCollection::Add(System.Object)
        //    //IL_0081: pop
        //    //IL_0082: nop
        //    //IL_0083: nop
        //    //IL_0084: ldloc.s V_4
        //    //IL_0086: ldc.i4.1
        //    //IL_0087: add
        //    //IL_0088: stloc.s V_4
        //    //IL_008a: ldloc.s V_4
        //    //IL_008c: ldloc.3
        //    //IL_008d: ldlen
        //    //IL_008e: conv.i4
        //    //IL_008f: blt.s IL_0031
        //    //IL_0091: nop
        //    //IL_0092: ldloc.1
        //    //IL_0093: ldc.i4.1
        //    //IL_0094: add
        //    //IL_0095: stloc.1
        //    //IL_0096: ldloc.1
        //    //IL_0097: ldloc.0
        //    //IL_0098: ldlen
        //    //IL_0099: conv.i4
        //    //IL_009a: blt.s IL_001f
        //    //IL_009c: ret
        //}

        //static void InjectToAssembly(string path)
        //{
        //    var assembly = AssemblyDefinition.ReadAssembly(path);

        //    // ссылка на GetCurrentMethod()
        //    var getCurrentMethodRef = assembly.MainModule.ImportReference(typeof(System.Reflection.MethodBase).GetMethod("GetCurrentMethod"));
        //    // ссылка на Attribute.GetCustomAttribute()
        //    var getCustomAttributeRef = assembly.MainModule.ImportReference(typeof(System.Attribute).GetMethod("GetCustomAttribute", new Type[] { typeof(System.Reflection.MethodInfo), typeof(Type) }));
        //    // ссылка на Type.GetTypeFromHandle() - аналог typeof()
        //    var getTypeFromHandleRef = assembly.MainModule.ImportReference(typeof(Type).GetMethod("GetTypeFromHandle"));
        //    // ссылка на тип MethodBase 
        //    var methodBaseRef = assembly.MainModule.ImportReference(typeof(System.Reflection.MethodBase));
        //    // ссылка на тип MethodInterceptionAttribute 
        //    var interceptionAttributeRef = assembly.MainModule.ImportReference(typeof(MethodInterceptionAttribute));
        //    // ссылка на MethodInterceptionAttribute.OnEnter
        //    var interceptionAttributeOnEnter = assembly.MainModule.ImportReference(typeof(MethodInterceptionAttribute).GetMethod("OnEnter"));
        //    // ссылка на тип Dictionary<string,object>
        //    var dictionaryType = Type.GetType("System.Collections.Generic.Dictionary`2[System.String,System.Object]");
        //    var dictStringObjectRef = assembly.MainModule.ImportReference(dictionaryType);
        //    var dictConstructorRef = assembly.MainModule.ImportReference(dictionaryType.GetConstructor(Type.EmptyTypes));
        //    var dictMethodAddRef = assembly.MainModule.ImportReference(dictionaryType.GetMethod("Add"));
        //    foreach (var typeDef in assembly.MainModule.Types)
        //    {
        //        foreach (var method in typeDef.Methods.Where(m => m.CustomAttributes.Where(
        //          attr => attr.AttributeType.Resolve().BaseType.Name == "MethodInterceptionAttribute").FirstOrDefault() != null))
        //        {
        //            var ilProc = method.Body.GetILProcessor();
        //            // необходимо установить InitLocals в true, так как если он находился в false (в методе изначально не было локальных переменных)
        //            // а теперь локальные переменные появятся - верификатор IL кода выдаст ошибку.
        //            method.Body.InitLocals = true;
        //            // создаем три локальных переменных для attribute, currentMethod и parameters
        //            var attributeVariable = new VariableDefinition(interceptionAttributeRef);
        //            var currentMethodVar = new VariableDefinition(methodBaseRef);
        //            var parametersVariable = new VariableDefinition(dictStringObjectRef);
        //            ilProc.Body.Variables.Add(attributeVariable);
        //            ilProc.Body.Variables.Add(currentMethodVar);
        //            ilProc.Body.Variables.Add(parametersVariable);
        //            Instruction firstInstruction = ilProc.Body.Instructions[0];
        //            ilProc.InsertBefore(firstInstruction, Instruction.Create(OpCodes.Nop));
        //            // получаем текущий метод
        //            ilProc.InsertBefore(firstInstruction, Instruction.Create(OpCodes.Call, getCurrentMethodRef));
        //            // помещаем результат со стека в переменную currentMethodVar
        //            ilProc.InsertBefore(firstInstruction, Instruction.Create(OpCodes.Stloc, currentMethodVar));
        //            // загружаем на стек ссылку на текущий метод
        //            ilProc.InsertBefore(firstInstruction, Instruction.Create(OpCodes.Ldloc, currentMethodVar));
        //            // загружаем ссылку на тип MethodInterceptionAttribute
        //            ilProc.InsertBefore(firstInstruction, Instruction.Create(OpCodes.Ldtoken, interceptionAttributeRef));
        //            // Вызываем GetTypeFromHandle (в него транслируется typeof()) - эквивалент typeof(MethodInterceptionAttribute)
        //            ilProc.InsertBefore(firstInstruction, Instruction.Create(OpCodes.Call, getTypeFromHandleRef));
        //            // теперь у нас на стеке текущий метод и тип MethodInterceptionAttribute. Вызываем Attribute.GetCustomAttribute
        //            ilProc.InsertBefore(firstInstruction, Instruction.Create(OpCodes.Call, getCustomAttributeRef));
        //            // приводим результат к типу MethodInterceptionAttribute
        //            ilProc.InsertBefore(firstInstruction, Instruction.Create(OpCodes.Castclass, interceptionAttributeRef));
        //            // сохраняем в локальной переменной attributeVariable
        //            ilProc.InsertBefore(firstInstruction, Instruction.Create(OpCodes.Stloc, attributeVariable));
        //            // создаем новый Dictionary<stirng, object>
        //            ilProc.InsertBefore(firstInstruction, Instruction.Create(OpCodes.Newobj, dictConstructorRef));
        //            // помещаем в parametersVariable
        //            ilProc.InsertBefore(firstInstruction, Instruction.Create(OpCodes.Stloc, parametersVariable));
        //            foreach (var argument in method.Parameters)
        //            {
        //                //для каждого аргумента метода
        //                // загружаем на стек наш Dictionary<string,object>
        //                ilProc.InsertBefore(firstInstruction, Instruction.Create(OpCodes.Ldloc, parametersVariable));
        //                // загружаем имя аргумента
        //                ilProc.InsertBefore(firstInstruction, Instruction.Create(OpCodes.Ldstr, argument.Name));
        //                // загружаем значение аргумента
        //                ilProc.InsertBefore(firstInstruction, Instruction.Create(OpCodes.Ldarg, argument));
        //                // вызываем Dictionary.Add(string key, object value)
        //                ilProc.InsertBefore(firstInstruction, Instruction.Create(OpCodes.Call, dictMethodAddRef));
        //            }
        //            // загружаем на стек сначала атрибут, потом параметры для вызова его метода OnEnter
        //            ilProc.InsertBefore(firstInstruction, Instruction.Create(OpCodes.Ldloc, attributeVariable));
        //            ilProc.InsertBefore(firstInstruction, Instruction.Create(OpCodes.Ldloc, currentMethodVar));
        //            ilProc.InsertBefore(firstInstruction, Instruction.Create(OpCodes.Ldloc, parametersVariable));
        //            // вызываем OnEnter. На стеке должен быть объект, на котором вызывается OnEnter и параметры метода
        //            ilProc.InsertBefore(firstInstruction, Instruction.Create(OpCodes.Callvirt, interceptionAttributeOnEnter));
        //        }
        //    }
        //    assembly.Write(path);
        //}
    }
}

