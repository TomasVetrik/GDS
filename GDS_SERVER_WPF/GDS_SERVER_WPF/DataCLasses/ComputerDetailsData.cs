using GDS_SERVER_WPF.DataCLasses;
using NetworkCommsDotNet.Connections;
using NetworkCommsDotNet.Tools;
using ProtoBuf;
using System;
using System.Collections.Generic;

namespace GDS_SERVER_WPF
{
    [ProtoContract]
    public class ComputerDetailsData
    {
        public ComputerDetailsData()
        {
            macAddresses = new List<string>();
            PostInstalls = new List<string>();
        }
        public ComputerDetailsData(string _name, string _macAddress, string _IPAddress, string _realPCName, string _detail, string _imageSource = "Images/Offline.ico")
        {
            this.ImageSource = _imageSource;
            this.Name = _name;
            this.MacAddress = _macAddress;
            this.IPAddress = _IPAddress;
            this.RealPCName = _realPCName;
            this.Detail = _detail;
            macAddresses = new List<string>();
            PostInstalls = new List<string>();
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

        public override string ToString()
        {
            return Name.ToString();
        }

        public void LoadDataFromList(List<string> list)
        {
            foreach (string line in list)
            {
                if (line != "")
                {
                    if (line.Contains("Computer Name||"))
                    {
                        string[] splitter = line.Split(new string[] { "||" }, StringSplitOptions.None);
                        RealPCName = splitter[1];                        
                    }
                    if (line.Contains("MacAddress||"))
                    {
                        string[] splitter = line.Split(new string[] { "||" }, StringSplitOptions.None);
                        if (splitter[1].Contains("&"))
                        {
                            macAddresses = new List<string>(splitter[1].Split('&'));
                            MacAddress = macAddresses[0];
                        }
                        else
                        {
                            MacAddress = splitter[1];
                            macAddresses.Add(MacAddress);
                        }
                    }
                    if (line.Contains("OS Informations||"))
                    {
                        string[] splitter = line.Split(new string[] { "||" }, StringSplitOptions.None);
                        OSInformations = splitter[1];
                    }
                    if (line.Contains("Processor Informations||"))
                    {
                        string[] splitter = line.Split(new string[] { "||" }, StringSplitOptions.None);
                        processorInfo = splitter[1];
                    }
                    if (line.Contains("Memory Size||"))
                    {
                        string[] splitter = line.Split(new string[] { "||" }, StringSplitOptions.None);
                        physicalMemoryInfo = splitter[1];
                    }
                    if (line.Contains("Used Memory Slots||"))
                    {
                        string[] splitter = line.Split(new string[] { "||" }, StringSplitOptions.None);
                        numberOfRamSLots = splitter[1];
                    }
                    if (line.Contains("BIOS Caption||"))
                    {
                        string[] splitter = line.Split(new string[] { "||" }, StringSplitOptions.None);
                        biosCaption = splitter[1];
                    }
                    if (line.Contains("Board ID||"))
                    {
                        string[] splitter = line.Split(new string[] { "||" }, StringSplitOptions.None);
                        boardProductId = splitter[1];
                    }
                    if (line.Contains("User Account Name||"))
                    {
                        string[] splitter = line.Split(new string[] { "||" }, StringSplitOptions.None);
                        accountName = splitter[1];
                    }                    
                    if (line.Contains("IP Address||"))
                    {
                        string[] splitter = line.Split(new string[] { "||" }, StringSplitOptions.None);
                        IPAddress = splitter[1];
                    }
                    if (line.Contains("BASE NAME||"))
                    {
                        string[] splitter = line.Split(new string[] { "||" }, StringSplitOptions.None);
                        baseImageName = splitter[1];
                    }
                    if (line.Contains("DRIVEE NAME||"))
                    {
                        string[] splitter = line.Split(new string[] { "||" }, StringSplitOptions.None);
                        driveEImageName = splitter[1];
                    }
                    if (line.Contains("Dart Viewer||"))
                    {
                        string[] splitter = line.Split(new string[] { "||" }, StringSplitOptions.None);
                        dartInfo = splitter[1];
                    }
                }
            }            
        }

        public List<ItemData> GetItems()
        {
            List<ItemData> items = new List<ItemData>
            {
                new ItemData("Computer Name", RealPCName),
                new ItemData("Mac Address", String.Join(" | ", macAddresses)),
                new ItemData("OS Informations", OSInformations),
                new ItemData("Processor Info", processorInfo),
                new ItemData("Memory Info", physicalMemoryInfo),
                new ItemData("Number of Slots", numberOfRamSLots),
                new ItemData("Bios Caption", biosCaption),
                new ItemData("Board Production ID", boardProductId),
                new ItemData("Account Name", accountName),
                new ItemData("IP Address", IPAddress),
                new ItemData("Base Image", baseImageName),
                new ItemData("DriveE Image", driveEImageName),
                new ItemData("Dart Info", dartInfo)
            };
            if (CustomLog != null)
            {
                items.Add(new ItemData("Custom Log", CustomLog.Replace(",", "\n")));
            }
            return items;
        }
    }
}
