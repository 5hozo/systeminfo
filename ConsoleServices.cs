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
            ret.Add("Name\tDescription\tStatus\tNetworkInterfaceType\tPhysicalAddress\tSpeed\tUnicastAddresses\tNetMask\tGatewayAddresses\tDhcpServerAddresses\tDnsAddresses\tWinsServersAddresses");

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

                foreach (var unicastaddress in ipproperties.UnicastAddresses)
                {
                    retline += unicastaddress.Address.ToString() + "\t";
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

        private static List<String> ListWMIC(string ClassName,string HeaderTSV)
        {
            String[] DateTimeFormats = {"yyyyMMddHHmmss","yyyyMMddHHmmss.f","yyyy/MM/dd","MM/dd/yyyy","M/dd/yyyy","M/d/yyyy"};
            List<String> ret = new List<string>();
            List<String> listline = new List<string>();
            List<String> propertynames = new List<string>();
 
            ManagementClass managementclass = new ManagementClass(ClassName);

            //Check Header
            listline.Clear();
            propertynames.AddRange(HeaderTSV.ToLower().Split("\t"));
            PropertyDataCollection properties = managementclass.Properties;
            foreach (PropertyData propertydata in properties)
            {
                if(propertynames.IndexOf(propertydata.Name.ToLower())>0)
                {
                    listline.Add(propertydata.Name);
                }
            }
            propertynames.Clear();
            propertynames.AddRange(listline);
            ret.Add(String.Join("\t",listline));

            //Get Value
            ManagementObjectCollection managementobjectcollection = managementclass.GetInstances();
            foreach (ManagementObject managementobject in managementobjectcollection)
            {
                listline.Clear();

                foreach (String propertyname in propertynames)
                {
                    PropertyData propertydata = managementobject.Properties[propertyname];
                    if(propertydata.Value != null)
                    {
                        if (propertydata.IsArray == false)
                        {
                            switch (propertydata.Type)
                            {
                                case CimType.DateTime:
                                    if(propertydata.Value.GetType().Name == "DateTime")
                                    {
                                        DateTime datetime = (DateTime)propertydata.Value;
                                        listline.Add(datetime.ToString("yyyy/MM/dd"));
                                    }
                                    else if(propertydata.Value.GetType().Name == "String")
                                    {
                                        DateTime dateTime = DateTime.Now;
                                        if(System.DateTime.TryParseExact(propertydata.Value.ToString(),DateTimeFormats,System.Globalization.DateTimeFormatInfo.InvariantInfo,System.Globalization.DateTimeStyles.None,out dateTime))
                                        {
                                            listline.Add(dateTime.ToString("yyyy/MM/dd"));
                                        }else if(System.DateTime.TryParseExact(propertydata.Value.ToString().Substring(0,14),DateTimeFormats,System.Globalization.DateTimeFormatInfo.InvariantInfo,System.Globalization.DateTimeStyles.None,out dateTime))
                                        {
                                            listline.Add(dateTime.ToString("yyyy/MM/dd"));
                                        }else
                                        {
                                            listline.Add(propertydata.Value.ToString().Replace("\t","").Replace("\n",""));
                                        }
                                        listline.Add((String)propertydata.Value);
                                    }
                                    else
                                    {
                                        listline.Add("");
                                    }                                    
                                    break;

                                case CimType.Object:
                                case CimType.Reference:
                                    listline.Add("Object");
                                    break;

                                case CimType.None:
                                    listline.Add("Null");
                                    break;
                                default:
                                    if(propertydata.Name.Contains("On")||propertydata.Name.Contains("Date"))
                                    {
                                        DateTime dateTime = DateTime.Now;
                                        if(System.DateTime.TryParseExact(propertydata.Value.ToString(),DateTimeFormats,System.Globalization.DateTimeFormatInfo.InvariantInfo,System.Globalization.DateTimeStyles.None,out dateTime))
                                        {
                                            listline.Add(dateTime.ToString("yyyy/MM/dd"));
                                        }else
                                        {
                                            listline.Add(propertydata.Value.ToString().Replace("\t","").Replace("\n",""));
                                        }
                                    }else
                                    {
                                        listline.Add(propertydata.Value.ToString().Replace("\t","").Replace("\n",""));
                                    }
                                    break;
                            }
                        }else
                        {
                            switch (propertydata.Type)
                            {
                                case CimType.DateTime:
                                    String[] aryDateTime = Array.ConvertAll((DateTime[])propertydata.Value, d => d.ToString("yyyy/mm/dd"));
                                    listline.Add(String.Join(";",aryDateTime));
                                    break;
                                case CimType.Char16:
                                    String[] aryChar16 = Array.ConvertAll((char[])propertydata.Value, d => d.ToString());
                                    listline.Add(String.Join(";",aryChar16));
                                    break;
                                case CimType.Real32:
                                    String[] aryReal32 = Array.ConvertAll((Single[])propertydata.Value, d => d.ToString());
                                    listline.Add(String.Join(";",aryReal32));
                                    break;
                                case CimType.Real64:
                                    String[] aryReal64 = Array.ConvertAll((Double[])propertydata.Value, d => d.ToString());
                                    listline.Add(String.Join(";",aryReal64));
                                    break;
                                case CimType.SInt8:
                                    String[] arySInt8 = Array.ConvertAll((SByte[])propertydata.Value, d => d.ToString());
                                    listline.Add(String.Join(";",arySInt8));
                                    break;
                                case CimType.SInt16:
                                    String[] arySInt16 = Array.ConvertAll((Int16[])propertydata.Value, d => d.ToString());
                                    listline.Add(String.Join(";",arySInt16));
                                    break;
                                case CimType.SInt32:
                                    String[] arySInt32 = Array.ConvertAll((Int32[])propertydata.Value, d => d.ToString());
                                    listline.Add(String.Join(";",arySInt32));
                                    break;
                                case CimType.SInt64:
                                    String[] arySInt64 = Array.ConvertAll((Int64[])propertydata.Value, d => d.ToString());
                                    listline.Add(String.Join(";",arySInt64));
                                    break;
                                case CimType.UInt8:
                                    String[] aryUInt8 = Array.ConvertAll((Byte[])propertydata.Value, d => d.ToString());
                                    listline.Add(String.Join(";",aryUInt8));
                                    break;
                                case CimType.UInt16:
                                    String[] aryUInt16 = Array.ConvertAll((UInt16[])propertydata.Value, d => d.ToString());
                                    listline.Add(String.Join(";",aryUInt16));
                                    break;
                                case CimType.UInt32:
                                    String[] aryUInt32 = Array.ConvertAll((UInt32[])propertydata.Value, d => d.ToString());
                                    listline.Add(String.Join(";",aryUInt32));
                                    break;
                                case CimType.UInt64:
                                    String[] aryUInt64 = Array.ConvertAll((UInt64[])propertydata.Value, d => d.ToString());
                                    listline.Add(String.Join(";",aryUInt64));
                                    break;

                                case CimType.Object:
                                case CimType.Reference:
                                    listline.Add("Object");
                                    break;

                                case CimType.None:
                                    listline.Add("Null");
                                    break;

                                default:
                                    String[] aryvar = Array.ConvertAll((String[])propertydata.Value, d => d.ToString().Replace("\t","").Replace("\n",""));
                                    listline.Add(String.Join(";",aryvar));
                                    break;
                            }
                        }
                    }else
                    {
                        listline.Add("");
                    }                 
                }
                ret.Add(String.Join("\t",listline));
            }
            return ret;
        }
                
        private static List<String> ListWMIC(string ClassName)
        {
            String[] DateTimeFormats = {"yyyyMMddHHmmss","yyyyMMddHHmmss.f","yyyy/MM/dd","MM/dd/yyyy","M/dd/yyyy","M/d/yyyy"};
            List<String> ret = new List<string>();
            List<String> listline = new List<string>();
            
            ManagementClass managementclass = new ManagementClass(ClassName);

            listline.Clear();
            PropertyDataCollection properties = managementclass.Properties;
            foreach (PropertyData propertydata in properties)
            {
                listline.Add(propertydata.Name);
            }
            ret.Add(String.Join("\t",listline));

            ManagementObjectCollection managementobjectcollection = managementclass.GetInstances();
            foreach (ManagementObject managementobject in managementobjectcollection)
            {
                listline.Clear();

                foreach (PropertyData propertydata in managementobject.Properties)
                {
                    if(propertydata.Value != null)
                    {
                        if (propertydata.IsArray == false)
                        {
                            switch (propertydata.Type)
                            {
                                case CimType.DateTime:
                                    if(propertydata.Value.GetType().Name == "DateTime")
                                    {
                                        DateTime datetime = (DateTime)propertydata.Value;
                                        listline.Add(datetime.ToString("yyyy/MM/dd"));
                                    }
                                    else if(propertydata.Value.GetType().Name == "String")
                                    {
                                        DateTime dateTime = DateTime.Now;
                                        if(System.DateTime.TryParseExact(propertydata.Value.ToString(),DateTimeFormats,System.Globalization.DateTimeFormatInfo.InvariantInfo,System.Globalization.DateTimeStyles.None,out dateTime))
                                        {
                                            listline.Add(dateTime.ToString("yyyy/MM/dd"));
                                        }else if(System.DateTime.TryParseExact(propertydata.Value.ToString().Substring(0,14),DateTimeFormats,System.Globalization.DateTimeFormatInfo.InvariantInfo,System.Globalization.DateTimeStyles.None,out dateTime))
                                        {
                                            listline.Add(dateTime.ToString("yyyy/MM/dd"));
                                        }else
                                        {
                                            listline.Add(propertydata.Value.ToString().Replace("\t","").Replace("\n",""));
                                        }
                                        listline.Add((String)propertydata.Value);
                                    }
                                    else
                                    {
                                        listline.Add("");
                                    }                                    
                                    break;

                                case CimType.Object:
                                case CimType.Reference:
                                    listline.Add("Object");
                                    break;

                                case CimType.None:
                                    listline.Add("Null");
                                    break;
                                default:
                                    if(propertydata.Name.Contains("On")||propertydata.Name.Contains("Date"))
                                    {
                                        DateTime dateTime = DateTime.Now;
                                        if(System.DateTime.TryParseExact(propertydata.Value.ToString(),DateTimeFormats,System.Globalization.DateTimeFormatInfo.InvariantInfo,System.Globalization.DateTimeStyles.None,out dateTime))
                                        {
                                            listline.Add(dateTime.ToString("yyyy/MM/dd"));
                                        }else
                                        {
                                            listline.Add(propertydata.Value.ToString().Replace("\t","").Replace("\n",""));
                                        }
                                    }else
                                    {
                                        listline.Add(propertydata.Value.ToString().Replace("\t","").Replace("\n",""));
                                    }
                                    break;
                            }
                        }else
                        {
                            switch (propertydata.Type)
                            {
                                case CimType.DateTime:
                                    String[] aryDateTime = Array.ConvertAll((DateTime[])propertydata.Value, d => d.ToString("yyyy/mm/dd"));
                                    listline.Add(String.Join(";",aryDateTime));
                                    break;
                                case CimType.Char16:
                                    String[] aryChar16 = Array.ConvertAll((char[])propertydata.Value, d => d.ToString());
                                    listline.Add(String.Join(";",aryChar16));
                                    break;
                                case CimType.Real32:
                                    String[] aryReal32 = Array.ConvertAll((Single[])propertydata.Value, d => d.ToString());
                                    listline.Add(String.Join(";",aryReal32));
                                    break;
                                case CimType.Real64:
                                    String[] aryReal64 = Array.ConvertAll((Double[])propertydata.Value, d => d.ToString());
                                    listline.Add(String.Join(";",aryReal64));
                                    break;
                                case CimType.SInt8:
                                    String[] arySInt8 = Array.ConvertAll((SByte[])propertydata.Value, d => d.ToString());
                                    listline.Add(String.Join(";",arySInt8));
                                    break;
                                case CimType.SInt16:
                                    String[] arySInt16 = Array.ConvertAll((Int16[])propertydata.Value, d => d.ToString());
                                    listline.Add(String.Join(";",arySInt16));
                                    break;
                                case CimType.SInt32:
                                    String[] arySInt32 = Array.ConvertAll((Int32[])propertydata.Value, d => d.ToString());
                                    listline.Add(String.Join(";",arySInt32));
                                    break;
                                case CimType.SInt64:
                                    String[] arySInt64 = Array.ConvertAll((Int64[])propertydata.Value, d => d.ToString());
                                    listline.Add(String.Join(";",arySInt64));
                                    break;
                                case CimType.UInt8:
                                    String[] aryUInt8 = Array.ConvertAll((Byte[])propertydata.Value, d => d.ToString());
                                    listline.Add(String.Join(";",aryUInt8));
                                    break;
                                case CimType.UInt16:
                                    String[] aryUInt16 = Array.ConvertAll((UInt16[])propertydata.Value, d => d.ToString());
                                    listline.Add(String.Join(";",aryUInt16));
                                    break;
                                case CimType.UInt32:
                                    String[] aryUInt32 = Array.ConvertAll((UInt32[])propertydata.Value, d => d.ToString());
                                    listline.Add(String.Join(";",aryUInt32));
                                    break;
                                case CimType.UInt64:
                                    String[] aryUInt64 = Array.ConvertAll((UInt64[])propertydata.Value, d => d.ToString());
                                    listline.Add(String.Join(";",aryUInt64));
                                    break;

                                case CimType.Object:
                                case CimType.Reference:
                                    listline.Add("Object");
                                    break;

                                case CimType.None:
                                    listline.Add("Null");
                                    break;

                                default:
                                    String[] aryvar = Array.ConvertAll((String[])propertydata.Value, d => d.ToString().Replace("\t","").Replace("\n",""));
                                    listline.Add(String.Join(";",aryvar));
                                    break;
                            }
                        }
                    }else
                    {
                        listline.Add("");
                    }                 
                    
                }
                ret.Add(String.Join("\t",listline));
            }
            return ret;        
        }

        public static List<String> ListOS()
        {
            return ListWMIC("Win32_OperatingSystem","Name\tCaption\tDescription\tVersion\tBuildNumber\tManufacturer\tLocale\tOSLanguage\tSerialNumber");
        }

        public static List<String> ListProcessor()
        {
            //https://docs.microsoft.com/en-us/windows/win32/cimwin32prov/win32-processor
            return ListWMIC("Win32_Processor","Name\tCaption\tDescription\tDeviceID\tManufacturer\tArchitecture\tCurrentClockSpeed\tMaxClockSpeed\tNumberOfCores\tPartNumber\tSerialNumber");
        }

        public static List<String> ListBaseBoard()
        {
            //https://docs.microsoft.com/en-us/windows/win32/cimwin32prov/win32-processor
            return ListWMIC("Win32_BaseBoard","Caption\tDescription\tManufacturer\tModel\tName\tOtherIdentifyingInfo\tPartNumber\tProduct\tSerialNumber\tSKU\tVersion");
        }

        public static List<String> ListBIOS()
        {
            //https://docs.microsoft.com/en-us/windows/win32/cimwin32prov/win32-bios
            return ListWMIC("Win32_BIOS","BuildNumber\tCaption\tCodeSet\tCurrentLanguage\tDescription\tIdentificationCode\tLanguageEdition\tManufacturer\tName\tOtherTargetOS\tSerialNumber\tSMBIOSBIOSVersion\tSoftwareElementID\tVersion");
        }

        public static List<String> ListComputerSystem()
        {
            //https://docs.microsoft.com/en-us/windows/win32/cimwin32prov/win32-computersystem
            return ListWMIC("Win32_ComputerSystem","BootupState\tCaption\tChassisSKUNumber\tDescription\tDNSHostName\tDomain\tLastLoadInfo\tManufacturer\tModel\tName\tNameFormat\tPrimaryOwnerContact\tPrimaryOwnerName\tStatus\tSystemFamily\tSystemSKUNumber\tSystemType\tUserName\tWorkgroup");
        }

        public static List<String> ListShareFolders()
        {
            return ListWMIC("Win32_Share","Name\tPath\tCaption");
        }
        
        public static List<String> ListProducts()
        {
            //https://docs.microsoft.com/en-us/previous-versions/windows/desktop/legacy/aa394378(v=vs.85)
            return ListWMIC("Win32_Product","Name\tCaption\tDescription\tIdentifyingNumber\tInstallDate\tInstallSource\tLocalPackage\tPackageCode\tPackageName\tProductID\tSKUNumber\tVendor\tVersion");
        }

        public static List<String> ListService()
        {
            //https://docs.microsoft.com/en-us/windows/win32/cimwin32prov/win32-service
            return ListWMIC("Win32_Service","Caption\tDisplayName\tName\tPathName\tServiceType\tStartMode\tStartName\tState\tStatus\tSystemName");
        }

        public static List<String> ListDiskDrive()
        {
            //https://docs.microsoft.com/en-us/windows/win32/cimwin32prov/win32-DiskDrive
            return ListWMIC("Win32_DiskDrive");
        }

        public static List<String> ListLogicalDisk()
        {
            //https://docs.microsoft.com/en-us/windows/win32/cimwin32prov/win32-LogicalDisk
            return ListWMIC("Win32_LogicalDisk");
        }

        public static List<String> ListLocalUser()
        {
            //https://docs.microsoft.com/en-us/windows/win32/cimwin32prov/win32-useraccount
            return ListWMIC("Win32_UserAccount");
//            return ListWMIC("Win32_Account","Caption\tDescription\tDomain\tInstallDate\tLocalAccount\tName\tSID\tSIDType\tStatus");
        }
        public static List<String> ListLocalGroup()
        {
            //https://docs.microsoft.com/en-us/windows/win32/cimwin32prov/win32-group
            return ListWMIC("Win32_Group");
        }
        public static List<String> ListLocalGroupUser()
        {
            List<String> ret = new List<string>();

            ret.Add("Group\tUser");

            String GroupComponent = "";
            String PartComponent = "";
            
            ManagementClass managementclass = new ManagementClass("Win32_GroupUser");
            ManagementObjectCollection managementobjectcollection = managementclass.GetInstances();
            foreach (ManagementObject managementobject in managementobjectcollection)
            {
                GroupComponent = managementobject.Properties["GroupComponent"].Value.ToString();
                PartComponent = managementobject.Properties["PartComponent"].Value.ToString();
                if(GroupComponent.IndexOf("Name=") > 0 && PartComponent.IndexOf("Name=") > 0 )
                {
                    GroupComponent = GroupComponent.Substring(GroupComponent.IndexOf("Name=")+5).Replace("\"","");
                    PartComponent = PartComponent.Substring(PartComponent.IndexOf("Name=")+5).Replace("\"","");
                    ret.Add(GroupComponent + "\t" + PartComponent);
                }
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
    }
}