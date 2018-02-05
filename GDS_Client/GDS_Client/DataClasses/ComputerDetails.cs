using NetworkCommsDotNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace GDS_Client
{
    public class ComputerDetails
    {       
        public ComputerDetailsData computerDetailsData { get; set; }

        public List<string> computerDetails;        
        public ComputerDetails()
        {
            computerDetailsData = new ComputerDetailsData();
            CheckIfImInWipne();
        }

        void CheckIfImInWipne()
        {
            try
            {
                if (File.Exists(@"X:\Windows\System32\wpeinit.exe"))
                {
                    computerDetailsData.inWinpe = true;
                }
                else
                {
                    computerDetailsData.inWinpe = false;
                }
            }
            catch
            {
                Thread.Sleep(1000);
                CheckIfImInWipne();
            }
        }

        string FileName = @"D:\Temp\GDSClient\GDS_Client_LOG.txt";

        public void WriteToLogs(string LOG)
        {
            Console.WriteLine(LOG);
            if (!computerDetailsData.inWinpe)
            {
                if (File.Exists(FileName))
                {
                    FileInfo FI = new FileInfo(FileName);
                    if (FI.Length > 2000000)
                    {
                        FI.Delete();
                    }
                }
                using (StreamWriter sw = File.AppendText(FileName))
                {
                    sw.WriteLine(DateTime.Now.ToString() + ": " + LOG);
                }
            }
        }

        public void SetComputerDetails()
        {
            try
            {
                computerDetails = new List<string>();
                computerDetailsData.RealPCName = System.Environment.MachineName;
                computerDetails.Add("Computer Name||" + computerDetailsData.RealPCName);

                var MacAddress = HardwareInfo.GetMacAddresses();                
                if (MacAddress.Count == 0)
                {
                    SetComputerDetails();
                    return;
                }
                computerDetailsData._sourceIdentifier = NetworkComms.NetworkIdentifier;
                computerDetailsData.macAddresses = MacAddress;
                computerDetailsData.MacAddress = MacAddress[0];
                computerDetails.Add("MacAddress||" + MacAddress);
                computerDetailsData.OSInformations = HardwareInfo.GetOSInformation();                
                computerDetails.Add("OS Informations||" + computerDetailsData.OSInformations);
                computerDetailsData.processorInfo = HardwareInfo.GetProcessorInformation();
                computerDetails.Add("Processor Informations||" + computerDetailsData.processorInfo);
                computerDetailsData.physicalMemoryInfo = HardwareInfo.GetPhysicalMemory();
                computerDetails.Add("Memory Size||" + computerDetailsData.physicalMemoryInfo);
                computerDetailsData.numberOfRamSLots = HardwareInfo.GetNoRamSlots();
                computerDetails.Add("Used Memory Slots||" + computerDetailsData.numberOfRamSLots);
                computerDetailsData.biosCaption = HardwareInfo.GetBIOScaption();
                computerDetails.Add("BIOS Caption||" + computerDetailsData.biosCaption);
                computerDetailsData.boardProductId = HardwareInfo.GetBoardProductId();
                computerDetails.Add("Board ID||" + computerDetailsData.boardProductId);
                computerDetailsData.accountName = HardwareInfo.GetAccountName();
                computerDetails.Add("User Account Name||" + computerDetailsData.accountName);
                computerDetailsData.baseImageName = GetBaseImageName();
                computerDetails.Add("BASE NAME||" + computerDetailsData.baseImageName);
                computerDetailsData.driveEImageName = GeDriveEImageName();
                computerDetails.Add("DRIVEE NAME||" + computerDetailsData.driveEImageName);
                computerDetailsData.dartInfo = GetDartViewerInfo(0);
                computerDetails.Add("Dart Viewer||" + computerDetailsData.dartInfo);
                computerDetailsData.CustomLog = "";
                computerDetails.Add("Custom Log||" + computerDetailsData.CustomLog);

            }
            catch (Exception ex)
            {
                WriteToLogs("SOMETHING WRONG WITH GET COMPUTER DETAILS: " + ex.ToString());
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
            if (computerDetailsData.inWinpe)
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
            if (computerDetailsData.inWinpe)
            {
                filePath = @"C:\DriveEImage.txt";
            }
            if (File.Exists(filePath))
            {
                name += File.ReadLines(filePath).First();
            }
            return name;
        }

        string GetDartViewerInfo(int counter)
        {
            if (computerDetailsData.inWinpe)
            {
                string path = @"X:\DartViewer.txt";
                if (File.Exists(path))
                {
                    string dartViewerDetails = File.ReadAllLines(path).First();
                    return dartViewerDetails;
                }
                else
                {
                    Console.WriteLine("Dart Viewer File not exists");
                    if (counter != 5)
                    {
                        Thread.Sleep(2000);
                        counter++;
                        return GetDartViewerInfo(counter);
                    }
                }
            }
            return "";
        }
    }
}
