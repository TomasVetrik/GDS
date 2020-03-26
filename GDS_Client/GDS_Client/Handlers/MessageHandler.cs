using GDS_Client.Handlers;
using NetworkCommsDotNet.Connections;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace GDS_Client
{
    public class MessageHandler
    {
        public Listener listener;

        Thread cloningThread;
        Thread shutdowningThread;
        public bool cloning = false;
        bool cloningDoneReceive = false;
        bool shutdowning = false;
        bool shutdowningDoneReceive = false;
        bool runningCommands = false;
        bool restarting = false;
        Process CloneProcess;
        TCP_UNICAST tcp_unicast = null;

        public MessageHandler(Listener _listener)
        {
            this.listener = _listener;
        }

        readonly string FileName = @"D:\Temp\GDSClient\GDS_Client_LOG.txt";

        public void WriteToLogs(string LOG)
        {
            try
            {
                Console.WriteLine(DateTime.Now.ToString() + ": " + LOG);
                if (!listener.computerDetails.computerDetailsData.inWinpe)
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
            catch
            {
                Console.WriteLine("Problem so zapisom do Logu");
            }
        }

        public void WorkingOnTask()
        {
            try
            {                
                File.WriteAllText(@"X:\CloneStatus.txt", "RUNNING POWERSHELL SCRIPT");                
                File.WriteAllText(@"X:\Error.txt", "False");               
                CloneProcess = System.Diagnostics.Process.Start(@"X:\windows\system32\WindowsPowershell\v1.0\powershell.exe", @"-WindowStyle Maximized -executionpolicy unrestricted -File W:\Cloning.ps1");                
                cloning = true;
                string CloneMessage = "CLONING";
                string CloneMessage_OLD = "OLD";
                while (listener.running && CloneMessage != "FALSE" && CloneMessage != "TRUE" && cloning && !CloneMessage.Contains("CLONE FAILED"))
                {                    
                    try
                    {                        
                        if (CloneMessage != null && listener.computerDetails.computerDetailsData != null && CloneMessage != "")
                        {
                            if (CloneMessage_OLD != CloneMessage)
                            {                                
                                listener.SendMessage(new Packet(FLAG.CLONING_STATUS, listener.computerDetails.computerDetailsData, CloneMessage));                                
                                CloneMessage_OLD = CloneMessage;                         
                            }
                        }                        
                        CloneMessage = File.ReadLines(@"X:\CloneStatus.txt").First();                 
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Problem with cloning: " + ex.ToString());
                    }
                    Thread.Sleep(5000);
                    Console.WriteLine(DateTime.Now + " " + CloneMessage);
                }                
                var dataIdentifier = FLAG.CLONING_DONE;
                if (CloneMessage == "FALSE" || CloneMessage.Contains("CLONE FAILED"))
                    dataIdentifier = FLAG.RESTART;
                cloningDoneReceive = false;                                
                if (cloning)
                {
                    while (!cloningDoneReceive && listener.running)
                    {
                        listener.SendMessage(new Packet(dataIdentifier, listener.computerDetails.computerDetailsData, CloneMessage));                        
                        Thread.Sleep(1000);
                    }
                }                
                cloning = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        void RunCommand(string FileName, string Arguments)
        {           
            Process proc = new Process();
            proc.StartInfo.FileName = FileName;
            proc.StartInfo.Arguments = Arguments;
            proc.StartInfo.Verb = "runas";
            proc.Start();
            if(listener.computerDetails.computerDetailsData.inWinpe)
            {
                Console.WriteLine("Waiting for process");
                proc.WaitForExit();
                Console.WriteLine("Closing proces");
            }
        }

        private void Shutdowning()
        {
            shutdowning = true;
            while(!shutdowningDoneReceive)
            {
                Thread.Sleep(500);
                listener.SendMessage(new Packet(FLAG.SHUTDOWN_DONE, listener.computerDetails.computerDetailsData));
            }
            Process.Start("shutdown", "/s /t 0");
        }

        public void HandleMessage(Packet packet, Connection connection)
        {
            try
            {
                WriteToLogs(packet.ID.ToString());
                switch (packet.ID)
                {
                    case FLAG.REFRESH_COMPUTER_DETAILS_DATA:
                        {
                            listener.computerDetails.computerDetailsData.CustomLog = HardwareInfo.GetInstalledApps();
                            listener.SendMessage(new Packet(FLAG.SYN_FLAG, listener.computerDetails.computerDetailsData));                            
                            break;
                        }
                    case FLAG.SYN_FLAG:
                        {
                            listener.SendMessage(new Packet(FLAG.SYN_FLAG, listener.computerDetails.computerDetailsData));
                            break;
                        }                    
                    case FLAG.CLOSE:
                        {
                            //Environment.Exit(0);
                            break;
                        }
                    case FLAG.SEND_TASK_CONFIG:
                        {
                            bool error = false;
                            try
                            {
                                packet.taskData.TargetComputers = new System.Collections.Generic.List<ComputerDetailsData>
                                {
                                    listener.computerDetails.computerDetailsData
                                };
                                if (listener.computerDetails.computerDetailsData.inWinpe)
                                {
                                    try
                                    {
                                        if (File.Exists(@"X:\TaskData.my"))
                                        {
                                            File.Delete(@"X:\TaskData.my");
                                            Thread.Sleep(1000);
                                        }                                
                                        FileHandler.Save<TaskData>(packet.taskData, @"X:\TaskData.my");
                                    }
                                    catch(Exception ex)
                                    {
                                        Console.WriteLine("There is problem with TASK DATA SAVING!!" + ex.ToString());
                                    }
                                }
                                else
                                    FileHandler.Save<TaskData>(packet.taskData, @".\TaskData.my");
                            }
                            catch(Exception ex)
                            {
                                error = true;
                                Console.WriteLine("There is problem with TASK DATA!!" + ex.ToString());
                            }
                            if(!error)
                                listener.SendMessage(new Packet(FLAG.SEND_TASK_CONFIG, listener.computerDetails.computerDetailsData));
                            break;
                        }
                    case FLAG.FINISH_RUN_COMMAND:
                        {
                            Console.WriteLine("WAITING FOR FINISH RUN COMMAND");
                            break;
                        }
                    case FLAG.RUN_COMMAND:
                        {
                            listener.SendMessage(new Packet(FLAG.RUN_COMMAND, listener.computerDetails.computerDetailsData));
                            if (!runningCommands)
                            {
                                runningCommands = true;
                                string pathTaskData = @".\TaskData.my";
                                if (listener.computerDetails.computerDetailsData.inWinpe)
                                    pathTaskData = @"X:\TaskData.my";
                                if (File.Exists(pathTaskData))
                                {
                                    var taskData = FileHandler.Load<TaskData>(pathTaskData);
                                    var listCommands = taskData.CommandsInOS;
                                    if (listener.computerDetails.computerDetailsData.inWinpe)
                                        listCommands = taskData.CommandsInWINPE;
                                    foreach (string arguments in listCommands)
                                    {
                                        RunCommand("cmd.exe", "/C " + arguments);
                                    }
                                    listener.SendMessage(new Packet(FLAG.FINISH_RUN_COMMAND, listener.computerDetails.computerDetailsData));
                                    runningCommands = false;
                                }
                            }
                            break;
                        }
                    case FLAG.CLONING:
                        {
                            listener.SendMessage(new Packet(FLAG.CLONING, listener.computerDetails.computerDetailsData));
                            if (!cloning)
                            {
                                cloningThread = new Thread(WorkingOnTask);
                                cloningThread.Start();
                            }
                            break;
                        }
                    case FLAG.RESTART:
                        {
                            if (cloning)
                            {
                                Process[] procs = Process.GetProcessesByName("wdsmcast");
                                foreach (Process p in procs) { p.Kill(); }
                                Process p2 = Process.GetProcessById(CloneProcess.Id);
                                p2.Kill();
                                Process[] procs2 = Process.GetProcessesByName("diskpart");
                                foreach (Process p in procs2) { p.Kill(); }
                                File.WriteAllText(@"X:\CloneStatus.txt", "RESTART");
                            }
                            cloningDoneReceive = true;                            
                            break;
                        }
                    case FLAG.CLONING_DONE:
                        {
                            cloning = false;
                            cloningDoneReceive = true;
                            break;
                        }
                    case FLAG.SEND_CONFIG:
                        {
                            listener.SendMessage(new Packet(FLAG.SEND_CONFIG, listener.computerDetails.computerDetailsData));
                            if (listener.computerDetails.computerDetailsData.inWinpe)
                            {
                                FileHandler.Save(packet.computerConfigData, @"X:\Configuration.my");
                                string pathTaskData = @"X:\TaskData.my";
                                if (File.Exists(pathTaskData))
                                {
                                    var taskData = FileHandler.Load<TaskData>(pathTaskData);
                                    if (taskData.WithoutVHD)
                                        FileHandler.Save(packet.computerConfigData, @"D:\Configuration.my");
                                    else
                                        FileHandler.Save(packet.computerConfigData, @"C:\Configuration.my");
                                }
                            }
                            else
                            {
                                FileHandler.Save(packet.computerConfigData, @"D:\Configuration.my");
                                var FileName = @"C:\windows\system32\WindowsPowershell\v1.0\powershell";
                                var args = @"(Get-WmiObject -Class win32_ComputerSystem).rename(" + "\'" + packet.computerConfigData.Name + "\')";
                                var processStartInfo = new ProcessStartInfo
                                {
                                    FileName = FileName,
                                    Arguments = args,
                                    RedirectStandardOutput = true,
                                    CreateNoWindow = true,
                                    WindowStyle = ProcessWindowStyle.Hidden,
                                    UseShellExecute = false
                                };
                                var process = Process.Start(processStartInfo);
                                process.WaitForExit();
                                args = @"(Get-WmiObject -Class Win32_ComputerSystem).JoinDomainOrWorkgroup(" + "\'" + packet.computerConfigData.Workgroup + "\')";
                                processStartInfo = new ProcessStartInfo
                                {
                                    FileName = FileName,
                                    Arguments = args,
                                    RedirectStandardOutput = true,
                                    CreateNoWindow = true,
                                    WindowStyle = ProcessWindowStyle.Hidden,
                                    UseShellExecute = false
                                };
                                process = Process.Start(processStartInfo);
                                process.WaitForExit();
                            }
                            break;
                        }
                    case FLAG.CLIENT_TO_WINPE:
                        {
                            if (listener.computerDetails.computerDetailsData.inWinpe)
                            {                                
                                listener.SendMessage(new Packet(FLAG.CLIENT_TO_WINPE, listener.computerDetails.computerDetailsData));
                            }
                            else
                            {
                                if (!restarting)
                                {
                                    restarting = true;
                                    RunCommand(@"D:\Temp\GDS_Initialize.bat", "");
                                }
                            }
                            break;
                        }
                    case FLAG.FINISH_COPY_FILES:
                        {
                            listener.SendMessage(new Packet(FLAG.FINISH_COPY_FILES, listener.computerDetails.computerDetailsData));
                            break;
                        }
                    case FLAG.START_COPY_FILES:
                        {
                            listener.SendMessage(new Packet(FLAG.START_COPY_FILES, listener.computerDetails.computerDetailsData));
                            if (tcp_unicast == null)
                            {
                                tcp_unicast = new TCP_UNICAST(listener.serverIP, Convert.ToInt32(packet.clonningMessage));
                                if (tcp_unicast.error)
                                {
                                    Packet packet_temp = new Packet(FLAG.ERROR_MESSAGE, listener.computerDetails.computerDetailsData)
                                    {
                                        clonningMessage = tcp_unicast.Message
                                    };
                                    listener.SendMessage(packet_temp);
                                }
                                tcp_unicast = null;
                            }
                            break;
                        }
                    case FLAG.TO_OPERATING_SYSTEM:
                        {
                            if (listener.computerDetails.computerDetailsData.inWinpe)
                            {
                                if (!restarting)
                                {
                                    restarting = true;
                                    Process.Start(@"X:\Windows\System32\wpeutil.exe", "reboot");
                                }
                            }
                            else
                            {
                                listener.SendMessage(new Packet(FLAG.TO_OPERATING_SYSTEM, listener.computerDetails.computerDetailsData));
                            }
                            break;
                        }
                    case FLAG.SHUTDOWN:
                        {
                            listener.SendMessage(new Packet(FLAG.SHUTDOWN, listener.computerDetails.computerDetailsData));
                            if (!shutdowning)
                            {
                                shutdowningThread = new Thread(Shutdowning);
                                shutdowningThread.Start();
                            }
                            break;
                        }
                    case FLAG.SHUTDOWN_DONE:
                        {
                            shutdowningDoneReceive = true;
                            break;
                        }
                    case FLAG.ERROR_MESSAGE:
                        {
                            if (cloning)
                            {
                                Process[] procs = Process.GetProcessesByName("wdsmcast");
                                foreach (Process p in procs) { p.Kill(); }
                                Process p2 = Process.GetProcessById(CloneProcess.Id);
                                p2.Kill();
                                Process[] procs2 = Process.GetProcessesByName("diskpart");
                                foreach (Process p in procs2) { p.Kill(); }
                                File.WriteAllText(@"X:\CloneStatus.txt", "FALSE");
                            }
                            cloning = false;
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        } 
    }
}
