using GDS_SERVER_WPF.DataCLasses;
using GDS_SERVER_WPF.Handlers;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace GDS_SERVER_WPF
{
    /// <summary>
    /// Interaction logic for BrowseTasks.xaml
    /// </summary>
    public partial class BrowseImages : Window
    {
        public string path = "";
        public bool baseImage = false;
        public string pathOutput = "";

        TreeViewHandler treeViewMachinesAndTasksHandler;
        ListViewBrowseImagesHandler listViewBrowseImagesHandler;

        public BrowseImages()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            treeViewMachinesAndTasksHandler = new TreeViewHandler(treeView);
            listViewBrowseImagesHandler = new ListViewBrowseImagesHandler(listView, treeViewMachinesAndTasksHandler);
            listViewBrowseImagesHandler.LoadTreeViewImages(path);
            listViewBrowseImagesHandler.Refresh();
            clipBoardImages = new List<ImageData>();
        }

        private void MenuItemRenameBrowseImage_Click(object sender, RoutedEventArgs e)
        {
            RenameItem();
        }
        private void MenuItemDeleteBrowseImage_Click(object sender, RoutedEventArgs e)
        {
            
        }
        private void MenuItemCreateFolderBrowseImage_Click(object sender, RoutedEventArgs e)
        {
            NewFolder();
        }
        private void MenuItemCopyBrowseImage_Click(object sender, RoutedEventArgs e)
        {
            copy = true;
            CopyToClipBoard();
        }
        private void MenuItemCutBrowseImage_Click(object sender, RoutedEventArgs e)
        {
            copy = false;
            CopyToClipBoard();
        }
        private void MenuItemPasteBrowseImage_Click(object sender, RoutedEventArgs e)
        {
            PasteClipBoard();
        }

        private string nodePathOld;
        private List<ImageData> clipBoardImages;
        private bool copy;
        private void CopyToClipBoard()
        {
            nodePathOld = treeViewMachinesAndTasksHandler.GetNodePath();
            clipBoardImages.Clear();
            for (int i = 0; i < listView.SelectedItems.Count; i++)
            {
                ImageData item = (ImageData)listView.SelectedItems[i];
                if (!item.ImageSource.Contains("_Selected.ico"))
                    item.ImageSource = item.ImageSource.Replace(".ico", "_Selected.ico");
                clipBoardImages.Add(item);
            }
            for (int i = 0; i < listView.Items.Count; i++)
            {
                ImageData item = (ImageData)listView.Items[i];
                int indexClipBoard = clipBoardImages.IndexOf(item);
                int index = listView.Items.IndexOf(item);
                listView.Items.Remove(item);
                if (indexClipBoard == -1)
                {
                    item.ImageSource = item.ImageSource.Replace("_Selected.ico", ".ico");
                    listView.Items.Insert(index, item);
                }
                else
                {
                    listView.Items.Insert(index, clipBoardImages[indexClipBoard]);
                }
            }
        }

        private void PasteClipBoard()
        {
            string nodePath = treeViewMachinesAndTasksHandler.GetNodePath();

            bool cancel = false;
            foreach (ImageData itemClipBoard in clipBoardImages)
            {
                if (cancel) { break; }
                bool exist = false;
                string oldPath = nodePathOld + "\\" + itemClipBoard.Name;
                string path = nodePath + "\\" + itemClipBoard.Name;
                if (!itemClipBoard.ImageSource.Contains("Folder_Selected.ico"))
                {
                    oldPath += ".my";
                    path += ".my";
                }

                for (int i = listView.Items.Count - 1; i >= 0; i--)
                {
                    ImageData item = (ImageData)listView.Items[i];
                    if (item.Name == itemClipBoard.Name)
                    {
                        exist = true;
                        if (nodePath == nodePathOld)
                        {
                            if (copy)
                            {
                                path = path.Replace(".my", "-(1).my");
                                if (!File.Exists(path))
                                {
                                    ImageData taskData = FileHandler.Load<ImageData>(oldPath);
                                    taskData.Name = itemClipBoard.Name + "-(1)";
                                    FileHandler.Save<ImageData>(taskData, path);
                                }
                            }
                        }
                        else
                        {
                            switch (MessageBox.Show("Replace Item: '" + itemClipBoard.Name + "'", "Warning", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning))
                            {
                                case MessageBoxResult.Yes:
                                    {
                                        if (copy)
                                        {
                                            if (File.Exists(path))
                                            {
                                                ImageData taskData = FileHandler.Load<ImageData>(oldPath);
                                                FileHandler.Save<ImageData>(taskData, path);
                                            }
                                        }
                                        else
                                        {
                                            if (File.Exists(path))
                                            {
                                                File.Delete(path);
                                            }
                                            File.Move(oldPath, path);
                                        }
                                        break;
                                    }
                                case MessageBoxResult.No:
                                    {
                                        break;
                                    }
                                case MessageBoxResult.Cancel:
                                    {
                                        cancel = true;
                                        break;
                                    }
                            }
                        }
                        break;
                    }
                }
                if (!exist)
                {
                    if (itemClipBoard.ImageSource.Contains("Folder_Selected.ico"))
                    {
                        try
                        {
                            if (!copy)
                                Directory.Move(oldPath, path);
                        }
                        catch { MessageBox.Show("Cannot Move Directory"); return; }
                    }
                    else
                    {
                        if (copy)
                            File.Copy(oldPath, path);
                        else
                            File.Move(oldPath, path);
                    }
                }
            }
            clipBoardImages.Clear();
            listViewBrowseImagesHandler.Refresh();
        }

        private void SelectedImage()
        {
            if (listView.SelectedItems.Count != 0)
            {
                var imagesDetail = (ImageData)listView.SelectedItems[0];
                var path = treeViewMachinesAndTasksHandler.GetNodePath() + "\\" + imagesDetail.Name;
                if (!imagesDetail.ImageSource.Contains("Folder.ico"))
                {
                    pathOutput = path;
                    this.Close();
                }
            }
            return;
        }

        private void button_OK_Click(object sender, RoutedEventArgs e)
        {
            SelectedImage();
        }

        private void EditSelectedItem()
        {
            if (listView.SelectedItems.Count != 0)
            {
                var imagesDetail = (ImageData)listView.SelectedItems[0];
                var path = treeViewMachinesAndTasksHandler.GetNodePath() + "\\" + imagesDetail.Name + ".my";
                if (!imagesDetail.ImageSource.Contains("Folder.ico"))
                {
                    var imageOptionsDialog = new ImageOptions();
                    foreach (ImageData item in listView.Items)
                    {
                        if (imagesDetail.Name != item.Name)
                            imageOptionsDialog.Names.Add(item.Name);
                    }
                    imageOptionsDialog.baseImage = baseImage;
                    imageOptionsDialog.path = path;                    
                    imageOptionsDialog.nodePath = treeViewMachinesAndTasksHandler.GetNodePath();
                    imageOptionsDialog.ShowDialog();                    
                    listViewBrowseImagesHandler.Refresh();
                    listViewBrowseImagesHandler.SelectItemByName(imageOptionsDialog.textBoxImageName.Text);
                }
                else
                {
                    treeViewMachinesAndTasksHandler.SetTreeNode(imagesDetail.Name);                    
                }
            }
        }

        private void button_Edit_Click(object sender, RoutedEventArgs e)
        {
            EditSelectedItem();
        }

        private void listView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            EditSelectedItem();
        }

        private void button_New_Click(object sender, RoutedEventArgs e)
        {
            var imageOptionsDialog = new ImageOptions();
            imageOptionsDialog.baseImage = baseImage;
            imageOptionsDialog.nodePath = treeViewMachinesAndTasksHandler.GetNodePath();            
            imageOptionsDialog.ShowDialog();
            listViewBrowseImagesHandler.Refresh();
            listViewBrowseImagesHandler.SelectItemByName(imageOptionsDialog.textBoxImageName.Text);
        }

        private void button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void treeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (treeView.SelectedItem != null)
            {
                (treeView.SelectedItem as TreeViewItem).IsExpanded = true;
            }
            var path = treeViewMachinesAndTasksHandler.GetNodePath();
            listViewBrowseImagesHandler.LoadImages(path);
            listView.SelectAll();
        }

        private void button_New_Folder_Click(object sender, RoutedEventArgs e)
        {
            NewFolder();
        }

        private void DeleteItem()
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Are you sure?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                for (int i = listView.SelectedItems.Count - 1; i >= 0; i--)
                {
                    ImageData item = (ImageData)listView.SelectedItems[i];
                    if (item.ImageSource.Contains("Folder"))
                    {
                        string path = treeViewMachinesAndTasksHandler.GetNodePath() + "\\" + item.Name;
                        if (Directory.Exists(path))
                            Directory.Delete(path, true);
                        treeViewMachinesAndTasksHandler.RemoveItem(item.Name);
                    }
                    else
                    {
                        string path = treeViewMachinesAndTasksHandler.GetNodePath() + "\\" + item.Name + ".my";
                        if (File.Exists(path))
                            File.Delete(path);
                    }
                }
                listViewBrowseImagesHandler.Refresh();
            }
        }

        private void NewFolder()
        {           
            var addFolderDialog = new EditItem();
            foreach (ImageData item in listView.Items)            
                addFolderDialog.Names.Add(item.Name);            
            addFolderDialog.ShowDialog();
            if (!addFolderDialog.cancel)
            {
                string path = treeViewMachinesAndTasksHandler.GetNodePath() + "\\" + addFolderDialog.textBoxNewName.Text;
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                    treeViewMachinesAndTasksHandler.AddItem(addFolderDialog.textBoxNewName.Text);
                    listViewBrowseImagesHandler.Refresh();
                }
            }

        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    {
                        this.Close();
                        break;
                    }
                case Key.Enter:
                    {
                        SelectedImage();
                        break;
                    }
                case Key.F2:
                    {
                        RenameItem();
                        break;
                    }
                case Key.Delete:
                    {
                        DeleteItem();
                        break;
                    }
            }
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                switch (e.Key)
                {
                    case Key.A:
                        {
                            listViewBrowseImagesHandler.SelectAll();
                            break;
                        }
                    case Key.X:
                        {
                            copy = false;
                            CopyToClipBoard();
                            break;
                        }
                    case Key.C:
                        {
                            copy = true;
                            CopyToClipBoard();
                            break;
                        }
                    case Key.V:
                        {
                            PasteClipBoard();
                            break;
                        }
                }
                if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                {
                    switch (e.Key)
                    {
                        case Key.N:
                            {
                                NewFolder();
                                break;
                            }
                    }
                }
            }
        }

        private void RenameItem()
        {
            var oldItem = (ImageData)listView.SelectedItem;
            if (oldItem != null)
            {
                string oldPath = treeViewMachinesAndTasksHandler.GetNodePath() + "\\" + oldItem.Name;
                var renameItemDialog = new EditItem();
                renameItemDialog.textBoxNewName.Text = oldItem.Name;
                renameItemDialog.labelOldName.Content = oldItem.Name;
                foreach (ImageData item in listView.Items)
                {
                    if (item.ImageSource == oldItem.ImageSource)
                        renameItemDialog.Names.Add(item.Name);
                }
                renameItemDialog.ShowDialog();
                if (!renameItemDialog.cancel)
                {
                    string path = treeViewMachinesAndTasksHandler.GetNodePath() + "\\" + renameItemDialog.textBoxNewName.Text;
                    if (oldItem.ImageSource.Contains("Folder"))
                    {
                        if (Directory.Exists(oldPath))
                            Directory.Move(oldPath, path);
                        treeViewMachinesAndTasksHandler.RenameItem(oldItem.Name, renameItemDialog.textBoxNewName.Text);
                    }
                    else
                    {
                        oldPath += ".my";
                        path += ".my";
                        ImageData imageData = FileHandler.Load<ImageData>(oldPath);
                        imageData.Name = renameItemDialog.textBoxNewName.Text;
                        if (File.Exists(oldPath))
                            File.Delete(oldPath);
                        FileHandler.Save<ImageData>(imageData, path);
                    }
                    listViewBrowseImagesHandler.Refresh();
                }
            }
        }

        private void listView_MouseDown(object sender, MouseButtonEventArgs e)
        {
            HitTestResult r = VisualTreeHelper.HitTest(this, e.GetPosition(this));
            if (r.VisualHit.GetType() != typeof(ListBoxItem))
            {
                listView.UnselectAll();
                listView.Focus();
            }
        }
    }
}
