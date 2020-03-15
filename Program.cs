using System;
//using TerminalServices;

namespace systeminfo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");


            Console.WriteLine(ConsoleServices.ConsoleManager.GetMachineName());
            Console.WriteLine(ConsoleServices.ConsoleManager.GetUserName());
            Console.WriteLine(ConsoleServices.ConsoleManager.GetIPAddress(false));

            var listrtn = ConsoleServices.ConsoleManager.ListNetworkInterface();
            foreach (var rtn in listrtn)
            {
                Console.WriteLine(rtn);                
            }

            listrtn = ConsoleServices.ConsoleManager.ListShareFolders();
            foreach (var rtn in listrtn)
            {
                Console.WriteLine(rtn);                
            }

            listrtn = ConsoleServices.ConsoleManager.ListWindowsupdates();
            foreach (var rtn in listrtn)
            {
                Console.WriteLine(rtn);                
            }

            listrtn = ConsoleServices.ConsoleManager.ListWindowsDefender();
            foreach (var rtn in listrtn)
            {
                Console.WriteLine(rtn);                
            }

            listrtn = ConsoleServices.ConsoleManager.ListProducts();
            foreach (var rtn in listrtn)
            {
                Console.WriteLine(rtn);                
            }

            listrtn = TerminalServices.TSManager.ListSessions("localhost");
            foreach (var rtn in listrtn)
            {
                Console.WriteLine(rtn);                
            }
        }
    }
}
