﻿using GDS_SERVER_WPF.DataCLasses;
using NetworkCommsDotNet.Connections;
using NetworkCommsDotNet.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GDS_SERVER_WPF.Handlers
{
    public class ComputerInTaskHandler
    {
        public ExecutedTaskData executedTaskData;
        public ExecutedTaskHandler executedTaskHandler;
        public Connection connection;
        public int index;
        public List<string> ipAddresses;
        public Semaphore semaphoreForTask = new Semaphore(1, 1);
        public Semaphore semaphoreForCloning = new Semaphore(0, 1);
        public Semaphore semaphoreForSaveFile;
        public ComputerDetailsData computer;
        public bool stopped = false;
        public bool restart = false;
        public bool failed = false;
        public bool finish = false;
        public bool cloning = false;
        public string cloneFaildMessage = "";
        public Dictionary<ShortGuid, ComputerWithConnection> ClientsDictionary;
        public string step;
        public ListView listViewAll;
        public ListView listViewSelected;
        public Packet receivePacket;
        public ProgressComputerData progressComputerData = new ProgressComputerData();
        public List<string> MailsTo;
        TCP_UNICAST tCP_UNICAST = null;
        
        Task waitingTask;
        TaskData taskData;
        FLAG stepDataIdentifier1;
        FLAG stepDataIdentifier2;

        public ComputerInTaskHandler(ExecutedTaskData _executedTaskData, int _index, List<string> _ipAddresses, Semaphore _semaphoreFotSaveFile, ListView _listViewAll, ListView _listViewSelected, List<string> _MailsTo)
        {
            try
            {
                step = "WAITING FOR ACK";
                this.executedTaskData = _executedTaskData;
                this.index = _index;
                this.ipAddresses = _ipAddresses;
                this.taskData = executedTaskData._TaskData;
                this.semaphoreForSaveFile = _semaphoreFotSaveFile;
                this.listViewAll = _listViewAll;
                this.listViewSelected = _listViewSelected;
                this.MailsTo = _MailsTo;
                receivePacket = new Packet(FLAG.Null);
                computer = executedTaskData._TaskData.TargetComputers[index];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void WaitForCloningDone()
        {
            try
            {
                cloning = true;
                receivePacket.ID = FLAG.Null;
                while (receivePacket.ID != FLAG.CLONING_DONE && receivePacket.ID != FLAG.CLONING_ERROR && !stopped)
                {
                    if (receivePacket.ID == FLAG.RESTART || receivePacket.ID == FLAG.SYN_FLAG_WINPE || restart)
                    {
                        ChangeProgressData("NEED RESTART");
                        if (receivePacket.ID == FLAG.RESTART)
                        {
                            try
                            {
                                if (connection != null)
                                {
                                    SendMessage(receivePacket, connection);
                                }
                            }
                            catch
                            {
                                connection = null;
                            }
                        }
                        if (receivePacket.clonningMessage != null && receivePacket.clonningMessage != "")
                        {
                            if (receivePacket.clonningMessage.Contains("CLONE FAILED"))
                            {
                                cloneFaildMessage = receivePacket.clonningMessage;
                                if (cloneFaildMessage.Contains("CLONE FAILED ACCESS DENIED") || cloneFaildMessage.Contains("CLONE FAILED DISK IS TOO SMALL"))
                                {
                                    Failed(cloneFaildMessage);
                                    return;
                                }
                                restart = true;
                                return;
                            }
                        }
                        cloneFaildMessage = "CLIENT NEED RESTART";
                        restart = true;
                        return;
                    }
                    if (receivePacket.clonningMessage != null && receivePacket.clonningMessage != "")
                    {
                        ChangeProgressData(receivePacket.clonningMessage);
                    }
                    semaphoreForCloning.WaitOne();
                }
                cloning = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void SendMessage(Packet packet, Connection connection)
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

        private void CheckFlags(FLAG _stepDataIdentifier1, Packet packet , FLAG _stepDataIdentifier2 = FLAG.CLOSE, int WaitingTime = 5000)
        {
            try
            {
                stepDataIdentifier1 = _stepDataIdentifier1;
                stepDataIdentifier2 = _stepDataIdentifier2;
                receivePacket.ID = FLAG.Null;
                ChangeProgressData(stepDataIdentifier1.ToString());
                try
                {
                    if (connection != null)
                    {
                        SendMessage(packet, connection);
                    }
                }
                catch
                {
                    connection = null;
                }
                Thread.Sleep(1000);
                while (receivePacket.ID != stepDataIdentifier1 && receivePacket.ID != stepDataIdentifier2 && !stopped)
                {
                    if (stopped || failed)
                    {
                        return;
                    }
                    try
                    {
                        if (connection != null)
                        {
                            SendMessage(packet, connection);
                        }
                    }
                    catch
                    {
                        connection = null;
                    }
                    if ((receivePacket.ID == FLAG.SYN_FLAG || receivePacket.ID == FLAG.SYN_FLAG_WINPE) && (receivePacket.ID != stepDataIdentifier1 || receivePacket.ID != stepDataIdentifier2))
                    {
                        cloneFaildMessage = "CLIENT NEED RESTART";
                        restart = true;
                        return;
                    }
                    Thread.Sleep(WaitingTime);
                }
                if (!failed && !stopped)
                {
                    progressComputerData = new ProgressComputerData("Images/Done.ico", computer.Name, receivePacket.ID.ToString(), executedTaskData.GetFileName(), "", computer.MacAddress);
                }
                SaveProgress();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void CheckSynFlag()
        {
            CheckFlags(FLAG.SYN_FLAG, new Packet(FLAG.SYN_FLAG), FLAG.SYN_FLAG_WINPE);
        }

        private void ClientToWinpe()
        {
            CheckFlags(FLAG.CLIENT_TO_WINPE, new Packet(FLAG.CLIENT_TO_WINPE), FLAG.SYN_FLAG_WINPE, 5000);
        }

        private void ClientToOperatingSystem()
        {
            CheckFlags(FLAG.TO_OPERATING_SYSTEM, new Packet(FLAG.TO_OPERATING_SYSTEM), FLAG.SYN_FLAG, 5000);
        }

        private void ClientShutDown()
        {
            CheckFlags(FLAG.SHUTDOWN, new Packet(FLAG.SHUTDOWN), FLAG.SHUTDOWN_DONE);
        }

        private void ClientSendingFileInOS()
        {
            try
            {
                Packet packet = new Packet(FLAG.START_COPY_FILES, computer, connection.ConnectionInfo.NetworkIdentifier)
                {
                    taskData = taskData
                };
                if (taskData.CopyFilesInOS.Count != 0)
                {
                    int PORT = 60000;
                    foreach (string number in computer.IPAddress.Split('.'))
                    {
                        PORT += Convert.ToInt32(number);
                    }
                    packet.clonningMessage = PORT.ToString();
                    CheckFlags(FLAG.START_COPY_FILES, packet);
                    tCP_UNICAST = new TCP_UNICAST(taskData.CopyFilesInOS, taskData.DestinationDirectoryInOS, taskData.SourceDirectoryInOS, computer.IPAddress, PORT);
                    tCP_UNICAST.SendingFiles();
                    CheckFlags(FLAG.FINISH_COPY_FILES, new Packet(FLAG.FINISH_COPY_FILES));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void ClientSendingFileInWinPE()
        {
            try
            {
                Packet packet = new Packet(FLAG.START_COPY_FILES, computer, connection.ConnectionInfo.NetworkIdentifier)
                {
                    taskData = taskData
                };
                if (taskData.CopyFilesInWINPE.Count != 0)
                {
                    int PORT = 60000;
                    foreach (string number in computer.IPAddress.Split('.'))
                    {
                        PORT += Convert.ToInt32(number);
                    }
                    packet.clonningMessage = PORT.ToString();
                    CheckFlags(FLAG.START_COPY_FILES, packet);
                    tCP_UNICAST = new TCP_UNICAST(taskData.CopyFilesInWINPE, taskData.DestinationDirectoryInWINPE, taskData.SourceDirectoryInWINPE, computer.IPAddress, PORT);
                    tCP_UNICAST.SendingFiles();
                    CheckFlags(FLAG.FINISH_COPY_FILES, new Packet(FLAG.FINISH_COPY_FILES));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }        

        private void ClientStartCloning()
        {
            CheckFlags(FLAG.CLONING, new Packet(FLAG.CLONING), FLAG.CLONING_STATUS);
        }

        private void RunCommands()
        {
            CheckFlags(FLAG.RUN_COMMAND, new Packet(FLAG.RUN_COMMAND), FLAG.FINISH_RUN_COMMAND);
        }

        private void RunCommandsWithWait()
        {
            CheckFlags(FLAG.RUN_COMMAND, new Packet(FLAG.RUN_COMMAND));
            CheckFlags(FLAG.FINISH_RUN_COMMAND, new Packet(FLAG.FINISH_RUN_COMMAND));
        }

        private void SendTaskDataFile()
        {
            var packet = new Packet(FLAG.SEND_TASK_CONFIG)
            {
                taskData = taskData
            };
            CheckFlags(FLAG.SEND_TASK_CONFIG, packet, FLAG.CLONING_STATUS);
        }

        private void SendConfigFile()
        {
            try
            {
                var packet = new Packet(FLAG.SEND_CONFIG);
                if (receivePacket.computerConfigData != null)
                    packet.computerConfigData = receivePacket.computerConfigData;
                else
                {
                    var computersInfoFiles = Directory.GetFiles(@".\Machine Groups\", "*.my", SearchOption.AllDirectories);
                    var filePath = Listener.GetFileNameByMac(computersInfoFiles, receivePacket.computerDetailsData.macAddresses);
                    packet.computerConfigData = new ComputerConfigData(receivePacket.computerDetailsData.RealPCName, "Workgroup");
                    if (filePath != "")
                    {
                        if (!File.Exists(filePath.Replace(".my", ".cfg")))
                        {
                            FileHandler.Save<ComputerConfigData>(packet.computerConfigData, filePath.Replace(".my", ".cfg"));
                        }
                        else
                        {
                            packet.computerConfigData = FileHandler.Load<ComputerConfigData>(filePath.Replace(".my", ".cfg"));
                        }
                    }
                }
                CheckFlags(FLAG.SEND_CONFIG, packet);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void SendMail()
        {
            try
            {
                if (MailsTo.Count != 0)
                {
                    MailMessage mail = new MailMessage();
                    SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

                    mail.From = new MailAddress("gdsconsole@gmail.com");
                    foreach (string MailTo in MailsTo)
                        mail.To.Add(MailTo);
                    mail.Subject = "GDS Console Warning";
                    mail.Body = "Computer: " + computer.Name + Environment.NewLine + "Warning TimeOut: " + stepDataIdentifier1 + " or " + stepDataIdentifier2 +
                        Environment.NewLine + "Task: " + taskData.Name;        
                    SmtpServer.Port = 587;
                    SmtpServer.UseDefaultCredentials = false;
                    SmtpServer.Credentials = new System.Net.NetworkCredential("gdsconsole@gmail.com", "GopasBlava");
                    SmtpServer.EnableSsl = true;

                    SmtpServer.Send(mail);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private bool RunAndWait(Action Function)
        {
            try
            {
                if (taskData.InfinityWaitingTime)
                {
                    while (!failed && !stopped)
                    {
                        waitingTask = Task.Run(() => { Function(); });
                        if (waitingTask.Wait(TimeSpan.FromMinutes(executedTaskData._TaskData.WaitingTime + 0.1)))
                        {
                            if (!stopped && !failed)
                            {
                                return true;
                            }
                        }
                        if (!stopped && !failed && taskData.SendWarningMails)
                        {
                            SendMail();
                        }
                    }
                }
                else
                {
                    if (!failed && !stopped)
                    {
                        waitingTask = Task.Run(() => { Function(); });
                        if (waitingTask.Wait(TimeSpan.FromMinutes(executedTaskData._TaskData.WaitingTime + 0.1)))
                        {
                            if (!stopped && !failed)
                            {
                                return true;
                            }
                        }
                        if (!stopped && !failed && taskData.SendWarningMails)
                        {
                            SendMail();
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

        private void SaveProgress()
        {
            try
            {
                if (progressComputerData.Step != "Null")
                {
                    semaphoreForSaveFile.WaitOne();
                    progressComputerData.MacAddress = computer.MacAddress;
                    executedTaskData.ProgressComputerData.Add(progressComputerData);
                    FileHandler.Save(executedTaskData, executedTaskData.GetFileName());
                    semaphoreForSaveFile.Release();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void SoftwareAndFileActionsInOS()
        {
            if (executedTaskData._TaskData.SoftwareAndFileAction)
            {
                if (!RunAndWait(ClientToOperatingSystem))
                {
                    Failed();
                    return;
                }
                if (!RunAndWait(SendTaskDataFile))
                {
                    Failed();
                    return;
                }
                if (!RunAndWait(ClientSendingFileInOS))
                {
                    Failed();
                    return;
                }
                if (!RunAndWait(RunCommands))
                {
                    Failed();
                    return;
                }
            }
        }

        private void SoftwareAndFileActionsWinPE()
        {            
            if (executedTaskData._TaskData.SoftwareAndFileAction_WINPE)
            {
                if (!RunAndWait(ClientToWinpe))
                {
                    Failed();
                    return;
                }
                if (!RunAndWait(SendTaskDataFile))
                {
                    Failed();
                    return;
                }
                if(!RunAndWait(ClientSendingFileInWinPE))
                {
                    Failed();
                    return;
                }
                if (!RunAndWait(RunCommands))
                {
                    Failed();
                    return;
                }
            }            
        }
        
        private void CloningImage()
        {
            if (taskData.Cloning)
            {
                if (!RunAndWait(ClientToWinpe))
                {
                    Failed();
                    return;
                }
                Configuration();
                if (!RunAndWait(SendTaskDataFile))
                {
                    Failed();
                    return;                        
                }
                if(!RunAndWait(ClientStartCloning))
                {
                    Failed();
                    return;
                }
                WaitForCloningDone();
                if (receivePacket.ID == FLAG.RESTART || receivePacket.ID == FLAG.SYN_FLAG_WINPE || restart)
                {
                    progressComputerData = new ProgressComputerData("Images/Failed.ico", computer.Name, "RESTART CLONNIG", executedTaskData.GetFileName(), cloneFaildMessage, computer.MacAddress);
                    SaveProgress();
                    return;
                }
                SoftwareAndFileActionsWinPE();
                if (!RunAndWait(ClientToOperatingSystem))
                {
                    Failed();
                    return;
                }
            }
            else
            {
                SoftwareAndFileActionsWinPE();
            }
        }

        private void Configuration()
        {
            if (taskData.Configuration)
            {
                if (!taskData.Cloning)
                {
                    if (!RunAndWait(ClientToOperatingSystem))
                    {
                        Failed();
                        return;
                    }
                    
                }
                if (!RunAndWait(SendConfigFile))
                {
                    Failed();
                    return;
                }                            
            }
        }

        private void CheckOnline()
        {
            if (!RunAndWait(CheckSynFlag))
            {
                Failed();
                return;
            }
        }

        private void ShutDown()
        {
            if (taskData.ShutDown)
            {
                if (!RunAndWait(ClientShutDown))
                {
                    Failed();
                    return;
                }
            }
        }

        private void WakeOnLan()
        {
            if(taskData.WakeOnLan)
            {
                WakeOnLanHandler.runWakeOnLan(computer.macAddresses, ipAddresses);
            }            
        }

        public void Steps()
        {
            Start:
            restart = false;
            cloning = false;
            failed = false;
            finish = false;
            stopped = false;
            FindClient();           
            WakeOnLan();
            CheckOnline();
            if (restart)
                goto Start;
            CloningImage();
            if (restart)
                goto Start;
            SoftwareAndFileActionsInOS();
            if (restart)
                goto Start;
            Configuration();
            if (restart)
                goto Start;
            ShutDown();
            if (restart)
                goto Start;
            if(failed)
                ChangeProgressData("FAILED");
            else
                ChangeProgressData("COMPLETED");
            finish = true;
        }        

        public void Start()
        {
            try
            {
                semaphoreForTask.WaitOne();
                Steps();
                semaphoreForTask.Release();
            }
            catch (Exception ex)
            {                             
                MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("PROBLEM START COMPUTER: " + ex.ToString() + " \n WILL YOU RESTART CLIENT?", "Confirmation", System.Windows.MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    Start();
                }
                else
                {
                    finish = true;
                    ChangeProgressData("FAILED");
                    semaphoreForTask.Release();
                }
            }
        }

        private void FindClient()
        {
            try
            {
                lock (ClientsDictionary)
                {
                    for (int index = ClientsDictionary.Count - 1; index >= 0; index--)
                    {
                        var cmp = ClientsDictionary.ElementAt(index);
                        if (cmp.Value.ComputerData.macAddresses != null && Listener.CheckMacsInREC(cmp.Value.ComputerData.macAddresses, computer.macAddresses))
                        {
                            connection = cmp.Value.connection;
                            break;
                        }
                    }
                }
            }
            catch
            {
                Thread.Sleep(10000);
                FindClient();                
            }
        }

        public void Failed(string Message = "Expired waiting time")
        {
            try
            {
                if (!failed && !stopped)
                {
                    failed = true;
                    if (stopped)
                        progressComputerData = new ProgressComputerData("Images/Stopped.ico", computer.Name, stepDataIdentifier1.ToString(), executedTaskData.GetFileName(), Message, computer.MacAddress);
                    else
                        progressComputerData = new ProgressComputerData("Images/Failed.ico", computer.Name, stepDataIdentifier1.ToString(), executedTaskData.GetFileName(), Message, computer.MacAddress);
                    SaveProgress();
                }
                if (tCP_UNICAST != null)
                {
                    tCP_UNICAST.DestroyConnection();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public void Stop()
        {
            try
            {
                semaphoreForCloning.Release();
                if (!stopped && !failed && !finish)
                {
                    stopped = true;
                    failed = true;
                    if (connection != null)
                    {
                        SendMessage(new Packet(FLAG.ERROR_MESSAGE), connection);
                    }
                }
                if (tCP_UNICAST != null)
                {
                    tCP_UNICAST.DestroyConnection();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public void Restart()
        {
            try
            {
                semaphoreForCloning.Release();
                if (!stopped && !failed && !finish)
                {
                    restart = true;
                    if (connection != null)
                    {
                        SendMessage(new Packet(FLAG.ERROR_MESSAGE), connection);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void ChangeProgressData(string _step)
        {
            try
            {
                step = _step;
                Application.Current.Dispatcher.Invoke(() =>
                {
                    progressComputerData = new ProgressComputerData("", computer.Name, step, executedTaskData.GetFileName(), "", computer.MacAddress);
                    int index = GetIndexOfItemFromList(listViewAll);
                    if (index != -1)
                    {
                        listViewAll.Items[index] = progressComputerData;
                    }
                    index = GetIndexOfItemFromList(listViewSelected);
                    if (index != -1)
                    {
                        listViewSelected.Items[index] = progressComputerData;
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private int GetIndexOfItemFromList(ListView listView)
        {
            try
            {
                for (int i = listView.Items.Count - 1; i >= 0; i--)
                {
                    ProgressComputerData item = (ProgressComputerData)listView.Items[i];
                    if (item.Task_ID == executedTaskData.GetFileName())
                    {
                        if (item.ComputerName == computer.Name)
                        {
                            if (item.MacAddress == "")
                                return i;
                            else
                            {
                                if (item.MacAddress == computer.MacAddress)
                                    return i;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            return -1;
        }

        public void RemoveFromListViewAll()
        {
            try
            {
                int index = GetIndexOfItemFromList(listViewAll);
                if (index != -1)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        ProgressComputerData item = (ProgressComputerData)listViewAll.Items[index];
                        listViewAll.Items.Remove(item);
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public void RemoveFromListViewSelected()
        {
            try
            {
                int index = GetIndexOfItemFromList(listViewSelected);
                if (index != -1)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        ProgressComputerData item = (ProgressComputerData)listViewSelected.Items[index];
                        listViewSelected.Items.Remove(item);
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
