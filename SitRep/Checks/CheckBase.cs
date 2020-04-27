using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static SitRep.Enums.Enums;

namespace SitRep.Checks
{
    public abstract class CheckBase
    {
        public bool Enabled { get; protected set; } = true;
        public string Message { get; protected set; }
        public abstract override string ToString();
    }
}
