using System;
using Microsoft.Win32;

namespace InstallRegister
{
    class Program
    {
        static public String VMActivityPath = @"C:\Program Files\Luavis\Lia VM";
        static public String VMActivityExcutePath = @"C:\Program Files\Luavis\Lia VM\LiaVMActivity.exe";
        static public String VMExcutePath = @"C:\Program Files\Luavis\Lia VM\LiaVM.exe";

        static void Main(string[] args)
        {
            string ext = ".li";
            RegistryKey key = Registry.ClassesRoot.CreateSubKey(ext);
            key.SetValue("", "LiaVM");
            key.Close();

            key = Registry.ClassesRoot.CreateSubKey(ext + "\\Shell\\Open\\command");
            //key = key.CreateSubKey("command");

            key.SetValue("", "\"" + VMActivityExcutePath + "\" \"%L\"");
            key.Close();

            key = Registry.ClassesRoot.CreateSubKey(ext + "\\DefaultIcon");
            key.SetValue("", VMActivityPath + "\\icon.ico");
            key.Close();

            RegistryKey rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            rkApp.SetValue("LiaVM", VMExcutePath);

            Console.WriteLine("Register Complete");

        }
    }
}
