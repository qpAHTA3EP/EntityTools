using System;
using System.ComponentModel;
using System.Reflection;
using Astral.Quester.Classes;

namespace ACTP0Tools.Classes.Quester
{
    /// <summary>
    /// Синглтон прокси-объекта, опосредующего доступ к активному загруженному профилю Quester'a: <br/>
    /// <see cref="AstralAccessors.Quester.Core.Profile"/>
    /// </summary>
    internal class ActiveProfileProxy : BaseQuesterProfileProxy
    {
        private MethodInfo engineSetProfile;
        
        internal ActiveProfileProxy(MethodInfo engineSetProfile)
        {
            if (engineSetProfile is null)
                throw new ArgumentNullException(nameof(engineSetProfile));
            this.engineSetProfile = engineSetProfile;
        }
        protected override Profile RealProfile 
        { 
            get => Astral.Quester.API.CurrentProfile;
#if false
            set
            {
                var currentProfile = Astral.Quester.API.CurrentProfile;
                if (value != null
                    && !ReferenceEquals(currentProfile, value))
                {
                    AstralAccessors.Quester.Core.SetProfileToEngine(value);
                }
            } 
#endif
        }

#if DEBUG
        [Browsable(true)]
        [Category("File")]
#else
        [Browsable(false)] 
#endif
        public override string ProfilePath
        {
            get => Astral.API.CurrentSettings.LastQuesterProfile;
            protected set
            {
                if (!string.Equals(Astral.API.CurrentSettings.LastQuesterProfile, value, System.StringComparison.OrdinalIgnoreCase))
                {
                    Astral.API.CurrentSettings.LastQuesterProfile = value;
                    OnPropertyChanged(); 
                    ResetCachedMeshes();
                }
            }
        }

        public override void SetProfile(Profile profile, string newProfileFilename)
        {
            var currentProfile = Astral.Quester.API.CurrentProfile;
            if (profile is null
                || ReferenceEquals(currentProfile, profile))
                return;

#if false
            if (AstralAccessors.Controllers.Roles.IsRunning)
                AstralAccessors.Controllers.Roles.ToggleRole(true); 
#endif

            engineSetProfile.Invoke(null, new[] { profile });
            Astral.API.CurrentSettings.LastQuesterProfile = newProfileFilename;

            AssignInternals(profile);

            if (AstralAccessors.Controllers.BotComs.BotServer.Server.IsRunning)
            {
                AstralAccessors.Controllers.BotComs.BotServer.SendQuesterProfileInfos();
            }

            AstralAccessors.Quester.Entrypoint.RefreshQuesterMainPanel();
        }
    }
}
