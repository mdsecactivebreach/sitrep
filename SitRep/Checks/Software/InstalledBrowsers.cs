using SitRep.Helpers;
using SitRep.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SitRep.Checks.Software
{
    class InstalledBrowsers : CheckBase, ICheck
    {
        public bool IsOpsecSafe => true;

        public int DisplayOrder => 1;

        public Enums.Enums.CheckType CheckType => Enums.Enums.CheckType.Software;
        public void Check()
        {
            try
            {
                //adapted from https://github.com/MintPlayer/PlatformBrowser/edit/master/MintPlayer.PlatformBrowser/PlatformBrowser.cs
                var builder = new StringBuilder();

                //get browser info from registry 
                var internetKey = RegistryHelper.GetRegSubkeys("HKLM", @"SOFTWARE\WOW6432Node\Clients\StartMenuInternet");
                if (internetKey == null)
                {
                    internetKey = RegistryHelper.GetRegSubkeys("HKLM", @"SOFTWARE\Clients\StartMenuInternet");
                }

                foreach (var browser in internetKey)
                {
                    builder.AppendLine("\t" + browser);
                }

                //Apparently Edge is special... 
                var systemAppsFolder = @"C:\Windows\SystemApps\";
                if (System.IO.Directory.Exists(systemAppsFolder))
                {
                    var directories = System.IO.Directory.GetDirectories(systemAppsFolder);
                    var edgeFolder = directories.FirstOrDefault(d => d.StartsWith($"{systemAppsFolder}Microsoft.MicrosoftEdge_"));

                    if (edgeFolder != null)
                    {
                        if (System.IO.File.Exists($@"{edgeFolder}\MicrosoftEdge.exe"))
                        {
                            builder.AppendLine("\t" + "Microsoft Edge");
                        }
                    }
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
            builder.AppendLine("Installed Browsers:");
            builder.AppendLine(Message);
            return builder.ToString().Trim();
        }
    }
}
