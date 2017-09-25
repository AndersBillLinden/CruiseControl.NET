using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.ServiceProcess;

namespace ThoughtWorks.CruiseControl.Service
{
    public class CCService : ServiceBase
    {
        private ServiceSkeleton skeleton;
        public const string DefaultServiceName = "CCService";

        public CCService()
        {
            if (string.Equals(ConfigurationManager.AppSettings["DebugCCService"], "yes", StringComparison.OrdinalIgnoreCase))
            {
                Debugger.Launch();
            }
            ServiceName = LookupServiceName();
        }

        protected override void OnStart(string[] args)
        {
            skeleton.OnStart();
        }

        // Should this be stop or abort?
        protected override void OnStop()
        {
            skeleton.OnStop();
        }

        protected override void OnPause()
        {
            skeleton.OnPause();
        }

        protected override void OnContinue()
        {
            skeleton.OnContinue();
        }

        private static string LookupServiceName()
        {
            string serviceName = ConfigurationManager.AppSettings["service.name"];
            return string.IsNullOrEmpty(serviceName) ? DefaultServiceName : serviceName;
        }

        private static void Main()
        {
            AllocateWin32Console();
            Run(new ServiceBase[] { new CCService() });
        }

        // Allocates a Win32 console if needed since Windows does not provide
        // one to Services by default. Normally that's okay, but we will be
        // launching console applications and they may fail unless the parent
        // process supplies them with a console.
        private static void AllocateWin32Console()
        {
            if (IsRunningOnWindows) AllocConsole();
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool AllocConsole();

        private static bool IsRunningOnWindows
        {
            get
            {
                // mono returns 128 when running on linux, .NET 2.0 returns 4
                // see http://www.mono-project.com/FAQ:_Technical
                int platform = (int)Environment.OSVersion.Platform;
                return ((platform != 4) && (platform != 128));
            }
        }
    }
}