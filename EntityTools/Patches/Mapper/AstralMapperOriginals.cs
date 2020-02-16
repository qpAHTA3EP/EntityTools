using Astral;
using DevExpress.XtraEditors;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Xml.Serialization;
using EntityTools.Tools.Reflection;
using System.Windows.Forms;
using MyNW.Classes;

namespace AstralMapperOriginals
{
    internal static class Class81
    {
        public static bool smethod_0(string string_0, Form form_0 = null)
        {
            Class81.Class82 @class = new Class81.Class82
            {
                form_0 = form_0,
                string_0 = string_0,
                dialogResult_0 = DialogResult.None
            };
            Astral.Controllers.Forms.InvokeOnMainThread(new Action(@class.method_0));
            return @class.dialogResult_0 == DialogResult.Yes;
        }

        public static bool smethod_1<T>(int int_0, List<T> list_0)
        {
            if (int_0 - 1 >= 0)
            {
                list_0.Reverse(int_0 - 1, 2);
                return true;
            }
            return false;
        }

        public static bool smethod_2<T>(int int_0, List<T> list_0)
        {
            if (int_0 + 1 < list_0.Count)
            {
                list_0.Reverse(int_0, 2);
                return true;
            }
            return false;
        }

        public static bool smethod_3<T>(T gparam_0, List<T> list_0)
        {
            return Class81.smethod_1<T>(list_0.IndexOf(gparam_0), list_0);
        }

        public static bool smethod_4<T>(T gparam_0, List<T> list_0)
        {
            return Class81.smethod_2<T>(list_0.IndexOf(gparam_0), list_0);
        }

        public static int smethod_5(Vector3 vector3_0, float float_0, Vector3 vector3_1, Vector3 vector3_2)
        {
            float num = vector3_2.X - vector3_1.X;
            float num2 = vector3_2.Y - vector3_1.Y;
            float num3 = num * num + num2 * num2;
            float num4 = 2f * (num * (vector3_1.X - vector3_0.X) + num2 * (vector3_1.Y - vector3_0.Y));
            float num5 = (vector3_1.X - vector3_0.X) * (vector3_1.X - vector3_0.X) + (vector3_1.Y - vector3_0.Y) * (vector3_1.Y - vector3_0.Y) - float_0 * float_0;
            float num6 = num4 * num4 - 4f * num3 * num5;
            if ((double)num3 <= 1E-07 || num6 < 0f)
            {
                return 0;
            }
            if (num6 == 0f)
            {
                return 1;
            }
            return 2;
        }
        private sealed class Class82
        {
            internal void method_0()
            {
                if (this.form_0 == null)
                {
                    this.form_0 = Form.ActiveForm;
                }
                this.dialogResult_0 = XtraMessageBox.Show(this.form_0, this.string_0, "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            }

            public Form form_0;
            public DialogResult dialogResult_0;
            public string string_0;
        }
    }

    internal static class Class88
    {
        private static readonly Func<Type[]> GetPluginsTypes = typeof(Astral.Controllers.Plugins).GetStaticFunction<Type[]>("GetTypes");

        public static MemoryStream smethod_0(MemoryStream memoryStream_0, string string_0)
        {
            MemoryStream memoryStream = new MemoryStream();
            ZipOutputStream zipOutputStream = new ZipOutputStream(memoryStream);
            zipOutputStream.SetLevel(3);
            zipOutputStream.PutNextEntry(new ZipEntry(string_0)
            {
                DateTime = DateTime.Now
            });
            StreamUtils.Copy(memoryStream_0, zipOutputStream, new byte[4096]);
            zipOutputStream.CloseEntry();
            zipOutputStream.IsStreamOwner = false;
            zipOutputStream.Close();
            memoryStream.Position = 0L;
            return memoryStream;
        }

