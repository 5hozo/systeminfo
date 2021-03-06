﻿using System;
using System.Collections.Generic;
//using TerminalServices;

namespace systeminfo
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("hostname\t" + ConsoleServices.ConsoleManager.GetMachineName());
            Console.WriteLine("username\t" + ConsoleServices.ConsoleManager.GetUserName());
            Console.WriteLine("IPAddress\t" + ConsoleServices.ConsoleManager.GetIPAddress(false));

            List<String> listrtn;

            listrtn = ConsoleServices.ConsoleManager.ListComputerSystem();
            foreach (var rtn in listrtn)
            {
                Console.WriteLine("ComputerSystem\t" + rtn);                
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

            listrtn = ConsoleServices.ConsoleManager.ListLocalUser();
            foreach (var rtn in listrtn)
            {
                Console.WriteLine("LocalUser\t" + rtn);                
            }

            listrtn = ConsoleServices.ConsoleManager.ListLocalGroup();
            foreach (var rtn in listrtn)
            {
                Console.WriteLine("LocalGroup\t" + rtn);                
            }

            listrtn = ConsoleServices.ConsoleManager.ListLocalGroupUser();
            foreach (var rtn in listrtn)
            {
                Console.WriteLine("LocalGroupUser\t" + rtn);                
            }

            listrtn = ConsoleServices.ConsoleManager.ListNetworkInterface();
            foreach (var rtn in listrtn)
            {
                Console.WriteLine("NetworkInterface\t" + rtn);                
            }

            listrtn = ConsoleServices.ConsoleManager.ListDiskDrive();
            foreach (var rtn in listrtn)
            {
                Console.WriteLine("DiskDrive\t" + rtn);                
            }

            listrtn = ConsoleServices.ConsoleManager.ListLogicalDisk();
            foreach (var rtn in listrtn)
            {
                Console.WriteLine("LogicalDisk\t" + rtn);                
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

            if(System.IO.File.Exists(@"C:\Program Files\Windows Defender\MpClient.dll"))
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

            listrtn = ConsoleServices.ConsoleManager.ListService();
            foreach (var rtn in listrtn)
            {
                Console.WriteLine("Service\t" + rtn);                
            }

            listrtn = TerminalServices.TSManager.ListSessions("localhost");
            foreach (var rtn in listrtn)
            {
                Console.WriteLine("Session\t" + rtn);                
            }
        }
    }
}
