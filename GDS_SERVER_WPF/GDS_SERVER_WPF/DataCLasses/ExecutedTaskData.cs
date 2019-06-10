using System;
using System.Collections.Generic;

namespace GDS_SERVER_WPF.DataCLasses
{
    public class ExecutedTaskData
    {
        public ExecutedTaskData()
        {
            ProgressComputerData = new List<ProgressComputerData>();
            _TaskData = new TaskData();
        }

        public string Name { get; set; }
        public string Started { get; set; }
        public string Status { get; set; }
        public string Finished { get; set; }
        public string Clients { get; set; }
        public string Done { get; set; }
        public string Failed { get; set; }
        public string MachineGroup { get; set; }
        public TaskData _TaskData { get; set; }
        public List<ProgressComputerData> ProgressComputerData { get; set; }
        public string FilePath { get; set; }

        public string GetFileName()
        {
            if(FilePath == null)
            {
                FilePath = @".\TaskDetails\" + Started + "_" + Name + ".my";
            }
            return FilePath;
        }

        public void LoadDataFromList(List<string> list)
        {
            foreach (string line in list)
            {
                if (line != "")
                {

                }
            }
        }
    }
}
