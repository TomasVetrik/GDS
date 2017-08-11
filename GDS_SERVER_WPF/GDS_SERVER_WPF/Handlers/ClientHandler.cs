using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace GDS_SERVER_WPF
{
    public class ClientHandler
    {
        public TcpClient clientSocket;        
        ListBox list;
        Label labelOnline;   
        byte[] dataStream = new byte[1024];
        List<ClientHandler> clients;
        ComputerDetailsData computerData;
        EndPoint epSender;

        public bool offline;
        public NetworkStream networkStream;
        public int clientNumber;
        public bool inWinpe;
        public List<string> macAddresses = new List<string>();
        public ListViewMachinesAndTasksHandler listViewMachinesAndTasksHandler;      

        public void startClient(TcpClient inClientSocket, int _clientNumber, ListBox _list, List<ClientHandler> _clients, Label _labelOnline, ListViewMachinesAndTasksHandler _listViewMachinesAndTasksHandler)
        {
            this.clientSocket = inClientSocket;
            this.clientNumber = _clientNumber;
            this.list = _list;
            this.offline = false;
            this.clients = _clients;
            this.labelOnline = _labelOnline;
            this.listViewMachinesAndTasksHandler = _listViewMachinesAndTasksHandler;
            Thread ctThread = new Thread(doChat);
            computerData = new ComputerDetailsData();           
            ctThread.Start();
        }

        public void SendMessage(DataIdentifier ID, string _message)
        {
            var sendData = new Packet(ID, "", _message);
            var sendBytes = sendData.GetDataStream();            
            networkStream.Write(sendBytes, 0, sendBytes.Length);
            networkStream.Flush();
        }

        public void myReadCallBack(IAsyncResult ar)
        {
            epSender = null;
            var sendData = new Packet(); 
            try
            {
                Packet receivedData = new Packet(this.dataStream);
                
                IPEndPoint clients = (IPEndPoint)clientSocket.Client.RemoteEndPoint;
                epSender = (EndPoint)clients;
                
                var myNetworkStream = (NetworkStream)ar.AsyncState;
                Application.Current.Dispatcher.Invoke(() =>
                {                    
                    list.Items.Add(clientNumber + " " + receivedData.Message + " " + receivedData.MacAddress); 
                });                
                HandleMessage(receivedData);
                if (receivedData.DataIdentifier == DataIdentifier.SYN_FLAG)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        listViewMachinesAndTasksHandler.Refresh();
                    });
                }
                myNetworkStream.BeginRead(dataStream, 0, dataStream.Length,
                                                          new AsyncCallback(this.myReadCallBack),
                                                          myNetworkStream);
            }
            catch
            {
                clients.Remove(this);
                Application.Current.Dispatcher.Invoke(() =>
                {
                    labelOnline.Content = "Online: " + clients.Count;
                    listViewMachinesAndTasksHandler.Refresh();
                });
                offline = true;
            }
        }

        private void HandleMessage(Packet data)
        {
            switch (data.DataIdentifier)
            {
                case DataIdentifier.SYN_FLAG:
                    {
                        inWinpe = false;
                        macAddresses.Clear();
                        if (data.MacAddress.Contains("&"))
                            macAddresses = new List<string>(data.MacAddress.Split('&'));
                        else
                            macAddresses.Add(data.MacAddress);
                        CheckIfClientNotDuplicate();
                        SaveComputerData(data.Message);
                        break;
                    }
                case DataIdentifier.SYN_FLAG_WINPE:
                    {
                        inWinpe = true;
                        macAddresses.Clear();
                        if (data.MacAddress.Contains("&"))
                            macAddresses = new List<string>(data.MacAddress.Split('&'));
                        else
                            macAddresses.Add(data.MacAddress);
                        CheckIfClientNotDuplicate();
                        SaveComputerData(data.Message);
                        break;
                    }
            }
        }

        private void CheckIfClientNotDuplicate()
        {
            for(int i = 0; i < clients.Count; i++)
            {
                if(CheckMacsInREC(clients[i].macAddresses, macAddresses) && clients[i].clientNumber != clientNumber)
                {
                    clientNumber = clients[i].clientNumber;
                    clients[i].SendMessage(DataIdentifier.CLOSE, "CLOSE");
                    clients[i].clientSocket.Close();
                    clients.Remove(clients[i]);
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        labelOnline.Content = "Online: " + clients.Count;
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

        private void SaveComputerData(string message)
        {            
            List<String> computerDetails_list = new List<string>(message.Split(new string[] { "|..|" }, StringSplitOptions.None));
            string IP = epSender.ToString();
            if(IP.Contains(":"))
                computerDetails_list.Add("IP Address||" + IP.Split(':')[0]);
            else
                computerDetails_list.Add("IP Address||" + IP);
            computerData.LoadDataFromList(computerDetails_list);
            var computersInfoFiles = Directory.GetFiles(@".\Machine Groups\", "*.my", SearchOption.AllDirectories);
            var filePath = GetFileNameByMac(computersInfoFiles);
            var computerConfigData = new ComputerConfigData(computerData.computerName, "Workgroup");
            if (filePath != "")
            {
                FileHandler.Save<ComputerDetailsData>(computerData, filePath);
                if (!File.Exists(filePath.Replace(".my", ".cfg")))
                {
                    FileHandler.Save<ComputerConfigData>(computerConfigData, filePath.Replace(".my", ".cfg"));
                }
            }
            else
            {
                bool exist = false;
                foreach (string computerFile in computersInfoFiles)
                {
                    if (computerFile == @".\Machine Groups\Default\" + computerData.computerName + ".my")
                    {
                        exist = true;
                        break;
                    }
                }
                if (!exist)
                {
                    filePath = @".\Machine Groups\Default\" + computerData.computerName + ".my";
                }
                else
                {
                    filePath = @".\Machine Groups\Default\" + computerData.computerName + "-NEW.my";                   
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
    }
}
