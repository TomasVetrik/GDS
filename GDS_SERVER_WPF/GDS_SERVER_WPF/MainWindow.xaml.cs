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
        string ReleaseInfoFile = @".\Release.txt";
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

        private void LoadReleaseInfo()
        {
            if(File.Exists(ReleaseInfoFile))
            {
                TextBlockInfo.Text = File.ReadAllText(ReleaseInfoFile);
            }
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
                catch { listBoxConsole.Items.Add("CHYBA1 SQL"); }
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
                    { listBoxConsole.Items.Add("CHYBA2 SQL: " + ex.ToString()); }
                }
            }
            catch (Exception ex)
            {
                listBoxConsole.Items.Add("CHYBA3 SQL: " + ex.ToString());
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
            LoadReleaseInfo();
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
        private void MenuItemMachineGroupsSetClassRoomID_Click(object sender, RoutedEventArgs e)
        {
            SetClassRoomID(treeViewMachinesAndTasksHandler.GetNodePath());
        }
        private void MenuItemMachineGroupsCreateRDPLock_Click(object sender, RoutedEventArgs e)
        {
            CreateRDP(listViewMachineGroupsLock);
        }        
        private void MenuItemMachineGroupsRenameLock_Click(object sender, RoutedEventArgs e)
        {
            RenameItem((ComputerDetailsData)listViewMachineGroupsLock.SelectedItem);
        }        
        private void MenuItemMachineGroupsCreateRDPFiles_Click(object sender, RoutedEventArgs e)
        {
            CreateRDP(listViewMachineGroups);
        }
        private void MenuItemMachineGroupsLock_Click(object sender, RoutedEventArgs e)
        {
            if((ComputerDetailsData)listViewMachineGroups.SelectedItem != null)
            {
                List<ComputerDetailsData> computers = new List<ComputerDetailsData>();
                foreach (ComputerDetailsData computer in listViewMachineGroups.SelectedItems)
                {
                    computers.Add(computer);
                }
                var editItemDialog = new EditItem();
                editItemDialog.skipControll = true;
                editItemDialog.ShowDialog();
                if (!editItemDialog.cancel)
                {
                    foreach (ComputerDetailsData computer in computers)
                    {
                        Lock(computer, editItemDialog.textBoxNewText.Text);
                    }
                }
                listViewMachinesAndTasksHandler.Refresh();
            }
        }
        private void MenuItemMachineGroupsUpdateClassRoomID_Click(object sender, RoutedEventArgs e)
        {
            UpdateClassRoomID(listViewMachineGroups);
        }        
        private void MenuItemMachineGroupsEditDetailLock_Click(object sender, RoutedEventArgs e)
        {
            EditLockDetail(listViewMachineGroupsLock);            
        }
        private void MenuItemMachineGroupsUnLockLock_Click(object sender, RoutedEventArgs e)
        {
            UnLock(listViewMachineGroupsLock);
        }
        private void MenuItemMachineGroupsWOLLock_Click(object sender, RoutedEventArgs e)
        {
            RunWakeOnLanOnSelectedItems(listViewMachineGroupsLock);
        }
        private void MenuItemMachineGroupsDartViewer_Click(object sender, RoutedEventArgs e)
        {
            DartViewer(listViewMachineGroups);
        }
        private void MenuItemMachineGroupsDartViewerLock_Click(object sender, RoutedEventArgs e)
        {
            DartViewer(listViewMachineGroupsLock);
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
        private void UnLock(ListView listView)
        {
            try
            {
                if ((ComputerDetailsData)listView.SelectedItem != null)
                {
                    List<ComputerDetailsData> computers = new List<ComputerDetailsData>();
                    foreach (ComputerDetailsData computer in listView.SelectedItems)
                    {
                        computers.Add(computer);
                    }
                    MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Are you sure?", "UnLock Confirmation", System.Windows.MessageBoxButton.YesNo);
                    if (messageBoxResult == MessageBoxResult.Yes)
                    {
                        foreach (ComputerDetailsData computer in computers)
                        {
                            UnLock(computer);
                        }
                    }
                    listViewMachinesAndTasksHandler.Refresh();
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void UpdateClassRoomID(ListView listView)
        {
            foreach (ComputerDetailsData computer in listView.SelectedItems)
            {
                listener.InsertOrUpdate(computer.macAddresses, computer.Name, listener.GetTheClassRoomID(treeViewMachinesAndTasksHandler.GetNodePath() + "\\" + computer.Name + ".my"));
            }
            listener.BindMyData();
        }
        private void EditLockDetail(ListView listView)
        {
            if ((ComputerDetailsData)listView.SelectedItem != null)
            {
                List<ComputerDetailsData> computers = new List<ComputerDetailsData>();
                foreach (ComputerDetailsData computer in listView.SelectedItems)
                {
                    computers.Add(computer);
                }
                var editItemDialog = new EditItem();
                if (computers[0].Detail == null)
                {
                    computers[0].Detail = "";
                }
                editItemDialog.labelOldText.Content = computers[0].Detail;
                editItemDialog.ShowDialog();
                if (!editItemDialog.cancel)
                {
                    foreach (ComputerDetailsData computer in computers)
                    {
                        EditLockDetail(computer, editItemDialog.textBoxNewText.Text);
                    }
                }
                listViewMachinesAndTasksHandler.Refresh();
            }
        }
        private void Lock(ComputerDetailsData computer, string Detail)
        {
            if (computer != null)
            {
                computer.pathNode = treeViewMachinesAndTasksHandler.GetNodePath();
                computer.Detail = Detail;
                string oldPath = computer.pathNode + "\\" + computer.Name + ".my";
                string path = LockPath + "\\" + computer.Name + ".my";
                string oldPathConfig = computer.pathNode + "\\" + computer.Name + ".cfg";
                string pathConfig = LockPath + "\\" + computer.Name + ".cfg";
                bool exist = false;
                bool cancel = false;
                for (int i = listViewMachineGroupsLock.Items.Count - 1; i >= 0; i--)
                {
                    ComputerDetailsData item = (ComputerDetailsData)listViewMachineGroupsLock.Items[i];
                    if (item.Name == computer.Name)
                    {
                        exist = true;
                        switch (MessageBox.Show("Replace Item: '" + computer.Name + "'", "Warning", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning))
                        {
                            case MessageBoxResult.Yes:
                                {
                                    if (File.Exists(path))
                                    {
                                        File.Delete(path);
                                    }
                                    if (File.Exists(oldPath))
                                    {
                                        File.Delete(oldPath);
                                    }
                                    FileHandler.Save(computer, path);
                                    if (File.Exists(pathConfig))
                                    {
                                        File.Delete(pathConfig);
                                    }
                                    File.Move(oldPathConfig, pathConfig);
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
                if (!exist && !cancel)
                {
                    if (File.Exists(oldPath))
                    {
                        File.Delete(oldPath);
                    }
                    FileHandler.Save(computer, path);
                    if (File.Exists(oldPathConfig))
                    {
                        File.Move(oldPathConfig, pathConfig);
                    }                    
                }
            }    
        }
        private void EditLockDetail(ComputerDetailsData computer, string Detail)
        {
            computer.Detail = Detail;
            FileHandler.Save(computer, LockPath + "\\" + computer.Name + ".my");            
        }
        private void UnLock(ComputerDetailsData computer)
        {
            try
            {
                if (computer != null)
                {
                    computer.Detail = "";
                    if (computer.pathNode == null)
                    {
                        computer.pathNode = @".\Machine Groups\Default\";
                    }
                    string path = computer.pathNode + "\\" + computer.Name + ".my";
                    string oldPath = LockPath + "\\" + computer.Name + ".my";
                    string pathConfig = computer.pathNode + "\\" + computer.Name + ".cfg";
                    string oldPathConfig = LockPath + "\\" + computer.Name + ".cfg";
                    bool exist = false;
                    bool cancel = false;
                    if (!Directory.Exists(computer.pathNode))
                    {
                        VytvorPriecinok(computer.pathNode);
                    }
                    var computersInfoFiles = Directory.GetFiles(computer.pathNode, "*.my", SearchOption.AllDirectories);
                    foreach (string filePath in computersInfoFiles)
                    {
                        string Name = Path.GetFileName(filePath).Replace(".my", "");
                        if (Name == computer.Name)
                        {
                            exist = true;
                            switch (MessageBox.Show("Replace Item: '" + computer.Name + "'", "Warning", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning))
                            {
                                case MessageBoxResult.Yes:
                                    {
                                        if (File.Exists(path))
                                        {
                                            File.Delete(path);
                                        }
                                        if (File.Exists(oldPath))
                                        {
                                            File.Delete(oldPath);
                                        }
                                        FileHandler.Save(computer, path);
                                        if (File.Exists(pathConfig))
                                        {
                                            File.Delete(pathConfig);
                                        }
                                        File.Move(oldPathConfig, pathConfig);
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
                    if (!exist && !cancel)
                    {
                        if (File.Exists(oldPath))
                        {
                            File.Delete(oldPath);
                        }
                        FileHandler.Save(computer, path);
                        if (File.Exists(oldPathConfig))
                        {
                            File.Move(oldPathConfig, pathConfig);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
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
        }
        private void MenuItemDetailsDelete_Click(object sender, RoutedEventArgs e)
        {
            if (listViewTasksDetails.SelectedItem != null)
            {
                for (int i = listViewTasksDetails.SelectedItems.Count - 1; i >= 0; i--)
                {
                    ItemDetailsDelete((ExecutedTaskData)listViewTasksDetails.SelectedItems[i]);
                }
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

        private void SetClassRoomID(string path)
        {
            string FilePath = path + "\\ClassRoomID.txt";            
            var setClassRoomIDDialog = new EditItem();
            if (File.Exists(FilePath))
            {
                string ID = File.ReadAllText(FilePath);
                setClassRoomIDDialog.Names.Add(ID);
                setClassRoomIDDialog.labelOldText.Content = ID;
            }
            setClassRoomIDDialog.ShowDialog();
            if (!setClassRoomIDDialog.cancel)
            {
                string ID = setClassRoomIDDialog.textBoxNewText.Text;
                File.WriteAllText(FilePath, ID);
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

        private void DartViewer(ListView listView)
        {
            var computer = (ComputerDetailsData)listView.SelectedItem;
            if (computer != null)
            {
                DartViewer(computer);
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
                if (temp != "")
                {
                    string[] details = temp.Split('#');
                    string ticket = details[0];
                    string port = details[2];
                    string IPAdd = details[1];
                    System.Diagnostics.Process.Start(@"C:\Windows\Sysnative\DartRemoteViewer.exe", "-ticket=" + ticket + " -IPaddress=" + IPAdd + " -port=" + port);
                }
            }
            catch(Exception e)
            {
                listBoxConsole.Items.Add("Dart Viewer CHYBA: " + e.ToString());
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
            if (!(File.Exists(@".\!NEMAZAT_temp")))
            {
                string riadky = "screen mode id:i:2" + "\n" + "use multimon:i:0" + "\n" + "desktopwidth:i:1920"
                    + "\n" + "desktopheight:i:1080" + "\n" + "session bpp:i:32" + "\n" + "winposstr:s:0,3,0,0,800,600" + "\n" + "compression:i:1" + "\n"
                    + "keyboardhook:i:2" + "\n" + "audiocapturemode:i:0" + "\n" + "videoplaybackmode:i:1" + "\n" + "connection type:i:7" + "\n"
                    + "networkautodetect:i:1" + "\n" + "bandwidthautodetect:i:1" + "\n" + "displayconnectionbar:i:1" + "\n" + "enableworkspacereconnect:i:0" + "\n"
                    + "disable wallpaper:i:0" + "\n" + "allow font smoothing:i:0" + "\n" + "allow desktop composition:i:0" + "\n" + "disable full window drag:i:1"
                    + "\n" + "disable menu anims:i:1" + "\n" + "disable themes:i:0" + "\n" + "disable cursor setting:i:0" + "\n" + "bitmapcachepersistenable:i:1"
                    + "\n" + "audiomode:i:0" + "\n" + "redirectprinters:i:1" + "\n" + "redirectcomports:i:0" + "\n" + "redirectsmartcards:i:1" + "\n"
                    + "redirectclipboard:i:1" + "\n" + "redirectposdevices:i:0" + "\n" + "drivestoredirect:s:" + "\n" + "autoreconnection enabled:i:1"
                    + "\n" + "authentication level:i:2" + "\n" + "prompt for credentials:i:0" + "\n" + "negotiate security layer:i:1"
                    + "\n" + "remoteapplicationmode:i:0" + "\n" + "alternate shell:s:" + "\n" + "shell working directory:s:" + "\n" + "gatewayusagemethod:i:2"
                    + "\n" + "gatewaycredentialssource:i:4" + "\n" + "gatewayprofileusagemethod:i:1" + "\n" + "promptcredentialonce:i:1"
                    + "\n" + "use redirection server name:i:0" + "\n" + "rdgiskdcproxy:i:0" + "\n" + "kdcproxyname:s:";
                File.WriteAllText(@".\!NEMAZAT_temp", riadky);
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
                listBoxConsole.Items.Add("TreeeView Error: " + ex.ToString());
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
            if ((listViewMachineGroups.Visibility == Visibility.Visible && listViewMachineGroups.SelectedItems.Count != 0) || (listViewTasks.Visibility == Visibility.Visible && listViewTasks.SelectedItems.Count != 0))
            {
                MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Are you sure?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    listener.semaphore.WaitOne();
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
                                        for (int index = listener.ClientsDictionary.Count - 1; index >= 0; index--)
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
                    listener.semaphore.Release();
                }
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
                string path = treeViewMachinesAndTasksHandler.GetNodePath() + "\\" + addFolderDialog.textBoxNewText.Text;                
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                    treeViewMachinesAndTasksHandler.AddItem(addFolderDialog.textBoxNewText.Text);                    
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
                renameItemDialog.textBoxNewText.Text = oldItem.Name;
                renameItemDialog.labelOldText.Content = oldItem.Name;
                foreach (TaskData item in listViewTasks.Items)
                {
                    if (item.ImageSource == oldItem.ImageSource)
                        renameItemDialog.Names.Add(item.Name);
                }
                renameItemDialog.ShowDialog();
                if (!renameItemDialog.cancel)
                {
                    string path = treeViewMachinesAndTasksHandler.GetNodePath() + "\\" + renameItemDialog.textBoxNewText.Text;
                    if (oldItem.ImageSource.Contains("Folder"))
                    {
                        if (Directory.Exists(oldPath))
                            Directory.Move(oldPath, path);
                        treeViewMachinesAndTasksHandler.RenameItem(oldItem.Name, renameItemDialog.textBoxNewText.Text);
                    }
                    else
                    {
                        oldPath += ".my";
                        path += ".my";
                        TaskData taksData = FileHandler.Load<TaskData>(oldPath);
                        taksData.Name = renameItemDialog.textBoxNewText.Text;
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
                listener.semaphore.WaitOne();
                string oldPath = treeViewMachinesAndTasksHandler.GetNodePath() + "\\" + oldItem.Name;
                if (oldPath == LockPath || oldPath == DefaultPath)
                {
                    MessageBox.Show("Cannot rename this folder", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                var renameItemDialog = new EditItem();
                renameItemDialog.textBoxNewText.Text = oldItem.Name;
                renameItemDialog.labelOldText.Content = oldItem.Name;
                foreach (ComputerDetailsData item in listViewMachineGroups.Items)
                {
                    if (item.ImageSource == oldItem.ImageSource)
                        renameItemDialog.Names.Add(item.Name);
                }
                renameItemDialog.ShowDialog();
                if (!renameItemDialog.cancel)
                {
                    string path = treeViewMachinesAndTasksHandler.GetNodePath() + "\\" + renameItemDialog.textBoxNewText.Text;
                    if (oldItem.ImageSource.Contains("Folder"))
                    {
                        if (Directory.Exists(oldPath))
                            Directory.Move(oldPath, path);
                        treeViewMachinesAndTasksHandler.RenameItem(oldItem.Name, renameItemDialog.textBoxNewText.Text);
                   } 
                    else
                    {
                        oldPath += ".my";
                        path += ".my";
                        ComputerDetailsData machineData = FileHandler.Load<ComputerDetailsData>(oldPath);
                        machineData.Name = renameItemDialog.textBoxNewText.Text;
                        if (File.Exists(oldPath))
                            File.Move(oldPath, path);
                        if (File.Exists(oldPath.Replace(".my", ".cfg")))
                            File.Move(oldPath.Replace(".my", ".cfg"), path.Replace(".my", ".cfg"));
                        FileHandler.Save<ComputerDetailsData>(machineData, path);
                    }
                    listViewMachinesAndTasksHandler.Refresh();                    
                }
                listener.semaphore.Release();
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
            listener.semaphore.WaitOne();               
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
                        string oldPathConfig = "";
                        string pathConfig = "";
                        if (!itemClipBoard.ImageSource.Contains("Folder_Selected.ico"))
                        {
                            oldPathConfig = oldPath + ".cfg";
                            oldPath += ".my";
                            pathConfig = path + ".cfg";
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
                                                if (File.Exists(pathConfig))
                                                {
                                                    File.Delete(pathConfig);
                                                }
                                                File.Move(oldPathConfig, pathConfig);
                                                listener.InsertOrUpdate(itemClipBoard.macAddresses, itemClipBoard.Name, listener.GetTheClassRoomID(path));
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
                                    if (File.Exists(oldPathConfig))
                                    {
                                        if (File.Exists(pathConfig))
                                        {
                                            File.Delete(pathConfig);
                                        }
                                        File.Move(oldPathConfig, pathConfig);
                                    }
                                    listener.InsertOrUpdate(itemClipBoard.macAddresses, itemClipBoard.Name, listener.GetTheClassRoomID(path));
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
            listener.semaphore.Release();
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
                listBoxConsole.Items.Add("TreeeView Error Post Installs: " + ex.ToString());
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
                            for (int i = listViewTasksDetails.SelectedItems.Count - 1; i >= 0; i--)
                            {
                                ItemDetailsDelete((ExecutedTaskData)listViewTasksDetails.SelectedItems[i]);
                            }
                            listViewTaskDetailsHandler.Refresh();
                            treeViewHistoryHandler.Refresh();
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
                listBoxConsole.Items.Add("TreeeView Error History: " + ex.ToString());
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
            var EditItemDialog = new EditItem();
            string PathCopyDestination = listViewPostInstallsHandler.postInstallerDestinationPath;
            string DestinationText = "";
            if(File.Exists(PathCopyDestination))
            {
                DestinationText = File.ReadAllText(PathCopyDestination);
            }
            EditItemDialog.skipControll = true;
            EditItemDialog.labelOldText.Content = DestinationText;
            EditItemDialog.ShowDialog();
            if (!EditItemDialog.cancel)
            {
                if (EditItemDialog.textBoxNewText.Text != "")
                {
                    File.WriteAllText(PathCopyDestination, EditItemDialog.textBoxNewText.Text);
                }
                else
                {
                    if (File.Exists(PathCopyDestination))
                    {
                        MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Do you want delete Copy Path Destination File?", "Confirmation", System.Windows.MessageBoxButton.YesNo);
                        if (messageBoxResult == MessageBoxResult.Yes)
                        {
                            File.Delete(PathCopyDestination);
                        }
                    }
                }
            }
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
                case Key.F8:
                    {
                        CreateRDP(listViewMachineGroups);
                        break;
                    }
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
                    case Key.D:
                        {
                            DartViewer(listViewMachineGroups);
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
            listener.UpdateDataGridByMacs(new List<string> { txtBoxMAC.Text }, txtBoxName.Text, txtBoxClassRoom.Text);
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                conn.Open();
                SqlCommand comm = new SqlCommand("UPDATE [Dynamics365_synchro].[dbo].[MachinesGroups]  SET MAC='" + txtBoxMAC.Text + "',Name='" + txtBoxName.Text + "',ClassRoom=" +txtBoxClassRoom.Text + " WHERE Mac='" + txtBoxMAC.Text + "'", conn);
                comm.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                listBoxConsole.Items.Add("CHYBA UPDATE SQL: " + ex.Message.ToString());
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
                listBoxConsole.Items.Add("CHYBA DELE SQL: " + ex.Message.ToString());
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

        private void listViewMachineGroupsLock_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListViewComputers_MouseDoubleClick(listViewMachineGroupsLock, treeViewMachinesAndTasksHandler);
        }

        void VytvorPriecinok(string path)
        {
            try
            {
                if (path.Substring(0, 1) == @"\")
                {
                    path = path.Substring(1, path.Length - 1);
                }
                if (!(Directory.Exists(path)))
                {
                    Directory.CreateDirectory(path);
                }
            }
            catch
            {
            }
        }        

        private void BtnMigrate_Click(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists(@".\Migrate Data"))
            {
                MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Do you want migrate old data?", "Migrate Confirmation", System.Windows.MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    string pathOSAbrivation = @".\OsAbrivations.my";
                    OSAbrivationsData osAbrivationsData = new OSAbrivationsData();
                    if (!File.Exists(pathOSAbrivation))
                    {
                        FileHandler.Save<OSAbrivationsData>(osAbrivationsData, pathOSAbrivation);
                    }
                    osAbrivationsData = FileHandler.Load<OSAbrivationsData>(pathOSAbrivation);
                    if (Directory.Exists(@".\Migrate Data\Machine Groups\"))
                    {
                        var computersInfoFiles = Directory.GetFiles(@".\Migrate Data\Machine Groups\", "*.my", SearchOption.AllDirectories);
                        foreach (string computerFile in computersInfoFiles)
                        {
                            ComputerDetailsData computer = new ComputerDetailsData(Path.GetFileName(computerFile).Replace(".my", ""), "", "", "", "");
                            computer.LoadDataFromList(File.ReadAllLines(computerFile).ToList());
                            string path = computerFile.Replace("\\Migrate Data", "");
                            VytvorPriecinok(path.Replace(Path.GetFileName(path), ""));
                            string lockFilePath = computerFile.Replace(".my", ".lock");
                            if (File.Exists(lockFilePath))
                            {
                                string textLockDetails = File.ReadAllText(lockFilePath);
                                string[] temp = textLockDetails.Split('#');
                                string LockDetails = temp[0];
                                computer.Detail = LockDetails;
                                computer.pathNode = temp[1].Replace(@"I:\Startup_WinPE\Deployment\GDS Console", ".");
                            }
                            string configPath = computerFile.Replace(".my", ".cfg");
                            if (File.Exists(configPath))
                            {
                                ComputerConfigData computerConfigData = new ComputerConfigData();
                                computerConfigData.LoadDataFromList(File.ReadAllLines(configPath).ToList());
                                string pathConfig = configPath.Replace("\\Migrate Data", "");
                                FileHandler.Save(computerConfigData, pathConfig);
                            }
                            FileHandler.Save(computer, path);
                        }
                    }
                    if (Directory.Exists(@".\Migrate Data\Tasks\"))
                    {
                        var tasksData = Directory.GetFiles(@".\Migrate Data\Tasks\", "*.my", SearchOption.AllDirectories);
                        foreach (string taskFile in tasksData)
                        {
                            TaskData taskData = new TaskData(Path.GetFileName(taskFile).Replace(".my", ""), "NONE", "NONE", new List<ComputerDetailsData>());
                            taskData.LoadDataFromList(File.ReadAllLines(taskFile).ToList());
                            string path = taskFile.Replace("\\Migrate Data", "");
                            VytvorPriecinok(path.Replace(Path.GetFileName(path), ""));
                            FileHandler.Save(taskData, path);
                        }
                    }
                    if (Directory.Exists(@".\Migrate Data\Images\"))
                    {
                        var imageDataBase = Directory.GetFiles(@".\Migrate Data\Images\", "*.my", SearchOption.AllDirectories);
                        foreach (string imageFile in imageDataBase)
                        {
                            ImageData imageData = new ImageData(Path.GetFileName(imageFile).Replace(".my", ""));
                            imageData.LoadDataFromList(File.ReadAllLines(imageFile).ToList());
                            foreach (string osAbrv in imageData.OSAbrivations)
                            {
                                bool exist = false;
                                foreach (string osAbrv2 in osAbrivationsData.osAbrivations)
                                {
                                    if (osAbrv2.ToUpper() == osAbrv.ToUpper())
                                    {
                                        exist = true;
                                        break;
                                    }
                                }
                                if (!exist)
                                {
                                    if (osAbrv != "")
                                        osAbrivationsData.osAbrivations.Add(osAbrv.ToUpper());
                                }
                            }
                            string path = imageFile.Replace("\\Migrate Data\\Images", "\\Base");
                            VytvorPriecinok(path.Replace(Path.GetFileName(path), ""));
                            FileHandler.Save(imageData, path);
                        }
                    }
                    if (Directory.Exists(@".\Migrate Data\DriveE\"))
                    {
                        var imageDataDriveE = Directory.GetFiles(@".\Migrate Data\DriveE\", "*.my", SearchOption.AllDirectories);
                        foreach (string imageFile in imageDataDriveE)
                        {
                            ImageData imageData = new ImageData(Path.GetFileName(imageFile).Replace(".my", ""));
                            imageData.LoadDataFromList(File.ReadAllLines(imageFile).ToList());
                            string path = imageFile.Replace("\\Migrate Data", "");
                            VytvorPriecinok(path.Replace(Path.GetFileName(path), ""));
                            FileHandler.Save(imageData, path);
                        }
                        osAbrivationsData.osAbrivations.Sort();
                        FileHandler.Save<OSAbrivationsData>(osAbrivationsData, pathOSAbrivation);
                    }
                    treeViewMachinesAndTasksHandler.Refresh();
                    treeViewPostInstallsHandler.Refresh();
                    MessageBox.Show("DONE");
                }
            }
            else
            {
                MessageBox.Show("There is not folder 'Migrate Data'");
            }
        }

        private void BtnRefreshReleaseInfo_Click(object sender, RoutedEventArgs e)
        {
            LoadReleaseInfo();
        }

        private void treeViewPostInstalls_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.F5:
                    {
                        treeViewPostInstallsHandler.Refresh();
                        break;
                    }
            }
        }

        string GetGateWayHostName(string IPAdd)
        {
            string gatewayHostName = "";
            if (IPAdd.StartsWith("10.201."))
            {
                gatewayHostName = "gopasblavards.class.skola.cz";
            }
            if (IPAdd.StartsWith("10.202."))
            {
                gatewayHostName = "gopasblavards.class.skola.cz";
            }
            if (IPAdd.StartsWith("10.101."))
            {
                gatewayHostName = "gopasbrnords.class.skola.cz";
            }
            if (IPAdd.StartsWith("10.102."))
            {
                gatewayHostName = "gopasbrnords.class.skola.cz";
            }
            if (IPAdd.StartsWith("10.1."))
            {
                gatewayHostName = "gopasprahards.class.skola.cz";
            }
            if (IPAdd.StartsWith("10.2."))
            {
                gatewayHostName = "gopasprahards.class.skola.cz";
            }
            return gatewayHostName;
        }

        public static List<string> GetIPAddress()
        {
            try
            {
                List<string> IPAddress = new List<string>();                
                foreach (IPAddress IP in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
                {
                    IPAddress.Add(IP.ToString());
                }
                return IPAddress;
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
            return new List<string>();
        }

        private void CreateRDP(ListView list)
        {
            string gateWayHostName = "";
            bool exists = false;
            foreach (string IP in GetIPAddress())
            {
                gateWayHostName = GetGateWayHostName(IP);
                if(gateWayHostName != "")
                {
                    exists = true;
                    break;
                }
            }
            if (exists)
            {
                var TempFile = new List<string>(System.IO.File.ReadAllLines(@".\!NEMAZAT_temp"));
                string nodePath = treeViewMachinesAndTasksHandler.GetNodePath() ;
                for (int i = list.Items.Count - 1; i >= 0; i--)
                {
                    ComputerDetailsData computer = (ComputerDetailsData)list.Items[i];                    
                    CreateRDP(computer.Name,gateWayHostName, nodePath + "\\" + computer.Name + ".rdp", TempFile);
                }                
                System.Diagnostics.Process.Start(nodePath);
            }            
        }

        private void CreateRDP(string ComputerName, string gateWayHostName, string pathToSave, List<string> TempFile)
        {
            TempFile.Add("full address:s:" + ComputerName + ".class.skola.cz");
            TempFile.Add(@"username:s:CLASS\" + ComputerName);
            TempFile.Add("gatewayhostname:s:" + gateWayHostName);
            File.WriteAllLines(pathToSave.Replace(".my", ".rdp"), TempFile.ToArray());
        }

        private void listViewMachineGroupsLock_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.F8:
                    {
                        CreateRDP(listViewMachineGroupsLock);
                        break;
                    }
                case Key.F5:
                    {
                        listViewMachinesAndTasksHandler.Refresh();
                        break;
                    }
                case Key.F2:
                    {
                        if (listViewMachineGroupsLock.Visibility == Visibility.Visible)
                            RenameItem((ComputerDetailsData)listViewMachineGroupsLock.SelectedItem);
                        else
                            RenameItem((TaskData)listViewTasks.SelectedItem);
                        break;
                    }

                case Key.F3:
                    {
                        EditLockDetail(listViewMachineGroupsLock);
                        break;
                    }
                case Key.F4:
                    {
                        UnLock(listViewMachineGroupsLock);
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
                    case Key.W:
                        {
                            RunWakeOnLanOnSelectedItems(listViewMachineGroupsLock);
                            break;
                        }
                    case Key.R:
                        {
                            RemoteDesktop(listViewMachineGroupsLock);
                            break;
                        }
                    case Key.D:
                        {
                            DartViewer(listViewMachineGroupsLock);
                            break;
                        }
                }
            }
        }

        private void btnPostInstallsRefresh_Click(object sender, RoutedEventArgs e)
        {
            listViewPostInstallsHandler.RefreshPostInstalls();
        }

        private void btnInsertClassroom_Click(object sender, RoutedEventArgs e)
        {
            var computersInfoFiles = Directory.GetFiles(@".\Machine Groups\" + txtBoxClassRoomName.Text, "*.my", SearchOption.AllDirectories);
            foreach (string computerFile in computersInfoFiles)
            {
                ComputerDetailsData computer = FileHandler.Load<ComputerDetailsData>(computerFile);
                listener.UpdateDataGridByMacs(computer.macAddresses, computer.Name, txtBoxClassRoomID.Text);
            }
        }

        private void btnPostInstallsSelect_Click(object sender, RoutedEventArgs e)
        {
            listViewPostInstallsHandler.SeLect();
            listViewMachinesAndTasksHandler.Refresh();
        }

        private void btnPostInstallsUnSelect_Click(object sender, RoutedEventArgs e)
        {
            listViewPostInstallsHandler.UnSeLect();
            listViewMachinesAndTasksHandler.Refresh();
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
