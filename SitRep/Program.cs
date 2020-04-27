using SitRep.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SitRep
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(@"  ___ _ _   ___          ");
            Console.WriteLine(@" / __(_) |_| _ \___ _ __ ");
            Console.WriteLine(@" \__ \ |  _|   / -_) '_ \");
            Console.WriteLine(@" |___/_|\__|_|_\___| .__/");
            Console.WriteLine(@"                   |_|   ");
            Console.WriteLine("Host triage by @two06");
            Console.WriteLine("");
            //parse the args into commands. - thanks GhostPack!
            var arguments = new Dictionary<string, string>();
            foreach (var argument in args)
            {
                var idx = argument.IndexOf(':');
                if (idx > 0)
                {
                    arguments[argument.Substring(0, idx).Remove(0, 1)] = argument.Substring(idx + 1);
                }
                else
                {
                    arguments[argument.Remove(0, 1)] = string.Empty;
                }
            }

            //load all the enabled checks
            var checks = GetAllChecks().Where(x => x.Enabled).ToList();

            //remove the checks tagged as not OpSec safe, unless the user has allowed them
            if (!arguments.ContainsKey("AllowUnsafe"))
            {
                checks.RemoveAll(x => x.IsOpsecSafe == false);
            }

            checks.ForEach(x => x.Check());

            Console.WriteLine("=============================================");
            Console.WriteLine("Envionment Checks");
            Console.WriteLine("=============================================");
            foreach (var check in checks.Where(x => x.CheckType == Enums.Enums.CheckType.Environment).ToList().OrderBy(x=> x.DisplayOrder))
            {
                Console.WriteLine(check.ToString());
            }

            Console.WriteLine();
            Console.WriteLine("=============================================");
            Console.WriteLine("Defence Checks");
            Console.WriteLine("=============================================");
            foreach (var check in checks.Where(x => x.CheckType == Enums.Enums.CheckType.Defences).ToList().OrderBy(x => x.DisplayOrder))
            {
                Console.WriteLine(check.ToString());
            }

            Console.WriteLine();
            Console.WriteLine("=============================================");
            Console.WriteLine("Software Checks");
            Console.WriteLine("=============================================");
            foreach (var check in checks.Where(x => x.CheckType == Enums.Enums.CheckType.Software).ToList().OrderBy(x => x.DisplayOrder))
            {
                Console.WriteLine(check.ToString());
            }

            Console.WriteLine();
            Console.WriteLine("=============================================");
            Console.WriteLine("Permissions Checks");
            Console.WriteLine("=============================================");
            foreach (var check in checks.Where(x => x.CheckType == Enums.Enums.CheckType.Permissions).ToList().OrderBy(x => x.DisplayOrder))
            {
                Console.WriteLine(check.ToString());
            }

            Console.WriteLine();
            Console.WriteLine("=============================================");
            Console.WriteLine("Credentials Checks");
            Console.WriteLine("=============================================");
            foreach (var check in checks.Where(x => x.CheckType == Enums.Enums.CheckType.Credential).ToList().OrderBy(x => x.DisplayOrder))
            {
                Console.WriteLine(check.ToString());
            }
        }

        //Instantiate all classes implementing the ICheck interface. 
        private static IEnumerable<ICheck> GetAllChecks()
        {
            var rnd = new Random();
            var interfaceType = typeof(ICheck);
            var all = AppDomain.CurrentDomain.GetAssemblies()
              .SelectMany(x => x.GetTypes())
              .Where(x => interfaceType.IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
              .Select(x => (ICheck)Activator.CreateInstance(x))
              .OrderBy(x => rnd.Next());
            return all;
        }
    }
}
