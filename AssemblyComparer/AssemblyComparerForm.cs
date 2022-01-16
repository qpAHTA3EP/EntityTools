using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Mono.Cecil;
using File = System.IO.File;

namespace AssemblyComparer
{
    public partial class AssemblyComparerForm : Form
    {
        public AssemblyComparerForm()
        {
            InitializeComponent();
        }

        private void btnFile1_Click(object sender, EventArgs e)
        {
            openFileDialog.FileName = tbFile1.Text;
            if(openFileDialog.ShowDialog() == DialogResult.OK)
            {
                tbFile1.Text = openFileDialog.FileName;
                btnCompare.Enabled = !string.IsNullOrEmpty(tbFile2.Text);
            }
        }

        private void btnFile2_Click(object sender, EventArgs e)
        {
            openFileDialog.FileName = tbFile2.Text;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                tbFile2.Text = openFileDialog.FileName;
                btnCompare.Enabled = !string.IsNullOrEmpty(tbFile1.Text); 
            }
        }

        private void btnCompare_Click(object sender, EventArgs e)
        {
#if false
            var fileName1 = tbFile1.Text;
            var fileName2 = tbFile2.Text;

            var log = new StringBuilder();
            var dictionary = new Dictionary<string, EnumPair>();
            var differentEnums = new Dictionary<string, EnumPair>();

            if (File.Exists(fileName1)
                && File.Exists(fileName2))
            {
                try
                {
                    var assembly1 = Assembly.LoadFile(fileName1);
                    //var assembly1 = AssemblyDefinition.ReadAssembly(fileName1);
                    log.AppendLine($"Load file {fileName1}");
                    var assembly2 = Assembly.LoadFile(fileName2);
                    //var assembly2 = AssemblyDefinition.ReadAssembly(fileName2);
                    log.AppendLine($"Load file {fileName2}");

                    // Сканируем типы в первой сборке
                    //foreach (var module in assembly1.GetModules())
                    foreach (var module in assembly1.Modules)
                    {
                        foreach (var type1 in module.GetTypes())
                        {
                            if (type1.IsEnum)
                            {
                                var typeName = type1.FullName;

                                if (!string.IsNullOrEmpty(typeName))
                                {
                                    //if(dictionary.TryGetValue(typeName, out EnumPair value))
                                    //{
                                    //    value.Enum1Type = type1;
                                    //}
                                    //else
                                    {
                                        dictionary.Add(typeName, new EnumPair(type1));
                                    }
                                }
                            }
                        }
                    }

                    // Сканируем типы во второй сборке
                    //foreach (var module in assembly2.GetModules())
                    foreach (var module in assembly2.Modules)
                    {
                        foreach (var type2 in module.GetTypes())
                        {
                            if (type2.IsEnum)
                            {
                                var typeName = type2.FullName;

                                if (!string.IsNullOrEmpty(typeName))
                                {
                                    if (dictionary.TryGetValue(typeName, out EnumPair value))
                                    {
                                        value.Enum2Type = type2;
                                    }
                                    else
                                    {
                                        dictionary.Add(typeName, new EnumPair(type2));
                                    }
                                }
                            }
                        }
                    }

                    log.AppendLine($"Total {dictionary.Count} enums was found");
                    log.AppendLine();

                    // Сравниваем типы
                    foreach (var pair in dictionary)
                    {
                        var difference = pair.Value.DifferentElementSimilarity;
                        if (difference.Count > 0)
                        {
                            log.AppendLine($"Enum '{pair.Key}' was changed:");
                            foreach (var tuple in difference)
                            {
                                log.AppendLine($"\t{tuple.Item1}\t{tuple.Item2}\t{tuple.Item3:N3}");
                            }

                            log.AppendLine();

                            differentEnums.Add(pair.Key, pair.Value);
                        }
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    log.AppendLine(exception.ToString());
                }

                var dir = Path.GetDirectoryName(fileName1);

                var logFile = string.IsNullOrEmpty(dir)
                    ? "AssemblyComparer.log"
                    : Path.Combine(dir, "AssemblyComparer.log");

                File.WriteAllText(logFile, log.ToString());
            }
#else
            var fileName1 = tbFile1.Text;
            var fileName2 = tbFile2.Text;

            var log = new StringBuilder();
            var dictionary = new Dictionary<string, EnumDefinitionPair>();
            var differentEnums = new Dictionary<string, EnumDefinitionPair>();

            if (File.Exists(fileName1)
                && File.Exists(fileName2))
            {
                try
                {
                    var assembly1 = AssemblyDefinition.ReadAssembly(fileName1);
                    log.AppendLine($"Load file {fileName1}");
                    var assembly2 = AssemblyDefinition.ReadAssembly(fileName2);
                    log.AppendLine($"Load file {fileName2}");

                    // Сканируем типы в первой сборке
                    foreach (var module in assembly1.Modules)
                    {
                        foreach (var type1 in module.GetTypes())
                        {
                            if (type1.IsEnum)
                            {
                                var typeName = type1.FullName;

                                if (!string.IsNullOrEmpty(typeName))
                                {
                                    //if(dictionary.TryGetValue(typeName, out EnumPair value))
                                    //{
                                    //    value.Enum1Type = type1;
                                    //}
                                    //else
                                    {
                                        dictionary.Add(typeName, new EnumDefinitionPair(type1));
                                    }
                                }
                            }
                        }
                    }

                    // Сканируем типы во второй сборке
                    foreach (var module in assembly2.Modules)
                    {
                        foreach (var type2 in module.GetTypes())
                        {
                            if (type2.IsEnum)
                            {
                                var typeName = type2.FullName;

                                if (!string.IsNullOrEmpty(typeName))
                                {
                                    if (dictionary.TryGetValue(typeName, out EnumDefinitionPair value))
                                    {
                                        value.Enum2Type = type2;
                                    }
                                    else
                                    {
                                        dictionary.Add(typeName, new EnumDefinitionPair(type2));
                                    }
                                }
                            }
                        }
                    }

                    log.AppendLine($"Total {dictionary.Count} enums was found");
                    log.AppendLine();

                    int changed = 0;
                    // Сравниваем типы
                    foreach (var pair in dictionary)
                    {
                        var difference = pair.Value.DifferentElementSimilarity;
                        if (difference.Count > 0)
                        {
                            changed++;
                            log.AppendLine($"Enum '{pair.Key}' was changed:");
                            foreach (var tuple in difference)
                            {
                                if (string.IsNullOrEmpty(tuple.Item1))
                                    log.Append("\tNOTHING");
                                else log.Append($"\t{tuple.Item1}");
                                if (!string.IsNullOrEmpty(tuple.Item2))
                                {
                                    log.Append($"\t{tuple.Item2}\t{tuple.Item3:N3}");
                                    if (!string.IsNullOrEmpty(tuple.Item4))
                                        log.Append($"\t{tuple.Item4}\t{tuple.Item5:N3}");
                                }
                                log.AppendLine();
                            }
                            log.AppendLine();

                            differentEnums.Add(pair.Key, pair.Value);
                        }
                    }
                    if(changed == 0)
                    {
                        log.AppendLine($"No one enum doesn't changed");
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    log.AppendLine(exception.ToString());
                }

                var dir = Path.GetDirectoryName(fileName1);

                var logFile = $"Comparing_{Path.GetFileName(fileName1)}_and_{Path.GetFileName(fileName2)}_Jaccard_{DateTime.Now:yyyy-MM-dd_hh-mm}.log";
                if(!string.IsNullOrEmpty(dir))
                    logFile = Path.Combine(dir, logFile);

                File.WriteAllText(logFile, log.ToString());

                Process.Start(logFile);
            }
#endif
        }
    }
}
