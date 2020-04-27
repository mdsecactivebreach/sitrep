using SitRep.Interfaces;
using SitRep.NativeMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;

namespace SitRep.Checks.Permissions
{
    class UserDomainGroups : CheckBase, ICheck
    {
        public bool IsOpsecSafe => false;

        public int DisplayOrder => 4;

        public Enums.Enums.CheckType CheckType => Enums.Enums.CheckType.Permissions;

        public void Check()
        {
            //First, check if we are domain joined. otherwise, we dont want to run this code...
            if (!IsDomainJoined())
            {
                Message = "\tHost is not domain joined";
                return;
            }
            var builder = new StringBuilder();
            //adapted from https://stackoverflow.com/questions/5309988/how-to-get-the-groups-of-a-user-in-active-directory-c-asp-net
            var groups = new List<string>();
            var UPN = System.DirectoryServices.AccountManagement.UserPrincipal.Current.UserPrincipalName;
            var wi = new WindowsIdentity(UPN);

            foreach (var group in wi.Groups)
            {
                try
                {
                    groups.Add(group.Translate(typeof(NTAccount)).ToString());
                }
                catch { }
            }
            groups.Sort();
            foreach (var group in groups)
            {
                builder.AppendLine("\t" + group);
            }
            Message = builder.ToString();
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendLine("User Domain Groups:");
            builder.AppendLine(Message);
            return builder.ToString();
        }

        private bool IsDomainJoined()
        {
            //adapted from https://stackoverflow.com/questions/926227/how-to-detect-if-machine-is-joined-to-domain
            var status = advapi32.NetJoinStatus.NetSetupUnknownStatus;
            var pDomain = IntPtr.Zero;
            var result = advapi32.NetGetJoinInformation(null, out pDomain, out status);
            if (pDomain != IntPtr.Zero)
            {
                advapi32.NetApiBufferFree(pDomain);
            }
            if (result == advapi32.ErrorSuccess)
            {
                return status == advapi32.NetJoinStatus.NetSetupDomainName;
            }
            else
            {
                return false;
            }

        }
    }
}
