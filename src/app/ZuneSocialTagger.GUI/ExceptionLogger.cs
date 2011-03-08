using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace ZuneSocialTagger.GUI
{
    /// <summary>
    /// Class to log unhandled exceptions
    /// </summary>
    public class ExceptionLogger
    {
        public static string LogException(Exception exception)
        {
            var error = new StringBuilder();

            error.AppendLine("Application:       " + Application.ProductName);
            error.AppendLine("Version:           " + Application.ProductVersion);
            error.AppendLine("Date:              " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
            error.AppendLine("Computer name:     " + SystemInformation.ComputerName);
            error.AppendLine("User name:         " + SystemInformation.UserName);
            error.AppendLine("OS:                " + Environment.OSVersion);
            error.AppendLine("Culture:           " + CultureInfo.CurrentCulture.Name);
            error.AppendLine("System up time:    " + GetSystemUpTime());
            error.AppendLine("App up time:       " + (DateTime.Now - Process.GetCurrentProcess().StartTime));

            var memStatus = new MEMORYSTATUSEX();
            if (GlobalMemoryStatusEx(memStatus))
            {
                error.AppendLine("Total memory:      " + memStatus.ullTotalPhys / (1024 * 1024) + "Mb");
                error.AppendLine("Available memory:  " + memStatus.ullAvailPhys / (1024 * 1024) + "Mb");
            }

            error.AppendLine("");

            error.AppendLine("Exception classes:   ");
            error.Append(GetExceptionTypeStack(exception));
            error.AppendLine("");
            error.AppendLine("Exception messages: ");
            error.Append(GetExceptionMessageStack(exception));

            error.AppendLine("");
            error.AppendLine("Stack Traces:");
            error.Append(GetExceptionCallStack(exception));
            error.AppendLine("");
            error.AppendLine("Loaded Modules:");
            Process thisProcess = Process.GetCurrentProcess();
            foreach (ProcessModule module in thisProcess.Modules)
            {
                error.AppendLine(module.FileName + " " + module.FileVersionInfo.FileVersion);
            }

            return error.ToString();
        }

        private static string GetExceptionTypeStack(Exception e)
        {
            if (e.InnerException != null)
            {
                var message = new StringBuilder();
                message.AppendLine(GetExceptionTypeStack(e.InnerException));
                message.AppendLine("   " + e.GetType());
                return (message.ToString());
            }

            return "   " + e.GetType();
        }

        private static string GetExceptionMessageStack(Exception e)
        {
            if (e.InnerException != null)
            {
                var message = new StringBuilder();
                message.AppendLine(GetExceptionMessageStack(e.InnerException));
                message.AppendLine("   " + e.Message);
                return (message.ToString());
            }

            return "   " + e.Message;
        }

        private static string GetExceptionCallStack(Exception e)
        {
            if (e.InnerException != null)
            {
                var message = new StringBuilder();
                message.AppendLine(GetExceptionCallStack(e.InnerException));
                message.AppendLine("--- Next Call Stack:");
                message.AppendLine(e.StackTrace);
                return (message.ToString());
            }

            return e.StackTrace;
        }

        private static TimeSpan GetSystemUpTime()
        {
            var upTime = new PerformanceCounter("System", "System Up Time");
            upTime.NextValue();
            return TimeSpan.FromSeconds(upTime.NextValue());
        }

        // use to get memory available
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private class MEMORYSTATUSEX
        {
            public uint dwLength;
            public uint dwMemoryLoad;
            public ulong ullTotalPhys;
            public ulong ullAvailPhys;
            public ulong ullTotalPageFile;
            public ulong ullAvailPageFile;
            public ulong ullTotalVirtual;
            public ulong ullAvailVirtual;
            public ulong ullAvailExtendedVirtual;

            public MEMORYSTATUSEX()
            {
                this.dwLength = (uint)Marshal.SizeOf(typeof(MEMORYSTATUSEX));
            }
        }

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool GlobalMemoryStatusEx([In, Out] MEMORYSTATUSEX lpBuffer);
    }
}


