using SitRep.Interfaces;
using SitRep.NativeMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace SitRep.Checks.Credentials
{
    //adapted from https://gist.github.com/meziantou/10311113
    class CredentialManagerL : CheckBase, ICheck
    {
        public bool IsOpsecSafe => false;

        public int DisplayOrder => 1;

        public Enums.Enums.CheckType CheckType => Enums.Enums.CheckType.Credential;

        public void Check()
        {
            try
            {
                var builder = new StringBuilder();
                Message = "No credentials found";
                var creds = new List<Credential>();
                int count;
                IntPtr pCredentials;
                bool ret = advapi32.CredEnumerate(null, 0, out count, out pCredentials);

                if (ret)
                {
                    for (int i = 0; i < count; i++)
                    {
                        IntPtr credential = Marshal.ReadIntPtr(pCredentials, i * Marshal.SizeOf(typeof(IntPtr)));
                        creds.Add(ReadCredential((advapi32.CREDENTIAL)Marshal.PtrToStructure(credential, typeof(advapi32.CREDENTIAL))));
                    }
                }
                else
                {
                    Message = "\tNo credentials found";
                }

                foreach (var cred in creds)
                {
                    builder.AppendLine(string.Format("\tApplication Name: {0}\r\n\t Username: {1} Password: {2} (Credential Type: {3})",
                        cred.ApplicationName, cred.UserName, cred.Password, cred.CredentialType.ToString()));
                }
                if (! string.IsNullOrWhiteSpace(builder.ToString()))
                {
                    Message = builder.ToString();
                }
            }
            catch
            {
                Message = "Check failed [*]";
            }
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendLine("Credential Manager");
            builder.AppendLine(Message);
            return builder.ToString();
        }

        private Credential ReadCredential(advapi32.CREDENTIAL credential)
        {
            string applicationName = Marshal.PtrToStringUni(credential.TargetName);
            string userName = Marshal.PtrToStringUni(credential.UserName);
            string secret = null;
            if (credential.CredentialBlob != IntPtr.Zero)
            {
                secret = Marshal.PtrToStringUni(credential.CredentialBlob, (int)credential.CredentialBlobSize / 2);
            }

            return new Credential(credential.Type, applicationName, userName, secret);
        }
    }

    class Credential
    {
        public string ApplicationName { get; set; }
        public string UserName { get; set; }
        public string Password  { get; set; }
        public advapi32.CredentialType CredentialType { get; set; }

        public Credential(advapi32.CredentialType credentialType, string applicationName, string userName, string password)
        {
            ApplicationName = applicationName;
            UserName = userName;
            Password = password;
            CredentialType = credentialType;
        }
    }
}
