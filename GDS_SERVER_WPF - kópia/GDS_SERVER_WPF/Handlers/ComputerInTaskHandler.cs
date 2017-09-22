using GDS_SERVER_WPF.DataCLasses;
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
        public List<ClientHandler> clients;
        public int index;
        public List<string> ipAddresses;
        public Semaphore semaphoreForTask = new Semaphore(1,1);
        public Semaphore semaphoreForSaveFile;
        public ComputerDetailsData computer;
        public ClientHandler client;
        public bool stopped = false;
        public bool restart = false;
        public bool failed = false;
        public bool finish = false;
        public string step;
        public ListView listViewAll;
        public ListView listViewSelected;
        public Packet receivePacket;
        public ComputerConfigData computerConfigData;
        

        Task waitingTask;
        TaskData taskData;
        DataIdentifier stepDataIdentifier1;
        DataIdentifier stepDataIdentifier2;

        public ComputerInTaskHandler(ExecutedTaskData _executedTaskData, List<ClientHandler> _clients, int _index, List<string> _ipAddresses, Semaphore _semaphoreFotSaveFile, ListView _listViewAll, ListView _listViewSelected)
        {
            step = "WAITING FO ACK";
            semaphoreForTask.WaitOne();
            this.executedTaskData = _executedTaskData;
            this.clients = _clients;
            this.index = _index;
            this.ipAddresses = _ipAddresses;
            this.taskData = executedTaskData.taskData;
            this.semaphoreForSaveFile = _semaphoreFotSaveFile;
            this.listViewAll = _listViewAll;
            this.listViewSelected = _listViewSelected;
            receivePacket = new Packet(DataIdentifier.CLOSE);
            computer = executedTaskData.taskData.TargetComputers[index];            
        }        

        private void FindClient()
        {
            for(int i = 0; i < clients.Count; i++)
            {
                if(clients[i].CheckMacsInREC(clients[i].macAddresses, computer.macAddresses))
                {
                    client = clients[i];
                    client.computerInTaskHandler = this;
                    client.receivePacket = receivePacket;
                }
            }
        }

        private void WaitForCloningDone()
        {            
            receivePacket.dataIdentifier = DataIdentifier.CLOSE;
            while (receivePacket.dataIdentifier != DataIdentifier.CLONING_DONE  && receivePacket.dataIdentifier != DataIdentifier.CLONING_ERROR && !stopped)
            {
                if (receivePacket.dataIdentifier == DataIdentifier.RESTART || receivePacket.dataIdentifier == DataIdentifier.SYN_FLAG_WINPE)
                {
                    ChangeProgressData("NEED RESTART");
                    return;
                }
                ChangeProgressData(receivePacket.clonningMessage);
                Thread.Sleep(1000);
            }
        }

        private void CheckFlags(DataIdentifier _stepDataIdentifier1, Packet packet , DataIdentifier _stepDataIdentifier2 = DataIdentifier.Null, int WaitingTime = 1000)
        {
            stepDataIdentifier1 = _stepDataIdentifier1;
            stepDataIdentifier2 = _stepDataIdentifier2;
            receivePacket.dataIdentifier = DataIdentifier.CLOSE;            
            ChangeProgressData(stepDataIdentifier1.ToString());
            while (receivePacket.dataIdentifier != stepDataIdentifier1 && receivePacket.dataIdentifier != stepDataIdentifier2 && !stopped)
            {
                if(stopped || failed)
                {
                    return;
                }
                try
                {
                    if (client != null)
                    {
                        client.SendMessage(packet);
                    }
                }
                catch
                {
                    client = null;
                }
                Thread.Sleep(WaitingTime);
            }
            if (!failed && !stopped)
            {
                SaveProgress(new ProgressComputerData("Images/Done.ico", computer.Name, receivePacket.dataIdentifier.ToString()));
            }
        }

        private void CheckSynFlag()
        {
            CheckFlags(DataIdentifier.SYN_FLAG, new Packet(DataIdentifier.SYN_FLAG), DataIdentifier.SYN_FLAG_WINPE);
        }

        private void ClientToWinpe()
        {
            CheckFlags(DataIdentifier.CLIENT_TO_WINPE, new Packet(DataIdentifier.CLIENT_TO_WINPE), DataIdentifier.SYN_FLAG_WINPE, 5000);
        }

        private void ClientToOperatingSystem()
        {
            CheckFlags(DataIdentifier.TO_OPERATING_SYSTEM, new Packet(DataIdentifier.TO_OPERATING_SYSTEM), DataIdentifier.SYN_FLAG, 5000);
        }

        private void ClientShutDown()
        {
            CheckFlags(DataIdentifier.SHUTDOWN, new Packet(DataIdentifier.SHUTDOWN));
        }

        private void ClientStartCloning()
        {
            CheckFlags(DataIdentifier.CLONING, new Packet(DataIdentifier.CLONING), DataIdentifier.CLONING_STATUS);
        }

        private void RunCommands()
        {
            CheckFlags(DataIdentifier.RUN_COMMAND, new Packet(DataIdentifier.RUN_COMMAND),DataIdentifier.FINISH_RUN_COMMAND);
        }

        private void SendTaskDataFile()
        {
            var packet = new Packet(DataIdentifier.SEND_TASK_CONFIG);
            packet.taskData = taskData;
            CheckFlags(DataIdentifier.SEND_TASK_CONFIG, packet, DataIdentifier.CLONING_STATUS);
        }

        private void SendConfigFile()
        {
            var packet = new Packet(DataIdentifier.SEND_CONFIG);
            packet.computerConfigData = computerConfigData;
            CheckFlags(DataIdentifier.SEND_CONFIG, packet);
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
                if (receivePacket.dataIdentifier == DataIdentifier.RESTART || receivePacket.dataIdentifier == DataIdentifier.SYN_FLAG_WINPE)
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
                if (client != null)
                {
                    client.SendMessage(new Packet(DataIdentifier.ERROR_MESSAGE));
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
