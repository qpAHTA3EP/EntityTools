using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.Windows.Forms;
using ACTP0Tools.Patches;
using ACTP0Tools.Reflection;
using Astral.Quester.Classes.Actions;
using PathHelper = ACTP0Tools.Classes.FileTools;
using QuesterEditor = EntityTools.Quester.Editor.QuesterEditor;

namespace EntityTools.Editors
{
    internal class RelativeProfileFilePathEditor : UITypeEditor
    {
        private static PropertyAccessor<PropertyGrid> pgAccessor;

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            var instance = context?.Instance;
            var type = instance?.GetType();
            if (type == ACTP0Serializer.PushProfileToStackAndLoad
                || type == typeof(LoadProfile))
            {
                string profileFile = string.Empty;

                if (pgAccessor is null)
                    pgAccessor = context.GetProperty<PropertyGrid>("OwnerGrid");

                if (pgAccessor.IsValid)
                {
                    if (pgAccessor.Value?.ParentForm is QuesterEditor questerEditor)
                    {
                        profileFile = questerEditor.Profile.ProfilePath;   
                    }
                }

                string astralProfileDir = Astral.Controllers.Directories.ProfilesPath;
                string relativeTargetProfilePath = value?.ToString() ?? string.Empty;
                string fullProfilePath = string.Empty;
                string fileName = string.Empty;
                string initialDir;
                if (string.IsNullOrEmpty(profileFile))
                {
                    if (string.IsNullOrEmpty(relativeTargetProfilePath))
                        initialDir = astralProfileDir;
                    else
                    {
                        var fullTargetFile = Path.GetFullPath(Path.Combine(astralProfileDir, relativeTargetProfilePath));
                        fileName = Path.GetFileName(fullTargetFile);
                        initialDir = Path.GetDirectoryName(fullTargetFile) ?? astralProfileDir;
                    }
                }
                else
                {
                    fullProfilePath = Path.GetFullPath(profileFile);
                    if (string.IsNullOrEmpty(relativeTargetProfilePath))
                    {
                        initialDir = Path.GetDirectoryName(fullProfilePath);
                    }
                    else
                    {
                        string profileDir = Path.GetDirectoryName(fullProfilePath) ?? astralProfileDir;
                        string fullTargetFile = Path.GetFullPath(Path.Combine(profileDir, relativeTargetProfilePath));
                        fileName = Path.GetFileName(fullTargetFile);
                        initialDir = Path.GetDirectoryName(fullTargetFile) ?? astralProfileDir;

                    }
                }

                var openDialog = PathHelper.GetOpenDialog(fileName,
                                                          initialDir,
                                                          filter: "Profile (*.amp.zip)|*.amp.zip",
                                                          defaultExtension: "amp.zip");
                if (openDialog.ShowDialog() == DialogResult.OK)
                {
                    if (string.IsNullOrEmpty(fullProfilePath))
                        return PathHelper.GetRelativePathTo(Path.Combine(astralProfileDir, "profile"), openDialog.FileName);
                    return PathHelper.GetRelativePathTo(fullProfilePath, openDialog.FileName);
                }
            }

            return value;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
    }
}
