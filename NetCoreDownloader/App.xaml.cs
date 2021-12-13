using System.Net;
using System.Windows;

namespace NetCoreDownloader
{
    public partial class App : Application
    {
    }
    
    
        private static Architecture GetPlatform()
        {
            if (RuntimeInformation.ProcessArchitecture == Architecture.Arm64)
            {
                return Architecture.Arm64;
            }
            else if (Environment.Is64BitOperatingSystem)
            {
                return Architecture.X64;
            }
            else
            {
                return Architecture.X86;
            }
        }
}
