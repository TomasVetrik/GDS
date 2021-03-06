﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GDS_SERVER_WPF
{
    public class ListViewMachinesAndTasksHandler
    {
        public List<ClientHandler> clients;
        

        public ListView machines;
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
                            if (client.inWinpe)
                                computerData.ImageSource = "Images/Winpe.ico";
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

        public void LoadTasks(string path)
        {
            tasks.Items.Clear();
            if (Directory.Exists(path))
            {
                var directoriesInfoFiles = Directory.GetDirectories(path);                
                foreach (var dir in directoriesInfoFiles)
                {
                    tasks.Items.Add(new TaskData(new DirectoryInfo(dir).Name, "", "", new List<ComputerDetailsData>(), "Images/Folder.ico"));
                }
                string[] tasksPath = Directory.GetFiles(path, "*.my");                
                foreach (string taskPath in tasksPath)
                {
                    if (File.Exists(taskPath))                    
                        tasks.Items.Add(FileHandler.Load<TaskData>(taskPath));                    
                }                
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

        public void SelectAll()
        {
            if (tasks.Visibility == Visibility.Visible)
                tasks.SelectAll();
            else
                machines.SelectAll();
        }

        public void LoadTreeViewMachinesAndTasks()
        {
            treeViewHandler.AddPath(@".\Machine Groups");
            treeViewHandler.AddPath(@".\Tasks");
            treeViewHandler.Refresh();
        }
    }
}
