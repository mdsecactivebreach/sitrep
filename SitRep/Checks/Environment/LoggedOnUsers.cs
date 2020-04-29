using SitRep.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Text.RegularExpressions;
using static SitRep.Enums.Enums;

namespace SitRep.Checks.Environment
{
    class LoggedOnUsers : CheckBase, ICheck
    {
        public bool IsOpsecSafe => true;

        public int DisplayOrder => 3;

        public CheckType CheckType => CheckType.Environment;

        public void Check()
        {
            try
            {
                var builder = new StringBuilder();
                var userList = new List<string>();
                var userDomainRegex = new Regex(@"Domain=""(.*)"",Name=""(.*)""");
                var logonIdRegex = new Regex(@"LogonId=""(\d+)""");
                //doing this via the win32 api killed execute-assembly. using the Seatbelt WMI query method instead
                var wmiData = new ManagementObjectSearcher(@"root\cimv2", "SELECT * FROM Win32_LoggedOnUser");
                var data = wmiData.Get();

                foreach (var result in data)
                {
                    var m = logonIdRegex.Match(result["Dependent"].ToString());
                    if (m.Success)
                    {
                        var logonID = m.Groups[1].ToString();
                        var m2 = userDomainRegex.Match(result["Antecedent"].ToString());
                        if (m2.Success)
                        {
                            var domain = m2.Groups[1].ToString();
                            var user = m2.Groups[2].ToString();
                            userList.Add("\t" + domain + "\\" + user);
                        }
                    }
                }
                //remove duplicates
                userList.Distinct().ToList().ForEach(x => builder.AppendLine(x));
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
            builder.AppendLine("Logged on users:");
            builder.AppendLine(Message);
            return builder.ToString().Trim();
        }
    }
}
