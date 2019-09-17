using System.Reflection;

namespace JustinCredible.SIEmulator
{
    public class Utilities
    {
        public static string AppVersion
        {
            get
            {
                return Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
            }
        }
    }
}
