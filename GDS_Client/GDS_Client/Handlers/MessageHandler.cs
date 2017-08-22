using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
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
            int counter = 0;
            cloning = true;
            while (counter != 5 && listener.running)
            {                
                listener.SendMessage(new Packet(DataIdentifier.CLONING_STATUS, listener.computerDetails.computerDetailsData, "OK"));
                Thread.Sleep(500);
                listener.SendMessage(new Packet(DataIdentifier.CLONING_STATUS, listener.computerDetails.computerDetailsData, "Cloning"));
                Thread.Sleep(500);
                counter++;
            }
            cloning = false;
            cloningDoneReceive = false;
            while (!cloningDoneReceive && listener.running)
            {
                Thread.Sleep(500);
                listener.SendMessage(new Packet(DataIdentifier.CLONING_DONE, listener.computerDetails.computerDetailsData));
            }
        }

        private void Shutdowning()
        {
            shutdowning = true;
            while(!shutdowningDoneReceive)
            {
                Thread.Sleep(500);
                listener.SendMessage(new Packet(DataIdentifier.SHUTDOWN, listener.computerDetails.computerDetailsData));
            }
            Process.Start("shutdown", "/s /t 0");
        }
            
        public void HandleMessage(Packet packet)
        {
            WriteToLogs(DateTime.Now.ToLongTimeString().ToString() + ": " + packet.dataIdentifier);
            switch (packet.dataIdentifier)
            {            
                case DataIdentifier.SYN_FLAG:
                    {                       
                        listener.SendMessage(new Packet(DataIdentifier.SYN_FLAG,listener.computerDetails.computerDetailsData));
                        break;
                    }
                case DataIdentifier.CLOSE:
                    {
                        Environment.Exit(0);
                        break;
                    }
                case DataIdentifier.SEND_TASK_CONFIG:
                    {                        
                        listener.SendMessage(new Packet(DataIdentifier.SEND_TASK_CONFIG, listener.computerDetails.computerDetailsData));
                        if (!cloning)
                        {
                            cloningThread = new Thread(WorkingOnTask);
                            cloningThread.Start();
                            if (listener.computerDetails.computerDetailsData.inWinpe)
                                FileHandler.Save<TaskData>(packet.taskData, @"X:\" + packet.taskData.Name + ".my");
                            else
                                FileHandler.Save<TaskData>(packet.taskData, @".\" + packet.taskData.Name + ".my");
                        }                   
                        break;
                    }
                case DataIdentifier.CLONING_DONE:
                    {                        
                        cloningDoneReceive = true;
                        break;
                    }                
                case DataIdentifier.SEND_CONFIG:
                    {
                        listener.SendMessage(new Packet(DataIdentifier.SEND_CONFIG, listener.computerDetails.computerDetailsData));
                        FileHandler.Save<ComputerConfigData>(packet.computerConfigData, @"D:\Temp\Config.my");       
                        break;
                    }
                case DataIdentifier.SHUTDOWN:
                    {
                        listener.SendMessage(new Packet(DataIdentifier.SHUTDOWN, listener.computerDetails.computerDetailsData));
                        if (!shutdowning)
                        {
                            shutdowningThread = new Thread(WorkingOnTask);
                            shutdowningThread.Start();
                        }
                        break;
                    }
                case DataIdentifier.SHUTDOWN_DONE:
                    {
                        shutdowningDoneReceive = true;
                        break;
                    }
                case DataIdentifier.CLIENT_TO_WINPE:
                    {
                        if (listener.computerDetails.computerDetailsData.inWinpe)
                        {
                            listener.SendMessage(new Packet(DataIdentifier.CLIENT_TO_WINPE, listener.computerDetails.computerDetailsData));                            
                        }
                        else
                        {
                            Process.Start(@"D:\Temp\GDS_Initialize.bat");
                        }
                        break;
                    }
                case DataIdentifier.TO_OPERATING_SYSTEM:
                    {
                        if (listener.computerDetails.computerDetailsData.inWinpe)
                        {                            
                            Process.Start(@"X:\Windows\System32\wpeutil.exe", "reboot");
                        }
                        else
                        {
                            listener.SendMessage(new Packet(DataIdentifier.TO_OPERATING_SYSTEM, listener.computerDetails.computerDetailsData));                            
                        }
                        break;
                    }
            }
        }
    }
}
