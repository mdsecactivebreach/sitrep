using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SitRep.Enums
{
    public class Enums
    {
        public enum CheckType
        {
            Environment = 0,
            Defences = 1,
            Permissions = 2,
            Software = 3,
            Credential = 4
        }

        public enum CredentialType
        {
            Generic = 1,
            DomainPassword,
            DomainCertificate,
            DomainVisiblePassword,
            GenericCertificate,
            DomainExtended,
            Maximum,
            MaximumEx = Maximum + 1000,
        }
    }
}
