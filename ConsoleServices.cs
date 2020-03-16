//https://docs.microsoft.com/ja-jp/dotnet/api/system.net.networkinformation.networkinterface.getallnetworkinterfaces?view=netframework-4.8
//https://docs.microsoft.com/ja-jp/dotnet/api/system.net.networkinformation?view=netframework-4.8
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;

//https://docs.microsoft.com/ja-jp/dotnet/api/system.management.managementclass?view=netframework-4.8
using System.Management;
 
namespace ConsoleServices
{
    public class ConsoleManager
    {
        public static string GetUserName()
        {
            return Environment.UserName;
        }
        public static string GetMachineName()
        {
            return Environment.MachineName;
        }

        public static string GetIPAddress(bool IsIpOnry = true)
        {
            string ret = "0.0.0.0";
            var interfaces = NetworkInterface.GetAllNetworkInterfaces();
         
            foreach (var adapter in interfaces)
            {
                if (OperationalStatus.Up == adapter.OperationalStatus)
                {
                    if ( (NetworkInterfaceType.Loopback != adapter.NetworkInterfaceType)
                      && (NetworkInterfaceType.Unknown != adapter.NetworkInterfaceType))
                    {

                        var ipproperties = adapter.GetIPProperties();
                        foreach (var unicastaddress in ipproperties.UnicastAddresses)
                        {
                            if (unicastaddress.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                            {
                                ret = unicastaddress.Address.ToString();
                                if(IsIpOnry) break;
                                ret += "/";
                                ret += unicastaddress.IPv4Mask.ToString();
                                ret += "/";
                                ret += ipproperties.GatewayAddresses[0].Address.MapToIPv4().ToString();
                                ret += "/";
                                ret += adapter.GetPhysicalAddress().ToString();
                                break;
                            }
                        }
                    }
                }
            }
            return ret;
        }
        
        public static List<String> ListNetworkInterface()
        {
            List<String> ret = new List<string>();
            ret.Add("Name\tDescription\tStatus\tNetworkInterfaceType\tPhysicalAddress\tSpeed\tUnicastAddresses\tGatewayAddresses\tDhcpServerAddresses\tDnsAddresses\tWinsServersAddresses");

            var interfaces = NetworkInterface.GetAllNetworkInterfaces();
         
            foreach (var adapter in interfaces)
            {
                string retline = "";

                retline += adapter.Name + "\t";
                retline += adapter.Description + "\t";
                retline += adapter.OperationalStatus.ToString() + "\t";
                retline += adapter.NetworkInterfaceType.ToString() + "\t";
                retline += adapter.GetPhysicalAddress().ToString() + "\t";
                retline += adapter.Speed.ToString() + "\t";

                var ipproperties = adapter.GetIPProperties();
                retline += ipproperties.DnsSuffix.ToString() + "\t";

                foreach (var unicastaddress in ipproperties.UnicastAddresses)
                {
                    retline += unicastaddress.Address.ToString() + "/";
                    retline += unicastaddress.IPv4Mask.ToString() + "\t";
                }

                foreach (var gatewayaddress in ipproperties.GatewayAddresses)
                {
                    retline += gatewayaddress.Address.ToString() + ";";
                }
                retline += "\t";

                foreach (var dhcpaddress in ipproperties.DhcpServerAddresses)
                {
                    retline += dhcpaddress.MapToIPv4().ToString() + ";";
                }
                retline += "\t";

                foreach (var dnsaddress in ipproperties.DnsAddresses)
                {
                    retline += dnsaddress.MapToIPv4().ToString() + ";";
                }
                retline += "\t";

                foreach (var winsaddress in ipproperties.WinsServersAddresses)
                {
                    retline += winsaddress.MapToIPv4().ToString() + ";";
                }
                retline += "\t";

                ret.Add(retline);
            }
            return ret;
        }

        public static List<String> ListOS()
        {
            //https://docs.microsoft.com/ja-jp/dotnet/api/system.management.managementpath?view=netframework-4.8
            List<String> ret = new List<string>();
            ret.Add("Name\tCaption\tDescription\tVersion\tBuildNumber\tManufacturer\tLocale\tOSLanguage\tSerialNumber");

            ManagementClass managementclass = new ManagementClass("Win32_OperatingSystem");
            ManagementObjectCollection managementobjectcollection = managementclass.GetInstances();
            foreach (ManagementObject managementobject in managementobjectcollection)
            {
                string retline = "";

                retline += managementobject.Properties["Name"].Value.ToString() + "\t";
                retline += managementobject.Properties["Caption"].Value.ToString() + "\t";
                retline += managementobject.Properties["Description"].Value.ToString() + "\t";
                retline += managementobject.Properties["Version"].Value.ToString() + "\t";
                retline += managementobject.Properties["BuildNumber"].Value.ToString() + "\t";
                retline += managementobject.Properties["Manufacturer"].Value.ToString() + "\t";
                retline += managementobject.Properties["Locale"].Value.ToString() + "\t";
                retline += managementobject.Properties["OSLanguage"].Value.ToString() + "\t";
                retline += managementobject.Properties["SerialNumber"].Value.ToString() + "\t";

                ret.Add(retline);
            }
            return ret;
        
        }

        public static List<String> ListProcessor()
        {
            //https://docs.microsoft.com/ja-jp/dotnet/api/system.management.managementpath?view=netframework-4.8
            //https://docs.microsoft.com/en-us/windows/win32/cimwin32prov/win32-processor
            List<String> ret = new List<string>();
            ret.Add("Name\tCaption\tDescription\tDeviceID\tManufacturer\tArchitecture\tCurrentClockSpeed\tMaxClockSpeed\tNumberOfCores\tPartNumber\tSerialNumber");

            ManagementClass managementclass = new ManagementClass("Win32_Processor");
            ManagementObjectCollection managementobjectcollection = managementclass.GetInstances();
            foreach (ManagementObject managementobject in managementobjectcollection)
            {
                string retline = "";

                retline += managementobject.Properties["Name"].Value.ToString() + "\t";
                retline += managementobject.Properties["Caption"].Value.ToString() + "\t";
                retline += managementobject.Properties["Description"].Value.ToString() + "\t";
                retline += managementobject.Properties["DeviceID"].Value.ToString() + "\t";
                retline += managementobject.Properties["Manufacturer"].Value.ToString() + "\t";
                retline += managementobject.Properties["Architecture"].Value.ToString() + "\t";
                retline += managementobject.Properties["CurrentClockSpeed"].Value.ToString() + "\t";
                retline += managementobject.Properties["MaxClockSpeed"].Value.ToString() + "\t";
                retline += managementobject.Properties["NumberOfCores"].Value.ToString() + "\t";
                retline += managementobject.Properties["PartNumber"].Value.ToString() + "\t";
                retline += managementobject.Properties["SerialNumber"].Value.ToString() + "\t";

                ret.Add(retline);
            }
            return ret;
        
        }

        public static List<String> ListBaseBoard()
        {
            //https://docs.microsoft.com/ja-jp/dotnet/api/system.management.managementpath?view=netframework-4.8
            //https://docs.microsoft.com/en-us/windows/win32/cimwin32prov/win32-processor
            List<String> ret = new List<string>();
            ret.Add("Caption\tDescription\tManufacturer\tModel\tName\tOtherIdentifyingInfo\tPartNumber\tProduct\tSerialNumber\tSKU\tVersion");

            string[] propertynames = new string[11] {"Caption","Description","Manufacturer","Model","Name","OtherIdentifyingInfo","PartNumber","Product","SerialNumber","SKU","Version"};

            ManagementClass managementclass = new ManagementClass("Win32_BaseBoard");
            ManagementObjectCollection managementobjectcollection = managementclass.GetInstances();
            foreach (ManagementObject managementobject in managementobjectcollection)
            {
                string retline = "";

                foreach (string propertyname in propertynames)
                {
                    if(managementobject.Properties[propertyname].Value != null)
                    {
                        retline += managementobject.Properties[propertyname].Value.ToString() + "\t";
                    }
                    else
                    {
                        retline += "\t";
                    }
                }
                ret.Add(retline);
            }
            return ret;
        
        }

        public static List<String> ListBIOS()
        {
            //https://docs.microsoft.com/ja-jp/dotnet/api/system.management.managementpath?view=netframework-4.8
            //https://docs.microsoft.com/en-us/windows/win32/cimwin32prov/win32-bios
            List<String> ret = new List<string>();
            ret.Add("BuildNumber\tCaption\tCodeSet\tCurrentLanguage\tDescription\tIdentificationCode\tLanguageEdition\tManufacturer\tName\tOtherTargetOS\tSerialNumber\tSMBIOSBIOSVersion\tSoftwareElementID\tVersion");

            string[] propertynames = new string[14] {"BuildNumber","Caption","CodeSet","CurrentLanguage","Description","IdentificationCode","LanguageEdition","Manufacturer","Name","OtherTargetOS","SerialNumber","SMBIOSBIOSVersion","SoftwareElementID","Version"};

            ManagementClass managementclass = new ManagementClass("Win32_BIOS");
            ManagementObjectCollection managementobjectcollection = managementclass.GetInstances();
            foreach (ManagementObject managementobject in managementobjectcollection)
            {
                string retline = "";

                foreach (string propertyname in propertynames)
                {
                    if(managementobject.Properties[propertyname].Value != null)
                    {
                        retline += managementobject.Properties[propertyname].Value.ToString() + "\t";
                    }
                    else
                    {
                        retline += "\t";
                    }
                }
                ret.Add(retline);
            }
            return ret;
        
        }

        public static List<String> ListShareFolders()
        {
            //https://docs.microsoft.com/ja-jp/dotnet/api/system.management.managementpath?view=netframework-4.8
            List<String> ret = new List<string>();
            ret.Add("Name\tPath\tCaption");

            ManagementClass managementclass = new ManagementClass("Win32_Share");
            ManagementObjectCollection managementobjectcollection = managementclass.GetInstances();
            foreach (ManagementObject managementobject in managementobjectcollection)
            {
                string retline = "";

                retline += managementobject.Properties["Name"].Value.ToString() + "\t";
                retline += managementobject.Properties["Path"].Value.ToString() + "\t";
                retline += managementobject.Properties["Caption"].Value.ToString() + "\t";

                ret.Add(retline);
            }
            return ret;
        
        }

        public static List<String> ListWindowsupdates()
        {
            //https://docs.microsoft.com/en-gb/windows/win32/cimwin32prov/win32-quickfixengineering?redirectedfrom=MSDN
            List<String> ret = new List<string>();
            ret.Add("Caption\tDescription\tHotFixID\tInstalledBy\tInstalledOn");

            ManagementClass managementclass = new ManagementClass("Win32_QuickFixEngineering");
            ManagementObjectCollection managementobjectcollection = managementclass.GetInstances();
            foreach (var managementobject in managementobjectcollection)
            {
                string retline = "";

                retline += managementobject.Properties["Caption"].Value.ToString() + "\t";
                retline += managementobject.Properties["Description"].Value.ToString() + "\t";
 //               retline += managementobject.Properties["Name"].Value.ToString() + "\t";
//                retline += managementobject.Properties["Status"].Value.ToString() + "\t";
//                retline += managementobject.Properties["CSName"].Value.ToString() + "\t";
//                retline += managementobject.Properties["FixComments"].Value.ToString() + "\t";
                retline += managementobject.Properties["HotFixID"].Value.ToString() + "\t";
                retline += managementobject.Properties["InstalledBy"].Value.ToString() + "\t";

//                retline += managementobject.Properties["ServicePackInEffect"].Value.ToString() + "\t";
//                retline += managementobject.Properties["InstallDate"].Value.ToString() + "\t";
//                retline += managementobject.Properties["InstalledOn"].Value.ToString() + "\t";
                string InstalledOn = managementobject.Properties["InstalledOn"].Value.ToString();
                try
                {
                    string[] arr =  InstalledOn.Split("/");
                    if(Int32.TryParse(arr[0],out Int32 i))
                    {
                        arr[0] = i.ToString("00");
                    }
                    if(Int32.TryParse(arr[1],out Int32 j))
                    {
                        arr[1] = j.ToString("00");
                    }
                    if(Int32.TryParse(arr[2],out Int32 k))
                    {
                        arr[2] = k.ToString("0000");
                    }
                    InstalledOn = arr[2] + "/" + arr[0] + "/" + arr[1];
                }
                finally
                {
                    
                }
                
                retline += InstalledOn + "\t";


                ret.Add(retline);
            }
            return ret;
        
        }


        [DllImport(@"C:\Program Files\Windows Defender\MpClient.dll")]
        private static extern int WDStatus(out bool pfEnabled);
        public static List<String> ListWindowsDefender()
        {
            //https://docs.microsoft.com/en-gb/previous-versions/windows/desktop/defender/msft-mpcomputerstatus?redirectedfrom=MSDN
            List<String> ret = new List<string>();
            ret.Add("AMProductVersion\tAMServiceVersion\tAntispywareSignatureVersion\tAntivirusSignatureVersion\tNISSignatureVersion\tAMEngineVersion\tNISEngineVersion");

            //https://code.i-harness.com/ja-jp/q/26fea11
            //https://docs.microsoft.com/en-gb/windows/win32/lwef/defender-ref-entry
            //https://docs.microsoft.com/en-gb/previous-versions/windows/desktop/defender/msft-mpcomputerstatus
            bool pfEnabled;
            int result = WDStatus(out pfEnabled); //Returns the defender status - It's working properly.
            if(!pfEnabled) {return ret;}


            ManagementObjectSearcher managementobjectsearcher = new ManagementObjectSearcher("root\\Microsoft\\Windows\\Defender","SELECT * FROM MSFT_MpComputerStatus");
            ManagementObjectCollection managementobjectcollection = managementobjectsearcher.Get();
            foreach (var managementobject in managementobjectcollection)
            {
                string retline = "";

                retline += managementobject.Properties["AMProductVersion"].Value.ToString() + "\t";
                retline += managementobject.Properties["AMServiceVersion"].Value.ToString() + "\t";

                retline += managementobject.Properties["AntispywareSignatureVersion"].Value.ToString() + "\t";
                retline += managementobject.Properties["AntivirusSignatureVersion"].Value.ToString() + "\t";
                retline += managementobject.Properties["NISSignatureVersion"].Value.ToString() + "\t";
                retline += managementobject.Properties["AMEngineVersion"].Value.ToString() + "\t";
                retline += managementobject.Properties["NISEngineVersion"].Value.ToString() + "\t";

                ret.Add(retline);
            }
            return ret;
        
        }
        
        public static List<String> ListProducts()
        {
            //https://docs.microsoft.com/en-us/previous-versions/windows/desktop/legacy/aa394378(v=vs.85)
            List<String> ret = new List<string>();
            ret.Add("Name\tCaption\tDescription\tIdentifyingNumber\tInstallDate\tInstallSource\tLocalPackage\tPackageCode\tPackageName\tProductID\tSKUNumber\tVendor\tVersion");

            string[] propertynames = new string[13] {"Name","Caption","Description","IdentifyingNumber","InstallDate","InstallSource","LocalPackage","PackageCode","PackageName","ProductID","SKUNumber","Vendor","Version"};

            ManagementClass managementclass = new ManagementClass("Win32_Product");
            ManagementObjectCollection managementobjectcollection = managementclass.GetInstances();
            foreach (ManagementObject managementobject in managementobjectcollection)
            {
                string retline = "";

                foreach (string propertyname in propertynames)
                {
                    if(managementobject.Properties[propertyname].Value != null)
                    {
                        retline += managementobject.Properties[propertyname].Value.ToString() + "\t";
                    }
                    else
                    {
                        retline += "\t";
                    }
                }
/*
                 retline += managementobject.Properties["Name"].Value.ToString() + "\t";
                retline += managementobject.Properties["Caption"].Value.ToString() + "\t";
                retline += managementobject.Properties["Description"].Value.ToString() + "\t";
                retline += managementobject.Properties["IdentifyingNumber"].Value.ToString() + "\t";
                retline += managementobject.Properties["InstallDate"].Value.ToString() + "\t";
//                retline += managementobject.Properties["InstallLocation"].Value.ToString() + "\t";
//                retline += managementobject.Properties["HelpLink"].Value.ToString() + "\t";
//                retline += managementobject.Properties["HelpTelephone"].Value.ToString() + "\t";
                retline += managementobject.Properties["InstallSource"].Value.ToString() + "\t";
//                retline += managementobject.Properties["Language"].Value.ToString() + "\t";
                retline += managementobject.Properties["LocalPackage"].Value.ToString() + "\t";
//                retline += managementobject.Properties["PackageCache"].Value.ToString() + "\t";
                retline += managementobject.Properties["PackageCode"].Value.ToString() + "\t";
                retline += managementobject.Properties["PackageName"].Value.ToString() + "\t";
                retline += managementobject.Properties["ProductID"].Value.ToString() + "\t";
//                retline += managementobject.Properties["RegOwner"].Value.ToString() + "\t";
//                retline += managementobject.Properties["RegCompany"].Value.ToString() + "\t";
                retline += managementobject.Properties["SKUNumber"].Value.ToString() + "\t";
//                retline += managementobject.Properties["Transforms"].Value.ToString() + "\t";
//                retline += managementobject.Properties["URLInfoAbout"].Value.ToString() + "\t";
//                retline += managementobject.Properties["URLUpdateInfo"].Value.ToString() + "\t";
                retline += managementobject.Properties["Vendor"].Value.ToString() + "\t";
                retline += managementobject.Properties["Version"].Value.ToString() + "\t";
*/


                ret.Add(retline);
            }
            return ret;
        
        }
    }
}