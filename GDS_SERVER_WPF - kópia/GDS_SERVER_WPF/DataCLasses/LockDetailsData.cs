using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDS_SERVER_WPF
{
    public class LockDetailsData
    {
        public LockDetailsData(string _details, string _path)
        {
            this.details = _details;
            this.path = _path;
        }

        public string details { get; set; }
        public string path { get; set; }
    }
}
