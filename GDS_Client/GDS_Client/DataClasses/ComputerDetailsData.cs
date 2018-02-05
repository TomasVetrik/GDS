using NetworkCommsDotNet.Connections;
using NetworkCommsDotNet.Tools;
using ProtoBuf;
using System;
using System.Collections.Generic;

namespace GDS_Client
{
    [ProtoContract]
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

        [ProtoMember(1)]
        public string Name { set; get; }
        [ProtoMember(2)]
        public string RealPCName { set; get; }
        [ProtoMember(3)]
        public List<string> macAddresses { set; get; }
        [ProtoMember(4)]
        public string MacAddress { set; get; }
        [ProtoMember(5)]
        public string OSInformations { set; get; }
        [ProtoMember(6)]
        public string processorInfo { set; get; }
        [ProtoMember(7)]
        public string physicalMemoryInfo { set; get; }
        [ProtoMember(8)]
        public string numberOfRamSLots { set; get; }
        [ProtoMember(9)]
        public string biosCaption { set; get; }
        [ProtoMember(10)]
        public string boardProductId { set; get; }
        [ProtoMember(11)]
        public string accountName { set; get; }
        [ProtoMember(12)]
        public string IPAddress { set; get; }
        [ProtoMember(13)]
        public string baseImageName { set; get; }
        [ProtoMember(14)]
        public string driveEImageName { set; get; }
        [ProtoMember(15)]
        public string dartInfo { set; get; }
        [ProtoMember(16)]
        public string Detail { set; get; }
        [ProtoMember(17)]
        public string ImageSource { set; get; }
        [ProtoMember(18)]
        public string pathNode { set; get; }
        [ProtoMember(19)]
        public bool inWinpe { get; set; }
        [ProtoMember(20)]
        public List<string> PostInstalls { get; set; }
        [ProtoMember(21)]
        public string _sourceIdentifier;
        public ShortGuid SourceIdentifier { get { return new ShortGuid(_sourceIdentifier); } }
        [ProtoMember(22)]
        public string CustomLog { get; set; }
    }
}
