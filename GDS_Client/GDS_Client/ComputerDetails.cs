using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace GDS_Client
{
    public class ComputerDetails
    {
        public string computerName { set; get; }
        public string macAddress { set; get; }
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

        public List<string> computerDetails;

        public bool inWinpe;
        public ComputerDetails()
        {
            CheckIfImInWipne();
        }

        void CheckIfImInWipne()
        {
            try
            {
                if (File.Exists(@"X:\Windows\System32\wpeinit.exe"))
                {
                    inWinpe = true;
                }
                else
                {
                    inWinpe = false;
                }
            }
            catch
            {
                Thread.Sleep(1000);
                CheckIfImInWipne();
            }
        }

        public void SetComputerDetails()
        {
            try
            {
                computerDetails = new List<string>();
                computerDetails.Add("Computer Name||" + System.Environment.MachineName);

                var MacAddress = HardwareInfo.GetMacAddresses();                
                if (MacAddress == "")
                {
                    SetComputerDetails();
                    return;
                }
                macAddress = MacAddress;
                computerDetails.Add("MacAddress||" + macAddress);
                OSInformations = HardwareInfo.GetOSInformation();                
                computerDetails.Add("OS Informations||" + OSInformations);
                processorInfo = HardwareInfo.GetProcessorInformation();
                computerDetails.Add("Processor Informations||" + processorInfo);
                physicalMemoryInfo = HardwareInfo.GetPhysicalMemory();
                computerDetails.Add("Memory Size||" + physicalMemoryInfo);
                numberOfRamSLots = HardwareInfo.GetNoRamSlots();
                computerDetails.Add("Used Memory Slots||" + numberOfRamSLots);
                biosCaption = HardwareInfo.GetBIOScaption();
                computerDetails.Add("BIOS Caption||" + biosCaption);
                boardProductId = HardwareInfo.GetBoardProductId();
                computerDetails.Add("Board ID||" + boardProductId);
                accountName = HardwareInfo.GetAccountName();
                computerDetails.Add("User Account Name||" + accountName);
                baseImageName = GetBaseImageName();
                computerDetails.Add("BASE NAME||" + baseImageName);
                driveEImageName = GeDriveEImageName();
                computerDetails.Add("DRIVEE NAME||" + driveEImageName);
                dartInfo = GetDartViewerInfo();
                computerDetails.Add("Dart Viewer||" + dartInfo + "END");                
            }
            catch
            {
                Console.WriteLine("SOMETHING WRONG WITH GET COMPUTER DETAILS");
                Thread.Sleep(5000);
                SetComputerDetails();
            }
        }

        public List<string> ToList()
        {
            return computerDetails;
        }

        string GetBaseImageName()
        {
            string name = "";
            string filePath = @"D:\BaseImage.txt";
            if (inWinpe)
            {
                filePath = @"C:\BaseImage.txt";
            }
            if (File.Exists(filePath))
            {
                name += File.ReadLines(filePath).First();
            }
            return name;
        }

        string GeDriveEImageName()
        {
            string name = "";
            string filePath = @"D:\DriveEImage.txt";
            if (inWinpe)
            {
                filePath = @"C:\DriveEImage.txt";
            }
            if (File.Exists(filePath))
            {
                name += File.ReadLines(filePath).First();
            }
            return name;
        }

        string GetDartViewerInfo()
        {
            if (inWinpe)
            {
                string path = @"X:\DartViewer.txt";
                if (File.Exists(path))
                {
                    string dartViewerDetails = File.ReadAllLines(path).First();
                    return dartViewerDetails;
                }
            }
            return "";
        }
    }
}
