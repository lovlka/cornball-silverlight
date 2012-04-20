using System.Diagnostics;
using System.IO;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace Cornball.Web.Azure
{
    public class AzureLocalStorageTraceListener : XmlWriterTraceListener
    {
        public AzureLocalStorageTraceListener()
            : base(Path.Combine(GetLogDirectory().Path, "Cornball.Service.svclog"))
        {
        }

        public static DirectoryConfiguration GetLogDirectory()
        {
            var directory = new DirectoryConfiguration
                                {
                                    Container = "wad-tracefiles",
                                    DirectoryQuotaInMB = 10,
                                    Path = RoleEnvironment.GetLocalResource("Cornball.Service.svclog").RootPath
                                };
            return directory;
        }
    }
}