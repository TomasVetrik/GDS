using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GDS_SERVER_WPF
{
    public class ListViewMachinesAndTasksHandler
    {
        public List<ClientHandler> clients;

        ListView machines;
        ListView tasks;
        TreeViewHandler treeViewHandler;
        
        public ListViewMachinesAndTasksHandler(ListView _machines, ListView _tasks, TreeViewHandler _treeViewHandler)
        {
            this.machines = _machines;
            this.tasks = _tasks;
            this.treeViewHandler = _treeViewHandler;
        }

        public void SetVisibility(string path)
        {
            if (path.Contains(treeViewHandler.Paths[0]))
            {
                tasks.Visibility = Visibility.Hidden;
                machines.Visibility = Visibility.Visible;
            }
            else
            {
                tasks.Visibility = Visibility.Visible;
                machines.Visibility = Visibility.Hidden;
            }
            LoadList(path);
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

        public void LoadTasks(string path)
        {
            if (Directory.Exists(path))
            {
                var directoriesInfoFiles = Directory.GetDirectories(path);
                var data = new List<TaskData>();
                foreach (var dir in directoriesInfoFiles)
                {
                    data.Add(new TaskData(new DirectoryInfo(dir).Name, "", "", new List<string>(), "Images/Folder.ico"));
                }
                string[] tasksPath = Directory.GetFiles(path, "*.my");                
                foreach (string taskPath in tasksPath)
                {                    
                    data.Add(FileHandler.Load<TaskData>(taskPath));
                }
                tasks.ItemsSource = data;                
            }
        }

        public void Refresh()
        {
            LoadList(treeViewHandler.GetNodePath());
        }

        public void LoadList(string path)
        {                        
            if (tasks.Visibility == Visibility.Visible)
            {
                LoadTasks(path);
            }
            else
            {
                LoadMachines(path);
            }
        }

        public void LoadTreeViewMachinesAndTasks()
        {
            treeViewHandler.AddPath(@".\Machine Groups");
            treeViewHandler.AddPath(@".\Tasks");
            treeViewHandler.Refresh();
        }
    }
}
