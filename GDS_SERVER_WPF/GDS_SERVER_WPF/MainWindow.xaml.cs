using GDS_SERVER_WPF.DataCLasses;
using GDS_SERVER_WPF.Handlers;
using NetworkCommsDotNet;
using NetworkCommsDotNet.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

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
        List<ExecutedTaskHandler> ExecutedTasksHandlers;
        Listener listener;
        TreeViewHandler treeViewMachinesAndTasksHandler;
        TreeViewHandler treeViewPostInstallsHandler;
        TreeViewHandler treeViewHistoryHandler;
        ListViewPostinstallsHandler listViewPostInstallsHandler;
        ListViewMachinesAndTasksHandler listViewMachinesAndTasksHandler;
        ListViewTaskDetailsHandler listViewTaskDetailsHandler;
        ListViewHistoryHandler listViewHistoryHandler;
        string LockPath = @".\Machine Groups\Lock";
        string DefaultPath = @".\Machine Groups\Default";        
        List<string> ipAddresses;
        SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConString"].ConnectionString);

        public MainWindow()
        {
            InitializeComponent();
        }

        private void FillDataGrid()
        {
            /*try
            {
                string ConString = "";
                string CmdString = "";
                try
                {
                    ConString = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;
                    CmdString = string.Empty;
                }                
                catch { MessageBox.Show("CHYBA1"); }
                try
                {
                    CmdString = "UPDATE [Dynamics365_synchro].[dbo].[MachinesGroups] SET MAC='68:05:CA:3C:35:81', Name='LEKTORSK1', ClassRoom=35 WHERE Id=1";
                    CmdString = "Insert into [Dynamics365_synchro].[dbo].[MachinesGroups] (MAC, Name, ClassRoom) Values ('68:05:CA:3C:35:81','LEKTORSK1','35')";
                    CmdString = "DELETE FROM [Dynamics365_synchro].[dbo].[MachinesGroups] WHERE Name='TestPC'";
                    SqlConnection connection = new SqlConnection(ConString);
                    SqlCommand command = new SqlCommand(CmdString, connection);
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
                catch (Exception ex)
                { MessageBox.Show("CHYBA3: " + ex.ToString()); }
                using (SqlConnection con = new SqlConnection(ConString))
                {
                    try
                    {
                        CmdString = "Select * from [Dynamics365_synchro].[dbo].[MachinesGroups]";                        
                        SqlCommand cmd = new SqlCommand(CmdString, con);
                        SqlDataAdapter sda = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable("MachinesGroups");
                        if (dt != null)
                        {
                            sda.Fill(dt);
                            grdMachinesGroups.ItemsSource = dt.DefaultView;
                        }
                    }
                    catch ( Exception ex)
                    { MessageBox.Show("CHYBA2: " + ex.ToString()); }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }     */            
        }

        private void FillDataGridClassRoom()
        {
            try
            {
                string ConString = "";
                string CmdString = "";
                try
                {
                    ConString = ConfigurationManager.ConnectionStrings["ConStringClassRooms"].ConnectionString;
                    CmdString = string.Empty;
                }
                catch { MessageBox.Show("CHYBA1"); }
                using (SqlConnection con = new SqlConnection(ConString))
                {
                    try
                    {
                        CmdString = "select * from [gopas0410].[dbo].[classroom_classroom]";
                        SqlCommand cmd = new SqlCommand(CmdString, con);
                        SqlDataAdapter sda = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable("classroom_classroom");
                        if (dt != null)
                        {
                            sda.Fill(dt);
                            gridClassRoomsID.ItemsSource = dt.DefaultView;
                        }
                    }
                    catch (Exception ex)
                    { MessageBox.Show("CHYBA2: " + ex.ToString()); }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }        
        
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {            
            FillDataGridClassRoom();
            CheckDirectories();            
            treeViewMachinesAndTasksHandler = new TreeViewHandler(treeViewMachinesAndTasks);            
            listViewMachinesAndTasksHandler = new ListViewMachinesAndTasksHandler(listViewMachineGroups,listViewMachineGroupsLock, listViewTasks, treeViewMachinesAndTasksHandler);
            listViewTaskDetailsHandler = new ListViewTaskDetailsHandler(listViewTasksDetails);
            listViewMachinesAndTasksHandler.LoadTreeViewMachinesAndTasks();
            listViewTaskDetailsHandler.LoadTasksDetails();
            ExecutedTasksHandlers = new List<ExecutedTaskHandler>();
            listener = new Listener
            {
                executedTasksHandlers = ExecutedTasksHandlers,
                labelOnlineClients = labelOnline,
                labelOfflineClients = labelOffilne,
                labelAllClients = labelClients,
                listViewMachinesAndTasksHandler = listViewMachinesAndTasksHandler,
                clientsAll = Directory.GetFiles(@".\Machine Groups\", "*.my", SearchOption.AllDirectories).Length,
                console = listBoxConsole,
                grdMachinesGroups = this.grdMachinesGroups,
                conn = this.conn
            };
            listener.BindMyData();
            treeViewPostInstallsHandler = new TreeViewHandler(treeViewPostInstalls);
            listViewPostInstallsHandler = new ListViewPostinstallsHandler(listViewPostInstalls, treeViewPostInstallsHandler, listBoxPostInstallsSelected, listBoxPostInstallsSelector, txtBlockPostInstalls)
            {
                ClientsDictionary = listener.ClientsDictionary
            };
            listViewPostInstallsHandler.LoadTreeViewMachines();
            treeViewHistoryHandler = new TreeViewHandler(treeViewHistory);
            listViewHistoryHandler = new ListViewHistoryHandler(listViewTasksDetailsHistory, @"TaskDetails History", treeViewHistoryHandler);            
            LoadIpAddresses();
            listener.StartListener();            
            //Server = new Thread(listener.StartListener);
            //Server.Start();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                //NetworkComms.Shutdown();
            }
            catch { }
            Environment.Exit(Environment.ExitCode);
        }
        private void MenuItemMachineGroupsRenameLock_Click(object sender, RoutedEventArgs e)
        {
            RenameItem((ComputerDetailsData)listViewMachineGroupsLock.SelectedItem);
        }
        private void MenuItemMachineGroupsEditDetailLock_Click(object sender, RoutedEventArgs e)
        {
            EditDetail((ComputerDetailsData)listViewMachineGroupsLock.SelectedItem);
        }
        private void MenuItemMachineGroupsUnLockLock_Click(object sender, RoutedEventArgs e)
        {
            UnLock((ComputerDetailsData)listViewMachineGroupsLock.SelectedItem);
        }
        private void MenuItemMachineGroupsWOLLock_Click(object sender, RoutedEventArgs e)
        {
            RunWakeOnLanOnSelectedItems(listViewMachineGroupsLock);
        }
        private void MenuItemMachineGroupsRDPLock_Click(object sender, RoutedEventArgs e)
        {
            RemoteDesktop(listViewMachineGroupsLock);
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
            RunWakeOnLanOnSelectedItems(listViewMachineGroups);
        }
        private void MenuItemMachineGroupsRDP_Click(object sender, RoutedEventArgs e)
        {
            RemoteDesktop(listViewMachineGroups);
        }
        private void MenuItemCreateFolder_Click(object sender, RoutedEventArgs e)
        {
            NewFolder();
        }     
        private void EditDetail(ComputerDetailsData computer)
        {

        }
        private void UnLock(ComputerDetailsData computer)
        {

        }
        private void NewTask()
        {
            var taskOptionsDialog = new TaskOptions
            {
                path = "",
                nodePath = treeViewMachinesAndTasksHandler.GetNodePath(),
                ClientsDictionary = listViewMachinesAndTasksHandler.ClientsDictionary,
                ExecutedTasksHandlers = ExecutedTasksHandlers
            };
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
        private void MenuItemTaskNew_Click(object sender, RoutedEventArgs e)
        {
            NewTask();
        }
        private void MenuItemTaskRename_Click(object sender, RoutedEventArgs e)
        {
            RenameItem((TaskData)listViewTasks.SelectedItem);
        }
        private void ItemDetailsDelete(ExecutedTaskData taskDetails)
        {
            DateTime date = DateTime.Parse(taskDetails.Started.Split(' ')[0]);
            string pathYear = ".\\TaskDetails History\\" + date.ToString("yyyy");
            if (!Directory.Exists(pathYear))
                Directory.CreateDirectory(pathYear);
            string pathYearMonth = pathYear + "\\" + date.ToString("MM");
            if (!Directory.Exists(pathYearMonth))
                Directory.CreateDirectory(pathYearMonth);
            string pathDestination = pathYearMonth + "\\" + taskDetails.GetFileName().Replace(".\\TaskDetails","");
            if (taskDetails.Status != "Images/Progress.ico")
            {
                string pathSource = taskDetails.GetFileName();
                taskDetails.FilePath = pathDestination;
                if (File.Exists(pathSource))
                {
                    FileHandler.Save(taskDetails, pathDestination);
                    try { File.Delete(pathSource); } catch { }                    
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
                    string pathSource = taskDetails.GetFileName();
                    taskDetails.Status = "Images/Failed.ico";
                    taskDetails.FilePath = pathDestination;
                    if (File.Exists(pathSource))
                    {
                        FileHandler.Save(taskDetails, pathDestination);
                        try { File.Delete(pathSource); } catch { }
                    }
                }
            }
            listViewTaskDetailsHandler.Refresh();
        }
        private void MenuItemDetailsDelete_Click(object sender, RoutedEventArgs e)
        {
            if (listViewTasksDetails.SelectedItem != null)
            {
                ItemDetailsDelete((ExecutedTaskData)listViewTasksDetails.SelectedItem);
                listViewTaskDetailsHandler.Refresh();
                treeViewHistoryHandler.Refresh();
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
                    ItemDetailsDelete((ExecutedTaskData)listViewTasksDetails.Items[i]);                    
                }
                listViewTaskDetailsHandler.Refresh();
                treeViewHistoryHandler.Refresh();
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
                    taskDetails.Status = "Images/Stopped.ico";
                    listViewTasksDetails.SelectedItem = taskDetails;
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

        private void RunWakeOnLanOnSelectedItems(ListView listView)
        {
            foreach (ComputerDetailsData item in listView.SelectedItems)
            {
                if (!item.ImageSource.Contains("Folder"))
                    WakeOnLanHandler.runWakeOnLan(item.macAddresses, ipAddresses);
            }
        }

        private void RemoteDesktop(ListView listView)
        {
            var computer = (ComputerDetailsData)listView.SelectedItem;
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
                        var computerConfig = FileHandler.Load<ComputerConfigData>(pathConfigFile);
                        computerConfig.Workgroup = ConfigureTemplateDialog.textBoxNewName.Text;
                        if(computerConfig.Name == "" || computerConfig.Name.Contains("MININT"))
                        {
                            computerConfig.Name = computer.Name;
                        }                        
                        FileHandler.Save(computerConfig, pathConfigFile);
                    }
                    else
                    {
                        FileHandler.Save(new ComputerConfigData(computer.Name, ConfigureTemplateDialog.textBoxNewName.Text), pathConfigFile);
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
            ExecutedTaskData executedTask = new ExecutedTaskData
            {
                Name = dialog.taskData.Name,
                Status = "Images/Progress.ico",
                Started = dialog.taskData.LastExecuted,
                Finished = "NONE",
                Clients = dialog.taskData.TargetComputers.Count.ToString(),
                Done = "0",
                Failed = "0",
                MachineGroup = dialog.taskData.MachineGroup,
                taskData = dialog.taskData
            };
            ExecutedTaskHandler taskHandler = new ExecutedTaskHandler(executedTask, ipAddresses, listViewComputersProgressAll, listViewComputersProgressSelected, listViewTaskDetailsProgress);
            ExecutedTasksHandlers.Add(taskHandler);
            taskHandler.handlers = ExecutedTasksHandlers;
            taskHandler.listViewHandler = listViewTaskDetailsHandler;
            taskHandler.ClientsDictionary = listViewMachinesAndTasksHandler.ClientsDictionary;
            Thread taskHandlerThread = new Thread(taskHandler.Start);            
            taskHandlerThread.Start();
        }                     

        private void CheckDirectories()
        {
            var paths = new List<string>() { @".\Machine Groups", @".\Tasks", DefaultPath, LockPath, @".\TaskDetails", @".\Base", @".\DriveE", @".\TaskDetails History" };
            foreach(string path in paths)
            {
                if(!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }
        }

        private void TreeViewMachinesAndTasks_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                if (treeViewMachinesAndTasks.SelectedItem != null)
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
            catch (Exception ex)
            {
                MessageBox.Show("TreeeView Error: " + ex.ToString());
            }
        }

        private void ListViewTaskDetailsProgress_SelectionChanged(object sender, SelectionChangedEventArgs e)
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

        private void ListViewComputers_MouseDoubleClick(ListView listView, TreeViewHandler treeViewHandler)
        {
            var selectedItem = (ComputerDetailsData)listView.SelectedItem;
            if (selectedItem != null)
            {
                var path = treeViewHandler.GetNodePath() + "\\" + selectedItem.Name;
                if (!selectedItem.ImageSource.Contains("Folder"))
                {
                    var dialogComputerDetails = new ComputerDetails
                    {
                        computerPath = path
                    };
                    dialogComputerDetails.ShowDialog();
                }
                else
                {
                    treeViewHandler.SetTreeNode(selectedItem.Name);
                }
            }
        }

        private void ListViewMachineGroups_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListViewComputers_MouseDoubleClick(listViewMachineGroups, treeViewMachinesAndTasksHandler);
        }

        private void ListViewTasks_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selectedItem = (TaskData)listViewTasks.SelectedItem;
            if (selectedItem != null)
            {
                var path = treeViewMachinesAndTasksHandler.GetNodePath() + "\\" + selectedItem.Name + ".my";
                if (!selectedItem.ImageSource.Contains("Folder"))
                {
                    var taskOptionsDialog = new TaskOptions
                    {
                        path = path,
                        nodePath = treeViewMachinesAndTasksHandler.GetNodePath(),
                        ClientsDictionary = listViewMachinesAndTasksHandler.ClientsDictionary,
                        ExecutedTasksHandlers = ExecutedTasksHandlers
                    };
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

        private void ListViewMachineGroups_SelectionChanged(object sender, SelectionChangedEventArgs e)
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

        private void ListViewTasks_SelectionChanged(object sender, SelectionChangedEventArgs e)
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
                                    for (int index = listener.ClientsDictionary.Count-1; index >= 0 ; index--)
                                    {
                                        var computer = listener.ClientsDictionary.ElementAt(index);                             
                                        if (computer.Value.ComputerData.macAddresses != null && Listener.CheckMacsInREC(computer.Value.ComputerData.macAddresses, computerData.macAddresses))
                                        {
                                            listener.SendMessage(new Packet(FLAG.CLOSE), computer.Value.connection);
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
                            foreach (KeyValuePair<ShortGuid, ComputerWithConnection> computer in listener.ClientsDictionary)
                            {
                                if (computer.Value.ComputerData.macAddresses != null && Listener.CheckMacsInREC(computer.Value.ComputerData.macAddresses, item.macAddresses))
                                {
                                    listener.SendMessage(new Packet(FLAG.CLOSE), computer.Value.connection);
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
                        if (Directory.Exists(oldPath))
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
                            try
                            {
                                if (!copy)
                                    Directory.Move(oldPath, path);
                            }
                            catch { MessageBox.Show("Cannot Move Directory"); return; }
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
           
            if (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt))
            { /* Your code */ }
            if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
            {
            }
        }

        private void ShowProgressComputerDetailsDialog(ExecutedTaskData executedTaskData)
        {
            if (executedTaskData != null)
            {
                var progressComputerDetailsDialog = new ProgressComputersDetails
                {
                    executedTaskData = executedTaskData
                };
                progressComputerDetailsDialog.ShowDialog();
            }
        }

        private void ListViewTasksDetails_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ShowProgressComputerDetailsDialog((ExecutedTaskData)listViewTasksDetails.SelectedItem);            
        }

        private void ListViewPostInstalls_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListViewComputers_MouseDoubleClick(listViewPostInstalls, treeViewPostInstallsHandler);
        }

        private void TreeViewPostInstalls_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                if (treeViewPostInstalls.SelectedItem != null)
                {
                    (treeViewPostInstalls.SelectedItem as TreeViewItem).IsExpanded = true;
                }
                var path = treeViewPostInstallsHandler.GetNodePath();
                listViewPostInstallsHandler.LoadMachines(path);
                listViewPostInstalls.SelectAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show("TreeeView Error Post Installs: " + ex.ToString());
            }
        }

        private void BtnPostInstallsSelect_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            listViewPostInstallsHandler.SeLect();
            listViewMachinesAndTasksHandler.Refresh();
        }

        private void ListViewTasksDetails_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Delete:
                    {
                        if (listViewTasksDetails.SelectedItem != null)
                        {
                            ItemDetailsDelete((ExecutedTaskData)listViewTasksDetails.SelectedItem);
                        }
                        break;
                    }
            }
        }

        private void ListViewTasks_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.F5:
                    {
                        listViewMachinesAndTasksHandler.Refresh();
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
                    case Key.T:
                        {
                            NewTask();
                            break;
                        }
                }
                if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                {
                    switch (e.Key)
                    {
                        case Key.N:
                            {
                                NewFolder();
                                break;
                            }
                    }
                }
            }
        }

        private void TreeViewMachinesAndTasks_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.F5:
                    {
                        treeViewMachinesAndTasksHandler.Refresh();
                        break;
                    }
            }            
        }

        private void TreeViewHistory_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                if (treeViewHistory.SelectedItem != null)
                {
                    (treeViewHistory.SelectedItem as TreeViewItem).IsExpanded = true;
                }
                if(listViewHistoryHandler != null)
                    listViewHistoryHandler.RefreshHistoryDetails();
            }
            catch (Exception ex)
            {
                MessageBox.Show("TreeeView Error History: " + ex.ToString());
            }
        }

        private void TreeViewHistory_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.F5:
                    {
                        treeViewHistoryHandler.Refresh();
                        break;
                    }
            }
        }

        private void ListViewTasksDetailsHistory_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ShowProgressComputerDetailsDialog((ExecutedTaskData)listViewTasksDetailsHistory.SelectedItem);
        }

        private void BtnPostInstallsUnSelect_MouseDown(object sender, MouseButtonEventArgs e)
        {
            listViewPostInstallsHandler.UnSeLect();
            listViewMachinesAndTasksHandler.Refresh();
        }

        private void BtnPostInstallsSelect_MouseDown(object sender, MouseButtonEventArgs e)
        {
            listViewPostInstallsHandler.SeLect();
            listViewMachinesAndTasksHandler.Refresh();
        }

        private void ListBoxPostInstallsSelector_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            listViewPostInstallsHandler.SeLect();
            listViewMachinesAndTasksHandler.Refresh();
        }

        private void ListBoxPostInstallsSelected_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            listViewPostInstallsHandler.UnSeLect();
            listViewMachinesAndTasksHandler.Refresh();
        }

        private void ListViewPostInstalls_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            listViewPostInstallsHandler.RefreshSelected();
        }

        private void ListBoxPostInstallsSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            listViewPostInstallsHandler.SelectNote();
        }

        private void BtnPostInstallsRefresh_MouseDown(object sender, MouseButtonEventArgs e)
        {
            listViewPostInstallsHandler.RefreshPostInstalls();
        }        

        private void BtnPostInstallsCopyPath_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnPostInstallsChangeNote_Click(object sender, RoutedEventArgs e)
        {
            var changeNoteWindow = new ChangeNote();
            changeNoteWindow.txtBoxPostInstalls.Text = listViewPostInstallsHandler.txtBlockPostInstalls.Text;
            changeNoteWindow.path = listViewPostInstallsHandler.postInstallerNotePath;
            changeNoteWindow.ShowDialog();
            listViewPostInstallsHandler.SelectNote();
        }

        private void ListViewMachineGroups_MouseDown(object sender, MouseButtonEventArgs e)
        {
            HitTestResult r = VisualTreeHelper.HitTest(this, e.GetPosition(this));
            if (r.VisualHit.GetType() != typeof(ListBoxItem))
            {                
                listViewMachineGroups.UnselectAll();                
                listViewMachineGroups.Focus();
            }
        }
        private void ListViewItem_MouseEnter(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                ListViewItem lbi = sender as ListViewItem;
                lbi.IsSelected = true;
                lbi.Focus();
                listViewMachineGroups.SelectedItems.Add(lbi);
            }
        }

        private void ListViewMachineGroups_KeyUp_1(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.F5:
                    {
                        listViewMachinesAndTasksHandler.Refresh();
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
                    case Key.V:
                        {
                            PasteClipBoard();
                            break;
                        }
                    case Key.W:
                        {
                            RunWakeOnLanOnSelectedItems(listViewMachineGroups);
                            break;
                        }
                    case Key.R:
                        {
                            RemoteDesktop(listViewMachineGroups);
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
                    switch (e.Key)
                    {
                        case Key.N:
                            {
                                NewFolder();
                                break;
                            }
                    }
                }
            }
        }

        private void ListViewTasks_MouseDown(object sender, MouseButtonEventArgs e)
        {
            HitTestResult r = VisualTreeHelper.HitTest(this, e.GetPosition(this));
            if (r.VisualHit.GetType() != typeof(ListBoxItem))
            {
                listViewTasks.UnselectAll();
                listViewTasks.Focus();
            }
        }

        private void btnInsert_Click(object sender, RoutedEventArgs e)
        {
            var computersInfoFiles = Directory.GetFiles(@".\Machine Groups\UCEBNASK03", "*.my", SearchOption.AllDirectories);
            foreach (string computerFile in computersInfoFiles)
            {
                ComputerDetailsData computer = FileHandler.Load<ComputerDetailsData>(computerFile);
                listener.UpdateDataGridByMacs(computer.macAddresses, computer.Name, "21");
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                conn.Open();
                SqlCommand comm = new SqlCommand("UPDATE [Dynamics365_synchro].[dbo].[MachinesGroups]  SET MAC='" + txtBoxMAC.Text + "',Name='" + txtBoxName.Text + "',ClassRoom=" +txtBoxClassRoom.Text + " WHERE Mac=" + txtBoxMAC.Text + "", conn);
                comm.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
            finally
            {
                conn.Close();
                listener.BindMyData();
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                conn.Open();
                SqlCommand comm = new SqlCommand("DELETE FROM [Dynamics365_synchro].[dbo].[MachinesGroups] WHERE ID=" + txtBoxID.Text + "", conn);
                comm.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
            finally
            {
                conn.Close();
                listener.BindMyData();
            }
        }

        private void grdMachinesGroups_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(grdMachinesGroups.SelectedItem != null)
            {
                try
                {
                    DataRowView dataRow = (DataRowView)grdMachinesGroups.SelectedItem;
                    txtBoxID.Text = dataRow.Row.ItemArray[0].ToString();
                    txtBoxMAC.Text = dataRow.Row.ItemArray[1].ToString();
                    txtBoxName.Text = dataRow.Row.ItemArray[2].ToString();
                    txtBoxClassRoom.Text = dataRow.Row.ItemArray[3].ToString();
                }
                catch { }
            }
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
