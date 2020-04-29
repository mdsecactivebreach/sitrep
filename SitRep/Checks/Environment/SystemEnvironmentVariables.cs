using SitRep.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SitRep.Checks.Environment
{
    class SystemEnvironmentVariables : CheckBase, ICheck
    {
        public bool IsOpsecSafe => true;

        public int DisplayOrder => 8;

        public Enums.Enums.CheckType CheckType => Enums.Enums.CheckType.Environment;

        public void Check()
        {
            try
            {
                var builder = new StringBuilder();
                //Adapted from Seatbelt 
                var items = Helpers.RegistryHelper.GetRegValues("HKLM", @"SYSTEM\CurrentControlSet\Control\Session Manager\Environment");
                if (items == null || items.Count == 0)
                {
                    Message = "\tNo values loaded";
                    return;
                }
                foreach (var item in items)
                {
                    builder.AppendLine(string.Format("\t{0} = {1}", item.Key, item.Value));
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
            builder.AppendLine("System environment Variables:");
            builder.AppendLine(Message);
            return builder.ToString().Trim();
        }
    }
}
