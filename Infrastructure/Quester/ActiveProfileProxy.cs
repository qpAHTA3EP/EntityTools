using System;
using System.ComponentModel;
using System.Reflection;
using Astral.Quester.Classes;

namespace Infrastructure.Quester
{
    /// <summary>
    /// Синглтон прокси-объекта, опосредующего доступ к активному загруженному профилю Quester'a: <br/>
    /// <see cref="Astral.Quester.API.CurrentProfile"/>
    /// </summary>
    public class ActiveProfileProxy : BaseQuesterProfileProxy
    {
        private readonly MethodInfo engineSetProfile;

        internal ActiveProfileProxy(MethodInfo engineSetProfile)
        {
            this.engineSetProfile = engineSetProfile
                                 ?? throw new ArgumentNullException(nameof(engineSetProfile));
        }
        protected override Profile RealProfile => Astral.Quester.API.CurrentProfile;

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

            ResetMeshesCache();
            AssignInternals(profile);

            if (AstralAccessors.Controllers.BotComs.BotServer.Server.IsRunning)
            {
                AstralAccessors.Controllers.BotComs.BotServer.SendQuesterProfileInfos();
            }

            AstralAccessors.Quester.Entrypoint.RefreshQuesterMainPanel();
        }
    }
}
