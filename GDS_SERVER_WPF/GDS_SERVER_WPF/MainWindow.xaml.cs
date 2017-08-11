using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GDS_SERVER_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Thread Server;
        Listener listener;
        TreeViewHandler treeViewMachinesAndTasksHandler;
        ListViewMachinesAndTasksHandler listViewMachinesAndTasksHandler;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(Environment.ExitCode);
        }

        private void MenuItemMachineGroupsRename_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("RENAME");
        }
        private void MenuItemMachineGroupsDelete_Click(object sender, RoutedEventArgs e)
        { }
        private void MenuItemMachineGroupsWOL_Click(object sender, RoutedEventArgs e)
        { }
        private void MenuItemMachineGroupsRDP_Click(object sender, RoutedEventArgs e)
        { }
        private void MenuItemMachineGroupsCreateFolder_Click(object sender, RoutedEventArgs e)
        { }
        private void MenuItemTasksNewTask_Click(object sender, RoutedEventArgs e)
        { }
        private void MenuItemTaskRename_Click(object sender, RoutedEventArgs e)
        { }
        private void MenuItemTaskDelete_Click(object sender, RoutedEventArgs e)
        { }
        private void MenuItemTaskCreateFolder_Click(object sender, RoutedEventArgs e)
        { }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CheckDirectories();            
            treeViewMachinesAndTasksHandler = new TreeViewHandler(treeViewMachinesAndTasks);
            listViewMachinesAndTasksHandler = new ListViewMachinesAndTasksHandler(listViewMachineGroups, listViewTasks, treeViewMachinesAndTasksHandler);
            listViewMachinesAndTasksHandler.LoadTreeViewMachinesAndTasks();
            listener = new Listener();
            listener.listBox1 = this.consoleGDS;
            listener.labelOnlineClients = labelOnline;
            listener.listViewMachinesAndTasksHandler = listViewMachinesAndTasksHandler;            
            Server = new Thread(listener.StartListener);
            Server.Start();            
        }    
        
        private void CheckDirectories()
        {
            var paths = new List<string>() { @".\Machine Groups", @".\Tasks", @".\Machine Groups\Default", @".\Machine Groups\Lock", @".\TaskDetails", @".\Base", @".\DriveE" };
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
            var path = treeViewMachinesAndTasksHandler.GetNodePath();
            listViewMachinesAndTasksHandler.SetVisibility(path);
        }

        private void listViewMachineGroups_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (listViewMachineGroups.SelectedItems.Count != 0)
            {
                var computerDetail = (MachinesGroupsData)listViewMachineGroups.SelectedItems[0];
                var path = treeViewMachinesAndTasksHandler.GetNodePath() + "\\" + computerDetail.Name;
                if (!computerDetail.ImageSource.Contains("Folder.ico"))
                {
                    var dialogComputerDetails = new ComputerDetails();
                    dialogComputerDetails.computerPath = path;
                    dialogComputerDetails.ShowDialog();
                }
                else
                {
                    treeViewMachinesAndTasksHandler.SetTreeNode(computerDetail.Name);                    
                }
            }
        }

        private void listViewMachineGroups_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = (MachinesGroupsData)listViewMachineGroups.SelectedItem;            
            if (selectedItem != null)
            {
                menuItemDeleteWG.IsEnabled = true;
                menuItemRenameWG.IsEnabled = true;                
                if (selectedItem.ImageSource.Contains("Folder.ico"))
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
                menuItemDeleteWG.IsEnabled = false;
                menuItemRenameWG.IsEnabled = false;
                menuItemFeaturesWG.IsEnabled = false;
            }
        }

        private void listViewTasks_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (listViewTasks.SelectedItems.Count != 0)
            {                
                var taskData = (TaskData)listViewTasks.SelectedItems[0];
                var path = treeViewMachinesAndTasksHandler.GetNodePath() + "\\" + taskData.Name + ".my";
                if (!taskData.ImageSource.Contains("Folder.ico"))
                {
                    var taskOptionsDialog = new TaskOptions();
                    taskOptionsDialog.path = path;
                    taskOptionsDialog.nodePath = treeViewMachinesAndTasksHandler.GetNodePath();
                    taskOptionsDialog.clients = listViewMachinesAndTasksHandler.clients;
                    taskOptionsDialog.ShowDialog();
                    listViewMachinesAndTasksHandler.Refresh();
                }
                else
                {
                    treeViewMachinesAndTasksHandler.SetTreeNode(taskData.Name);
                }
            }                
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            string name = @".\OsAbrivations.my";
            string lines = File.ReadAllText(name);
            List<string> fileInfo = new List<string>();
            fileInfo.Add(name);
            fileInfo.Add(lines);

            listViewMachinesAndTasksHandler.clients[0].SendMessage(DataIdentifier.SEND_CONFIG, String.Join("|..|", fileInfo.ToArray()));
        }
    }
}
