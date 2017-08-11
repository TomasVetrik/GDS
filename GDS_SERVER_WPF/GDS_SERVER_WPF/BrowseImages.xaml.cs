using GDS_SERVER_WPF.DataCLasses;
using GDS_SERVER_WPF.Handlers;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
        }

        private void button_OK_Click(object sender, RoutedEventArgs e)
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
            var newFolderDialog = new EditItem();
            foreach (ImageData item in listView.Items)
                newFolderDialog.Names.Add(item.Name);
            newFolderDialog.ShowDialog();
            string path = treeViewMachinesAndTasksHandler.GetNodePath() + "\\" + newFolderDialog.textBoxNewName.Text;
            Directory.CreateDirectory(path);            
            treeViewMachinesAndTasksHandler.Refresh();
            treeViewMachinesAndTasksHandler.SetTreeNodeByLastSelectedNode(path, treeView);
        }
    }
}
