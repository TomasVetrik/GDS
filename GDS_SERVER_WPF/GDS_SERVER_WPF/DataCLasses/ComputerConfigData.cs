using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDS_SERVER_WPF
{
    public class ComputerConfigData
    {
        public ComputerConfigData() { }
        public ComputerConfigData(string _name = "", string _workgroup = "")
        {
            this.Name = _name;
            this.WorkGroup = _workgroup;
        }

        public string Name { get; set; }
        public string WorkGroup { get; set; }
    }
}
