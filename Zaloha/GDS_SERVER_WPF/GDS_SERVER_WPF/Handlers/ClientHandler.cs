﻿using GDS_SERVER_WPF.DataCLasses;
using GDS_SERVER_WPF.Handlers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Xml.Serialization;

namespace GDS_SERVER_WPF
{
    public class ClientHandler
    {
        Label labelOnline;
        Label labelOffline;
        Label labelAllClients;
        static int length = 60000;
        byte[] dataStream = new byte[length];
        List<ClientHandler> clients;
        ComputerDetailsData computerData;
        int clientsAll = 0;
        XmlSerializer xs = new XmlSerializer(typeof(Packet));
        DateTime IDTimeOLD = DateTime.Now;
        Listener listener;

        public ComputerInTaskHandler computerInTaskHandler = null;
        public List<ExecutedTaskHandler> executedTasksHandlers;
        public Packet receivePacket;
        public TcpClient clientSocket;
        public bool offline;
        public bool deleting = false;
        public Stream networkStream;
        public int clientNumber;
        public bool inWinpe;
        public List<string> macAddresses = new List<string>();
        public ListViewMachinesAndTasksHandler listViewMachinesAndTasksHandler;
        public ComputerConfigData computerConfigData;
        public ListBox console;


        public void startClient(TcpClient inClientSocket, int _clientNumber, List<ClientHandler> _clients, Label _labelOnline, Label _labelOffline, ListViewMachinesAndTasksHandler _listViewMachinesAndTasksHandler, List<ExecutedTaskHandler> _executedTasksHandlers, Label _labelAllClients, ListBox _console, Listener _listener)
        {
            this.clientSocket = inClientSocket;
            this.clientNumber = _clientNumber;
            this.offline = false;
            this.clients = _clients;
            this.labelOnline = _labelOnline;
            this.labelOffline = _labelOffline;
            this.listViewMachinesAndTasksHandler = _listViewMachinesAndTasksHandler;
            this.executedTasksHandlers = _executedTasksHandlers;
            this.labelAllClients = _labelAllClients;
            this.console = _console;
            this.listener = _listener;
            Thread ctThread = new Thread(doChat);
            computerData = new ComputerDetailsData();
            ctThread.Start();
        }

        public bool SendMessage(Packet packet)
        {
            try {
                byte[] bytes = new byte[length];
                using (MemoryStream memStream = new MemoryStream(bytes))
                {
                    xs.Serialize(memStream, packet);
                    memStream.WriteTo(networkStream);
                }
                return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine("Dojebalo sa posielanie: " + ex.ToString());
            }
            return false;
        }

