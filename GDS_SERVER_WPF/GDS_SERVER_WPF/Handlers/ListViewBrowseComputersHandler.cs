using System.Collections.Generic;
using System.IO;
using System.Windows.Controls;

namespace GDS_SERVER_WPF.Handlers
{
    public class ListBoxBrowseComputersHandler
    {
        public List<ClientHandler> clients;

        ListView machines;
        TreeViewHandler treeViewHandler;

        public ListBoxBrowseComputersHandler(ListView _machines, TreeViewHandler _treeViewHandler)
        {
            this.machines = _machines;
            this.treeViewHandler = _treeViewHandler;
        }

        public void LoadMachines(string path)
        {
            if (Directory.Exists(path))
            {
                var directoriesInfoFiles = Directory.GetDirectories(path);
                var data = new List<MachinesGroupsData>();
                foreach (var dir in directoriesInfoFiles)
                {
                    data.Add(new MachinesGroupsData(new DirectoryInfo(dir).Name, "", "", "", "", "Images/Folder.ico"));
                }
                string[] computersInfoFiles = Directory.GetFiles(path, "*.my");
                foreach (string computerFile in computersInfoFiles)
                {
                    string Name = Path.GetFileName(computerFile).Replace(".my", "");
                    var computerData = FileHandler.Load<ComputerDetailsData>(computerFile);
                    string imageSource = "Images/Offline.ico";
                    foreach (ClientHandler client in clients)
                    {
                        if (client.macAddress != null && client.CheckMacsInREC(client.macAddress, computerData.macAddress))
                        {
                            imageSource = "Images/Online.ico";
                            break;
                        }
                    }
                    string lockFilePath = computerFile.Replace(".my", ".lock");
                    string detail = "";
                    if (File.Exists(lockFilePath))
                    {
                        var lockDetailsData = FileHandler.Load<LockDetailsData>(lockFilePath);
                        detail = lockDetailsData.details;
                    }
                    data.Add(new MachinesGroupsData(Name, computerData.macAddress, computerData.ipAddress, computerData.computerName, detail, imageSource));
                }
                machines.ItemsSource = data;
            }
        }

        public void Refresh()
        {
            LoadMachines(treeViewHandler.GetNodePath());
        }

        public void LoadTreeViewMachinesAndTasks()
        {
            treeViewHandler.AddPath(@".\Machine Groups");
            treeViewHandler.Refresh();
        }
    }
}
