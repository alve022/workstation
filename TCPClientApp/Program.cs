using Microsoft.Win32;
using System.Diagnostics;
using System.Windows.Forms;

namespace TCPClientApp
{
    internal static class Program
    {

        [STAThread]
        static void Main()
        {           
           
            //RegistryKey regStlChatRoom = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            //regStlChatRoom.SetValue("STLChatRoom", Application.ExecutablePath.ToString());     
            ApplicationConfiguration.Initialize();
            Application.Run(new Client()); 

        }

        //public static Process Priorprocess()
        //{  
        //    Process proc = Process.GetCurrentProcess();
        //    Process[] procs = Process.GetProcessesByName(proc.ProcessName);
        //    foreach(Process p in procs)
        //    {
        //        if((p.Id !=proc.Id)&& (p.MainModule.FileName==proc.MainModule.FileName))
        //        {
        //            return p;
                    
        //        }
        //    }
        //    return null;
        //}

    }
}