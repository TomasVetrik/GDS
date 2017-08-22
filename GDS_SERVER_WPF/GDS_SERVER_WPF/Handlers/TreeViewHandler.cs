using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GDS_SERVER_WPF
{
    public class TreeViewHandler
    {
        TreeView treeView;
        public List<string> Paths;
        public bool refreshing = false;

        public TreeViewHandler(TreeView _treeView)
        {
            this.treeView = _treeView;
            Paths = new List<string>();
        }

        public void AddPath(string path)
        {
            Paths.Add(path);
        }

        public void ListDirectory(string path)
        {
            var rootDirectoryInfo = new DirectoryInfo(path);
            treeView.Items.Add(CreateDirectoryNode(rootDirectoryInfo));
        }

        public TreeViewItem CreateDirectoryNode(DirectoryInfo directoryInfo)
        {
            if (directoryInfo.Exists)
            {
                var directoryNode = new TreeViewItem { Header = directoryInfo.Name };
                foreach (var directory in directoryInfo.GetDirectories())
                    directoryNode.Items.Add(CreateDirectoryNode(directory));

                return directoryNode;
            }
            return null;
        }

        public void ClearNodes()
        {
            treeView.Items.Clear();
        }

        public void Refresh()
        {
            ClearNodes();
            foreach (string path in Paths)
            {
                ListDirectory(path);
            }
            ((TreeViewItem)treeView.Items[0]).IsSelected = true;
        }
                
        public void AddItem(string item)
        {
            var directoryNode = new TreeViewItem { Header = item };
            var items = (treeView.SelectedItem as TreeViewItem).Items;
            items.Add(directoryNode);
        }

        public void RemoveItem(string item)
        {
            var items = (treeView.SelectedItem as TreeViewItem).Items;
            for (int i = 0; i < items.Count; i++)
            {
                if (((TreeViewItem)items[i]).Header.ToString() == item)
                {
                    items.Remove(items[i]);
                    break;
                }
            }
        }
        public void RenameItem(string itemOld, string item)
        {
            var items = (treeView.SelectedItem as TreeViewItem).Items;
            for (int i = 0; i < items.Count; i++)
            {
                if (((TreeViewItem)items[i]).Header.ToString() == itemOld)
                {
                    ((TreeViewItem)items[i]).Header = item;
                    break;
                }
            }
        }


        public void SelectNode(TreeViewItem node)
        {
            node.IsSelected = true;
        }

        static TreeViewItem GetParentItem(TreeViewItem item)
        {
            for (var i = VisualTreeHelper.GetParent(item); i != null; i = VisualTreeHelper.GetParent(i))
                if (i is TreeViewItem)
                    return (TreeViewItem)i;

            return null;
        }


        public string GetNodePath()
        {
            var node = (TreeViewItem)treeView.SelectedItem;
            if (node != null)
            {
                var result = Convert.ToString(node.Header);

                for (var i = GetParentItem(node); i != null; i = GetParentItem(i))
                    result = i.Header + "\\" + result;

                return @".\" + result;
            }
            return "";
        }

        public void SetTreeNode(string name)
        {
            foreach (TreeViewItem item in (treeView.SelectedItem as TreeViewItem).Items)
            {
                if ((string)item.Header == name)
                {
                    item.IsSelected = true;
                    break;
                }
            }
        }
    }         
}
