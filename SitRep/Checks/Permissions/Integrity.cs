using SitRep.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;

namespace SitRep.Checks.Permissions
{
    class Integrity : CheckBase, ICheck
    {
        public bool IsOpsecSafe => true;

        public int DisplayOrder => 1;

        public Enums.Enums.CheckType CheckType => Enums.Enums.CheckType.Permissions;

        public void Check()
        {
            try
            {
                Message = "Not in High Integrity context";
                var identity = WindowsIdentity.GetCurrent();
                var principal = new WindowsPrincipal(identity);
                if (principal.IsInRole(WindowsBuiltInRole.Administrator))
                {
                    Message = "In High Integrity context [*]";
                }
            }
            catch
            {
                Message = "Check failed [*]";
            }
        }

        public override string ToString()
        {
            return string.Format("Context Integrity Check: {0}", Message);
        }
    }
}
