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
        ListView images;


        public ListViewBrowseImagesHandler(ListView _tasks, TreeViewHandler _treeViewHandler)
        {
            this.images = _tasks;
            images.Focusable = true;
            this.treeViewHandler = _treeViewHandler;
        }

        public void LoadImages(string path)
        {
            images.Items.Clear();
            if (Directory.Exists(path))
            {
                var directoriesInfoFiles = Directory.GetDirectories(path);
                foreach (var dir in directoriesInfoFiles)
                {
                    images.Items.Add(new ImageData(new DirectoryInfo(dir).Name, "Images/Folder.ico"));
                }
                string[] tasksPath = Directory.GetFiles(path, "*.my");
                foreach (string taskPath in tasksPath)
                {
                    images.Items.Add(FileHandler.Load<ImageData>(taskPath));
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
            foreach (ImageData item in images.Items)
            {
                if (item.Name == name)
                {
                    images.SelectedItem = item;
                    break;
                }
            }
        }

        public void SelectAll()
        {
            images.SelectAll();
        }
    }
}
