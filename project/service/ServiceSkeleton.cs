using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;

namespace ThoughtWorks.CruiseControl.Service
{
    public class ServiceSkeleton
    {
        private AppRunner runner;
        private const int restartTime = 10;
        private object lockObject = new object();
        private FileSystemWatcher watcher;
        private AppDomain runnerDomain;
        private System.Timers.Timer waitTimer = new System.Timers.Timer(restartTime * 1000);

        public ServiceSkeleton()
        {
            // Initialise the wait timer
            waitTimer.AutoReset = false;
            waitTimer.Elapsed += delegate(object sender, System.Timers.ElapsedEventArgs e)
            {
                lock (lockObject)
                {
                    if (runner == null) RunApplication("File change delay finished");
                }
            };
        }

        public void StopRunner(string reason)
        {
            AppRunner runnerToStop = null;

            // Retrieve the runner in a thread-safe block and then clear it so we are not holding up otherwise processing

            lock (lockObject)
            {
                if (runner != null)
                {
                    runnerToStop = runner;
                    runner = null;
                }
            }

            // If a runner needs to be stopped, do it here
            if (runnerToStop != null)
            {
                runnerToStop.Stop(reason);
                AppDomain.Unload(runnerDomain);
            }
        }

        public void RunApplication(string action)
        {
            if (watcher == null)
            {
                watcher = new FileSystemWatcher(AppDomain.CurrentDomain.BaseDirectory, "*.dll");
                watcher.Changed += delegate(object sender, FileSystemEventArgs e)
                {
                    StopRunner(string.Format(System.Globalization.CultureInfo.CurrentCulture,"One or more DLLs have changed - waiting {0}s", restartTime));
                    waitTimer.Stop();
                    waitTimer.Start();
                };
                watcher.EnableRaisingEvents = true;
                watcher.NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.LastWrite | NotifyFilters.Size;
            }

            // Allow the user to turn shadow-copying off
            var setting = ConfigurationManager.AppSettings["ShadowCopy"] ?? string.Empty;
            var useShadowCopying = !(string.Equals(setting, "off", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(setting, "false", StringComparison.OrdinalIgnoreCase));
            try
            {
                this.runnerDomain = CreateNewDomain(useShadowCopying);
            }
            catch (FileLoadException)
            {
                // Unable to use shadow-copying (no user profile?), therefore turn off shadow-copying
                useShadowCopying = false;
                this.runnerDomain = CreateNewDomain(useShadowCopying);
            }
            runner = runnerDomain.CreateInstanceFromAndUnwrap(Assembly.GetExecutingAssembly().Location,
                typeof(AppRunner).FullName) as AppRunner;
            try
            {
                runner.Run(action, useShadowCopying);
            }
            catch (SerializationException)
            {
                var configFilename = ConfigurationManager.AppSettings["ccnet.config"];
                configFilename = string.IsNullOrEmpty(configFilename) ? Path.Combine(Environment.CurrentDirectory, "ccnet.log") : configFilename;
                throw new ApplicationException(
                    string.Format(System.Globalization.CultureInfo.CurrentCulture,"A fatal error has occurred while starting CCService. Please check '{0}' for any details.", configFilename));
            }
        }

        /// <summary>
        /// Creates the new runner domain.
        /// </summary>
        /// <param name="useShadowCopying">If set to <c>true</c> shadow copying will be used.</param>
        /// <returns>The new <see cref="AppDomain"/>.</returns>
        private AppDomain CreateNewDomain(bool useShadowCopying)
        {
            return AppDomain.CreateDomain(
                "CC.Net",
                null,
                AppDomain.CurrentDomain.BaseDirectory,
                AppDomain.CurrentDomain.RelativeSearchPath,
                useShadowCopying);
        }

        public void OnStart()
        {
            RunApplication("SCM start");
        }

        public void OnStop()
        {
            StopRunner("Service is stopped");
        }

        public void OnPause()
        {
            StopRunner("Service is paused");
        }

        public void OnContinue()
        {
            RunApplication("SCM continue");
        }
    }
}
