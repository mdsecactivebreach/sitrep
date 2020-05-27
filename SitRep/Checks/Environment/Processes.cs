using SitRep.Interfaces;
using System;
using System.Text;
using static SitRep.Enums.Enums;
using SitRep.NativeMethods;
using System.Diagnostics;
using System.Security.Principal;

namespace SitRep.Checks.Credentials
{
    class TestCheck : CheckBase, ICheck
    {
        public int DisplayOrder => 10;

        public bool IsOpsecSafe => false;

        public CheckType CheckType => CheckType.Environment;

        public void Check()
        {
            var builder = new StringBuilder();
            var allProcesses = Process.GetProcesses();
            string user;
            try
            {
                builder.AppendLine($"\t{"Process ID",10}\t{"Session ID",10}\t{"Process Owner",10}\t{"Process Name",10}");

                foreach (Process p in allProcesses)
                {
                    user = GetProcessOwner(p);
                    builder.AppendLine(string.Format($"\t{p.Id,-10}\t{p.SessionId,-10}\t{user,-10}\t{p.ProcessName,-10}"));
                }

                Message = builder.ToString();
            }
            catch
            {
                Message = "\tCheck failed [*]";
            }
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendLine("Running Processes:");
            builder.AppendLine(Message);
            return builder.ToString().Trim();
        }

        // Thanks bytecode77 @ https://stackoverflow.com/a/38676215
        private static string GetProcessOwner(Process process)
        {
            IntPtr processHandle = IntPtr.Zero;
            try
            {
                advapi32.OpenProcessToken(process.Handle, 8, out processHandle);
                WindowsIdentity wi = new WindowsIdentity(processHandle);
                string user = wi.Name;
                return user.Contains(@"\") ? user.Substring(user.IndexOf(@"\") + 1) : user;
            }
            catch
            {
                return null;
            }
            finally
            {
                if (processHandle != IntPtr.Zero)
                {
                    kernel32.CloseHandle(processHandle);
                }
            }
        }
    }
}
