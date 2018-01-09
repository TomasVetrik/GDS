using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace GDS_SERVER_WPF.Handlers
{
    public class ListViewHistoryHandler: ListViewTaskDetailsHandler
    {
        public TreeViewHandler treeViewHandler;

        public ListViewHistoryHandler(ListView _tasksDetails, string _path, TreeViewHandler _treeViewHandler) :base(_tasksDetails, _path)
        {
            this.tasksDetails = _tasksDetails;
            this.path = _path;
            treeViewHandler = _treeViewHandler;
            treeViewHandler.AddPath(path);
            treeViewHandler.Refresh();
            RefreshHistoryDetails();
        }              

        public void RefreshHistoryDetails()
        {
            path = treeViewHandler.GetNodePath();
            LoadTasksDetails();
        }
    }
}
