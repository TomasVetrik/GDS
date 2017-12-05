using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace GDS_SERVER_WPF.Handlers
{
    public class ListViewPostinstallsHandler : ListBoxBrowseComputersHandler
    {
        public ListBox listBoxSelected;
        public ListBox listBoxPostInstalls;
        public string path = @"D:\StartUP_OS\PostInstallers\";

        public ListViewPostinstallsHandler(ListView _machines, TreeViewHandler _treeViewHandler, ListBox _listBoxSelected, ListBox _listBoxPostInstalls) :base(_machines, _treeViewHandler)
        {
            listBoxSelected = _listBoxSelected;
            listBoxPostInstalls = _listBoxPostInstalls;
            Refresh();
        }

        public void Refresh()
        {
            listBoxPostInstalls.Items.Clear();
            foreach (string directory in Directory.GetFiles(path, "Start.bat", SearchOption.AllDirectories))
            {
                string FileName = @"\" + Path.GetFileName(directory);
                string directory2 = directory.Replace(FileName, "");
                listBoxPostInstalls.Items.Add(directory2.Replace(path, ""));
            }
        }

        public void SeLect()
        {
            if (machines.SelectedItems.Count != 0 && listBoxPostInstalls.SelectedItems.Count != 0)
            {
                string postInstaller = (string)listBoxPostInstalls.SelectedItems[0];
                foreach (ComputerDetailsData machineGroupData in machines.SelectedItems)
                {
                    if (machineGroupData.ImageSource.Contains("Folder.ico"))
                    {
                        foreach (string file in Directory.GetFiles(treeViewHandler.GetNodePath() + "\\" + machineGroupData.Name, "*.my", SearchOption.AllDirectories))
                        {                            
                            string cfgFile = file.Replace(".my", ".cfg");
                            var config = new ComputerConfigData();
                            if (File.Exists(cfgFile))
                            {
                                config = FileHandler.Load<ComputerConfigData>(cfgFile);
                            }
                            else
                            {
                                var machine = FileHandler.Load<ComputerDetailsData>(file);
                                config = new ComputerConfigData(machine.RealPCName, "Workgroup");
                            }
                            if (!config.PostInstalls.Contains(postInstaller))
                                config.PostInstalls.Add(postInstaller);
                            if (!listBoxSelected.Items.Contains(postInstaller))
                                listBoxSelected.Items.Add(postInstaller);
                            FileHandler.Save(config, cfgFile);
                        }
                    }
                    else
                    {
                        string cfgFile = treeViewHandler.GetNodePath() + "\\" + machineGroupData.Name + ".cfg";
                        var config = new ComputerConfigData();
                        if (File.Exists(cfgFile))
                        {
                            config = FileHandler.Load<ComputerConfigData>(cfgFile);
                        }
                        else
                        {
                            config = new ComputerConfigData(machineGroupData.RealPCName, "Workgroup");
                        }
                        if (!config.PostInstalls.Contains(postInstaller))
                            config.PostInstalls.Add(postInstaller);
                        if (!listBoxSelected.Items.Contains(postInstaller))
                            listBoxSelected.Items.Add(postInstaller);
                        FileHandler.Save(config, cfgFile);
                    }
                }
            }
        }
    }
}
