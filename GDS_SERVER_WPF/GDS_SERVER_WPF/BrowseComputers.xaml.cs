using GDS_SERVER_WPF.DataCLasses;
using GDS_SERVER_WPF.Handlers;
using NetworkCommsDotNet.Tools;
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
    /// Interaction logic for BrowseComputers.xaml
    /// </summary>
    public partial class BrowseComputers : Window
    {
        public Dictionary<ShortGuid, ComputerWithConnection> ClientsDictionary = new Dictionary<ShortGuid, ComputerWithConnection>();
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
            listViewBrowseComputersHandler.ClientsDictionary = ClientsDictionary;
            listViewBrowseComputersHandler.LoadTreeViewMachines();
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
                var computerDetail = (ComputerDetailsData)listView.SelectedItems[0];
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

        private void SelectComputers()
        {
            listBoxOut.Items.Clear();
            if (listView.SelectedItems.Count != 0)
            {
                foreach (ComputerDetailsData machineGroupData in listView.SelectedItems)
                {
                    if (machineGroupData.ImageSource.Contains("Folder.ico"))
                    {
                        foreach (string file in Directory.GetFiles(treeViewMachinesAndTasksHandler.GetNodePath() + "\\" + machineGroupData.Name, "*.my", SearchOption.AllDirectories))
                        {
                            var machine = FileHandler.Load<ComputerDetailsData>(file);
                            listBoxOut.Items.Add(machine);
                        }
                    }
                    else
                    {
                        listBoxOut.Items.Add(machineGroupData);
                    }
                }
            }
        }

        private void button_OK_Click(object sender, RoutedEventArgs e)
        {
            SelectComputers();
            this.Close();
        }

        private void Window_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    {
                        this.Close();
                        break;
                    }
                case Key.Enter:
                    {
                        SelectComputers();
                        this.Close();
                        break;
                    }
            }
        }
    }
}
