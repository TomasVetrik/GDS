﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDS_SERVER_WPF.DataCLasses
{
    public class ProgressComputerData
    {
        public string Status { get; set; }
        public string ComputerName { get; set; }
        public string Step { get; set; }
        public string Time { get; set; }
        public string MacAddress { get; set; }

        public ProgressComputerData()
        { }

        public ProgressComputerData(string _status, string _computerName, string _step, string _MacAddress = "")
        {
            this.Status = _status;
            this.ComputerName = _computerName;
            this.Step = _step;
            this.Time = DateTime.Now.ToLongTimeString();
            this.MacAddress = _MacAddress;
        }
    }
}
