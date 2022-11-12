using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.Windows.Forms;
using ACTP0Tools.Reflection;
using FileTools = ACTP0Tools.Classes.FileTools;
using QuesterEditor = EntityTools.Quester.Editor.QuesterEditor;

namespace EntityTools.Editors
{
    internal class RelativeMeshesFilePathEditor : UITypeEditor
    {
        private PropertyAccessor<PropertyGrid> pgAccessor;

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            string profileFile = string.Empty;

            if (pgAccessor is null)
                pgAccessor = context.GetProperty<PropertyGrid>("OwnerGrid");

            if (pgAccessor.IsValid)
            {
                if (pgAccessor.Value?.ParentForm is QuesterEditor questerEditor)
                {
                    profileFile = questerEditor.Profile.FileName;
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
                    var profileDir = Path.GetDirectoryName(fullProfilePath) ?? astralProfileDir;
                    var fullTargetFile = Path.GetFullPath(Path.Combine(profileDir, relativeTargetProfilePath));
                    fileName = Path.GetFileName(fullTargetFile);
                    initialDir = Path.GetDirectoryName(fullTargetFile) ?? astralProfileDir;
                }
            }

            var openDialog = FileTools.GetOpenDialog(fileName,
                                                     initialDir,
                                                     filter: "Maps meshes (*.mesh.zip)|*.mesh.zip",
                                                     defaultExtension: "mesh.zip");
            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                if (string.IsNullOrEmpty(fullProfilePath))
                    return FileTools.GetRelativePathTo(Path.Combine(astralProfileDir, "profile"), openDialog.FileName);
                return FileTools.GetRelativePathTo(fullProfilePath, openDialog.FileName);
            }

            return value;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
    }
}
