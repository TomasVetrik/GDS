using GDS_Client.Handlers;
using NetworkCommsDotNet;
using NetworkCommsDotNet.Connections;
using NetworkCommsDotNet.Connections.TCP;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Threading;

namespace GDS_Client
{
    public class Listener
    {
        public ComputerDetails computerDetails = new ComputerDetails();
        public bool running;
        public MessageHandler messageHandler;
        public string serverIP = null;
        public int serverPort = 10000;
        public Connection connection;        

        void GetServerIP()
        {
            try
            {
                while (serverIP == null)
                {
                    List<string> IPAdds = (HardwareInfo.GetIPAddress());
                    if (IPAdds.Count != 0)
                    {
                        foreach (string IP in IPAdds)
                        {
                            Console.WriteLine(IP);                           
                            SetServerIP(IP);
                            if (serverIP != null)
                                break;
                        }
                    }
                    Thread.Sleep(10000);
                }
            }
            catch
            {
                GetServerIP();
            }
        }

        void SetServerIP(string IPAdd)
        {
            serverIP = null;
            if (IPAdd.StartsWith("10.201."))
            {
                serverIP = "10.202.0.6";
            }
            if (IPAdd.StartsWith("10.202."))
            {
                serverIP = "10.202.0.6";
            }
            if (IPAdd.StartsWith("10.101."))
            {
                serverIP = "10.102.0.6";
            }
            if (IPAdd.StartsWith("10.102."))
            {
                serverIP = "10.102.0.6";
            }
            if (IPAdd.StartsWith("10.1."))
            {
                serverIP = "10.2.0.6";
            }
            if (IPAdd.StartsWith("10.2."))
            {
                serverIP = "10.2.0.6";
            }
            //serverIP = "10.201.20.14";
        }

        readonly string FileName = @"D:\Temp\GDSClient\GDS_Client_LOG.txt";

        public void WriteToLogs(string LOG)
        {
            Console.WriteLine(LOG);
            if (!computerDetails.computerDetailsData.inWinpe)
            {
                if (File.Exists(FileName))
                {
                    FileInfo FI = new FileInfo(FileName);
                    if (FI.Length > 2000000)
                    {
                        FI.Delete();
                    }   
                    using (StreamWriter sw = File.AppendText(FileName))
                    {
                        sw.WriteLine(DateTime.Now.ToString() + ": " + LOG);
                    }
                }
            }
        }

        void RunCommand(string FileName, string Arguments)
        {
            Process proc = new Process();
            proc.StartInfo.FileName = FileName;
            proc.StartInfo.Arguments = Arguments;
            proc.StartInfo.Verb = "runas";
            proc.Start();
            if (computerDetails.computerDetailsData.inWinpe)
            {
                Console.WriteLine("Waiting for process");
                proc.WaitForExit();
                Console.WriteLine("Closing proces");
            }
        }

        public void SetLectorDisplayDuplicate()
        {
            try
            {
                string PCName = System.Environment.MachineName.ToUpper();                
                if (PCName.Contains("LEKTOR") || PCName.Contains("STUDENTSK11-10"))
                {
                    if (File.Exists(@"D:\Temp\SetDisplayDuplicate.ps1"))
                    {
                        RunCommand("cmd.exe", "/C " + @"C:\windows\system32\WindowsPowershell\v1.0\powershell D:\Temp\SetDisplayDuplicate.ps1");
                        Thread.Sleep(2000);
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void StartListener()
        {
            messageHandler = new MessageHandler(this);
            Start:            
            serverIP = null;
            WriteToLogs("Getting IP");
            GetServerIP();
            WriteToLogs("Server IP: " + serverIP);
            computerDetails.SetComputerDetails();
            WriteToLogs("Getting computer details");
            try
            {
                if (File.Exists(@"X:\TaskData.my"))
                {
                    File.Delete(@"X:\TaskData.my");
                    Thread.Sleep(1000);
                }
            }
            catch
            { }
            SetLectorDisplayDuplicate(); 
            bool created = false;
            int counter = 0;
            int MAX_COUNT_REFRESH = 5;
            while (!created)
            {
                created = true;
                ConnectionInfo serverConnectionInfo = null;
                try
                {
                    serverConnectionInfo = new ConnectionInfo(serverIP.Trim(), serverPort);
                    try
                    {
                        connection = TCPConnection.GetConnection(serverConnectionInfo);
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine("Failed with Creating connection");
                        //Console.WriteLine(ex.ToString());
                        Thread.Sleep(10000);
                        created = false;                        
                        if (counter == MAX_COUNT_REFRESH)
                        {                            
                            goto Start;
                        }
                        counter++;
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("Failed to parse the server IP and port. Please ensure it is correct and try again");
                    Thread.Sleep(10000);
                    created = false;
                }
            }
            FLAG ID = FLAG.SYN_FLAG;
            if (computerDetails.computerDetailsData.inWinpe)
            {
                ID = FLAG.SYN_FLAG_WINPE;
            }
            Packet packet = new Packet(ID, computerDetails.computerDetailsData)
            {
                computerDetailsData = computerDetails.computerDetailsData
            };
            connection.AppendIncomingPacketHandler<byte[]>("Packet", IncommingMessage);
            connection.AppendShutdownHandler(HandleConnectionClosed);
            Console.WriteLine("Connected to: " + serverIP);            
            SendMessage(packet);
            WriteToLogs("Sending SYN FLAG");
            running = true;
        }

        private void HandleConnectionClosed(Connection connection)
        {
            if(messageHandler.cloning)
            {
                messageHandler.HandleMessage(new Packet(FLAG.ERROR_MESSAGE), connection);
            }
            Thread.Sleep(10000);
            StartListener();
            return;
        }

        private void IncommingMessage(PacketHeader packetHeader, Connection connection, byte[] data)
        {
            try
            {
                Packet packet = Proto.ProtoDeserialize<Packet>(data);
                if (!connection.ToString().StartsWith("[UDP-E-E] 127."))
                    messageHandler.HandleMessage(packet, connection);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Problem: " + ex.ToString());
            }
        }

        public void SendMessage(Packet packet)
        {
            byte[] data = Proto.ProtoSerialize<Packet>(packet);
            WriteToLogs("SENDINIG: " + packet.ID);
            try { connection.SendObject("Packet", data); }
            catch (CommsException) { Console.WriteLine("A CommsException occurred while trying to send message"); }
        }
    }
}
