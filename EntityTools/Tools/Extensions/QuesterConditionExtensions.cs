using Astral.Controllers;
using Astral.Quester.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace EntityTools.Tools.Extensions
{
    public static class QuesterConditionExtensions
    {
        internal static Condition Clone(this Condition sourse)
        {
            return Astral.Functions.XmlSerializer.XmlCopy(sourse);
        }

        #region Вызывает ошибку во время исполнения
        //internal static Condition Clone(this Condition condition)
        //{
        //    List<Type> extraTypes = GetExtraTypes();
        //    System.Xml.Serialization.XmlSerializer xmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(Condition), extraTypes.ToArray());
        //    MemoryStream memoryStream = new MemoryStream();
        //    xmlSerializer.Serialize(memoryStream, condition);
        //    memoryStream.Position = 0L;
        //    Condition result = xmlSerializer.Deserialize(memoryStream) as Condition;
        //    memoryStream.Close();
        //    return result;
        //}

        //internal static List<Type> GetExtraTypes()
        //{
        //    if (Plugins.Assemblies == null)
        //        return null;

        //    List<Type> list = new List<Type>();
        //    foreach (Assembly assembly in Plugins.Assemblies)
        //        list.AddRange(assembly.GetTypes());

        //    return list;
        //}
        #endregion
    }
}
