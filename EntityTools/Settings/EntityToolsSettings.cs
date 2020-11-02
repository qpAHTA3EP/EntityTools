using System;
using EntityTools.Settings;

namespace EntityTools
{
    [Serializable]
    public class EntityToolsSettings
    {
        /// <summary>
        /// Настройки UnstuckSpellTask
        /// </summary>
        public UnstuckSpellsSettings UnstuckSpells { get; set; } = new UnstuckSpellsSettings();

#if DEVELOPER
        /// <summary>
        /// Настройки Mapper'a
        /// </summary>
        public MapperSettings Mapper { get; set; } = new MapperSettings();

#endif
        /// <summary>
        /// Настройки EntityToolsLogger
        /// </summary>
        public ETLoggerSettings Logger { get; set; } = new ETLoggerSettings();

        /// <summary>
        /// Настройки EntityCache
        /// </summary>
        public EntityCacheSettings EntityCache { get; set; } = new EntityCacheSettings();

        /// <summary>
        /// Настройки службы SlideMonitor
        /// </summary>
        public SlideMonitorSettings SlideMonitor { get; set; } = new SlideMonitorSettings();

#if false
        #region Сериализация/десериализация статических полей класса статического класса
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static bool Serialize(string filename)
        {
            try
            {
                FieldInfo[] fields = typeof(EntityToolsSettings).GetFields(BindingFlags.Static | BindingFlags.NonPublic);

                object[,] a = new object[fields.Length, 2];
                int i = 0;
                foreach (FieldInfo field in fields)
                {
                    a[i, 0] = field.Name;
                    a[i, 1] = field.GetValue(null);
                    i++;
                };
                Stream f = File.Open(filename, FileMode.Create);
                SoapFormatter formatter = new SoapFormatter();
                formatter.Serialize(f, a);
                f.Close();
                return true;
            }
            catch (Exception e)
            {
                EntityToolsLogger.WriteLine(LogType.Error, e.Message);
                Astral.Logger.WriteLine(Astral.Logger.LogType.Debug, e.Message);
                return false;
            }
        }

        public static bool Deserialize(string filename)
        {
            try
            {
                FieldInfo[] fields = typeof(EntityToolsSettings).GetFields(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
                object[,] a;
                Stream f = File.Open(filename, FileMode.Open);
                SoapFormatter formatter = new SoapFormatter();
                a = formatter.Deserialize(f) as object[,];
                f.Close();
                if (a.GetLength(0) != fields.Length) return false;
                int i = 0;
                foreach (FieldInfo field in fields)
                {
                    if (field.Name == (a[i, 0] as string))
                    {
                        if (a[i, 1] != null)
                            field.SetValue(null, a[i, 1]);
                    }
                    i++;
                };
                return true;
            }
            catch
            {
                EntityToolsLogger.WriteLine(LogType.Error, e.Message);
                Astral.Logger.WriteLine(Astral.Logger.LogType.Debug, e.Message);
                return false;
            }
        }
        #endregion

        #region запись/чтение коллекции в XML (реализация IXmlSerializable)
        public void ReadXml(XmlReader reader)
        {
            if (reader.IsStartElement()
                && reader.Name == nameof(EntityToolsSettings))
            {
                reader.ReadStartElement(nameof(EntityToolsSettings));
                while (reader.ReadState == ReadState.Interactive)
                {
                    if (reader.Name == nameof(UnstuckSpells))
                    {
                        object obj = reader.ReadContentAsObject(typeof(UnstuckSpellsSettings));
                        if (obj is UnstuckSpellsSettings unstuckSpellsSettings)
                            UnstuckSpells = unstuckSpellsSettings;
                        else
                        {
                            // ошибка чтения данных
                        }
                    }
                    else if (reader.Name == nameof(MapperSettings))
                    {
                        object obj = reader.ReadContentAs(typeof(MapperSettings), null);
                        if (obj is MapperSettings mapperSettings)
                            Mapper = mapperSettings;
                        else
                        {
                            // ошибка чтения данных
                        }
                    }
                    else if (reader.Name == nameof(EntityToolLoggerSettings))
                    {
                        object obj = reader.ReadContentAs(typeof(EntityToolLoggerSettings), null);
                        if (obj is EntityToolLoggerSettings entityToolLoggerSettings)
                            Logger = entityToolLoggerSettings;
                        else
                        {
                            // ошибка чтения данных
                        }
                    }
                    else if (reader.Name == nameof(EntityCacheSettings))
                    {
                        object obj = reader.ReadContentAs(typeof(EntityCacheSettings), null);
                        if (obj is EntityCacheSettings entityCacheSettings)
                            EntityCache = entityCacheSettings;
                        else
                        {
                            // ошибка чтения данных
                        }
                    }
                    else reader.Skip();
                }
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            nameof(EntityToolsSettings)

            //using (var varEnumerator = collection.GetEnumerator())
            //{
            //    while (varEnumerator.MoveNext())
            //    {
            //        if (varEnumerator.Current.Save)
            //        {
            //            writer.WriteStartElement(nameof(VariableContainer), "");
            //            varEnumerator.Current.WriteXml(writer);
            //            writer.WriteEndElement();
            //        }
            //    }
            //}
        }
        #endregion

        // 
        #region Запись/чтение элементов коллекции в XML (реализация IXmlSerializable)
        //public XmlSchema GetSchema()
        //{
        //    return null;
        //}

        public void ReadXml(XmlReader reader)
        {
            if (reader.IsStartElement()
                && reader.Name == GetType().Name)
            {
                reader.ReadStartElement(nameof(VariableContainer));
                while (reader.ReadState == ReadState.Interactive)
                {
                    switch (reader.Name)
                    {
                        case "Name":
                            name = reader.ReadElementContentAsString(nameof(Name), "");
                            break;
                        case "Value":
                            val = reader.ReadElementContentAsDouble(nameof(Value), "");
                            break;
                        case "Save":
                            string save_str = reader.ReadElementString(nameof(Save));
                            if (Parser.TryParse(save_str, out bool s))
                                Save = s;
                            else Save = false;
                            break;
                        case "AccountScope":
                            string scope_str = reader.ReadElementContentAsString(nameof(AccountScope), "");
                            if (!Enum.TryParse(scope_str, out accountScope))
                                accountScope = AccountScopeType.Global;
                            break;
                        case "ProfileScope":
                            string prof_str = reader.ReadElementString(nameof(ProfileScope));
                            if (Parser.TryParse(prof_str, out ProfileScopeType p))
                                profileScope = p;
                            else if (Parser.TryParse(prof_str, out bool p_bool))
                            {
                                if (p_bool)
                                    profileScope = ProfileScopeType.Profile;
                                else profileScope = ProfileScopeType.Common;
                            }
                            else profileScope = ProfileScopeType.Common;
                            break;
                        case "ScopeQualifier":
                            qualifier = reader.ReadElementContentAsString(nameof(ScopeQualifier), "");
                            break;
                        case "VariableContainer":
                            if (reader.NodeType == XmlNodeType.EndElement)
                            {
                                reader.ReadEndElement();
                                return;
                            }
                            break;
                        default:
                            reader.Skip();
                            break;
                    }
                }
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteElementString(nameof(Name), name);
            writer.WriteElementString(nameof(Value), val.ToString());
            writer.WriteElementString(nameof(Save), Save.ToString());
            writer.WriteElementString(nameof(AccountScope), accountScope.ToString());
            writer.WriteElementString(nameof(ProfileScope), profileScope.ToString());
            writer.WriteElementString(nameof(ScopeQualifier), qualifier);
        }
        #endregion

#endif
    }
}
