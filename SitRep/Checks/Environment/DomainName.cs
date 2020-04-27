using SitRep.Interfaces;
using System;
using System.Net.NetworkInformation;
using static SitRep.Enums.Enums;

namespace SitRep.Checks.Environment
{
    class DomainName : CheckBase, ICheck
    {
        public bool IsOpsecSafe => true;

        public int DisplayOrder => 1;
        public CheckType CheckType => CheckType.Environment;

        public void Check()
        {
            IPGlobalProperties properties = IPGlobalProperties.GetIPGlobalProperties();
            string dnsDomain = properties.DomainName;
            Message = string.IsNullOrEmpty(dnsDomain) ? "NOT DOMAIN JOINED [*]" : dnsDomain;
        }

        public override string ToString()
        {
            return string.Format("Domain Name: {0}", Message);
        }
    }
}