        public void myReadCallBack(IAsyncResult ar)
        {
            try
            {
                using (MemoryStream memStream = new MemoryStream(dataStream, 0, dataStream.Length, false))
                {
                    var myNetworkStream = (NetworkStream)ar.AsyncState;                    
                    try
                    {                        
                        var receivePacket2 = xs.Deserialize(memStream) as Packet;
                        if (receivePacket2.IDTime != IDTimeOLD)
                        {
                            receivePacket = receivePacket2;
                            if (computerInTaskHandler != null)
                                computerInTaskHandler.receivePacket = receivePacket;
                            HandleMessage(receivePacket);
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                for (int i = listViewMachinesAndTasksHandler.machines.Items.Count - 1; i >= 0; i--)
                                {
                                    try
                                    {
                                        ComputerDetailsData computer = (ComputerDetailsData)listViewMachinesAndTasksHandler.machines.Items[i];
                                        if (!computer.ImageSource.Contains("Folder"))
                                        {
                                            if (CheckMacsInREC(computer.macAddresses, macAddresses))
                                            {
                                                if (computerData.inWinpe)
                                                    computerData.ImageSource = "Images/WinPE.ico";
                                                else
                                                    computerData.ImageSource = "Images/Online.ico";
                                                listViewMachinesAndTasksHandler.machines.Items.RemoveAt(i);
                                                listViewMachinesAndTasksHandler.machines.Items.Insert(i, computerData);
                                                break;
                                            }
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        MessageBox.Show("BEZIM: " + e.ToString());
                                    }
                                }
                            });
                        }
                        IDTimeOLD = receivePacket.IDTime;
                    }
                    catch (Exception ex)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            //console.Items.Add("XML CHYBA: " + ex.ToString());
                        });
                    }
                    myNetworkStream.BeginRead(dataStream, 0, dataStream.Length,
                                                                  new AsyncCallback(myReadCallBack),
                                                                  myNetworkStream);
                }
            }
            catch (Exception ex)
            {
                try
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        if (!ex.ToString().Contains("Cannot access a disposed object.") && !ex.ToString().Contains("An existing connection was forcibly closed by the remote host."))
                            console.Items.Add("Client: " + ex.ToString());
                        try
                        {
                            networkStream.Close();
                            clientSocket.Close();
                        }
                        catch { }
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            for (int i = listViewMachinesAndTasksHandler.machines.Items.Count - 1; i >= 0; i--)
                            {
                                ComputerDetailsData computer = (ComputerDetailsData)listViewMachinesAndTasksHandler.machines.Items[i];
                                if (computer.macAddresses != null && macAddresses != null && CheckMacsInREC(computer.macAddresses, macAddresses))
                                {
                                    computer.ImageSource = "Images/Offline.ico";
                                    listViewMachinesAndTasksHandler.machines.Items.RemoveAt(i);
                                    listViewMachinesAndTasksHandler.machines.Items.Insert(i, computer);
                                    break;
                                }
                            }                                                        
                            offline = true;
                            listener.Disconnect(this);
                            if (receivePacket != null)
                                receivePacket.dataIdentifier = DataIdentifier.Null;
                            if (computerInTaskHandler != null)
                                computerInTaskHandler.receivePacket = receivePacket;
                            this.Close();
                        });
                    });
                }
                catch (Exception e)
                {
                    MessageBox.Show("ZOMREL SOM: " + e.ToString());
                }
            }
        }

        private void HandleMessage(Packet packet)
        {
            if (!deleting)
            {
                switch (packet.dataIdentifier)
                {
                    case DataIdentifier.SYN_FLAG:
                        {
                            inWinpe = false;
                            macAddresses = packet.computerDetailsData.macAddresses;
                            CheckIfClientNotDuplicate();
                            SaveComputerData(packet.computerDetailsData);
                            FindComputerInTask();
                            break;
                        }
                    case DataIdentifier.SYN_FLAG_WINPE:
                        {
                            inWinpe = true;
                            macAddresses = packet.computerDetailsData.macAddresses;
                            CheckIfClientNotDuplicate();
                            SaveComputerData(packet.computerDetailsData);
                            FindComputerInTask();
                            break;
                        }
                    case DataIdentifier.RESTART:
                        {
                            SendMessage(new Packet(DataIdentifier.RESTART));
                            break;
                        }
                    case DataIdentifier.CLONING_DONE:
                        {
                            SendMessage(new Packet(DataIdentifier.CLONING_DONE));
                            break;
                        }
                    case DataIdentifier.SHUTDOWN:
                        {
                            SendMessage(new Packet(DataIdentifier.SHUTDOWN_DONE));                                                                                    
                            break;
                        }
                    case DataIdentifier.CLOSE:
                        {
                            networkStream.Close();
                            clientSocket.Close();
                            break;
                        }
                }
            }
        }                

        private void FindComputerInTask()
        {
            for (int i = 0; i < executedTasksHandlers.Count; i++)
            {
                ExecutedTaskHandler executedTaskHandler = executedTasksHandlers[i];
                foreach (ComputerInTaskHandler computer in executedTaskHandler.computers)
                {
                    if (CheckMacsInREC(computer.computer.macAddresses, macAddresses))
                    {
                        computerInTaskHandler = computer;
                        computerInTaskHandler.client = this;
                        computerInTaskHandler.receivePacket = receivePacket;
                        computerInTaskHandler.computerConfigData = computerConfigData;
                        break;
                    }
                }
            }
        }

        private void CheckIfClientNotDuplicate()
        {
            for (int i = 0; i < clients.Count; i++)
            {
                if (CheckMacsInREC(clients[i].macAddresses, macAddresses) && clients[i].clientNumber != clientNumber)
                {
                    clientNumber = clients[i].clientNumber;
                    clients[i].SendMessage(new Packet(DataIdentifier.CLOSE));
                    clients[i].clientSocket.Close();
                    clients.Remove(clients[i]);                    
                }
            }
        }

        private bool IsMacAddressIn(List<string> array1, List<string> array2)
        {
            foreach (string text1 in array1)
            {
                foreach (string text2 in array2)
                {
                    if (text1 == text2)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool CheckMacsInREC(List<string> Macs, List<string> Recs)
        {
            if (Recs != null)
            {
                if (Macs.Count != 0 && Recs.Count != 0)
                    return IsMacAddressIn(Macs, Recs);
            }
            return false;
        }

        private void SaveComputerData(ComputerDetailsData computerDetailsData)
        {
            string IP = ((IPEndPoint)clientSocket.Client.RemoteEndPoint).Address.ToString();
            computerDetailsData.IPAddress = IP;
            computerData = computerDetailsData;
            var computersInfoFiles = Directory.GetFiles(@".\Machine Groups\", "*.my", SearchOption.AllDirectories);
            clientsAll = computersInfoFiles.Length;
            Application.Current.Dispatcher.Invoke(() =>
            {
                labelAllClients.Content = "Clients: " + clientsAll;
            });
            var filePath = GetFileNameByMac(computersInfoFiles);            
            computerConfigData = new ComputerConfigData(computerData.RealPCName, "Workgroup");
            if (filePath != "")
            {
                computerData.Name = Path.GetFileName(filePath).Replace(".my", "");
                FileHandler.Save<ComputerDetailsData>(computerData, filePath);
                if (!File.Exists(filePath.Replace(".my", ".cfg")))
                {
                    FileHandler.Save<ComputerConfigData>(computerConfigData, filePath.Replace(".my", ".cfg"));
                }
                else
                {
                    computerConfigData = FileHandler.Load<ComputerConfigData>(filePath.Replace(".my", ".cfg"));
                }
            }
            else
            {
                computerData.Name = computerData.RealPCName;
                bool exist = false;
                foreach (string computerFile in computersInfoFiles)
                {
                    if (computerFile == @".\Machine Groups\Default\" + computerData.RealPCName + ".my")
                    {
                        exist = true;
                        break;
                    }
                }
                if (!exist)
                {
                    filePath = @".\Machine Groups\Default\" + computerData.RealPCName + ".my";
                }
                else
                {
                    filePath = @".\Machine Groups\Default\" + computerData.RealPCName + "-NEW.my";
                }                
                FileHandler.Save<ComputerDetailsData>(computerData, filePath);
                FileHandler.Save<ComputerConfigData>(computerConfigData, filePath.Replace(".my", ".cfg"));
            }
        }

        private string GetFileNameByMac(string[] computersInfoFiles)
        {
            string filePath = "";
            foreach (string computerFile in computersInfoFiles)
            {
                var computerData = FileHandler.Load<ComputerDetailsData>(computerFile);
                if (CheckMacsInREC(computerData.macAddresses, macAddresses))
                {
                    filePath = computerFile;
                    break;
                }
            }
            return filePath;
        }

        private void doChat()
        {
            networkStream = clientSocket.GetStream();
            networkStream.BeginRead(dataStream, 0, dataStream.Length,
                                                   new AsyncCallback(this.myReadCallBack),
                                                   networkStream);
        }

        public void Close()
        {
            networkStream.Close();
            clientSocket.Close();
        }
    }
}