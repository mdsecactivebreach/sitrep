using SitRep.Interfaces;
using SitRep.NativeMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;

namespace SitRep.Checks.Permissions
{
    class LocalAdmin : CheckBase, ICheck
    {
        public bool IsOpsecSafe => true;

        public int DisplayOrder => 0;

        public Enums.Enums.CheckType CheckType => Enums.Enums.CheckType.Permissions;

        public void Check()
        {
            Message = "Not a local admin";
            
            //adapted from Seatbelt
            //checks if the "S-1-5-32-544" in the current token groups set, meaning the user is a local administrator
            string[] SIDs = GetTokenGroupSIDs();

            foreach (string SID in SIDs)
            {
                if (SID == "S-1-5-32-544")
                {
                    Message = "Current user is a Local admin [*]";
                }
            }
        }

        private string[] GetTokenGroupSIDs()
        {
            // Returns all SIDs that the current user is a part of, whether they are disabled or not.
            // slightly adapted from https://stackoverflow.com/questions/2146153/how-to-get-the-logon-sid-in-c-sharp/2146418#2146418

            int TokenInfLength = 0;

            // first call gets length of TokenInformation
            bool Result = advapi32.GetTokenInformation(WindowsIdentity.GetCurrent().Token, advapi32.TOKEN_INFORMATION_CLASS.TokenGroups, IntPtr.Zero, TokenInfLength, out TokenInfLength);
            IntPtr TokenInformation = Marshal.AllocHGlobal(TokenInfLength);
            Result = advapi32.GetTokenInformation(WindowsIdentity.GetCurrent().Token, advapi32.TOKEN_INFORMATION_CLASS.TokenGroups, TokenInformation, TokenInfLength, out TokenInfLength);

            if (!Result)
            {
                Marshal.FreeHGlobal(TokenInformation);
                return null;
            }

            advapi32.TOKEN_GROUPS groups = (advapi32.TOKEN_GROUPS)Marshal.PtrToStructure(TokenInformation, typeof(advapi32.TOKEN_GROUPS));
            string[] userSIDS = new string[groups.GroupCount];
            int sidAndAttrSize = Marshal.SizeOf(new advapi32.SID_AND_ATTRIBUTES());
            for (int i = 0; i < groups.GroupCount; i++)
            {
                advapi32.SID_AND_ATTRIBUTES sidAndAttributes = (advapi32.SID_AND_ATTRIBUTES)Marshal.PtrToStructure(
                    new IntPtr(TokenInformation.ToInt64() + i * sidAndAttrSize + IntPtr.Size), typeof(advapi32.SID_AND_ATTRIBUTES));

                IntPtr pstr = IntPtr.Zero;
                advapi32.ConvertSidToStringSid(sidAndAttributes.Sid, out pstr);
                userSIDS[i] = Marshal.PtrToStringAuto(pstr);
                kernel32.LocalFree(pstr);
            }

            Marshal.FreeHGlobal(TokenInformation);
            return userSIDS;
        }

        public override string ToString()
        {
            return string.Format("Local admin check: {0}", Message);
        }
    }
}
