using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GDS_SERVER_WPF.Handlers
{
    public class WakeOnLanHandler
    {
        public static IPAddress getBroadcastIP(string IPAdd)
        {
            try
            {
                IPAddress maskIP = IPAddress.Parse("255.255.0.0");
                IPAddress hostIP = IPAddress.Parse(IPAdd);

                if (maskIP == null || hostIP == null)
                    return null;

                byte[] complementedMaskBytes = new byte[4];
                byte[] broadcastIPBytes = new byte[4];

                for (int i = 0; i < 4; i++)
                {
                    complementedMaskBytes[i] = (byte)~(maskIP.GetAddressBytes().ElementAt(i));
                    broadcastIPBytes[i] = (byte)((hostIP.GetAddressBytes().ElementAt(i)) | complementedMaskBytes[i]);
                }
                return new IPAddress(broadcastIPBytes);
            }
            catch
            {

                return IPAddress.Parse("255.255.255.255");
            }
        }


        public static void runWakeOnLan(List<string> macAddress, List<string> ipAddresses)
        {
            for (int i = 0; i < macAddress.Count; i++)
            {
                string MacAddress = macAddress[i];
                if (MacAddress != "")
                {
                    try
                    {
                        MacAddress = MacAddress.Replace(":", "-");
                        MacAddress = MacAddress.Replace(".", "-");
                        MacAddress = MacAddress.Replace("_", "-");
                        MacAddress = MacAddress.Replace(" ", "-");
                        MacAddress = MacAddress.Replace(",", "-");
                        MacAddress = MacAddress.Replace(";", "-");

                        MacAddress = MacAddress.Replace("-", "");
                        byte[] mac = new byte[6];
                        for (int k = 0; k < 6; k++)
                        {
                            mac[k] = Byte.Parse(MacAddress.Substring(k * 2, 2), System.Globalization.NumberStyles.HexNumber);
                        }
                        foreach (string IP in ipAddresses)
                        {
                            var endPoint = new IPEndPoint(getBroadcastIP(IP), 0);
                            endPoint.SendWol(mac[0], mac[1], mac[2], mac[3], mac[4], mac[5]);
                        }
                    }
                    catch
                    {

                    }
                }
            }
        }
    }
}
