using SitRep.Helpers;
using SitRep.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SitRep.Checks.Permissions
{
    class UACLevel : CheckBase, ICheck
    {
        public bool IsOpsecSafe => true;

        public int DisplayOrder => 2;

        public Enums.Enums.CheckType CheckType => Enums.Enums.CheckType.Permissions;

        public void Check()
        {
            string ConsentPromptBehaviorAdmin = RegistryHelper.GetRegValue("HKLM", "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Policies\\System", "ConsentPromptBehaviorAdmin");
            switch (ConsentPromptBehaviorAdmin)
            {
                case "0":
                    Message = "No prompting [*]";
                    break;
                case "1":
                    Message = "PromptOnSecureDesktop";
                    break;
                case "2":
                    Message = " PromptPermitDenyOnSecureDesktop";
                    break;
                case "3":
                    Message = "PromptForCredsNotOnSecureDesktop";
                    break;
                case "4":
                    Message = "PromptForPermitDenyNotOnSecureDesktop";
                    break;
                case "5":
                    Message = "PromptForNonWindowsBinaries";
                    break;
                default:
                    Message = "PromptForNonWindowsBinaries";
                    break;
            }
        }

        public override string ToString()
        {
            return string.Format("UAC Level: {0}", Message);
        }
    }
}
