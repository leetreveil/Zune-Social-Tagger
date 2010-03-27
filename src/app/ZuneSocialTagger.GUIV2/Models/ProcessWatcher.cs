using System;
using System.Management;

namespace ZuneSocialTagger.GUIV2.Models
{
    public class ProcessWatcher : IDisposable
    {
        private readonly string _processName;
        public event Action<string> ProcessStarted = delegate { };
        public event Action<string> ProcessEnded = delegate { };

        public ProcessWatcher(string processName)
        {
            _processName = processName;
        }

        public ManagementEventWatcher WatchForProcessStart()
        {
            string queryString =
                "SELECT TargetInstance" +
                "  FROM __InstanceCreationEvent " +
                "WITHIN  10 " +
                " WHERE TargetInstance ISA 'Win32_Process' " +
                "   AND TargetInstance.Name = '" + _processName + "'";

            // The dot in the scope means use the current machine
            string scope = @"\\.\root\CIMV2";

            // Create a watcher and listen for events
            ManagementEventWatcher watcher = new ManagementEventWatcher(scope, queryString);
            watcher.EventArrived += ProcessStartedEv;
            watcher.Start();
            return watcher;
        }

        public ManagementEventWatcher WatchForProcessEnd()
        {
            string queryString =
                "SELECT TargetInstance" +
                "  FROM __InstanceDeletionEvent " +
                "WITHIN  10 " +
                " WHERE TargetInstance ISA 'Win32_Process' " +
                "   AND TargetInstance.Name = '" + _processName + "'";

            // The dot in the scope means use the current machine
            string scope = @"\\.\root\CIMV2";

            // Create a watcher and listen for events
            ManagementEventWatcher watcher = new ManagementEventWatcher(scope, queryString);
            watcher.EventArrived += ProcessEndedEv;
            watcher.Start();
            return watcher;
        }

        private void ProcessEndedEv(object sender, EventArrivedEventArgs e)
        {
            ManagementBaseObject targetInstance = (ManagementBaseObject)e.NewEvent.Properties["TargetInstance"].Value;
            string processName = targetInstance.Properties["Name"].Value.ToString();
            ProcessEnded.Invoke(processName);
        }

        private void ProcessStartedEv(object sender, EventArrivedEventArgs e)
        {
            ManagementBaseObject targetInstance = (ManagementBaseObject)e.NewEvent.Properties["TargetInstance"].Value;
            string processName = targetInstance.Properties["Name"].Value.ToString();
            ProcessStarted.Invoke(processName);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}