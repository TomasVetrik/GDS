using GDS_SERVER_WPF.DataCLasses;
using System;
using System.Collections.Generic;

namespace GDS_SERVER_WPF
{    
    public class ComputerDetailsData     
    {        
        public string computerName { set; get; }
        public List<string> macAddresses { set; get; }
        public string MacAddress { set; get; }
        public string OSInformations { set; get; }
        public string processorInfo { set; get; }
        public string physicalMemoryInfo { set; get; }
        public string numberOfRamSLots { set; get; }
        public string biosCaption { set; get; }
        public string boardProductId { set; get; }
        public string accountName { set; get; }
        public string ipAddress { set; get; }
        public string baseImageName { set; get; }
        public string driveEImageName { set; get; }
        public string dartInfo { set; get; }
        public string pathNode { set; get; }        

        public void LoadDataFromList(List<string> list)
        {
            foreach (string line in list)
            {
                if (line != "")
                {
                    if (line.Contains("Computer Name||"))
                    {
                        string[] splitter = line.Split(new string[] { "||" }, StringSplitOptions.None);
                        computerName = splitter[1];
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
                        ipAddress = splitter[1];
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
            List<ItemData> items = new List<ItemData>();
            items.Add(new ItemData("Computer Name", computerName));
            items.Add(new ItemData("Mac Address", String.Join(" | ", macAddresses)));
            items.Add(new ItemData("OS Informations", OSInformations));
            items.Add(new ItemData("Processor Info", processorInfo));
            items.Add(new ItemData("Memory Info", physicalMemoryInfo));
            items.Add(new ItemData("Number of Slots", numberOfRamSLots));
            items.Add(new ItemData("Bios Caption", biosCaption));
            items.Add(new ItemData("Board Production ID", boardProductId));
            items.Add(new ItemData("Account Name", accountName));
            items.Add(new ItemData("IP Address", ipAddress));
            items.Add(new ItemData("Base Image", baseImageName));
            items.Add(new ItemData("DriveE Image", driveEImageName));
            items.Add(new ItemData("Dart Info", dartInfo));
            return items;
        }
    }
}
