using System;
using System.Collections.Generic;
using System.Collections;
using Mono.Cecil;
using System.IO;

namespace ClassPatcher
{
    class Program
    {
        static void Main(string[] args)
        {
            AssemblyDefinition aDef;

            string fileName = (args.Length == 0) ? "Astral.exe" : args[0];
            if (File.Exists(fileName))
            {
                aDef = AssemblyDefinition.ReadAssembly(fileName);
                if (aDef != null)
                {
                    foreach (TypeDefinition tDef in aDef.MainModule.GetTypes())
                    {
                        if (tDef.Name == "DialogEditor")
                        {
                            tDef.IsPublic = true;

                            // Все методы internal делаем публичными
                            //if (tDef.HasMethods)
                            //    foreach (MethodDefinition mDef in tDef.Methods)
                            //        if (mDef.IsAssembly)
                            //            mDef.IsPublic = true;
                        }

                        if (tDef.Name == "Plugins")
                        {
                            // Открываем доступ к классу
                            tDef.IsPublic = true;

                            
                            foreach (PropertyDefinition pDef in tDef.Properties)
                                if (pDef.Name == "Assemblies")
                                {
                                    // Открываем доступ к свойству для чтения
                                    pDef.GetMethod.IsPublic = true;
                                }
                        }

                        //if(tDef.Name == "XmlSerializer")
                        //{
                        //    if (tDef.HasMethods)
                        //        foreach (MethodDefinition mDef in tDef.Methods)
                        //            if (mDef.Name == "GetExtraTypes")
                        //                mDef.IsPublic = true;


                        //}
                    }

                    
                    // (!) Функция не находит тип по имени.. что-то указываю не так
                    //if (aDef.MainModule.TryGetTypeReference("XmlSerializer", out TypeReference astrXmlSerializer))
                    //{
                    //    // Делаем публичными метод Astral.Functions.XmlSerializer.GetExtraTypes(...)
                    //    foreach (MethodDefinition mDef in astrXmlSerializer.Resolve()?.Methods)
                    //    {
                    //        if (mDef.Name == "GetExtraTypes")
                    //            mDef.IsPublic = true;
                    //    }
                    //}

                    // (!) Файл занят, поэтому переименовать не удается 
                    //File.Move(fileName, Path.Combine( Path.GetFullPath(fileName), Path.GetFileNameWithoutExtension(fileName)+".bak"));

                    // Записываем измененную сборку 
                    aDef.Write("freeAstral.exe");
                }
            }
        }
    }
}
