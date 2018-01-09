using GDS_SERVER_WPF.Handlers;
using NetworkCommsDotNet;
using NetworkCommsDotNet.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using NetworkCommsDotNet.Connections;
using GDS_SERVER_WPF.DataCLasses;
using System.Linq;

namespace GDS_SERVER_WPF
{
    public class Listener
    {

        public Dictionary<ShortGuid, ComputerWithConnection> ClientsDictionary = new Dictionary<ShortGuid, ComputerWithConnection>();
        public Label labelOnlineClients;
        public Label labelOfflineClients;
        public Label labelAllClients;
        public int clientsAll = 0;
        public Semaphore semaphore = new Semaphore(1, 500);
        public ListViewMachinesAndTasksHandler listViewMachinesAndTasksHandler;
        public List<ExecutedTaskHandler> executedTasksHandlers;
        public ListBox console;

        public void StartListener()
        {
            listViewMachinesAndTasksHandler.ClientsDictionary = ClientsDictionary;
            clientsAll = Directory.GetFiles(@".\Machine Groups\", "*.my", SearchOption.AllDirectories).Length;
            Application.Current.Dispatcher.Invoke(() =>
            {
                labelOnlineClients.Content = "Online: " + ClientsDictionary.Count;
                labelOfflineClients.Content = "Offline: " + (clientsAll - ClientsDictionary.Count);
                labelAllClients.Content = "Clients: " + clientsAll;
            });

            NetworkComms.AppendGlobalIncomingPacketHandler<byte[]>("Packet", IncommingMessage);
            NetworkComms.AppendGlobalConnectionCloseHandler(HandleConnectionClosed);
            Connection.StartListening(ConnectionType.TCP, new IPEndPoint(IPAddress.Any, 10000));
        }

        private void HandleConnectionClosed(Connection connection)
        {
            semaphore.WaitOne();
            ShortGuid remoteIdentifier = connection.ConnectionInfo.NetworkIdentifier;
            if (ClientsDictionary.ContainsKey(remoteIdentifier))
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    for (int i = listViewMachinesAndTasksHandler.machines.Items.Count - 1; i >= 0; i--)
                    {
                        ComputerDetailsData computer = (ComputerDetailsData)listViewMachinesAndTasksHandler.machines.Items[i];
                        if (computer.macAddresses != null && ClientsDictionary[remoteIdentifier].ComputerData.macAddresses != null && CheckMacsInREC(computer.macAddresses, ClientsDictionary[remoteIdentifier].ComputerData.macAddresses))
                        {
                            computer.ImageSource = "Images/Offline.ico";
                            listViewMachinesAndTasksHandler.machines.Items.RemoveAt(i);
                            listViewMachinesAndTasksHandler.machines.Items.Insert(i, computer);
                            break;
                        }
                    }
                });
            }
            ClientsDictionary.Remove(connection.ConnectionInfo.NetworkIdentifier);
            Application.Current.Dispatcher.Invoke(() =>
            {
                labelOnlineClients.Content = "Online: " + ClientsDictionary.Count;
                labelOfflineClients.Content = "Offline: " + (clientsAll - ClientsDictionary.Count);
            });
            semaphore.Release(1);
        }

        public static bool CheckMacsInREC(List<string> Macs, List<string> Recs)
        {
            if (Recs != null)
            {
                if (Macs.Count != 0 && Recs.Count != 0)
                    return IsMacAddressIn(Macs, Recs);
            }
            return false;
        }

        private static bool IsMacAddressIn(List<string> array1, List<string> array2)
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

        private void IncommingMessage(PacketHeader packetHeader, Connection connection, byte[] data)
        {
            try
            {
                semaphore.WaitOne();
                Packet packet = Proto.ProtoDeserialize<Packet>(data);

                if (ClientsDictionary.ContainsKey(packet.computerDetailsData.SourceIdentifier))
                {
                    ClientsDictionary[packet.computerDetailsData.SourceIdentifier] = new ComputerWithConnection { ComputerData = packet.computerDetailsData, connection = connection };
                }
                else
                {
                    ClientsDictionary.Add(packet.computerDetailsData.SourceIdentifier, new ComputerWithConnection { ComputerData = packet.computerDetailsData, connection = connection });
                }
                semaphore.Release();
                if (!connection.ToString().StartsWith("[UDP-E-E] 127."))
                {
                    packet.computerDetailsData.IPAddress = connection.ConnectionInfo.RemoteEndPoint.ToString().Split(':').First();

                    MessageHandler(packet, connection);                    
                }
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MessageBox.Show("PROBLEM AKO HOVADO: " + ex.ToString());
                });
                StartListener();
            }
        }

        private void FindComputerInTask(Packet packet, Connection connection, bool failed)
        {
            try
            {
                for (int i = 0; i < executedTasksHandlers.Count; i++)
                {
                    ExecutedTaskHandler executedTaskHandler = executedTasksHandlers[i];
                    foreach (ComputerInTaskHandler computer in executedTaskHandler.computers)
                    {
                        if (CheckMacsInREC(computer.computer.macAddresses, packet.computerDetailsData.macAddresses))
                        {
                            computer.ClientsDictionary = ClientsDictionary;
                            computer.receivePacket = packet;
                            computer.connection = connection;
                            if(computer.cloning)
                            {
                                computer.semaphoreForCloning.Release();
                            }
                            if (failed)
                            {
                                computer.Failed(packet.clonningMessage);
                            }
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MessageBox.Show("PROBLEM AKO HOVADO4: " + ex.ToString());
                });
            }
        }

        private void MessageHandler(Packet packet, Connection connection)
        {
            try
            {
                bool failed = false;
                switch (packet.ID)
                {
                    case FLAG.SYN_FLAG:
                    case FLAG.SYN_FLAG_WINPE:
                        {
                            SaveComputerData(packet, connection);
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                for (int i = listViewMachinesAndTasksHandler.machines.Items.Count - 1; i >= 0; i--)
                                {
                                    try
                                    {
                                        ComputerDetailsData computer = (ComputerDetailsData)listViewMachinesAndTasksHandler.machines.Items[i];
                                        if (!computer.ImageSource.Contains("Folder"))
                                        {
                                            if (CheckMacsInREC(computer.macAddresses, packet.computerDetailsData.macAddresses))
                                            {
                                                if (packet.computerDetailsData.inWinpe)
                                                    packet.computerDetailsData.ImageSource = "Images/WinPE.ico";
                                                else
                                                    packet.computerDetailsData.ImageSource = "Images/Online.ico";
                                                packet.computerDetailsData.PostInstalls = packet.computerConfigData.PostInstalls;
                                                listViewMachinesAndTasksHandler.machines.Items.RemoveAt(i);
                                                listViewMachinesAndTasksHandler.machines.Items.Insert(i, packet.computerDetailsData);
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

                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                labelOnlineClients.Content = "Online: " + ClientsDictionary.Count;
                                labelOfflineClients.Content = "Offline: " + (clientsAll - ClientsDictionary.Count);
                            });
                            break;
                        }
                    case FLAG.ERROR_MESSAGE:
                        {
                            failed = true;
                            break;
                        }
                        
                }
                FindComputerInTask(packet, connection, failed);
                if (packet.ID == FLAG.SHUTDOWN_DONE || packet.ID == FLAG.CLONING_DONE)
                {
                    SendMessage(packet, connection);
                }
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MessageBox.Show("PROBLEM AKO HOVADO2: " + ex.ToString());
                });
            }
        }

        private string GetFileNameByMac(string[] computersInfoFiles, List<string> macAddresses)
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

        private void SaveComputerData(Packet packet, Connection connection)
        {
            try
            {
                var computersInfoFiles = Directory.GetFiles(@".\Machine Groups\", "*.my", SearchOption.AllDirectories);
                clientsAll = computersInfoFiles.Length;
                Application.Current.Dispatcher.Invoke(() =>
                {
                    labelAllClients.Content = "Clients: " + clientsAll;
                });
                var filePath = GetFileNameByMac(computersInfoFiles, packet.computerDetailsData.macAddresses);
                packet.computerConfigData = new ComputerConfigData(packet.computerDetailsData.RealPCName, "Workgroup");
                if (filePath != "")
                {
                    packet.computerDetailsData.Name = Path.GetFileName(filePath).Replace(".my", "");
                    FileHandler.Save<ComputerDetailsData>(packet.computerDetailsData, filePath);
                    if (!File.Exists(filePath.Replace(".my", ".cfg")))
                    {
                        FileHandler.Save<ComputerConfigData>(packet.computerConfigData, filePath.Replace(".my", ".cfg"));
                    }
                    else
                    {
                        packet.computerConfigData = FileHandler.Load<ComputerConfigData>(filePath.Replace(".my", ".cfg"));
                    }
                }
                else
                {
                    packet.computerDetailsData.Name = packet.computerDetailsData.RealPCName;
                    bool exist = false;
                    foreach (string computerFile in computersInfoFiles)
                    {
                        if (computerFile == @".\Machine Groups\Default\" + packet.computerDetailsData.RealPCName + ".my")
                        {
                            exist = true;
                            break;
                        }
                    }
                    if (!exist)
                    {
                        filePath = @".\Machine Groups\Default\" + packet.computerDetailsData.RealPCName + ".my";
                    }
                    else
                    {
                        filePath = @".\Machine Groups\Default\" + packet.computerDetailsData.RealPCName + "-NEW.my";
                    }
                    FileHandler.Save<ComputerDetailsData>(packet.computerDetailsData, filePath);
                    FileHandler.Save<ComputerConfigData>(packet.computerConfigData, filePath.Replace(".my", ".cfg"));
                }
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MessageBox.Show("PROBLEM AKO HOVADO3: " + ex.ToString());
                });
            }
        }

        public void SendMessage(Packet packet, Connection connection)
        {
            try
            {
                if (connection.ConnectionAlive())
                {
                    byte[] data = Proto.ProtoSerialize<Packet>(packet);
                    connection.SendObject("Packet", data);
                }
            }
            catch { }
        }
    }
}
