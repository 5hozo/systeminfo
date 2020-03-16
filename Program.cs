using System;
//using TerminalServices;

namespace systeminfo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");


            Console.WriteLine("hostname\t" + ConsoleServices.ConsoleManager.GetMachineName());
            Console.WriteLine("username\t" + ConsoleServices.ConsoleManager.GetUserName());
            Console.WriteLine("IPAddress\t" + ConsoleServices.ConsoleManager.GetIPAddress(false));

            var listrtn = ConsoleServices.ConsoleManager.ListNetworkInterface();
            foreach (var rtn in listrtn)
            {
                Console.WriteLine("NetworkInterface\t" + rtn);                
            }

            listrtn = ConsoleServices.ConsoleManager.ListOS();
            foreach (var rtn in listrtn)
            {
                Console.WriteLine("OS\t" + rtn);                
            }

            listrtn = ConsoleServices.ConsoleManager.ListProcessor();
            foreach (var rtn in listrtn)
            {
                Console.WriteLine("Processor\t" + rtn);                
            }

            listrtn = ConsoleServices.ConsoleManager.ListBaseBoard();
            foreach (var rtn in listrtn)
            {
                Console.WriteLine("BaseBoard\t" + rtn);                
            }

            listrtn = ConsoleServices.ConsoleManager.ListBIOS();
            foreach (var rtn in listrtn)
            {
                Console.WriteLine("BIOS\t" + rtn);                
            }

            listrtn = ConsoleServices.ConsoleManager.ListShareFolders();
            foreach (var rtn in listrtn)
            {
                Console.WriteLine("ShareFolder\t" + rtn);                
            }

            listrtn = ConsoleServices.ConsoleManager.ListWindowsupdates();
            foreach (var rtn in listrtn)
            {
                Console.WriteLine("Windowsupdate\t" + rtn);                
            }

            if(System.Environment.OSVersion.Version.Major > 8)//win10以降
            {
                listrtn = ConsoleServices.ConsoleManager.ListWindowsDefender();
                foreach (var rtn in listrtn)
                {
                    Console.WriteLine("WindowsDefender\t" + rtn);                
                }
            }

            listrtn = ConsoleServices.ConsoleManager.ListProducts();
            foreach (var rtn in listrtn)
            {
                Console.WriteLine("Products\t" + rtn);                
            }

            listrtn = TerminalServices.TSManager.ListSessions("localhost");
            foreach (var rtn in listrtn)
            {
                Console.WriteLine("Session\t" + rtn);                
            }
        }
    }
}
