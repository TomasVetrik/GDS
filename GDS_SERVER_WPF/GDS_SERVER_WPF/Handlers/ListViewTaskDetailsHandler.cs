using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace GDS_SERVER_WPF
{
    public class ListViewTaskDetailsHandler
    {
        ListView tasksDetails;
        string path;

        public ListViewTaskDetailsHandler(ListView _tasksDetails, string _path)
        {
            this.tasksDetails = _tasksDetails;
            this.path = _path;
        }

        public void Refresh()
        {

        }
    }
}
