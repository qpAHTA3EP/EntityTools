using ACTP0Tools;
using ACTP0Tools.Classes.Quester;
using ACTP0Tools.Patches;
using ACTP0Tools.Reflection;
using EntityCore.Forms;
using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.Windows.Forms;
using FileTools = ACTP0Tools.Classes.FileTools;

namespace EntityCore.Editors
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
                string relativeTargetProfilePath = value?.ToString() ?? string.Empty;
                string fullProfilePath = string.Empty;
                string profileDir = string.Empty;
                string astralProfilePath = Astral.Controllers.Directories.ProfilesPath;
                QuesterProfileProxy questerProfile = null;

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
                            var profilePath = questerProfile.FileName;
                            fullProfilePath = Path.GetFullPath(Path.Combine(Astral.Controllers.Directories.AstralStartupPath,
                                                                            profilePath));
                            profileDir = string.IsNullOrEmpty(fullProfilePath)
                                       ? astralProfilePath
                                       : Path.GetDirectoryName(fullProfilePath);
                        }
                    }
                }

                if (string.IsNullOrEmpty(profileDir))
                {
                    var lastProfilePath = Astral.API.CurrentSettings.LastQuesterProfile;
                    if(!string.IsNullOrEmpty(lastProfilePath))
                        profileDir = Path.GetDirectoryName(Path.GetFullPath(Path.Combine(Astral.Controllers.Directories.AstralStartupPath, lastProfilePath)));
                }

                if (string.IsNullOrEmpty(profileDir))
                    profileDir = astralProfilePath;

                var targetProfilePath = Path.GetFullPath(Path.Combine(profileDir, relativeTargetProfilePath));

                if (questerProfile is null)
                    questerProfile = AstralAccessors.Quester.Core.ActiveProfileProxy;

                var openDialog = FileTools.GetOpenDialog(fileName: Path.GetFileName(targetProfilePath),
                                                         initialDir: Path.GetDirectoryName(targetProfilePath) ?? astralProfilePath,
                                                         filter: "Profile (*.amp.zip)|*.amp.zip",
                                                         defaultExtension: "amp.zip");
                if (openDialog.ShowDialog() == DialogResult.OK)
                {
                    return FileTools.GetRelativePathTo(fullProfilePath, openDialog.FileName);
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
