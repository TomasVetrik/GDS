using System.Collections.Generic;
using System.IO;
using System.Threading;
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
                    foreach (ClientHandler client in clients)
                    {
                        if (client.macAddresses != null && client.CheckMacsInREC(client.macAddresses, computerData.macAddresses))
                        {
                            computerData.ImageSource = "Images/Online.ico";
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
                    machines.Items.Add(computerData);
                }
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
