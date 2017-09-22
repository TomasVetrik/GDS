using GDS_SERVER_WPF.DataCLasses;
using GDS_SERVER_WPF.Handlers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GDS_SERVER_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    ///   

    public partial class MainWindow : Window
    {        

        List<ComputerDetailsData> clipBoardMachines = new List<ComputerDetailsData>();
        List<TaskData> clipBoardTasks = new List<TaskData>();
        string nodePathOld = "";
        bool copy;
        bool machines;
        Thread Server;
        List<ExecutedTaskHandler> ExecutedTasksHandlers;
        Listener listener;
        TreeViewHandler treeViewMachinesAndTasksHandler;
        ListViewMachinesAndTasksHandler listViewMachinesAndTasksHandler;
        ListViewTaskDetailsHandler listViewTaskDetailsHandler;
        string LockPath = @".\Machine Groups\Lock";
        string DefaultPath = @".\Machine Groups\Default";
        List<string> ipAddresses;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CheckDirectories();
            treeViewMachinesAndTasksHandler = new TreeViewHandler(treeViewMachinesAndTasks);                        
            listViewMachinesAndTasksHandler = new ListViewMachinesAndTasksHandler(listViewMachineGroups, listViewTasks, treeViewMachinesAndTasksHandler);
            listViewTaskDetailsHandler = new ListViewTaskDetailsHandler(listViewTasksDetails);
            listViewMachinesAndTasksHandler.LoadTreeViewMachinesAndTasks();
            listViewTaskDetailsHandler.LoadTasksDetails();
            ExecutedTasksHandlers = new List<ExecutedTaskHandler>();
            listener = new Listener();
            listener.executedTasksHandlers = ExecutedTasksHandlers;
            listener.listBox1 = this.consoleGDS;
            listener.labelOnlineClients = labelOnline;
            listener.labelOfflineClients = labelOffilne;
            listener.labelAllClients = labelClients;
            listener.listViewMachinesAndTasksHandler = listViewMachinesAndTasksHandler;
            listener.clientsAll = Directory.GetFiles(@".\Machine Groups\", "*.my", SearchOption.AllDirectories).Length;
            LoadIpAddresses();
            listener.StartListener();
            //Server = new Thread(listener.StartListener);
            //Server.Start();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                for (int i = listViewMachinesAndTasksHandler.clients.Count; i >= 0; i--)
                    listViewMachinesAndTasksHandler.clients[i].Close();
            }
            catch { }
            Environment.Exit(Environment.ExitCode);
        }

        private void MenuItemMachineGroupsRename_Click(object sender, RoutedEventArgs e)
        {
            RenameItem((ComputerDetailsData)listViewMachineGroups.SelectedItem);
        }       
        private void MenuItemDelete_Click(object sender, RoutedEventArgs e)
        {
            DeleteItem();
        }
        private void MenuItemMachineGroupsWOL_Click(object sender, RoutedEventArgs e)
        {
            runWakeOnLanOnSelectedItems();
        }
        private void MenuItemMachineGroupsRDP_Click(object sender, RoutedEventArgs e)
        {
            RemoteDesktop();
        }
        private void MenuItemCreateFolder_Click(object sender, RoutedEventArgs e)
        {
            NewFolder();
        }        
        private void MenuItemTaskNew_Click(object sender, RoutedEventArgs e)
        {            
            var taskOptionsDialog = new TaskOptions();
            taskOptionsDialog.path = "";
            taskOptionsDialog.nodePath = treeViewMachinesAndTasksHandler.GetNodePath();
            taskOptionsDialog.clients = listViewMachinesAndTasksHandler.clients;
            taskOptionsDialog.ExecutedTasksHandlers = ExecutedTasksHandlers;
            foreach (TaskData item in listViewTasks.Items)
            {
                taskOptionsDialog.Names.Add(item.Name);
            }
            taskOptionsDialog.ShowDialog();
            if (taskOptionsDialog.executed)
            {
                RunTask(taskOptionsDialog);
            }
            listViewMachinesAndTasksHandler.Refresh();
        }
        private void MenuItemTaskRename_Click(object sender, RoutedEventArgs e)
        {
            RenameItem((TaskData)listViewTasks.SelectedItem);
        }
        private void MenuItemDetailsDelete_Click(object sender, RoutedEventArgs e)
        {
            if (listViewTasksDetails.SelectedItem != null)
            {
                var taskDetails = (ExecutedTaskData)listViewTasksDetails.SelectedItem;
                if (taskDetails.Status != "Images/Progress.ico")
                {
                    string path = taskDetails.GetFileName();
                    if (File.Exists(path))
                    {
                        File.Delete(path);
                        listViewTaskDetailsHandler.Refresh();
                    }
                }
                else
                {
                    bool exist = false;
                    for (int j = ExecutedTasksHandlers.Count - 1; j >= 0; j--)
                    {
                        ExecutedTaskHandler item = ExecutedTasksHandlers[j];
                        if (item.executedTaskData.Name == taskDetails.Name && item.executedTaskData.Started == taskDetails.Started)
                        {
                            exist = true;
                            break;
                        }
                    }
                    if (!exist)
                    {
                        string path = taskDetails.GetFileName();
                        if (File.Exists(path))
                        {
                            File.Delete(path);
                            listViewTaskDetailsHandler.Refresh();
                        }
                    }
                }
            }
        }
        private void MenuItemMachineGroupsConfigureTemplate_Click(object sender, RoutedEventArgs e)
        {
            ConfigureTemplate();
        }
        private void MenuItemDetailsDeleteAll_Click(object sender, RoutedEventArgs e)
        {
            if (listViewTasksDetails.SelectedItems.Count != 0)
            {
                for (int i = listViewTasksDetails.Items.Count-1; i >= 0; i--)
                {
                    ExecutedTaskData taskDetails = (ExecutedTaskData)listViewTasksDetails.Items[i];
                    if (taskDetails.Status != "Images/Progress.ico")
                    {
                        string path = taskDetails.GetFileName();
                        if (File.Exists(path))
                        {
                            File.Delete(path);
                            listViewTaskDetailsHandler.Refresh();
                        }
                    }
                    else
                    {
                        bool exist = false;
                        for(int j = ExecutedTasksHandlers.Count - 1; j >= 0; j--)
                        {
                            ExecutedTaskHandler item = ExecutedTasksHandlers[j];
                            if(item.executedTaskData.Name == taskDetails.Name && item.executedTaskData.Started == taskDetails.Started)
                            {
                                exist = true;
                                break;
                            }
                        }
                        if(!exist)
                        {
                            string path = taskDetails.GetFileName();
                            if (File.Exists(path))
                            {
                                File.Delete(path);
                                listViewTaskDetailsHandler.Refresh();
                            }
                        }
                    }
                }
            }
        }
        private void MenuItemDetailsStop_Click(object sender, RoutedEventArgs e)
        {
            if (listViewTasksDetails.SelectedItem != null)
            {
                var taskDetails = (ExecutedTaskData)listViewTasksDetails.SelectedItem;
                if (taskDetails.Status == "Images/Progress.ico")
                {
                    for (int i = ExecutedTasksHandlers.Count - 1; i >= 0; i--)                        
                    {
                        ExecutedTaskHandler executedTaskDataHandler = ExecutedTasksHandlers[i];
                        if (executedTaskDataHandler.executedTaskData.Name == taskDetails.Name && executedTaskDataHandler.executedTaskData.Started == taskDetails.Started)
                        {
                            executedTaskDataHandler.Stop();
                        }   
                    }
                }
            }
        }

        private void LoadIpAddresses()
        {
            try
            {
                ipAddresses = new List<string>();
                NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
                foreach (NetworkInterface networkInterface in networkInterfaces)
                {
                    if ((!networkInterface.Supports(NetworkInterfaceComponent.IPv4)) ||
                        (networkInterface.OperationalStatus != OperationalStatus.Up))
                    {
                        continue;
                    }

                    IPInterfaceProperties adapterProperties = networkInterface.GetIPProperties();
                    UnicastIPAddressInformationCollection unicastIPAddresses = adapterProperties.UnicastAddresses;
                    IPAddress ipAddress = null;

                    foreach (UnicastIPAddressInformation unicastIPAddress in unicastIPAddresses)
                    {
                        if (unicastIPAddress.Address.AddressFamily != AddressFamily.InterNetwork)
                        {
                            continue;
                        }

                        ipAddress = unicastIPAddress.Address;
                        break;
                    }

                    if (ipAddress == null)
                    {
                        continue;
                    }
                    ipAddresses.Add(ipAddress.ToString());
                }
            }
            catch
            {

            }
        }

        private void runWakeOnLanOnSelectedItems()
        {
            foreach (ComputerDetailsData item in listViewMachineGroups.SelectedItems)
            {
                if (item.ImageSource.Contains("Folder"))
                    WakeOnLanHandler.runWakeOnLan(item.macAddresses, ipAddresses);
            }
        }

        private void RemoteDesktop()
        {
            var computer = (ComputerDetailsData)listViewMachineGroups.SelectedItem;
            if (computer != null)
            {
                if (computer.inWinpe && computer.ImageSource.Contains("Winpe"))
                    DartViewer(computer);
                else
                    System.Diagnostics.Process.Start("mstsc", "/v:" + computer.IPAddress);
            }
        }

        private void ConfigureTemplate()
        {
            var ConfigureTemplateDialog = new ConfigureTemplate();
            ConfigureTemplateDialog.ShowDialog();
            if (!ConfigureTemplateDialog.cancel)
            {
                foreach (ComputerDetailsData computer in listViewMachineGroups.SelectedItems)
                {
                    string pathConfigFile = treeViewMachinesAndTasksHandler.GetNodePath() + "\\" + computer.Name + ".cfg";
                    if (File.Exists(pathConfigFile))
                    {
                        var computerConfigFile = FileHandler.Load<ComputerConfigData>(pathConfigFile);
                        computerConfigFile.Workgroup = ConfigureTemplateDialog.textBoxNewName.Text;
                        FileHandler.Save(computerConfigFile, pathConfigFile);
                    }
                }
            }
        }

        private void DartViewer(ComputerDetailsData computer)
        {
            try
            {                
                string temp = computer.dartInfo;
                string[] details = temp.Split('#');
                string ticket = details[0];
                string port = details[2];
                string IPAdd = details[1];
                System.Diagnostics.Process.Start(@"C:\Windows\Sysnative\DartRemoteViewer.exe", "-ticket=" + ticket + " -IPaddress=" + IPAdd + " -port=" + port);
            }
            catch(Exception e)
            {
                MessageBox.Show(e.ToString());
            }
       } 

        private void RunTask(TaskOptions dialog)
        {
            ExecutedTaskData executedTask = new ExecutedTaskData();
            executedTask.Name = dialog.taskData.Name;
            executedTask.Status = "Images/Progress.ico";
            executedTask.Started = dialog.taskData.LastExecuted;
            executedTask.Finished = "NONE";
            executedTask.Clients = dialog.taskData.TargetComputers.Count.ToString();
            executedTask.Done = "0";
            executedTask.Failed = "0";
            executedTask.MachineGroup = dialog.taskData.MachineGroup;
            executedTask.taskData = dialog.taskData;
            ExecutedTaskHandler taskHandler = new ExecutedTaskHandler(executedTask, ipAddresses, listViewComputersProgressAll, listViewComputersProgressSelected, listViewTaskDetailsProgress);
            ExecutedTasksHandlers.Add(taskHandler);
            taskHandler.handlers = ExecutedTasksHandlers;
            taskHandler.listViewHandler = listViewTaskDetailsHandler;
            taskHandler.clients = listViewMachinesAndTasksHandler.clients;
            Thread taskHandlerThread = new Thread(taskHandler.Start);            
            taskHandlerThread.Start();
        }                     

        private void CheckDirectories()
        {
            var paths = new List<string>() { @".\Machine Groups", @".\Tasks", DefaultPath, LockPath, @".\TaskDetails", @".\Base", @".\DriveE" };
            foreach(string path in paths)
            {
                if(!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }
        }

        private void treeViewMachinesAndTasks_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {            
            if(treeViewMachinesAndTasks.SelectedItem != null)
            {
                (treeViewMachinesAndTasks.SelectedItem as TreeViewItem).IsExpanded = true;
            }
            if (!treeViewMachinesAndTasksHandler.refreshing)
            {
                var path = treeViewMachinesAndTasksHandler.GetNodePath();
                listViewMachinesAndTasksHandler.SetVisibility(path);
                if (listViewMachineGroups.Visibility == Visibility.Visible)
                    ChnageLabels(listViewMachineGroups.Items.Count, listViewMachineGroups.SelectedItems.Count);
                else
                    ChnageLabels(listViewTasks.Items.Count, listViewTasks.SelectedItems.Count);
            }
        }

        private void listViewTaskDetailsProgress_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = (ExecutedTaskData)listViewTaskDetailsProgress.SelectedItem;
            if (selectedItem != null)
            {
                for (int i = ExecutedTasksHandlers.Count - 1; i >= 0; i--)
                {
                    var item = ExecutedTasksHandlers[i];
                    if(item.executedTaskData.Name == selectedItem.Name && item.executedTaskData.Started == selectedItem.Started)
                    {
                        item.AddComputersToListViewSelected();
                        break;
                    }
                }
            }
        }

        private void listViewMachineGroups_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selectedItem = (ComputerDetailsData)listViewMachineGroups.SelectedItem;
            if (selectedItem != null)
            {
                var path = treeViewMachinesAndTasksHandler.GetNodePath() + "\\" + selectedItem.Name;
                if (!selectedItem.ImageSource.Contains("Folder"))
                {
                    var dialogComputerDetails = new ComputerDetails();
                    dialogComputerDetails.computerPath = path;
                    dialogComputerDetails.ShowDialog();
                }
                else
                {
                    treeViewMachinesAndTasksHandler.SetTreeNode(selectedItem.Name);                    
                }
            }
        }

        private void listViewTasks_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selectedItem = (TaskData)listViewTasks.SelectedItem;
            if (selectedItem != null)
            {
                var path = treeViewMachinesAndTasksHandler.GetNodePath() + "\\" + selectedItem.Name + ".my";
                if (!selectedItem.ImageSource.Contains("Folder"))
                {
                    var taskOptionsDialog = new TaskOptions();
                    taskOptionsDialog.path = path;
                    taskOptionsDialog.nodePath = treeViewMachinesAndTasksHandler.GetNodePath();
                    taskOptionsDialog.clients = listViewMachinesAndTasksHandler.clients;
                    taskOptionsDialog.ExecutedTasksHandlers = ExecutedTasksHandlers;
                    foreach (TaskData item in listViewTasks.Items)
                    {
                        if (item.Name != selectedItem.Name)
                            taskOptionsDialog.Names.Add(item.Name);
                    }
                    taskOptionsDialog.ShowDialog();
                    if (taskOptionsDialog.executed)
                    {
                        RunTask(taskOptionsDialog);
                    }
                    listViewMachinesAndTasksHandler.Refresh();
                }
                else
                {
                    treeViewMachinesAndTasksHandler.SetTreeNode(selectedItem.Name);
                }
            }
        }

        private void listViewMachineGroups_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = (ComputerDetailsData)listViewMachineGroups.SelectedItem;            
            if (selectedItem != null)
            {
                menuItemRenameWG.IsEnabled = true;
                menuItemDeleteWG.IsEnabled = true;
                if (selectedItem.ImageSource.Contains("Folder"))
                {
                    menuItemFeaturesWG.IsEnabled = false;
                }
                else
                {
                    menuItemFeaturesWG.IsEnabled = true;
                }
            }
            else
            {                
                menuItemRenameWG.IsEnabled = false;
                menuItemDeleteWG.IsEnabled = false;
                menuItemFeaturesWG.IsEnabled = false;
            }
            ChnageLabels(listViewMachineGroups.Items.Count, listViewMachineGroups.SelectedItems.Count);
        }

        private void listViewTasks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = (TaskData)listViewTasks.SelectedItem;
            if (selectedItem != null)
            {
                menuItemRenameTask.IsEnabled = true;
                menuItemDeleteTask.IsEnabled = true;                
            }
            else
            {
                menuItemRenameTask.IsEnabled = false;
                menuItemDeleteTask.IsEnabled = false;                
            }
            ChnageLabels(listViewTasks.Items.Count, listViewTasks.SelectedItems.Count);
        }    
        
        private void ChnageLabels(int folderContains, int selectedItems)
        {
            labelFolderContains.Content = "Folder Contains: " + folderContains.ToString();
            labelSelected.Content = "Selected: " + selectedItems.ToString();
        }              

        private void DeleteItem()
        {            
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Are you sure?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                if (listViewMachineGroups.Visibility == Visibility.Visible)
                {
                    for (int i = listViewMachineGroups.SelectedItems.Count - 1; i >= 0; i--)
                    {
                        ComputerDetailsData item = (ComputerDetailsData)listViewMachineGroups.SelectedItems[i];
                        if (item.ImageSource.Contains("Folder"))
                        {
                            string path = treeViewMachinesAndTasksHandler.GetNodePath() + "\\" + item.Name;
                            if (path != LockPath && path != DefaultPath)
                            {
                                var computersInfoFiles = Directory.GetFiles(treeViewMachinesAndTasksHandler.GetNodePath(), "*.my", SearchOption.AllDirectories);
                                foreach (string computerFile in computersInfoFiles)
                                {
                                    var computerData = FileHandler.Load<ComputerDetailsData>(computerFile);
                                    for (int j = listViewMachinesAndTasksHandler.clients.Count - 1; j >= 0; j--)
                                    {
                                        ClientHandler client = listViewMachinesAndTasksHandler.clients[j];
                                        if (client.CheckMacsInREC(client.macAddresses, computerData.macAddresses))
                                        {
                                            client.SendMessage(new Packet(DataIdentifier.CLOSE));
                                            client.deleting = true;
                                            break;
                                        }
                                    }
                                }
                                if (Directory.Exists(path))
                                    Directory.Delete(path, true);
                                treeViewMachinesAndTasksHandler.RemoveItem(item.Name);
                            }
                            else
                            {
                                MessageBox.Show("Cannot delete this folder", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                                return;
                            }
                        }
                        else
                        {
                            string path = treeViewMachinesAndTasksHandler.GetNodePath() + "\\" + item.Name + ".my";
                            for (int j = listViewMachinesAndTasksHandler.clients.Count - 1; j >= 0; j--)
                            {
                                ClientHandler client = listViewMachinesAndTasksHandler.clients[j];
                                if (client.CheckMacsInREC(client.macAddresses, item.macAddresses))
                                {
                                    client.SendMessage(new Packet(DataIdentifier.CLOSE));
                                    client.deleting = true;
                                    break;
                                }
                            }
                            if (File.Exists(path))
                                File.Delete(path);
                            if (File.Exists(path.Replace(".my", ".cfg")))
                                File.Delete(path.Replace(".my", ".cfg"));
                        }
                    }
                }
                else
                {
                    for (int i = listViewTasks.SelectedItems.Count - 1; i >= 0; i--)                        
                    {
                        TaskData item = (TaskData)listViewTasks.SelectedItems[i];
                        if (item.ImageSource.Contains("Folder"))
                        {
                            string path = treeViewMachinesAndTasksHandler.GetNodePath() + "\\" + item.Name;
                            if (Directory.Exists(path))
                                Directory.Delete(path, true);
                            treeViewMachinesAndTasksHandler.RemoveItem(item.Name);
                        }
                        else
                        {
                            string path = treeViewMachinesAndTasksHandler.GetNodePath() + "\\" + item.Name + ".my";
                            if (File.Exists(path))
                                File.Delete(path);
                        }                        
                    }
                }
                listViewMachinesAndTasksHandler.Refresh();
            }
        }

        private void NewFolder()
        {
            if (treeViewMachinesAndTasksHandler.GetNodePath().Contains(LockPath))
            {
                MessageBox.Show("Cannot create new folder in lock folder", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var addFolderDialog = new EditItem();
            if (listViewMachineGroups.Visibility == Visibility.Visible)
            {
                foreach (ComputerDetailsData item in listViewMachineGroups.Items)
                {
                    addFolderDialog.Names.Add(item.Name);
                }                
            }
            else
            {
                foreach (TaskData item in listViewTasks.Items)
                {
                    addFolderDialog.Names.Add(item.Name);
                }
            }
            addFolderDialog.ShowDialog();
            if (!addFolderDialog.cancel)
            {
                string path = treeViewMachinesAndTasksHandler.GetNodePath() + "\\" + addFolderDialog.textBoxNewName.Text;                
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                    treeViewMachinesAndTasksHandler.AddItem(addFolderDialog.textBoxNewName.Text);                    
                    listViewMachinesAndTasksHandler.Refresh();
                }
            }
        }

        private void RenameItem(TaskData oldItem)
        {
            if (oldItem != null)
            {
                string oldPath = treeViewMachinesAndTasksHandler.GetNodePath() + "\\" + oldItem.Name;                
                var renameItemDialog = new EditItem();
                renameItemDialog.textBoxNewName.Text = oldItem.Name;
                renameItemDialog.labelOldName.Content = oldItem.Name;
                foreach (TaskData item in listViewTasks.Items)
                {
                    if (item.ImageSource == oldItem.ImageSource)
                        renameItemDialog.Names.Add(item.Name);
                }
                renameItemDialog.ShowDialog();
                if (!renameItemDialog.cancel)
                {
                    string path = treeViewMachinesAndTasksHandler.GetNodePath() + "\\" + renameItemDialog.textBoxNewName.Text;
                    if (oldItem.ImageSource.Contains("Folder"))
                    {
                        if (Directory.Exists(oldPath))
                            Directory.Move(oldPath, path);
                        treeViewMachinesAndTasksHandler.RenameItem(oldItem.Name, renameItemDialog.textBoxNewName.Text);
                    }
                    else
                    {
                        oldPath += ".my";
                        path += ".my";
                        TaskData taksData = FileHandler.Load<TaskData>(oldPath);
                        taksData.Name = renameItemDialog.textBoxNewName.Text;
                        if (File.Exists(oldPath))
                            File.Delete(oldPath);
                        FileHandler.Save<TaskData>(taksData, path);
                    }
                    listViewMachinesAndTasksHandler.Refresh();
                }
            }
        }

        private void RenameItem(ComputerDetailsData oldItem)
        {
            if (oldItem != null)
            {
                string oldPath = treeViewMachinesAndTasksHandler.GetNodePath() + "\\" + oldItem.Name;
                if (oldPath == LockPath || oldPath == DefaultPath)
                {
                    MessageBox.Show("Cannot rename this folder", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                var renameItemDialog = new EditItem();
                renameItemDialog.textBoxNewName.Text = oldItem.Name;
                renameItemDialog.labelOldName.Content = oldItem.Name;
                foreach (ComputerDetailsData item in listViewMachineGroups.Items)
                {
                    if (item.ImageSource == oldItem.ImageSource)
                        renameItemDialog.Names.Add(item.Name);
                }
                renameItemDialog.ShowDialog();
                if (!renameItemDialog.cancel)
                {
                    string path = treeViewMachinesAndTasksHandler.GetNodePath() + "\\" + renameItemDialog.textBoxNewName.Text;
                    if (oldItem.ImageSource.Contains("Folder"))
                    {
                        if (Directory.Exists(path))
                            Directory.Move(oldPath, path);
                        treeViewMachinesAndTasksHandler.RenameItem(oldItem.Name, renameItemDialog.textBoxNewName.Text);
                    }
                    else
                    {
                        oldPath += ".my";
                        path += ".my";
                        ComputerDetailsData machineData = FileHandler.Load<ComputerDetailsData>(oldPath);
                        machineData.Name = renameItemDialog.textBoxNewName.Text;
                        if (File.Exists(oldPath))
                            File.Move(oldPath, path);
                        if (File.Exists(oldPath.Replace(".my", ".cfg")))
                            File.Move(oldPath.Replace(".my", ".cfg"), path.Replace(".my", ".cfg"));
                        FileHandler.Save<ComputerDetailsData>(machineData, path);
                    }
                    listViewMachinesAndTasksHandler.Refresh();
                }
            }
        }

        private void CopyToClipBoard()
        {
            nodePathOld = treeViewMachinesAndTasksHandler.GetNodePath();
            if (nodePathOld == LockPath)
            {
                MessageBox.Show("Cannot Copy from Lock folder", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (listViewMachineGroups.Visibility == Visibility.Visible)
            {
                clipBoardMachines.Clear();
                machines = true;
                foreach (ComputerDetailsData item in listViewMachineGroups.SelectedItems)
                {
                    if (!copy)
                    {
                        if (!item.ImageSource.Contains("_Selected.ico"))
                            item.ImageSource = item.ImageSource.Replace(".ico", "_Selected.ico");
                        if(nodePathOld + "\\" + item.Name != DefaultPath && nodePathOld + "\\" + item.Name != LockPath)
                            clipBoardMachines.Add(item);
                    }
                }
                for (int i = 0; i < listViewMachineGroups.Items.Count; i++)
                {
                    ComputerDetailsData item = (ComputerDetailsData)listViewMachineGroups.Items[i];
                    int indexClipBoard = clipBoardMachines.IndexOf(item);
                    int index = listViewMachineGroups.Items.IndexOf(item);
                    listViewMachineGroups.Items.Remove(item);
                    if (indexClipBoard == -1)
                    {
                        item.ImageSource = item.ImageSource.Replace("_Selected.ico", ".ico");
                        listViewMachineGroups.Items.Insert(index, item);
                    }
                    else
                    {
                        listViewMachineGroups.Items.Insert(index, clipBoardMachines[indexClipBoard]);
                    }
                }
            }
            else
            {
                clipBoardTasks.Clear();
                machines = false;
                for (int i = 0; i < listViewTasks.SelectedItems.Count; i++)
                {
                    TaskData item = (TaskData)listViewTasks.SelectedItems[i];
                    if (!item.ImageSource.Contains("_Selected.ico"))
                        item.ImageSource = item.ImageSource.Replace(".ico", "_Selected.ico");                    
                    clipBoardTasks.Add(item);                
                }
                for (int i = 0; i < listViewTasks.Items.Count; i++)
                {
                    TaskData item = (TaskData)listViewTasks.Items[i];
                    int indexClipBoard = clipBoardTasks.IndexOf(item);
                    int index = listViewTasks.Items.IndexOf(item);
                    listViewTasks.Items.Remove(item);
                    if (indexClipBoard == -1)
                    {
                        item.ImageSource = item.ImageSource.Replace("_Selected.ico", ".ico");
                        listViewTasks.Items.Insert(index, item);
                    }
                    else
                    {
                        listViewTasks.Items.Insert(index, clipBoardTasks[indexClipBoard]);
                    }
                }
            }
        }

        private void PasteClipBoard()
        {
            string nodePath = treeViewMachinesAndTasksHandler.GetNodePath();
            if (nodePath == LockPath)
            {
                MessageBox.Show("Cannot Paste to Lock folder", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (machines && listViewMachineGroups.Visibility == Visibility.Visible)
            {                              
                bool cancel = false;
                if (nodePath != nodePathOld)
                {
                    bool exist = false;

                    foreach (ComputerDetailsData itemClipBoard in clipBoardMachines)
                    {
                        if (cancel) { break; }
                        string oldPath = nodePathOld + "\\" + itemClipBoard.Name;
                        string path = nodePath + "\\" + itemClipBoard.Name;
                        if (!itemClipBoard.ImageSource.Contains("Folder_Selected.ico"))
                        {
                            oldPath += ".my";
                            path += ".my";
                        }
                        for(int i = listViewMachineGroups.Items.Count - 1; i >= 0; i--)                        
                        {
                            ComputerDetailsData item = (ComputerDetailsData)listViewMachineGroups.Items[i];
                            if (item.Name == itemClipBoard.Name)
                            {
                                exist = true;
                                if (itemClipBoard.ImageSource.Contains("Folder_Selected.ico"))
                                {
                                    MessageBox.Show("Cannot move folder with same name", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                                    break;
                                }                                
                                    switch (MessageBox.Show("Replace Item: '" + itemClipBoard.Name + "'", "Warning", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning))
                                { 
                                    case MessageBoxResult.Yes:
                                        {
                                            if (!copy)
                                            {
                                                if (File.Exists(path))
                                                {
                                                    File.Delete(path);
                                                }
                                                File.Move(oldPath, path);
                                            }
                                            break;
                                        }
                                    case MessageBoxResult.No:
                                        {
                                            break;
                                        }
                                    case MessageBoxResult.Cancel:
                                        {
                                            cancel = true;
                                            break;
                                        }
                                }
                                break;
                            }
                        }
                        if (!exist)
                        {
                            if (!copy)
                            {
                                if (itemClipBoard.ImageSource.Contains("Folder_Selected.ico"))
                                {
                                    Directory.Move(oldPath, path);
                                }
                                else
                                {
                                    File.Move(oldPath, path);
                                }
                            }
                        }
                    }
                }
                clipBoardMachines.Clear();
            }
            else
            {
                bool cancel = false;                
                foreach (TaskData itemClipBoard in clipBoardTasks)
                { 
                    if (cancel) { break; }
                    bool exist = false;
                    string oldPath = nodePathOld + "\\" + itemClipBoard.Name;
                    string path = nodePath + "\\" + itemClipBoard.Name;
                    if (!itemClipBoard.ImageSource.Contains("Folder_Selected.ico"))
                    {
                        oldPath += ".my";
                        path += ".my";
                    }

                    for (int i = listViewTasks.Items.Count - 1; i >= 0; i--)
                    {
                        TaskData item = (TaskData)listViewTasks.Items[i];
                        if (item.Name == itemClipBoard.Name)
                        {
                            exist = true;
                            if (nodePath == nodePathOld)
                            {
                                if (copy)
                                {
                                    path = path.Replace(".my", "-(1).my");
                                    if (!File.Exists(path))
                                    {
                                        TaskData taskData = FileHandler.Load<TaskData>(oldPath);
                                        taskData.Name = itemClipBoard.Name + "-(1)";
                                        FileHandler.Save<TaskData>(taskData, path);
                                    }
                                }
                            }
                            else
                            {
                                switch (MessageBox.Show("Replace Item: '" + itemClipBoard.Name + "'", "Warning", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning))
                                {
                                    case MessageBoxResult.Yes:
                                        {
                                            if (copy)
                                            {
                                                if (File.Exists(path))
                                                {
                                                    TaskData taskData = FileHandler.Load<TaskData>(oldPath);
                                                    FileHandler.Save<TaskData>(taskData, path);
                                                }
                                            }
                                            else
                                            {
                                                if (File.Exists(path))
                                                {
                                                    File.Delete(path);
                                                }
                                                File.Move(oldPath, path);
                                            }
                                            break;
                                        }
                                    case MessageBoxResult.No:
                                        {
                                            break;
                                        }
                                    case MessageBoxResult.Cancel:
                                        {
                                            cancel = true;
                                            break;
                                        }
                                }
                            }
                            break;                  
                        }
                    }
                    if(!exist)
                    {
                        if (itemClipBoard.ImageSource.Contains("Folder_Selected.ico"))
                        {
                            if (!copy)                                
                                Directory.Move(oldPath, path);
                        }
                        else
                        {
                            if (copy)
                                File.Copy(oldPath, path);
                            else
                                File.Move(oldPath, path);
                        }
                    }                                        
                }
                clipBoardTasks.Clear();
            }
            listViewMachinesAndTasksHandler.Refresh();
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.F5:
                    {
                        treeViewMachinesAndTasksHandler.Refresh();
                        break;
                    }
                case Key.F2:
                    {
                        if (listViewMachineGroups.Visibility == Visibility.Visible)
                            RenameItem((ComputerDetailsData)listViewMachineGroups.SelectedItem);
                        else
                            RenameItem((TaskData)listViewTasks.SelectedItem);
                        break;
                    }
                case Key.Delete:
                    {
                        DeleteItem();
                        break;
                    }
            }
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                switch (e.Key)
                {
                    case Key.A:
                        {
                            listViewMachinesAndTasksHandler.SelectAll();
                            break;
                        }
                    case Key.X:
                        {
                            copy = false;
                            CopyToClipBoard();
                            break;
                        }
                    case Key.C:
                        {
                            copy = true;
                            CopyToClipBoard();
                            break;
                        }
                    case Key.V:
                        {
                            PasteClipBoard();
                            break;
                        }
                    case Key.W:
                        {
                            runWakeOnLanOnSelectedItems();
                            break;
                        }
                    case Key.R:
                        {
                            RemoteDesktop();
                            break;
                        }
                    case Key.T:
                        {
                            ConfigureTemplate();
                            break;
                        }
                }
                if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                {
                    switch(e.Key)
                    {
                        case Key.N:
                            {
                                NewFolder();                                                                
                                break;
                            }
                    }
                }
            }
            if (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt))
            { /* Your code */ }
            if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
            {
            }
        }

        private void listViewTasksDetails_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selectedItem = (ExecutedTaskData)listViewTasksDetails.SelectedItem;
            if (selectedItem != null)
            {
                var progressComputerDetailsDialog = new ProgressComputersDetails();
                progressComputerDetailsDialog.executedTaskData = selectedItem;
                progressComputerDetailsDialog.ShowDialog();
            }
        }

        private void buttonClear_Click(object sender, RoutedEventArgs e)
        {
            consoleGDS.Items.Clear();
        }
    }

    public static class CharExtensions
    {
        public static string Repeat(this char c, int count)
        {
            return new String(c, count);
        }
    }
}