        public static List<string> smethod_1(string string_0, string string_1)
        {
            List<string> list = new List<string>();
            if (File.Exists(string_0))
            {
                ZipFile zipFile = null;
                try
                {
                    zipFile = new ZipFile(string_0) { UseZip64 = UseZip64.Off };
                    foreach (object obj in zipFile)
                    {
                        ZipEntry zipEntry = obj as ZipEntry;
                        if (zipEntry.Name.EndsWith(string_1))
                        {
                            list.Add(zipEntry.Name);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.WriteLine(ex.ToString());
                }
                finally
                {
                    if (zipFile != null)
                    {
                        zipFile.IsStreamOwner = true;
                        zipFile.Close();
                    }
                }
            }
            return list;
        }

        public static void SaveMeshes2Files(string fileName, List<Class88.Class89> baseTypeList, bool bool_0 = false)
        {
            ZipOutputStream zipOutputStream = null;
            ZipFile zipFile = null;
            try
            {
                if (bool_0 && File.Exists(fileName))
                {
                    zipFile = new ZipFile(File.Open(fileName, FileMode.Open)) { UseZip64 = UseZip64.Off };
                    zipFile.BeginUpdate();
                }
                else
                {
                    zipOutputStream = new ZipOutputStream(File.Open(fileName, FileMode.Create));
                    zipOutputStream.SetLevel(3);
                    zipOutputStream.UseZip64 = UseZip64.Off;
                }
                foreach (Class88.Class89 @class in baseTypeList)
                {
                    MemoryStream memoryStream = new MemoryStream();
                    if (@class.Mode == Class88.Class89.Enum2.const_0)
                    {
                        List<Type> list = new List<Type>();
                        foreach (Type baseType in @class.ExtraBaseTypes)
                        {
                            List<Type> typeList = Assembly.GetExecutingAssembly().GetTypes().ToList<Type>();
                            if(GetPluginsTypes != null)
                                typeList.AddRange(GetPluginsTypes.Invoke());
                            foreach (Type type in typeList)
                            {
                                if (type.BaseType == baseType)
                                {
                                    list.Add(type);
                                }
                            }
                        }
                        new XmlSerializer(@class.Object.GetType(), list.ToArray()).Serialize(memoryStream, @class.Object);
                    }
                    else
                    {
                        BinaryFormatter binaryFormatter = new BinaryFormatter();
                        Logger.WriteLine(@class.FileName);
                        binaryFormatter.Serialize(memoryStream, @class.Object);
                    }
                    memoryStream.Position = 0L;
                    if (zipOutputStream != null)
                    {
                        zipOutputStream.PutNextEntry(new ZipEntry(@class.FileName)
                        {
                            DateTime = DateTime.Now
                        });
                        StreamUtils.Copy(memoryStream, zipOutputStream, new byte[4096]);
                        zipOutputStream.CloseEntry();
                    }
                    if (zipFile != null)
                    {
                        Class88.Class90 class2 = new Class88.Class90();
                        class2.method_0(memoryStream);
                        zipFile.Add(class2, @class.FileName);
                    }
                }
                if (zipFile != null)
                {
                    zipFile.CommitUpdate();
                }
            }
            catch (ThreadAbortException)
            {
            }
            catch (Exception ex)
            {
                Logger.WriteLine(ex.ToString());
                XtraMessageBox.Show(ex.ToString());
            }
            finally
            {
                if (zipOutputStream != null)
                {
                    zipOutputStream.IsStreamOwner = true;
                    zipOutputStream.Close();
                }
                if (zipFile != null)
                {
                    zipFile.IsStreamOwner = true;
                    zipFile.Close();
                }
            }
        }

        public static void smethod_3(string string_0, Class88.Class89 class89_0)
        {
            Class88.smethod_4(string_0, new List<Class88.Class89>
            {
                class89_0
            });
        }

        public static void smethod_4(string string_0, List<Class88.Class89> list_0)
        {
            ZipFile zipFile = null;
            try
            {
                zipFile = new ZipFile(File.OpenRead(string_0)) { UseZip64 = UseZip64.Off };
                foreach (Class88.Class89 @class in list_0)
                {
                    ZipEntry entry = zipFile.GetEntry(@class.FileName);
                    if (entry == null)
                    {
                        @class.Success = false;
                    }
                    else
                    {
                        Stream inputStream = zipFile.GetInputStream(entry);
                        if (@class.Mode == Class88.Class89.Enum2.const_0)
                        {
                            List<Type> list = new List<Type>();
                            foreach (Type baseType in @class.ExtraBaseTypes)
                            {
                                List<Type> pluginsTypeList = Assembly.GetExecutingAssembly().GetTypes().ToList<Type>();
                                if (GetPluginsTypes != null)
                                    pluginsTypeList.AddRange(GetPluginsTypes.Invoke());
                                foreach (Type type in pluginsTypeList)
                                {
                                    if (type.BaseType == baseType)
                                    {
                                        list.Add(type);
                                    }
                                }
                            }
                            XmlSerializer xmlSerializer = new XmlSerializer(@class.Object.GetType(), list.ToArray());
                            @class.Object = xmlSerializer.Deserialize(inputStream);
                            @class.Success = true;
                        }
                        else
                        {
                            BinaryFormatter binaryFormatter = new BinaryFormatter();
                            @class.Object = binaryFormatter.Deserialize(inputStream);
                            @class.Success = true;
                        }
                        inputStream.Close();
                    }
                }
            }
            catch (ThreadAbortException)
            {
            }
            catch (Exception ex)
            {
                Logger.WriteLine(ex.ToString());
                XtraMessageBox.Show(ex.ToString());
            }
            finally
            {
                if (zipFile != null)
                {
                    zipFile.IsStreamOwner = true;
                    zipFile.Close();
                }
            }
        }

        public class Class89
        {
            public object Object { get; set; }

            public Class88.Class89.Enum2 Mode { get; set; }

            public string FileName { get; set; }

            public List<Type> ExtraBaseTypes { get; set; }

            public bool Success { get; set; }

            public Class89(object object_1, Class88.Class89.Enum2 enum2_1, string string_1, List<Type> list_1 = null)
            {
                this.Object = object_1;
                this.Mode = enum2_1;
                this.FileName = string_1;
                this.ExtraBaseTypes = new List<Type>();
                this.Success = false;
                if (list_1 != null)
                {
                    this.ExtraBaseTypes.AddRange(list_1);
                }
            }

            private readonly object object_0;
            private readonly Class88.Class89.Enum2 enum2_0;
            private readonly string string_0;
            private readonly List<Type> list_0;
            private readonly bool bool_0;
            public enum Enum2
            {
                const_0,
                const_1
            }
        }

        public class Class90 : IStaticDataSource
        {
            public Stream GetSource()
            {
                return this.stream_0;
            }

            public void method_0(Stream stream_1)
            {
                this.stream_0 = stream_1;
                this.stream_0.Position = 0L;
            }

            private Stream stream_0;
        }
    }
}