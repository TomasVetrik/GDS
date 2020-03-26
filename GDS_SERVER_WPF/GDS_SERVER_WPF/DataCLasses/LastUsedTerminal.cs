using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDS_SERVER_WPF.DataCLasses
{
    public class LastUsedTerminal
    {
        public List<string> commands { get; set; }
        public string userName { get; set; }
        public string userPassword { get; set; }

        public LastUsedTerminal()
        {
            commands = new List<string>();
        }
    }
}
