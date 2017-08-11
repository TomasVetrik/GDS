using GDS_SERVER_WPF.DataCLasses;
using GDS_SERVER_WPF.Handlers;
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
        public string path;
        public bool baseImage;            

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

        private void button_Edit_Click(object sender, RoutedEventArgs e)
        {

        }

        private void button_OK_Click(object sender, RoutedEventArgs e)
        {

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

        private void listView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (listView.SelectedItems.Count != 0)
            {
                var imagesDetail = (ImageData)listView.SelectedItems[0];
                var path = treeViewMachinesAndTasksHandler.GetNodePath() + "\\" + imagesDetail.Name;
                if (!imagesDetail.ImageSource.Contains("Folder.ico"))
                {
                    var imageOptionsDialog = new ImageOptions();
                    imageOptionsDialog.baseImage = baseImage;
                    imageOptionsDialog.path = treeViewMachinesAndTasksHandler.GetNodePath() + "\\" + imagesDetail.Name + ".my";
                    imageOptionsDialog.nodePath = treeViewMachinesAndTasksHandler.GetNodePath();
                    imageOptionsDialog.ShowDialog();
                }
                else
                {
                    treeViewMachinesAndTasksHandler.SetTreeNode(imagesDetail.Name);
                    listView.SelectAll();
                }
                
            }
        }

        private void button_New_Click(object sender, RoutedEventArgs e)
        {
            var imageOptionsDialog = new ImageOptions();
            imageOptionsDialog.baseImage = baseImage;
            imageOptionsDialog.nodePath = treeViewMachinesAndTasksHandler.GetNodePath();
            imageOptionsDialog.ShowDialog();
        }
    }
}
