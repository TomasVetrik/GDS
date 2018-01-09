using GDS_SERVER_WPF.DataCLasses;
using System;
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
    public class ListViewTaskDetailsHandler
    {
        public ListView tasksDetails;
        public string path;
        
        public ListViewTaskDetailsHandler(ListView _tasksDetails, string _path = @".\TaskDetails\")
        {
            this.tasksDetails = _tasksDetails;            
            this.path = _path;
        }

        public void LoadTasksDetails()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                tasksDetails.Items.Clear();
                if (Directory.Exists(path))
                {
                    string[] tasksPath = Directory.GetFiles(path, "*.my");
                    foreach (string taskPath in tasksPath)
                    {
                        if (File.Exists(taskPath))                        
                            tasksDetails.Items.Insert(0,FileHandler.Load<ExecutedTaskData>(taskPath));                        
                    }
                }                                
            });
        }

        public void Refresh()
        {
            LoadTasksDetails();
        }
    }
}
