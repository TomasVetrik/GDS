using GDS_SERVER_WPF.Handlers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace GDS_SERVER_WPF
{
    /// <summary>
    /// Interaction logic for BrowseComputers.xaml
    /// </summary>
    public partial class BrowseComputers : Window
    {
        public List<ClientHandler> clients;
        public ListBox listBoxOut;

        TreeViewHandler treeViewMachinesAndTasksHandler;
        ListBoxBrowseComputersHandler listViewBrowseComputersHandler;             
        public BrowseComputers()
        {
            InitializeComponent();
        }

        private void button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            treeViewMachinesAndTasksHandler = new TreeViewHandler(treeView);
            listViewBrowseComputersHandler = new ListBoxBrowseComputersHandler(listView, treeViewMachinesAndTasksHandler);
            listViewBrowseComputersHandler.clients = clients;
            listViewBrowseComputersHandler.LoadTreeViewMachinesAndTasks();
        }

        private void treeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (treeView.SelectedItem != null)
            {
                (treeView.SelectedItem as TreeViewItem).IsExpanded = true;
            }
            var path = treeViewMachinesAndTasksHandler.GetNodePath();
            listViewBrowseComputersHandler.LoadMachines(path);
            listView.SelectAll();
        }

        private void listView_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (listView.SelectedItems.Count != 0)
            {
                var computerDetail = (MachinesGroupsData)listView.SelectedItems[0];
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
                    listView.SelectAll();
                }
            }
        }

        private void button_OK_Click(object sender, RoutedEventArgs e)
        {
            listBoxOut.Items.Clear();
            if(listView.SelectedItems.Count != 0)
            {
                foreach (MachinesGroupsData machineGroupData in listView.SelectedItems)
                {
                    if (machineGroupData.ImageSource.Contains("Folder.ico"))
                    {
                        foreach(string file in Directory.GetFiles(treeViewMachinesAndTasksHandler.GetNodePath() + "\\" + machineGroupData.Name, "*.my", SearchOption.AllDirectories))
                        {
                            listBoxOut.Items.Add(Path.GetFileName(file).Replace(".my",""));
                        }
                    }
                    else
                    {
                        listBoxOut.Items.Add(machineGroupData.Name);
                    }
                }
            }
            this.Close();
        }       
    }
}
