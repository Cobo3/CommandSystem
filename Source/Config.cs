using System.Collections.Generic;

namespace SickDev.CommandSystem{
    public static class Config{
        public static List<string> dllsToExclude{
            get { return _dllsToExclude; }
        }

        static List<string> _dllsToExclude = new List<string> {
           "mscorlib.dll",
           "Microsoft.VisualStudio.HostingProcess.Utilities.dll",
           "System.Windows.Forms.dll",
           "System.dll",
           "System.Drawing.dll",
           "Microsoft.VisualStudio.HostingProcess.Utilities.Sync.dll",
           "Microsoft.VisualStudio.Debugger.Runtime.dll",
           "mscorlib.dll",
           "System.Core.dll",
           "System.Configuration.dll",
           "System.Xml.dll"
        };
    }
}
