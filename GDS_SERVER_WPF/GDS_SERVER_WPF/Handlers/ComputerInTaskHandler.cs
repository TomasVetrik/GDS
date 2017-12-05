using GDS_SERVER_WPF.DataCLasses;
using NetworkCommsDotNet.Connections;
using NetworkCommsDotNet.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GDS_SERVER_WPF.Handlers
{
    public class ComputerInTaskHandler
    {
        public ExecutedTaskData executedTaskData;
        public Connection connection;
        public int index;
        public List<string> ipAddresses;
        public Semaphore semaphoreForTask = new Semaphore(1,1);
        public Semaphore semaphoreForSaveFile;
        public ComputerDetailsData computer;
        public bool stopped = false;
        public bool restart = false;
        public bool failed = false;
        public bool finish = false;
        public Dictionary<ShortGuid, ComputerWithConnection> ClientsDictionary;
        public string step;
        public ListView listViewAll;
        public ListView listViewSelected;
        public Packet receivePacket;   
        

        Task waitingTask;
        TaskData taskData;
        FLAG stepDataIdentifier1;
        FLAG stepDataIdentifier2;

        public ComputerInTaskHandler(ExecutedTaskData _executedTaskData, int _index, List<string> _ipAddresses, Semaphore _semaphoreFotSaveFile, ListView _listViewAll, ListView _listViewSelected)
        {
            step = "WAITING FO ACK";
            semaphoreForTask.WaitOne();
            this.executedTaskData = _executedTaskData;            
            this.index = _index;
            this.ipAddresses = _ipAddresses;
            this.taskData = executedTaskData.taskData;
            this.semaphoreForSaveFile = _semaphoreFotSaveFile;
            this.listViewAll = _listViewAll;
            this.listViewSelected = _listViewSelected;
            receivePacket = new Packet(FLAG.CLOSE);
            computer = executedTaskData.taskData.TargetComputers[index];            
        }

        private void WaitForCloningDone()
        {            
            receivePacket.ID = FLAG.CLOSE;
            while (receivePacket.ID != FLAG.CLONING_DONE  && receivePacket.ID != FLAG.CLONING_ERROR && !stopped)
            {
                if (receivePacket.ID == FLAG.RESTART || receivePacket.ID == FLAG.SYN_FLAG_WINPE)
                {
                    ChangeProgressData("NEED RESTART");
                    return;
                }
                if (receivePacket.clonningMessage != "")
                    ChangeProgressData(receivePacket.clonningMessage);
                Thread.Sleep(1000);
            }
        }

        private void SendMessage(Packet packet, Connection connection)
        {
            byte[] data = Proto.ProtoSerialize<Packet>(packet);
            connection.SendObject("Packet", data);
        }

        private void CheckFlags(FLAG _stepDataIdentifier1, Packet packet , FLAG _stepDataIdentifier2 = FLAG.Null, int WaitingTime = 1000)
        {
            stepDataIdentifier1 = _stepDataIdentifier1;
            stepDataIdentifier2 = _stepDataIdentifier2;
            receivePacket.ID = FLAG.CLOSE;            
            ChangeProgressData(stepDataIdentifier1.ToString());
            while (receivePacket.ID != stepDataIdentifier1 && receivePacket.ID != stepDataIdentifier2 && !stopped)
            {
                if(stopped || failed)
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
                Thread.Sleep(WaitingTime);
            }
            if (!failed && !stopped)
            {
                SaveProgress(new ProgressComputerData("Images/Done.ico", computer.Name, receivePacket.ID.ToString()));
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

        private void ClientStartCloning()
        {
            CheckFlags(FLAG.CLONING, new Packet(FLAG.CLONING), FLAG.CLONING_STATUS);
        }

        private void RunCommands()
        {
            CheckFlags(FLAG.RUN_COMMAND, new Packet(FLAG.RUN_COMMAND),FLAG.FINISH_RUN_COMMAND);
        }

        private void SendTaskDataFile()
        {
            var packet = new Packet(FLAG.SEND_TASK_CONFIG);
            packet.taskData = taskData;
            CheckFlags(FLAG.SEND_TASK_CONFIG, packet, FLAG.CLONING_STATUS);
        }

        private void SendConfigFile()
        {
            var packet = new Packet(FLAG.SEND_CONFIG);
            packet.computerConfigData = receivePacket.computerConfigData;
            CheckFlags(FLAG.SEND_CONFIG, packet);
        }

        private bool RunAndWait(Action Function)
        {
            if (!failed && !stopped)
            {
                waitingTask = Task.Run(() => { Function(); });
                if (waitingTask.Wait(TimeSpan.FromMinutes(executedTaskData.taskData.WaitingTime+0.1)))
                {
                    if (!stopped && !failed)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void SaveProgress(ProgressComputerData progress)
        {
            if (progress.Step != "Null")
            {
                semaphoreForSaveFile.WaitOne();
                executedTaskData.progressComputerData.Add(progress);
                FileHandler.Save(executedTaskData, executedTaskData.GetFileName());
                semaphoreForSaveFile.Release();
            }
        }

        private void SoftwareAndFileActionsInOS()
        {
            if (executedTaskData.taskData.SoftwareAndFileAction)
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
                if (!RunAndWait(RunCommands))
                {
                    Failed();
                    return;
                }
            }
        }

        private void SoftwareAndFileActionsWinPE()
        {            
            if (executedTaskData.taskData.SoftwareAndFileAction_WINPE)
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
                if (receivePacket.ID == FLAG.RESTART || receivePacket.ID == FLAG.SYN_FLAG_WINPE)
                {
                    SaveProgress(new ProgressComputerData("Images/Failed.ico", computer.Name, "RESTART CLONNIG"));
                    CloningImage();
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
                    if (!RunAndWait(SendConfigFile))
                    {
                        Failed();
                        return;
                    }
                }    
                else
                {
                    if (!RunAndWait(SendConfigFile))
                    {
                        Failed();
                        return;
                    }
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
        
        public void Start()
        {
            try
            {
                FindClient();
                WakeOnLanHandler.runWakeOnLan(computer.macAddresses, ipAddresses);
                CheckOnline();
                CloningImage();
                SoftwareAndFileActionsInOS();
                Configuration();
                ShutDown();
                ChangeProgressData("FINISH");
                finish = true;
                semaphoreForTask.Release();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void FindClient()
        {
            foreach (KeyValuePair<ShortGuid, ComputerWithConnection> cmp in ClientsDictionary)
            {
                if (cmp.Value.ComputerData.macAddresses != null && Listener.CheckMacsInREC(cmp.Value.ComputerData.macAddresses, computer.macAddresses))
                {
                    connection = cmp.Value.connection;
                }
            }
        }

        public void Failed()
        {
            if (!failed && !stopped)
            {
                failed = true;
                SaveProgress(new ProgressComputerData("Images/Failed.ico", computer.Name, stepDataIdentifier1.ToString()));                
            }
        }

        public void Stop()
        {
            if (!stopped && !failed && !finish)
            {
                stopped = true;
                SaveProgress(new ProgressComputerData("Images/Stopped.ico", computer.Name, stepDataIdentifier1.ToString()));
                if (connection != null)
                {
                    SendMessage(new Packet(FLAG.ERROR_MESSAGE), connection);
                }
            }
        }

        private void ChangeProgressData(string _step)
        {
            step = _step;
            Application.Current.Dispatcher.Invoke(() =>
            {
                int index = GetIndexOfItemFromList(listViewAll);
                if (index != -1)
                {
                    listViewAll.Items[index] = new ProgressComputerData("", computer.Name, step, computer.MacAddress);
                }
                index = GetIndexOfItemFromList(listViewSelected);
                if (index != -1)
                {
                    listViewSelected.Items[index] = new ProgressComputerData("", computer.Name, step, computer.MacAddress);
                }
            });
        }

        private int GetIndexOfItemFromList(ListView listView)
        {
            for (int i = listView.Items.Count - 1; i >= 0; i--)
            {
                ProgressComputerData item = (ProgressComputerData)listView.Items[i];
                if (item.ComputerName == computer.Name && item.MacAddress == computer.MacAddress)
                {
                    return i;                    
                }
            }
            return -1;
        }

        public void RemoveFromListViewAll()
        {
            int index = GetIndexOfItemFromList(listViewAll);
            if(index != -1)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    ProgressComputerData item = (ProgressComputerData)listViewAll.Items[index];
                    listViewAll.Items.Remove(item);
                });
            }
        }

        public void RemoveFromListViewSelected()
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
    }
}
