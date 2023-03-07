using Microsoft.Win32;
namespace TCPClientApp
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            RegistryKey regStlChatRoom = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            regStlChatRoom.SetValue("STL Chat Room",Application.ExecutablePath.ToString());
            Application.Run(new Client());
        }
    }
}