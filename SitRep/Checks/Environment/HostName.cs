using SitRep.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using static SitRep.Enums.Enums;

namespace SitRep.Checks.Environment
{
    class HostName : CheckBase, ICheck
    {
        public bool IsOpsecSafe => true;

        public int DisplayOrder => 2;
        public CheckType CheckType => CheckType.Environment;

        public void Check()
        {
            try
            {
                var strHostName = Dns.GetHostName();
                Message = strHostName;
            }
            catch
            {
                Message = "Check failed [*]";
            }
        }

        public override string ToString()
        {
            return string.Format("Hostname: {0}", Message);
        }
    }
}
