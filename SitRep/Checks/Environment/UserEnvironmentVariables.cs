using SitRep.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SitRep.Checks.Environment
{
    class UserEnvironmentVariables : CheckBase, ICheck
    {
        public bool IsOpsecSafe => true;

        public int DisplayOrder => 7;

        public Enums.Enums.CheckType CheckType => Enums.Enums.CheckType.Environment;

        public void Check()
        {
            var builder = new StringBuilder();
            foreach(DictionaryEntry item in System.Environment.GetEnvironmentVariables())
            {
                builder.AppendLine(string.Format("\t{0} = {1}", item.Key, item.Value));
            }
            Message = builder.ToString();
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendLine("User environment variables:");
            builder.AppendLine(Message);
            return builder.ToString().Trim();
        }
    }
}
