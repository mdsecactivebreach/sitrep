using SitRep.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SitRep.Checks.Environment
{
    class VirtualEnvionment : CheckBase, ICheck
    {
        public bool IsOpsecSafe => true;

        public int DisplayOrder => 5;

        public Enums.Enums.CheckType CheckType => Enums.Enums.CheckType.Environment;

        public void Check()
        {
            try
            {
                //seems WMI is the only way to do this. 
                //https://stackoverflow.com/questions/498371/how-to-detect-if-my-application-is-running-in-a-virtual-machine
                Message = "Host not virtualised";
                using (var searcher = new System.Management.ManagementObjectSearcher("Select * from Win32_ComputerSystem"))
                {
                    using (var items = searcher.Get())
                    {
                        foreach (var item in items)
                        {
                            string manufacturer = item["Manufacturer"].ToString().ToLower();
                            if (manufacturer == "microsoft corporation" && item["Model"].ToString().ToUpperInvariant().Contains("VIRTUAL"))
                            {
                                Message = "Virtualisation detected (Microsoft) [*]";
                            }
                            else if (manufacturer.Contains("vmware"))
                            {
                                Message = "Virtualisation detected (VMWare) [*]";
                            }
                            else if (item["Model"].ToString() == "VirtualBox")
                            {
                                Message = "Virtualisation detected (VirtualBox) [*]";
                            }
                        }
                    }
                }
            }
            catch
            {
                Message = "Check failed [*]";
            }
        }

        public override string ToString()
        {
            return string.Format("Virtualisation Check: {0}", Message);
        }
    }
}
