using GDS_SERVER_WPF.DataCLasses;
using NetworkCommsDotNet.Tools;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Controls;

namespace GDS_SERVER_WPF.Handlers
{
    public class ListBoxBrowseComputersHandler
    {    
        public Dictionary<ShortGuid, ComputerWithConnection> ClientsDictionary = new Dictionary<ShortGuid, ComputerWithConnection>();

        public ListView machines;
        public TreeViewHandler treeViewHandler;

        public ListBoxBrowseComputersHandler(ListView _machines, TreeViewHandler _treeViewHandler)
        {
            this.machines = _machines;
            this.treeViewHandler = _treeViewHandler;            
        }

        public void LoadMachines(string path)
        {
            machines.Items.Clear();            
            if (Directory.Exists(path))
            {
                var directoriesInfoFiles = Directory.GetDirectories(path);                
                foreach (var dir in directoriesInfoFiles)
                {
                    machines.Items.Add(new ComputerDetailsData(new DirectoryInfo(dir).Name, "", "", "", "", "Images/Folder.ico"));
                }
                string[] computersInfoFiles = Directory.GetFiles(path, "*.my");
                foreach (string computerFile in computersInfoFiles)
                {
                    string Name = Path.GetFileName(computerFile).Replace(".my", "");
                    var computerData = FileHandler.Load<ComputerDetailsData>(computerFile);
                    computerData.ImageSource = "Images/Offline.ico";
                    foreach (KeyValuePair<ShortGuid, ComputerWithConnection> computer in ClientsDictionary)
                    {
                        if (computer.Value.ComputerData.macAddresses != null && Listener.CheckMacsInREC(computer.Value.ComputerData.macAddresses, computerData.macAddresses))
                        {
                            computerData.ImageSource = "Images/Online.ico";
                            if (computer.Value.ComputerData.inWinpe)
                                computerData.ImageSource = "Images/Winpe.ico";
                            break;
                        }
                    }                                                         
                    machines.Items.Add(computerData);
                }
            }
        }

        public void Refresh()
        {
            LoadMachines(treeViewHandler.GetNodePath());
        }

        public void LoadTreeViewMachines()
        {
            treeViewHandler.AddPath(@".\Machine Groups");
            treeViewHandler.Refresh();
        }
    }
}
