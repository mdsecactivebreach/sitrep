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
    class Privileges : CheckBase, ICheck
    {
        public bool IsOpsecSafe => true;

        public int DisplayOrder => 3;

        public Enums.Enums.CheckType CheckType => Enums.Enums.CheckType.Permissions;

        public void Check()
        {
            var builder = new StringBuilder();

            var specialPrivileges = new List<string>()
            {
                "SeSecurityPrivilege",
                "SeTakeOwnershipPrivilege",
                "SeLoadDriverPrivilege",
                "SeBackupPrivilege",
                "SeRestorePrivilege",
                "SeDebugPrivilege",
                "SeSystemEnvironmentPrivilege",
                "SeImpersonatePrivilege",
                "SeTcbPrivilege"
            };
            int TokenInfLength = 0;
            IntPtr ThisHandle = WindowsIdentity.GetCurrent().Token;
            advapi32.GetTokenInformation(ThisHandle, advapi32.TOKEN_INFORMATION_CLASS.TokenPrivileges, IntPtr.Zero, TokenInfLength, out TokenInfLength);
            IntPtr TokenInformation = Marshal.AllocHGlobal(TokenInfLength);

            if (advapi32.GetTokenInformation(WindowsIdentity.GetCurrent().Token, advapi32.TOKEN_INFORMATION_CLASS.TokenPrivileges, TokenInformation, TokenInfLength, out TokenInfLength))
            {
                var ThisPrivilegeSet = (advapi32.TOKEN_PRIVILEGES)Marshal.PtrToStructure(TokenInformation, typeof(advapi32.TOKEN_PRIVILEGES));
                for (int i = 0; i < ThisPrivilegeSet.PrivilegeCount; i++)
                {
                    advapi32.LUID_AND_ATTRIBUTES laa = ThisPrivilegeSet.Privileges[i];
                    System.Text.StringBuilder StrBuilder = new System.Text.StringBuilder();
                    int LuidNameLen = 0;
                    IntPtr LuidPointer = Marshal.AllocHGlobal(Marshal.SizeOf(laa.Luid));
                    Marshal.StructureToPtr(laa.Luid, LuidPointer, true);
                    advapi32.LookupPrivilegeName(null, LuidPointer, null, ref LuidNameLen);
                    StrBuilder.EnsureCapacity(LuidNameLen + 1);

                    if(advapi32.LookupPrivilegeName(null, LuidPointer, StrBuilder, ref LuidNameLen))
                    {
                        var privilage = StrBuilder.ToString();
                        if (specialPrivileges.Contains(privilage))
                        {
                            builder.AppendLine("\t" + privilage + " [*]");
                        }
                        else
                        {
                            builder.AppendLine("\t" + privilage);
                        }
                    }
                    Marshal.FreeHGlobal(LuidPointer);
                }
            }
            Message = builder.ToString();

        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendLine("User Privileges:");
            builder.AppendLine(Message);
            return builder.ToString().Trim();
        }
    }
}
