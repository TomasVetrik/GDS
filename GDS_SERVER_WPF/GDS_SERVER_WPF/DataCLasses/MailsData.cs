using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDS_SERVER_WPF.DataCLasses
{
    public class MailsData
    {
        public List<string> Emails { get; set; }
        
        public MailsData()
        {
            Emails = new List<string>();
        }
    }
}
