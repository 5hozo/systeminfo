//http://pinvoke.net/default.aspx/wtsapi32/WTSQuerySessionInformation.html


using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace TerminalServices
{
    public class TSManager
    {
        [DllImport("wtsapi32.dll", SetLastError=true)]
        static extern IntPtr WTSOpenServer([MarshalAs(UnmanagedType.LPStr)] String pServerName);

        [DllImport("wtsapi32.dll")]
        static extern void WTSCloseServer(IntPtr hServer);

        [DllImport("wtsapi32.dll", SetLastError=true)]
        static extern Int32 WTSEnumerateSessions(
            IntPtr hServer,
            [MarshalAs(UnmanagedType.U4)] Int32 Reserved,
            [MarshalAs(UnmanagedType.U4)] Int32 Version,
            ref IntPtr ppSessionInfo,
            [MarshalAs(UnmanagedType.U4)] ref Int32 pCount);

        [DllImport("Wtsapi32.dll")]
        static extern bool WTSQuerySessionInformation(
            IntPtr hServer,
            Int32 sessionId,
            WTS_INFO_CLASS wtsInfoClass,
            out System.IntPtr ppBuffer,
            out uint pBytesReturned);

        [DllImport("wtsapi32.dll")]
        static extern void WTSFreeMemory(IntPtr pMemory);

        [StructLayout(LayoutKind.Sequential)]
        private struct WTS_SESSION_INFO
        {
            public Int32 SessionID;

            [MarshalAs(UnmanagedType.LPStr)]
            public String pWinStationName;

            public WTS_CONNECTSTATE_CLASS State;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct WTS_CLIENT_ADDRESS
        {
            public AddressFamilyType AddressFamily;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
            public byte[] Address;
        }

        public enum WTS_CONNECTSTATE_CLASS
        {
            WTSActive,
            WTSConnected,
            WTSConnectQuery,
            WTSShadow,
            WTSDisconnected,
            WTSIdle,
            WTSListen,
            WTSReset,
            WTSDown,
            WTSInit
        }
        public enum WTS_INFO_CLASS
        {
            WTSInitialProgram,
            WTSApplicationName,
            WTSWorkingDirectory,
            WTSOEMId,
            WTSSessionId,
            WTSUserName,
            WTSWinStationName,
            WTSDomainName,
            WTSConnectState,
            WTSClientBuildNumber,
            WTSClientName,
            WTSClientDirectory,
            WTSClientProductId,
            WTSClientHardwareId,
            WTSClientAddress,
            WTSClientDisplay,
            WTSClientProtocolType,
            WTSIdleTime,
            WTSLogonTime,
            WTSIncomingBytes,
            WTSOutgoingBytes,
            WTSIncomingFrames,
            WTSOutgoingFrames,
            WTSClientInfo,
            WTSSessionInfo
        }
        
        public enum AddressFamilyType
        {
            AF_INET,
            AF_INET6, 
            AF_IPX, 
            AF_NETBIOS, 
            AF_UNSPEC
        }

        public static IntPtr OpenServer(String Name)
        {
            IntPtr server = WTSOpenServer(Name);
            return server;
        }
        public static void CloseServer(IntPtr ServerHandle)
        {
            WTSCloseServer(ServerHandle);
        }
        public static List<String> ListSessions(String ServerName)
        {
            IntPtr server = IntPtr.Zero;
            List<String> ret = new List<string>();
            server = OpenServer(ServerName);

            try
            {
                IntPtr ppSessionInfo = IntPtr.Zero;

                Int32 count = 0;
                Int32 retval = WTSEnumerateSessions(server, 0, 1, ref ppSessionInfo, ref count);
                Int32 dataSize = Marshal.SizeOf(typeof(WTS_SESSION_INFO));
                Int64 current = (Int64)ppSessionInfo;

                if (retval != 0)
                {
                    for (int i = 0; i < count; i++)
                    {
                        WTS_SESSION_INFO si = (WTS_SESSION_INFO)Marshal.PtrToStructure((System.IntPtr)current, typeof(WTS_SESSION_INFO));
                        current += dataSize;

                        string clientname = GetWTSQuerySessionInformation(server,si.SessionID,WTS_INFO_CLASS.WTSClientName);
                        string domainname = GetWTSQuerySessionInformation(server,si.SessionID,WTS_INFO_CLASS.WTSDomainName);
                        string username = GetWTSQuerySessionInformation(server,si.SessionID,WTS_INFO_CLASS.WTSUserName);


                        if (("console" == si.pWinStationName.ToLower())
                        ||  ("services" == si.pWinStationName.ToLower()))
                        {
                            ret.Add(si.SessionID + "\t" + si.State + "\t" + si.pWinStationName + "\t" + ConsoleServices.ConsoleManager.GetIPAddress() + "\t" + clientname + "\t" + domainname + "\t" + username);
                        }
                        else
                        {
                            ret.Add(si.SessionID + "\t" + si.State + "\t" + si.pWinStationName + "\t" + GetIPAddress(server,si.SessionID) + "\t" + clientname + "\t" + domainname + "\t" + username);
                        }   
                    }

                    WTSFreeMemory(ppSessionInfo);
                }
            }
            finally
            {
                CloseServer(server);
            }

            return ret;
        }

        private static string GetIPAddress(IntPtr hServer,Int32 sessionId)
        {

            string ret = "";
            IntPtr ppBuffer;
            uint iReturned;

            if (WTSQuerySessionInformation(hServer,
                sessionId, 
                WTS_INFO_CLASS.WTSClientAddress,
                out ppBuffer,
                out iReturned))
            {
                var clientAddres = (WTS_CLIENT_ADDRESS)Marshal.PtrToStructure(ppBuffer, typeof(WTS_CLIENT_ADDRESS));
                if (clientAddres.AddressFamily == AddressFamilyType.AF_INET)
                {
                    ret = string.Join(".", clientAddres.Address.Skip(2).Take(4));
                }
            }

            return ret;
        }
        private static string GetWTSQuerySessionInformation(IntPtr hServer,Int32 sessionId, WTS_INFO_CLASS wtsInfoClass)
        {
            string ret = "";
            IntPtr ppBuffer;
            uint iReturned;

            if (WTSQuerySessionInformation(hServer,
                sessionId, 
                wtsInfoClass,
                out ppBuffer,
                out iReturned))
            {
                ret = Marshal.PtrToStringAnsi(ppBuffer);
            }

            return ret;
        }
    }
}