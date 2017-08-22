using System;
using System.Collections.Generic;

namespace GDS_Client
{
    public class ComputerDetailsData
    {
        public ComputerDetailsData()
        {
            macAddresses = new List<string>();
        }
        public ComputerDetailsData(string _name, string _macAddress, string _IPAddress, string _realPCName, string _detail, string _imageSource = "Images/Offline.ico")
        {
            this.ImageSource = _imageSource;
            this.Name = _name;
            this.MacAddress = _macAddress;
            this.IPAddress = _IPAddress;
            this.RealPCName = _realPCName;
            this.Detail = _detail;
        }

        public string Name { set; get; }
        public string RealPCName { set; get; }
        public List<string> macAddresses { set; get; }
        public string MacAddress { set; get; }
        public string OSInformations { set; get; }
        public string processorInfo { set; get; }
        public string physicalMemoryInfo { set; get; }
        public string numberOfRamSLots { set; get; }
        public string biosCaption { set; get; }
        public string boardProductId { set; get; }
        public string accountName { set; get; }
        public string IPAddress { set; get; }
        public string baseImageName { set; get; }
        public string driveEImageName { set; get; }
        public string dartInfo { set; get; }
        public string Detail { set; get; }
        public string ImageSource { set; get; }
        public string pathNode { set; get; }
        public bool inWinpe { get; set; }
    }
}
