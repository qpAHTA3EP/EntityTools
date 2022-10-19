using ACTP0Tools.Classes.Quester;
using ACTP0Tools.Patches;
using ACTP0Tools.Reflection;
using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.Windows.Forms;
using PathHelper = ACTP0Tools.Classes.FileTools;

namespace EntityTools.Editors
{
    class RelativeProfileFilePathEditor : UITypeEditor
    {
        private static PropertyAccessor<PropertyGrid> pgAccessor;
        private static PropertyAccessor<QuesterProfileProxy> profileProxyAccessor;
        private static PropertyAccessor<string> profileNameAccessor;
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            var instance = context?.Instance;
            if (instance?.GetType() == ACTP0Serializer.PushProfileToStackAndLoad)
            {
                //string fullProfilePath = string.Empty;

                //string profileDir = string.Empty;
                //string astralProfilePath = Astral.Controllers.Directories.ProfilesPath;
                
                QuesterProfileProxy questerProfile = null;
                string profileFile = string.Empty;

                if (profileNameAccessor is null)
                    profileNameAccessor = ACTP0Serializer.PushProfileToStackAndLoad.GetProperty<string>("ProfileName");

                if (pgAccessor is null)
                    pgAccessor = context.GetProperty<PropertyGrid>("OwnerGrid");

                if (pgAccessor.IsValid)
                {
                    var parentForm = pgAccessor.Value?.ParentForm;
                    if (parentForm != null)
                    {
                        if (profileProxyAccessor is null)
                            profileProxyAccessor = parentForm.GetProperty<QuesterProfileProxy>("Profile");
                        if (profileProxyAccessor.IsValid)
                        {
                            questerProfile = profileProxyAccessor.Value;
                            profileFile = questerProfile.FileName;
                        }
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
