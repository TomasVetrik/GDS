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
using System.Data.SqlClient;
using System.Data;
using System.Windows.Media;
using System.Windows.Controls.Primitives;

namespace GDS_SERVER_WPF
{
    public class Listener
    {
        private static readonly object computersInfo = new object();
        public Dictionary<ShortGuid, ComputerWithConnection> ClientsDictionary = new Dictionary<ShortGuid, ComputerWithConnection>();
        public Label labelOnlineClients;
        public Label labelOfflineClients;
        public Label labelAllClients;
        public int clientsAll = 0;
        public Semaphore semaphore = new Semaphore(1, 5000);
        public ListViewMachinesAndTasksHandler listViewMachinesAndTasksHandler;
        public List<ExecutedTaskHandler> executedTasksHandlers;
        public ListBox console;
        public DataGrid grdMachinesGroups;
        public SqlConnection conn;

        public void StartListener()
        {
            try
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
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void SetOffline(ListView list, ShortGuid remoteIdentifier)
        {
            try
            {
                for (int i = list.Items.Count - 1; i >= 0; i--)
                {
                    ComputerDetailsData computer = (ComputerDetailsData)list.Items[i];
                    if (computer.macAddresses != null && ClientsDictionary[remoteIdentifier].ComputerData.macAddresses != null && CheckMacsInREC(computer.macAddresses, ClientsDictionary[remoteIdentifier].ComputerData.macAddresses))
                    {
                        computer.ImageSource = "Images/Offline.ico";
                        list.Items.RemoveAt(i);
                        list.Items.Insert(i, computer);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }        
        }

        private void HandleConnectionClosed(Connection connection)
        {
            try
            {
                semaphore.WaitOne();
                ShortGuid remoteIdentifier = connection.ConnectionInfo.NetworkIdentifier;
                if (ClientsDictionary.ContainsKey(remoteIdentifier))
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        if (listViewMachinesAndTasksHandler.machines.Visibility == Visibility.Visible)
                            SetOffline(listViewMachinesAndTasksHandler.machines, remoteIdentifier);
                        if (listViewMachinesAndTasksHandler.machines_lock.Visibility == Visibility.Visible)
                            SetOffline(listViewMachinesAndTasksHandler.machines_lock, remoteIdentifier);
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
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public static bool CheckMacsInREC(List<string> Macs, List<string> Recs)
        {
            try
            {
                if (Recs != null)
                {
                    if (Macs.Count != 0 && Recs.Count != 0)
                        return IsMacAddressIn(Macs, Recs);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            return false;
        }

        private static bool IsMacAddressIn(List<string> array1, List<string> array2)
        {
            try
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
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
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
                    console.Items.Add("PROBLEM LISTENER CHYBA 1: " + ex.ToString());
                });
                StartListener();
            }
        }

        private void FindComputerInTask(Packet packet, Connection connection, bool failed)
        {
            try
            {
                bool exist = false;
                for (int i = executedTasksHandlers.Count - 1; i >= 0; i--)
                {
                    ExecutedTaskHandler executedTaskHandler = executedTasksHandlers[i];
                    for(int j = executedTaskHandler.computers.Count - 1; j >= 0; j--)
                    {
                        ComputerInTaskHandler computer = executedTaskHandler.computers[j];
                        if (CheckMacsInREC(computer.computer.macAddresses, packet.computerDetailsData.macAddresses))
                        {
                            computer.ClientsDictionary = ClientsDictionary;
                            computer.receivePacket = packet;
                            computer.connection = connection;
                            if(computer.cloning)
                            {
                                try
                                {
                                    computer.semaphoreForCloning.Release();
                                }
                                catch(Exception ex)
                                {
                                    Application.Current.Dispatcher.Invoke(() =>
                                    {
                                        console.Items.Add("PROBLEM LISTENER CHYBA Sempahore release: " + ex.ToString());
                                    });
                                }
                            }
                            if (failed)
                            {
                                computer.Failed(packet.clonningMessage);
                            }
                            exist = true;
                            break;
                        }
                    }
                    if (exist)
                        break;
                }
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    console.Items.Add("PROBLEM LISTENER CHYBA 4: " + ex.ToString());
                });
            }
        }

        public void UpdateDataGridByMacs(List<string> Macs, string Name, string ClassRoom)
        {
            try
            {
                foreach (string Mac in Macs)
                {
                    switch (IsMacInDataGrid(Mac, Name, ClassRoom))
                    {
                        case 0:
                            {
                                Update(Mac, Name, ClassRoom);
                                break;
                            }
                        case 1:
                            {
                                Insert(Mac, Name, ClassRoom);
                                break;
                            }
                        case 2:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private int IsMacInDataGrid(string Mac, string Name, string ClassRoom)
        {
            try
            {
                for (int i = grdMachinesGroups.Items.Count - 1; i >= 0; i--)
                {
                    DataGridCell cell = GetCell(i, 1);
                    if (cell != null)
                    {
                        TextBlock txtBoxMac = cell.Content as TextBlock;
                        if (txtBoxMac.Text != "")
                        {
                            if (txtBoxMac.Text == Mac)
                            {
                                TextBlock txtBoxName = GetCell(i, 2).Content as TextBlock;
                                if (txtBoxName.Text != "")
                                {
                                    if (txtBoxName.Text != Name)
                                    {
                                        return 0;
                                    }
                                }
                                TextBlock txtBoxClassRoom = GetCell(i, 3).Content as TextBlock;
                                if (txtBoxClassRoom.Text != "")
                                {
                                    if (txtBoxClassRoom.Text != ClassRoom)
                                    {
                                        return 0;
                                    }
                                }
                                return 2;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            return 1;
        }

        public DataGridCell GetCell(int row, int column)
        {
            try
            {
                DataGridRow rowContainer = GetRow(row);

                if (rowContainer != null)
                {
                    DataGridCellsPresenter presenter = GetVisualChild<DataGridCellsPresenter>(rowContainer);
                    if (presenter == null)
                        return null;
                    DataGridCell cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
                    if (cell == null)
                    {
                        grdMachinesGroups.ScrollIntoView(rowContainer, grdMachinesGroups.Columns[column]);
                        cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
                    }
                    return cell;
                }
            }            
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            return null;
        }

        public static T GetVisualChild<T>(Visual parent) where T : Visual
        {
            try
            {
                T child = default(T);
                int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
                for (int i = 0; i < numVisuals; i++)
                {
                    Visual v = (Visual)VisualTreeHelper.GetChild(parent, i);
                    child = v as T;
                    if (child == null)
                    {
                        child = GetVisualChild<T>(v);
                    }
                    if (child != null)
                    {
                        break;
                    }
                }
                return child;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            return null;
        }

        public DataGridRow GetRow(int index)
        {
            try
            {
                DataGridRow row = (DataGridRow)grdMachinesGroups.ItemContainerGenerator.ContainerFromIndex(index);
                if (row == null)
                {
                    grdMachinesGroups.UpdateLayout();
                    grdMachinesGroups.ScrollIntoView(grdMachinesGroups.Items[index]);
                    row = (DataGridRow)grdMachinesGroups.ItemContainerGenerator.ContainerFromIndex(index);
                }
                return row;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return null;
            }
        }

        private void SetOnline(ListView list, Packet packet)
        {
            try
            {
                for (int i = list.Items.Count - 1; i >= 0; i--)
                {
                    try
                    {
                        ComputerDetailsData computer = (ComputerDetailsData)list.Items[i];
                        if (!computer.ImageSource.Contains("Folder"))
                        {
                            if (CheckMacsInREC(computer.macAddresses, packet.computerDetailsData.macAddresses))
                            {
                                if (packet.computerDetailsData.inWinpe)
                                    packet.computerDetailsData.ImageSource = "Images/WinPE.ico";
                                else
                                    packet.computerDetailsData.ImageSource = "Images/Online.ico";
                                packet.computerDetailsData.PostInstalls = packet.computerConfigData.PostInstalls;
                                list.Items.RemoveAt(i);
                                list.Items.Insert(i, packet.computerDetailsData);
                                break;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show("BEZIM: " + e.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
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
                                if (listViewMachinesAndTasksHandler.machines.Visibility == Visibility.Visible)
                                    SetOnline(listViewMachinesAndTasksHandler.machines, packet);                                
                                if (listViewMachinesAndTasksHandler.machines_lock.Visibility == Visibility.Visible)
                                    SetOnline(listViewMachinesAndTasksHandler.machines_lock, packet);
                            });

                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                labelOnlineClients.Content = "Online: " + ClientsDictionary.Count;
                                labelOfflineClients.Content = "Offline: " + (clientsAll - ClientsDictionary.Count);
                                labelAllClients.Content = "Clients: " + clientsAll;
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
                    console.Items.Add("PROBLEM LISTENER CHYBA 2: " + ex.ToString());
                });
            }
        }

        public static string GetFileNameByMac(string[] computersInfoFiles, List<string> macAddresses)
        {
            lock (computersInfo)
            {
                try
                {
                    string filePath = "";
                    foreach (string computerFile in computersInfoFiles)
                    {
                        var computerData = FileHandler.Load<ComputerDetailsData>(computerFile);
                        if (computerData != null && computerData.macAddresses != null)
                        {
                            if (CheckMacsInREC(computerData.macAddresses, macAddresses))
                            {
                                filePath = computerFile;
                                break;
                            }
                        }
                    }
                    return filePath;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                return "";
            }
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
                    if (filePath.Contains(@".\Machine Groups\Lock"))
                    {
                        ComputerDetailsData computer = FileHandler.Load<ComputerDetailsData>(filePath);
                        packet.computerDetailsData.Detail = computer.Detail;
                        packet.computerDetailsData.pathNode = computer.pathNode;
                    }
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
                    clientsAll++;
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
                    console.Items.Add("PROBLEM LISTENER CHYBA 3: " + ex.ToString());
                });
            }
        }

        public string GetTheClassRoomID(string ComputerFilePath)
        {
            try
            {
                if (ComputerFilePath.Contains("\\"))
                {
                    string Name = Path.GetFileName(ComputerFilePath);
                    string ClassroomIDFilePath = ComputerFilePath.Replace(Name, "") + "ClassRoomID.txt";
                    if (File.Exists(ClassroomIDFilePath))
                    {
                        return File.ReadAllText(ClassroomIDFilePath);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            return "";
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

        public void BindMyData()
        {
            try
            {
                conn.Open();
                SqlCommand comm = new SqlCommand("SELECT * FROM [Dynamics365_synchro].[dbo].[MachinesGroups]", conn);
                DataSet ds = new DataSet();
                SqlDataAdapter da = new SqlDataAdapter(comm);
                da.Fill(ds);
                Application.Current.Dispatcher.Invoke(() =>
                {
                    grdMachinesGroups.ItemsSource = ds.Tables[0].DefaultView;
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
        }

        public void InsertOrUpdate(List<string> MACs, string Name, string ClassRoom)
        {
            try
            {
                if (ClassRoom != "")
                {
                    foreach (string MAC in MACs)
                    {
                        string command = "IF NOT EXISTS ( SELECT * FROM [Dynamics365_synchro].[dbo].[MachinesGroups] WHERE MAC='" + MAC + "' ) BEGIN INSERT INTO MachinesGroups VALUES('" + MAC + "','" + Name + "'," + ClassRoom + ") END ELSE BEGIN UPDATE [Dynamics365_synchro].[dbo].[MachinesGroups]  SET MAC='" + MAC + "',Name='" + Name + "',ClassRoom=" + ClassRoom + " WHERE MAC='" + MAC + "' END";
                        try
                        {
                            conn.Open();
                            SqlCommand comm = new SqlCommand(command, conn);
                            comm.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message.ToString());
                        }
                        finally
                        {
                            conn.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
}

        public void Update(string MAC, string Name, string ClassRoom)
        {
            try
            {
                conn.Open();
                SqlCommand comm = new SqlCommand("UPDATE [Dynamics365_synchro].[dbo].[MachinesGroups]  SET MAC='" + MAC + "',Name='" + Name + "',ClassRoom=" + ClassRoom + " WHERE MAC='" + MAC + "'", conn);
                comm.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
            finally
            {
                conn.Close();
                BindMyData();
            }
        }

            public void Insert(string MAC, string Name, string ClassRoom)
        {
            try
            {
                conn.Open();
                SqlCommand comm = new SqlCommand("INSERT INTO MachinesGroups VALUES('" + MAC + "','" + Name + "'," + ClassRoom + ")", conn);
                comm.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
            finally
            {
                conn.Close();
                BindMyData();
            }
        }
    }
}
