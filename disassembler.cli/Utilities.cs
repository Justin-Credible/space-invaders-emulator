using System.Reflection;

namespace JustinCredible.I8080Disassembler.CLI
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
