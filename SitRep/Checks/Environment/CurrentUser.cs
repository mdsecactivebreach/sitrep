using SitRep.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static SitRep.Enums.Enums;

namespace SitRep.Checks.Environment
{
    class CurrentUser : CheckBase, ICheck
    {
        public bool IsOpsecSafe => true;

        public int DisplayOrder => 0;

        public CheckType CheckType => CheckType.Environment;

        public void Check()
        {
            Message = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
        }

        public override string ToString()
        {
            return string.Format("Current User: {0}", Message);
        }
    }
}
