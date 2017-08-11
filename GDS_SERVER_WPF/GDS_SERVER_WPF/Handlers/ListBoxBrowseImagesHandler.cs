using GDS_SERVER_WPF.DataCLasses;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace GDS_SERVER_WPF.Handlers
{
    public class ListViewBrowseImagesHandler
    {
        TreeViewHandler treeViewHandler;
        ListView tasks;

        public ListViewBrowseImagesHandler(ListView _tasks, TreeViewHandler _treeViewHandler)
        {
            this.tasks = _tasks;
            this.treeViewHandler = _treeViewHandler;
        }

        public void LoadImages(string path)
        {            
            if (Directory.Exists(path))
            {
                var directoriesInfoFiles = Directory.GetDirectories(path);
                var data = new List<ImageData>();
                foreach (var dir in directoriesInfoFiles)
                {
                    data.Add(new ImageData(new DirectoryInfo(dir).Name, "Images/Folder.ico"));
                }
                string[] tasksPath = Directory.GetFiles(path, "*.my");
                foreach (string taskPath in tasksPath)
                {
                    data.Add(FileHandler.Load<ImageData>(taskPath));
                }
                tasks.ItemsSource = data;
            }
        }

        public void Refresh()
        {
            LoadImages(treeViewHandler.GetNodePath());
        }

        public void LoadTreeViewImages(string path)
        {            
            treeViewHandler.AddPath(path);
            treeViewHandler.Refresh();
        }
    }
}
