using GDS_SERVER_WPF.DataCLasses;
using System.Collections.Generic;
using System.IO;
using System.Threading;
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
            tasks.Items.Clear();
            if (Directory.Exists(path))
            {
                var directoriesInfoFiles = Directory.GetDirectories(path);                
                foreach (var dir in directoriesInfoFiles)
                {
                    tasks.Items.Add(new ImageData(new DirectoryInfo(dir).Name, "Images/Folder.ico"));
                }
                string[] tasksPath = Directory.GetFiles(path, "*.my");
                foreach (string taskPath in tasksPath)
                {
                    tasks.Items.Add(FileHandler.Load<ImageData>(taskPath));
                }                
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

        public void SelectItemByName(string name)
        {
            foreach(ImageData item in tasks.Items)
            {
                if(item.Name == name)
                {
                    tasks.SelectedItem = item;
                    break;
                }
            }
        }
    }
}
