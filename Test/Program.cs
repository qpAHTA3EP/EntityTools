using EntityTools.Forms;
using EntityTools.Tools;
using EntityTools.Tools.MountInsignias;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            ///
            /// Экспорт интерфейсов в XML
            /// 
            //InterfaceWrapper Interfaces = new InterfaceWrapper();
            ////UIInterfaceDef uIInterface = new UIInterfaceDef();

            //string fullFileName = @".\test.xml";

            //if (!Directory.Exists(Path.GetDirectoryName(fullFileName)))
            //    Directory.CreateDirectory(Path.GetDirectoryName(fullFileName));

            //XmlSerializer serialiser = new XmlSerializer(typeof(InterfaceWrapper));
            //TextWriter FileStream = new StreamWriter(fullFileName);
            //serialiser.Serialize(FileStream, Interfaces);
            //FileStream.Close();

            ///
            /// Экспорт списка бонусов знаков
            /// 
            //BindingList<MountBonusesDef> Bonuses = new BindingList<MountBonusesDef>();
            //Bonuses.Add(new MountBonusesDef()
            //                    {
            //                        Barbed = 0,
            //                        Crescent = 0,
            //                        Illuminated = 1,
            //                        Enlightened = 1,
            //                        Regal = 1,
            //                        InternalName = "testName",
            //                        NameGL = "NameGL",
            //                        NameRU = "NameRU",
            //                        DescriptionGL = "DescriptionGL",
            //                        DescriptionRU = "DescriptionRU"

            //                    });
            //Bonuses.Add(new MountBonusesDef());


            ///
            /// Экспорт списка бонусов знаков в файла
            /// 
            /// //string fullFileName = @".\test.xml";

            //if (!Directory.Exists(Path.GetDirectoryName(fullFileName)))
            //    Directory.CreateDirectory(Path.GetDirectoryName(fullFileName));

            //XmlSerializer serialiser = new XmlSerializer(typeof(BindingList<MountBonusesDef>));
            //TextWriter FileStream = new StreamWriter(fullFileName);
            //serialiser.Serialize(FileStream, Bonuses);
            //FileStream.Close();

            ///
            /// Bvgjhn списка бонусов знаков из файла
            /// 
            //XmlSerializer serializer = new XmlSerializer(typeof(BindingList<MountBonusesDef>));
            //using (FileStream fs = new FileStream(@".\test.xml", FileMode.OpenOrCreate))
            //{
            //    BindingList<MountBonusesDef> bonuses = (BindingList<MountBonusesDef>)serializer.Deserialize(fs);

            //    foreach (MountBonusesDef bon in bonuses)
            //    {
            //        Console.WriteLine($"{bon.InternalName} <{bon.Barbed}, {bon.Crescent}, {bon.Enlightened}, {bon.Illuminated}, {bon.Regal}>");
            //        Console.WriteLine($"\t{bon.NameGL}\t{bon.DescriptionGL}");
            //        Console.WriteLine($"\t{bon.NameRU}\t{bon.DescriptionRU}");
            //    }
            //}

            //Console.ReadKey();

            InsigniaBonusSelectForm.GetMountBonuses();
        }
    }

    //internal class InsigniaBonuses
    //{
    //    public BindingList<MountBonusesDef> Bonuses = new BindingList<MountBonusesDef>();
    //}
}
