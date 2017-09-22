using GDS_SERVER_WPF.DataCLasses;
using GDS_SERVER_WPF.Handlers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Serialization;

namespace GDS_SERVER_WPF
{
    public class ClientHandler
    {
        ListBox list;
        Label labelOnline;
        Label labelOffline;
        Label labelAllClients;
        static int length = 20000;
        byte[] dataStream = new byte[length];
        List<ClientHandler> clients;
        ComputerDetailsData computerData;
        int clientsAll = 0;
        XmlSerializer xs = new XmlSerializer(typeof(Packet));
        DateTime IDTimeOLD = DateTime.Now;

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
        

        public void startClient(TcpClient inClientSocket, int _clientNumber, ListBox _list, List<ClientHandler> _clients, Label _labelOnline, Label _labelOffline, ListViewMachinesAndTasksHandler _listViewMachinesAndTasksHandler, List<ExecutedTaskHandler> _executedTasksHandlers, Label _labelAllClients)
        {
            this.clientSocket = inClientSocket;
            this.clientNumber = _clientNumber;
            this.list = _list;
            this.offline = false;
            this.clients = _clients;
            this.labelOnline = _labelOnline;
            this.labelOffline = _labelOffline;
            this.listViewMachinesAndTasksHandler = _listViewMachinesAndTasksHandler;
            this.executedTasksHandlers = _executedTasksHandlers;
            this.labelAllClients = _labelAllClients;
            Thread ctThread = new Thread(doChat);
            computerData = new ComputerDetailsData();
            ctThread.Start();
        }

        public void SendMessage(Packet packet)
        {
            try {
                byte[] bytes = new byte[length];
                using (MemoryStream memStream = new MemoryStream(bytes))
                {
                    xs.Serialize(memStream, packet);
                    memStream.WriteTo(networkStream);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("Dojebalo sa posielanie: " + ex.ToString());
            }
        }

        public void myReadCallBack(IAsyncResult ar)
        {
            try
            {
                using (MemoryStream memStream = new MemoryStream(dataStream, 0, dataStream.Length, false))
                {
                    var myNetworkStream = (NetworkStream)ar.AsyncState;
                    var receivePacket2 = xs.Deserialize(memStream) as Packet;
                    if (receivePacket2.IDTime != IDTimeOLD)
                    {
                        receivePacket = receivePacket2;                        
                        if (computerInTaskHandler != null)
                            computerInTaskHandler.receivePacket = receivePacket;
                        HandleMessage(receivePacket);
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            list.Items.Add(computerData.Name + " " + receivePacket.dataIdentifier + " " + receivePacket.computerDetailsData.MacAddress);
                        });
                        if (receivePacket.dataIdentifier == DataIdentifier.SYN_FLAG || receivePacket.dataIdentifier == DataIdentifier.SYN_FLAG_WINPE)
                        {
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                listViewMachinesAndTasksHandler.Refresh();
                            });
                        }
                    }
                    else
                    {
                        /*Application.Current.Dispatcher.Invoke(() =>
                        {
                            list.Items.Add(computerData.Name + " " + receivePacket.dataIdentifier + " " + receivePacket.computerDetailsData.MacAddress + " SHUTDOWN??");
                        });*/
                        //SendMessage(new Packet(DataIdentifier.SYN_FLAG));
                    }
                    IDTimeOLD = receivePacket.IDTime;
                    myNetworkStream.BeginRead(dataStream, 0, dataStream.Length,
                                                                  new AsyncCallback(myReadCallBack),
                                                                  myNetworkStream);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                clients.Remove(this);
                Application.Current.Dispatcher.Invoke(() =>
                {
                    labelOnline.Content = "Online: " + clients.Count;
                    labelOffline.Content = "Offline: " + (clientsAll - clients.Count);
                    listViewMachinesAndTasksHandler.Refresh();
                });
                offline = true;
                if (receivePacket != null)
                    receivePacket.dataIdentifier = DataIdentifier.Null;
                if (computerInTaskHandler != null)
                    computerInTaskHandler.receivePacket = receivePacket;
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
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        labelOnline.Content = "Online: " + clients.Count;
                        labelOffline.Content = "Offline: " + (clientsAll - clients.Count);
                        listViewMachinesAndTasksHandler.Refresh();
                    });
                    break;
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