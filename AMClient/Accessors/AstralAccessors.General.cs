using AcTp0Tools.Reflection;
using Astral;
using Astral.Functions;

namespace AcTp0Tools
{
    public static partial class AstralAccessors
    {
        public static class General
        {
            public static Credentials Credentials => _credentials.Value;

            private static readonly StaticFieldAccessor<Credentials> _credentials;

            static General()
            {
                var tGeneral = ReflectionHelper.GetTypeByName("\u0018.\u0004", true);

                if (tGeneral is null)
                {
                    Logger.WriteLine(Logger.LogType.Debug, "Credentials container class not found");
                    return;
                }

                _credentials = tGeneral.GetStaticField<Credentials>("\u0002");

                if(!_credentials.IsValid)
                    Logger.WriteLine(Logger.LogType.Debug, "Fail to access to field 'Credentials'");
            }
        }
    }
}
