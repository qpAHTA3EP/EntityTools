using ACTP0Tools.Annotations;
using Astral.Classes.ItemFilter;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace ACTP0Tools
{
    public static partial class AstralAccessors
    {
        public static partial class Quester
        {
            /// <summary>
            /// Настройки предобработки quester-профиля
            /// для переименования "на лету" элементов, подвергнутых изменениям в обновлениях
            /// </summary>
            public static ProfilePreprocessor Preprocessor
            {
                get
                {
                    if (_preprocessor is null)
                    {
#if false
                        var settingsPath = Path.Combine(Astral.Controllers.Directories.SettingsPath, nameof(ProfilePreprocessor) + ".xml");
                        if (File.Exists(settingsPath))
                        {
                            var serialiser = new XmlSerializer(typeof(ProfilePreprocessor));
                            using (var fileStream = new StreamReader(settingsPath))
                            {
                                if (serialiser.Deserialize(fileStream) is ProfilePreprocessor proc)
                                {
                                    _preprocessor = proc;
                                    return _preprocessor;
                                }
                            }
                        } 
#else
                        if(!LoadPreprocessor())
#endif

                            _preprocessor = new ProfilePreprocessor();
                    }
                    return _preprocessor;
                }
            }
            private static ProfilePreprocessor _preprocessor;

            /// <summary>
            /// Сохранение <see cref="Preprocessor"/> в файл
            /// </summary>
            public static void SavePreprocessor()
            {
                var settingsPath = Path.Combine(Astral.Controllers.Directories.SettingsPath, nameof(ProfilePreprocessor) + ".xml");
                if (!Directory.Exists(Path.GetDirectoryName(settingsPath)))
                {
                    var dir = Path.GetDirectoryName(settingsPath);
                    if (!string.IsNullOrEmpty(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }
                }

                XmlSerializer serializer = new XmlSerializer(typeof(ProfilePreprocessor));
                using (TextWriter fileStream = new StreamWriter(settingsPath, false))
                {
                    serializer.Serialize(fileStream, _preprocessor);
                }
            }

            /// <summary>
            /// Загрузка <see cref="Preprocessor"/> из файла
            /// </summary>
            /// <returns></returns>
            public static bool LoadPreprocessor()
            {
                var settingsPath = Path.Combine(Astral.Controllers.Directories.SettingsPath, nameof(ProfilePreprocessor) + ".xml");
                if (File.Exists(settingsPath))
                {
                    var serialiser = new XmlSerializer(typeof(ProfilePreprocessor));
                    using (var fileStream = new StreamReader(settingsPath))
                    {
                        if (serialiser.Deserialize(fileStream) is ProfilePreprocessor proc)
                        {
                            _preprocessor = proc;
                            return true;
                        }
                    }
                }

                return false;
            }

            public class ProfilePreprocessor : INotifyPropertyChanged
            {
                /// <summary>
                /// Активация предобработки quester-профиля
                /// </summary>
                [Bindable(true)]
                [Description("Активация или деактивация предобработки quester-профиля")]
                public bool Active
                {
                    get => _active;
                    set
                    {
                        if (_active != value)
                        {
                            _active = value;
                        }
                    }
                }
                private bool _active;

                /// <summary>
                /// Активация сохранения quester-профиля после успешной предобработки
                /// </summary>
                [Bindable(true)]
                [Description("Активация сохранения quester-профиля после успешной предобработки")]
                public bool AutoSave
                {
                    get => _autoSave;
                    set
                    {
                        if (_autoSave != value)
                        {
                            _autoSave = value;

                            OnPropertyChanged();
                        }
                    }
                }
                private bool _autoSave;

                [Bindable(true)]
                [Description("Коллекция элементов автозамены при предобработке quester-профиля")]
                [NotifyParentProperty(true)]
                public ObservableCollection<ReplacementItem> Replacement
                {
                    get => _replacement;
                    set
                    {
                        if (value != _replacement)
                        {
                            _replacement = value;
                            OnPropertyChanged();
                        }
                    }
                }

                private ObservableCollection<ReplacementItem> _replacement = new ObservableCollection<ReplacementItem>();

                /// <summary>
                /// Чтение профиля из входного потока <paramref name="input"/>, модификация его в соответствии с <see cref="Replacement"/>
                /// </summary>
                /// <param name="input"></param>
                /// <param name="mods">количество модификаций</param>
                /// <returns>Поток, содержащий модифицированный профиль если заданы <see cref="Replacement"/>, а в противном случае - исходный поток</returns>
                public TextReader Replace(Stream input, out int mods)
                {
                    mods = 0;
                    if (input is null
                        || !input.CanRead)
                        return TextReader.Null;

                    var replacement = _replacement;

                    if (replacement.Count == 0)
                        return new StreamReader(input);

                    int lineNum = -1;

                    var modifiedProfile = new StringBuilder();

                    //var report = new StringBuilder("Preprocessing");

                    using (var reader = new StreamReader(input))
                    {

                        string inStr;
                        while ((inStr = reader.ReadLine()) != null)
                        {
                            lineNum++;
                            var line = inStr;
                            //int lineChanges = 0;
                            foreach (var item in replacement)
                            {
                                if (item.Replace(line, out string outStr))
                                {
                                    mods++;
                                    //lineChanges++;
                                    line = outStr;
                                }
                            }

                            //if (lineChanges > 0)
                            //{
                            //    modifiedProfile.AppendLine(line);
                            //    report.AppendFormat("[{0:########}] line '{1}'\n", lineNum, inStr.Trim());
                            //    report.Append("\t=> '").Append(line.Trim()).AppendLine("'");
                            //    //report.Append("                with replacement {").Append(item).AppendLine("}");
                            //}
                            modifiedProfile.AppendLine(line);
                        }

                    }

                    //if (mods > 0)
                    //{
                    //    report.Append(mods).AppendLine(" modifications are done");
                    //    Logger.WriteLine(report.ToString());
                    //}

                    return new StringReader(modifiedProfile.ToString());
                }

                public event PropertyChangedEventHandler PropertyChanged;

                [NotifyPropertyChangedInvocator]
                protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
                {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                }
            }

            /// <summary>
            /// Элемент предобработки, определяющий искомую строку <see cref="Pattern"/>, которая подлежит замене на строку <see cref="Replacement"/>
            /// Тип подстроки задается <see cref="Type"/>
            /// </summary>
            [Serializable]
            public class ReplacementItem : INotifyPropertyChanged, IEquatable<ReplacementItem>
            {
                private string pattern;
                private string replacement;
                private ItemFilterStringType type;

                /// <summary>
                /// Тип элемента:
                /// </summary>
                [Description("The type of the preprocessing element:\n" +
                             "Simple : the elementary text string. Wildcard '*' does not supported.\n" +
                             "Regext : regular expression in both '" + nameof(Pattern) + "' and '" + nameof(Replacement) + "' element")]
                public ItemFilterStringType Type
                {
                    get => type;
                    set
                    {
                        type = value;
                        processor = null;
                        OnPropertyChanged();
                    }
                }

                /// <summary>
                /// Текстовая строка, подлежащая замене
                /// </summary>
                public string Pattern
                {
                    get => pattern;
                    set
                    {
                        pattern = value;
                        processor = null;
                        OnPropertyChanged();
                    }
                }

                /// <summary>
                /// Строка  замены
                /// </summary>
                public string Replacement
                {
                    get => replacement;
                    set
                    {
                        replacement = value;
                        OnPropertyChanged();
                    }
                }

                public event PropertyChangedEventHandler PropertyChanged;

                [NotifyPropertyChangedInvocator]
                protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
                {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                }
                /// <summary>
                /// Проверка корректности 
                /// </summary>
                [Browsable(false)]
                public bool IsValid
                {
                    get
                    {
                        return GetProcessor() != null;
                    }
                }

                /// <summary>
                /// Замена в исходной строке <paramref name="input"/> всех подстрок, соответствующих <see cref="Pattern"/>, на <see cref="Replacement"/>.
                /// </summary>
                /// <param name="input"></param>
                /// <param name="output">Преобразованный текст</param>
                /// <returns>True, если была произведена хотя бы одна замена</returns>
                public bool Replace(string input, out string output)
                {
                    output = input;
                    var proc = GetProcessor();
                    if (proc != null)
                    {
                        output = proc(input);
                        return !ReferenceEquals(input, output);
                    }
                    return false;
                }

                private Func<string, string> GetProcessor()
                {
                    if (processor is null)
                    {
                        if (string.IsNullOrEmpty(pattern)
                            || string.IsNullOrEmpty(replacement))
                            return null;

                        if (type == ItemFilterStringType.Simple)
                        {
                            string SimpleReplace(string input)
                            {
                                return input.Replace(pattern, replacement);
                            }

                            processor = SimpleReplace;
                        }
                        else if (type == ItemFilterStringType.Regex)
                        {
                            var regex = new Regex(pattern, RegexOptions.Compiled | RegexOptions.Singleline);

                            string RegexReplace(string input)
                            {
                                return regex.Replace(input, replacement);
                            }

                            processor = RegexReplace;
                        }
                    }

                    return processor;
                }
                private Func<string, string> processor;


                public override string ToString() => $"[{type}] {pattern} => {replacement}";

                public bool Equals(ReplacementItem other)
                {
                    if (ReferenceEquals(null, other)) return false;
                    if (ReferenceEquals(this, other)) return true;
                    return pattern == other.pattern && type == other.type;
                }

                public override bool Equals(object obj)
                {
                    if (ReferenceEquals(null, obj)) return false;
                    if (ReferenceEquals(this, obj)) return true;
                    if (obj.GetType() != GetType()) return false;
                    return Equals((ReplacementItem) obj);
                }

                public override int GetHashCode()
                {
                    unchecked
                    {
                        return (pattern.GetHashCode() * 397) ^ (int) type;
                    }
                }

                public static bool operator ==(ReplacementItem left, ReplacementItem right)
                {
                    return Equals(left, right);
                }

                public static bool operator !=(ReplacementItem left, ReplacementItem right)
                {
                    return !Equals(left, right);
                }
            }
        }
    }
}
