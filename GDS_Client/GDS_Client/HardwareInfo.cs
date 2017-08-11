using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;

namespace GDS_Client
{
    public class HardwareInfo
    {
        public static string GetBoardProductId()
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_BaseBoard");

            foreach (ManagementObject wmi in searcher.Get())
            {
                try
                {
                    string BoardID = wmi.GetPropertyValue("Product").ToString();
                    return BoardID;
                }
                catch { }
            }
            return "Unknown";
        }

        public static string GetIPAddress()
        {
            string wmiQuery = "SELECT * FROM Win32_NetworkAdapter WHERE NetConnectionId != NULL";
            ManagementObjectSearcher moSearch = new ManagementObjectSearcher(wmiQuery);
            ManagementObjectCollection moCollection = moSearch.Get();
            string IPAddress = "Not Found,";
            foreach (ManagementObject mo in moCollection)
            {
                string pnp = mo["PNPDeviceID"].ToString();
                if (pnp.Contains("PCI\\"))
                {
                    ManagementObjectSearcher query = new ManagementObjectSearcher("SELECT * FROM Win32_NetworkAdapterConfiguration WHERE IPEnabled = 'TRUE' AND Index =" + mo["Index"]);
                    ManagementObjectCollection queryCollection = query.Get();
                    foreach (ManagementObject mo2 in queryCollection)
                    {
                        string[] addresses = (string[])mo2["IPAddress"];
                        {
                            IPAddress += addresses[0] + ",";
                        }
                    }
                }
            }
            if (IPAddress != "Not Found,")
            {
                IPAddress = IPAddress.Replace("Not Found,", "");
            }
            else
            {
                string hostName = Dns.GetHostName();
                IPAddress = Dns.GetHostByName(hostName).AddressList[0].ToString();
            }
            if (IPAddress.Length != 0)
            {
                IPAddress = IPAddress.Substring(0, IPAddress.Length - 1);
            }
            return IPAddress;
        }

        public static string GetBIOScaption()
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_BIOS");

            foreach (ManagementObject wmi in searcher.Get())
            {
                try
                {
                    string BiosCaption = wmi.GetPropertyValue("Caption").ToString();
                    return BiosCaption;
                }
                catch { }
            }
            return "Unknown";
        }

        public static string GetWorkGroup()
        {
            ManagementObject computer_system = new ManagementObject(
                        string.Format(
                        "Win32_ComputerSystem.Name='{0}'",
                        Environment.MachineName));

            object result = computer_system["Workgroup"];
            return result.ToString();
        }

        public static string GetAccountName()
        {
            if (File.Exists(@"D:\Temp\User.txt"))
            {
                return File.ReadLines(@"D:\Temp\User.txt").First();
            }
            /*ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_UserAccount");

            foreach (ManagementObject wmi in searcher.Get())
            {
                try
                {
                    string UserAccName = wmi.GetPropertyValue("Name").ToString();
                    return "User Account Name||" + UserAccName;
                }
                catch { }
            }*/
            if(Environment.UserName != null)
            {
                return Environment.UserName;
            }
            return "Unknown";
        }

        public static string GetPhysicalMemory()
        {
            ManagementScope oMs = new ManagementScope();
            ObjectQuery oQuery = new ObjectQuery("SELECT Capacity FROM Win32_PhysicalMemory");
            ManagementObjectSearcher oSearcher = new ManagementObjectSearcher(oMs, oQuery);
            ManagementObjectCollection oCollection = oSearcher.Get();

            long MemSize = 0;
            long mCap = 0;

            // In case more than one Memory sticks are installed
            foreach (ManagementObject obj in oCollection)
            {
                mCap = Convert.ToInt64(obj["Capacity"]);
                MemSize += mCap;
            }
            MemSize = (MemSize / 1024) / 1024;

            string MemorySizeMB = MemSize.ToString() + "MB";
            return MemorySizeMB;
        }

        public static string GetNoRamSlots()
        {
            int MemSlots = 0;
            ManagementScope oMs = new ManagementScope();
            ObjectQuery oQuery2 = new ObjectQuery("SELECT MemoryDevices FROM Win32_PhysicalMemoryArray");
            ManagementObjectSearcher oSearcher2 = new ManagementObjectSearcher(oMs, oQuery2);
            ManagementObjectCollection oCollection2 = oSearcher2.Get();
            foreach (ManagementObject obj in oCollection2)
            {
                MemSlots = Convert.ToInt32(obj["MemoryDevices"]);
            }
            return MemSlots.ToString();
        }

        public static string GetOSInformation()
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");
            foreach (ManagementObject wmi in searcher.Get())
            {
                try
                {
                    return ((string)wmi["Caption"]).Trim() + ", " + (string)wmi["OSArchitecture"];
                }
                catch { }
            }
            return "Unknown";
        }

        public static String GetProcessorInformation()
        {
            ManagementClass mc = new ManagementClass("win32_processor");
            ManagementObjectCollection moc = mc.GetInstances();
            String info = String.Empty;
            foreach (ManagementObject mo in moc)
            {
                string name = (string)mo["Name"];
                name = name.Replace("(TM)", "™").Replace("(tm)", "™").Replace("(R)", "®").Replace("(r)", "®").Replace("(C)", "©").Replace("(c)", "©").Replace("    ", " ").Replace("  ", " ");

                info = name;
            }
            return info;
        }

        public static string GetMacAddresses()
        {
            try
            {
                List<string> macAddresses = new List<string>();
                ManagementObjectSearcher searcher = new ManagementObjectSearcher
    ("Select MACAddress,PNPDeviceID FROM Win32_NetworkAdapter WHERE MACAddress IS NOT NULL AND PNPDeviceID IS NOT NULL");
                ManagementObjectCollection mObject = searcher.Get();

                foreach (ManagementObject obj in mObject)
                {
                    string pnp = obj["PNPDeviceID"].ToString();
                    if (pnp.Contains("PCI\\"))
                    {
                        macAddresses.Add(obj["MACAddress"].ToString()); 
                    }
                }

                return String.Join("&", macAddresses);
                /*string macAddresses = "";
                ManagementObjectSearcher mos = new ManagementObjectSearcher("select MACAddress, PNPDeviceID from Win32_NetworkAdapter Where AdapterType='Ethernet 802.3'");
                foreach (ManagementObject mo in mos.Get())
                {
                    string pnp = mo["PNPDeviceID"].ToString();
                    if (pnp.Contains("PCI\\"))
                    {
                        string mac = mo["MACAddress"].ToString();
                        macAddresses += mac + "&";
                    }
                }

                macAddresses = macAddresses.Substring(0, macAddresses.Length - 1);
                if (macAddresses != "")
                {
                    return macAddresses;
                }
                GetMacAddresses();
                return "";                
                /*
                List<string> macAddresses = new List<string>();

                foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (nic.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                        macAddresses.Add(nic.GetPhysicalAddress().ToString());
                }

               
                return String.Join("&", macAddresses);  
                /*
                ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
                ManagementObjectCollection moc = mc.GetInstances();
                List<string> MACAddress = new List<string>();
                foreach (ManagementObject mo in moc)
                {
                    if ((bool)mo["IPEnabled"] == true) MACAddress.Add(mo["MacAddress"].ToString());
                    mo.Dispose();
                }
                return String.Join("&", MACAddress);*/
            }
            catch
            {
                Thread.Sleep(1000);
                GetMacAddresses();
                return "";
            }
        }
    }
}
