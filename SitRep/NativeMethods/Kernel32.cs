using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace SitRep.NativeMethods
{
    class kernel32
    {
        [DllImport("kernel32.dll")]
        public static extern IntPtr LocalFree(IntPtr hMem);
        
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(IntPtr hObject);
    }
    
}
