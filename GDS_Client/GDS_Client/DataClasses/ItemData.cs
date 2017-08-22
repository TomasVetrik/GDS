using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDS_Client
{
    public class ItemData
    {
        public ItemData(string _Header, string _Content)
        {
            this.Header = _Header;
            this.Content = _Content;
        }
        public string Header { get; set; }
        public string Content { get; set; }
    }
}
