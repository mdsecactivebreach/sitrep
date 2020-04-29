using SitRep.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;

namespace SitRep.Checks.Environment
{
    class NameServers : CheckBase, ICheck
    {
        public bool IsOpsecSafe => true;

        public int DisplayOrder => 9;

        public Enums.Enums.CheckType CheckType => Enums.Enums.CheckType.Environment;

        public void Check()
        {
            try
            {
                Message = "No network interfaces found [*]";
                var builder = new StringBuilder();
                var adapters = NetworkInterface.GetAllNetworkInterfaces();
                foreach (var adapter in adapters)
                {
                    var adapterProperties = adapter.GetIPProperties();
                    var dnsServers = adapterProperties.DnsAddresses;
                    if (dnsServers.Count > 0)
                    {
                        builder.AppendLine(string.Format("\t{0}", adapter.Description));
                        foreach (var dns in dnsServers)
                        {
                            builder.AppendLine(string.Format("\t\t{0}", dns.ToString()));
                        }
                    }
                }
                Message = builder.ToString();
            }
            catch
            {
                Message = "Check failed [*]";
            }
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendLine("Nameservers:");
            builder.AppendLine(Message);
            return builder.ToString().Trim();
        }
    }
}
