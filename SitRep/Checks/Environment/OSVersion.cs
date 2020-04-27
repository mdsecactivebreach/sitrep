using SitRep.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;

namespace SitRep.Checks.Environment
{
    class OSVersion : CheckBase, ICheck
    {
        public bool IsOpsecSafe => true;

        public int DisplayOrder => 7;

        public Enums.Enums.CheckType CheckType => Enums.Enums.CheckType.Environment;

        public void Check()
        {
            var builder = new StringBuilder();
            using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem"))
            {
                var information = searcher.Get();
                if (information != null)
                {
                    foreach (var obj in information)
                    {
                        builder.AppendLine(obj["Caption"].ToString() + " - " + obj["OSArchitecture"].ToString());
                    }
                }
         
            }
            Message = builder.ToString();
        }

        public override string ToString()
        {
            return string.Format("OS Version: {0}", Message);
        }
    }
}
