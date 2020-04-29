using SitRep.Interfaces;
using System;
using System.Linq;
using System.Text;
using System.Net;
using System.DirectoryServices.AccountManagement;

namespace SitRep.Checks.Permissions
{
    class ComputerDomainGroups : CheckBase, ICheck
    {
        public bool IsOpsecSafe => false;

        public int DisplayOrder => 5;

        public Enums.Enums.CheckType CheckType => Enums.Enums.CheckType.Permissions;

        public void Check()
        {
            try
            {
                var builder = new StringBuilder();
                var domainName = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName;
                if (string.IsNullOrWhiteSpace(domainName))
                {
                    Message = "\tNot domain joined";
                    return;
                }

                ComputerPrincipal principal = ComputerPrincipal.FindByIdentity(new PrincipalContext(ContextType.Domain, domainName), Dns.GetHostName());
                foreach (GroupPrincipal group in principal.GetGroups())
                {
                    builder.AppendLine(string.Format("\t{0}", group));
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
            builder.AppendLine("Computer domain groups:");
            builder.AppendLine(Message);
            return builder.ToString().Trim();
        }
    }
}
