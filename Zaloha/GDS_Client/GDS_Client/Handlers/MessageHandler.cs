using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace GDS_Client
{
    public class MessageHandler
    {
        public Listener listener;

        Thread cloningThread;
        Thread shutdowningThread;
        bool cloning = false;
        bool cloningDoneReceive = false;
        bool shutdowning = false;
        bool shutdowningDoneReceive = false;
        bool runningCommands = false;
        bool restarting = false;
        Process CloneProcess;

        public MessageHandler(Listener _listener)
        {
            this.listener = _listener;
        }

        void WriteToLogs(string LOG)
        {
            Console.WriteLine(LOG);
            if (!listener.computerDetails.computerDetailsData.inWinpe)
            {
                if (File.Exists(@".\LOG.txt"))
                {
                    FileInfo FI = new FileInfo(@".\LOG.txt");
                    if (FI.Length > 2000000)
                    {
                        FI.Delete();
                    }
                }
                using (StreamWriter sw = File.AppendText(@".\LOG.txt"))
                {
                    sw.WriteLine(DateTime.Now.ToString() + ": " + LOG);
                }
            }
        }

        public void WorkingOnTask()
        {
            try
            {
                if(!File.Exists(@"X:\CloneStatus.txt"))
                    File.WriteAllText(@"X:\CloneStatus.txt", "CLONING");
                File.WriteAllText(@"X:\Error.txt", "False");
                CloneProcess = System.Diagnostics.Process.Start(@"X:\windows\system32\WindowsPowershell\v1.0\powershell.exe", @"-WindowStyle Maximized -executionpolicy unrestricted -File W:\Cloning.ps1");
                cloning = true;
                string CloneMessage = "CLONING";
                while (listener.running && CloneMessage != "FALSE" && CloneMessage != "TRUE" && cloning)
                {                    
                    try
                    {
                        if(CloneMessage != null && listener.computerDetails.computerDetailsData != null && CloneMessage != "")
                            listener.SendMessage(new Packet(FLAG.CLONING_STATUS, listener.computerDetails.computerDetailsData, CloneMessage));
                        CloneMessage = File.ReadLines(@"X:\CloneStatus.txt").First();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Problem with cloning: " + ex.ToString());
                    }
                    Thread.Sleep(1000);
                    Console.WriteLine(DateTime.Now + " " + CloneMessage);
                }
                var dataIdentifier = FLAG.CLONING_DONE;
                if (CloneMessage == "FALSE")
                    dataIdentifier = FLAG.RESTART;
                cloningDoneReceive = false;                
                if (cloning)
                {
                    while (!cloningDoneReceive && listener.running)
                    {
                        Thread.Sleep(500);
                        listener.SendMessage(new Packet(dataIdentifier, listener.computerDetails.computerDetailsData));
                    }
                }
                cloning = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        static void RunCommand(string FileName, string Arguments)
        {
            Process proc = new Process();
            proc.StartInfo.FileName = FileName;
            proc.StartInfo.Arguments = Arguments;
            proc.StartInfo.Verb = "runas";
            proc.Start();
        }

        private void Shutdowning()
        {
            shutdowning = true;
            while(!shutdowningDoneReceive)
            {
                Thread.Sleep(500);
                listener.SendMessage(new Packet(FLAG.SHUTDOWN, listener.computerDetails.computerDetailsData));
            }
            Process.Start("shutdown", "/s /t 0");
        }
            
        public void HandleMessage(Packet packet)
        {
            WriteToLogs(DateTime.Now.ToLongTimeString().ToString() + ": " + packet.dataIdentifier);
            switch (packet.dataIdentifier)
            {            
                case FLAG.SYN_FLAG:
                    {                       
                        listener.SendMessage(new Packet(FLAG.SYN_FLAG,listener.computerDetails.computerDetailsData));
                        break;
                    }
                case FLAG.CLOSE:
                    {
                        Environment.Exit(0);
                        break;
                    }
                case FLAG.SEND_TASK_CONFIG:
                    {
                        listener.SendMessage(new Packet(FLAG.SEND_TASK_CONFIG, listener.computerDetails.computerDetailsData));
                        packet.taskData.TargetComputers.Clear();
                        packet.taskData.TargetComputers.Add(listener.computerDetails.computerDetailsData);
                        if (listener.computerDetails.computerDetailsData.inWinpe)
                            FileHandler.Save(packet.taskData, @"X:\TaskData.my");
                        else
                            FileHandler.Save(packet.taskData, @".\TaskData.my");
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
                            if(File.Exists(pathTaskData))
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
                case FLAG.CLONING_DONE:
                    {                        
                        cloningDoneReceive = true;
                        break;
                    }                
                case FLAG.SEND_CONFIG:
                    {
                        listener.SendMessage(new Packet(FLAG.SEND_CONFIG, listener.computerDetails.computerDetailsData));
                        if (listener.computerDetails.computerDetailsData.inWinpe)
                            FileHandler.Save(packet.computerConfigData, @"X:\Configuration.my");
                        else
                        {
                            FileHandler.Save(packet.computerConfigData, @"D:\Temp\Configuration.my");
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
                        }
                        cloning = false;
                        break;
                    }
            }
        }
    }
}
