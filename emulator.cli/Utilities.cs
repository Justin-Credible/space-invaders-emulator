using System.Reflection;

namespace JustinCredible.SIEmulator.CLI
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
